using System.Collections.Generic;

namespace ScripterLang
{
    public class Parser
    {
        public static ModuleExpression Parse(string source, GlobalLexicalContext globalLexicalContext)
        {
            var tokens = new List<Token>(Tokenizer.Tokenize(source));
            return new Parser(tokens).Parse(globalLexicalContext);
        }

        private readonly IList<Token> _tokens;
        private int _position;

        private Parser(IList<Token> tokens)
        {
            _tokens = tokens;
            _position = 0;
        }

        private void MoveNext(int tokens = 1)
        {
            _position += tokens;
        }

        private Token Peek()
        {
            if (IsAtEnd()) return Token.None;
            return _tokens[_position];
        }

        private Token Consume()
        {
            var token = Peek();
            MoveNext();
            return token;
        }

        private bool IsAtEnd()
        {
            return _position == _tokens.Count;
        }

        private ModuleExpression Parse(GlobalLexicalContext globalLexicalContext)
        {
            var expressions = new List<Expression>();
            var lexicalContext = new ModuleLexicalContext(globalLexicalContext);
            while (!IsAtEnd())
            {
                expressions.Add(ParseExpression(lexicalContext));
            }
            return new ModuleExpression(expressions, lexicalContext);
        }

        private Expression ParseExpression(ScopeLexicalContext lexicalContext)
        {
            var token = Peek();
            if (token.Match(TokenType.Keyword))
            {
                if (token.Value == "if") return ParseIfStatement(lexicalContext);
                if (token.Value == "for") return ParseForStatement(lexicalContext);
                if (token.Value == "while") return ParseWhileStatement(lexicalContext);
                if (token.Value == "return") return ParseReturnStatement(lexicalContext);
                if (token.Value == "var" || token.Value == "let") return ParseVariableDeclaration(lexicalContext);
                if (token.Value == "throw") return ParseThrowDeclaration(lexicalContext);
                if (token.Value == "function") return ParseFunctionDeclaration(lexicalContext);
                if (token.Value == "import") return ParseImportDeclaration(lexicalContext);
                if (token.Value == "export") return ParseExportDeclaration(lexicalContext);
                throw new ScripterParsingException($"Unexpected keyword: {token.Value}", token.Location);
            }

            if (token.Match(TokenType.Identifier))
            {
                return ParseVariableStatement(lexicalContext, token);
            }

            if (token.Match(TokenType.LeftBrace))
            {
                return ParseCodeBlock(lexicalContext);
            }

            if (token.Match(TokenType.SemiColon))
            {
                #warning We probably don't always validate that lines indeed finish with a semicolon?
                Consume();
                return UndefinedExpression.Instance;
            }

            throw new ScripterParsingException($"Unexpected token: '{token.Value}'", token.Location);
        }

        private Expression ParseExportDeclaration(ScopeLexicalContext lexicalContext)
        {
            MoveNext();
            var token = Peek();
            DeclarationExpression expression;
            if (!token.Match(TokenType.Keyword))
                throw new ScripterParsingException("Expected var or function after export");
            if (token.Value == "var" || token.Value == "let") expression = ParseVariableDeclaration(lexicalContext);
            else if (token.Value == "function") expression = ParseFunctionDeclaration(lexicalContext);
            else throw new ScripterParsingException("Expected var or function after export");
            lexicalContext.GetModuleContext().Exports.Add(expression.Name, expression);
            return expression;
        }

        private Expression ParseImportDeclaration(ScopeLexicalContext lexicalContext)
        {
            MoveNext();
            Consume().Expect(TokenType.LeftBrace);
            var arguments = new List<string>();
            while (!Peek().Match(TokenType.RightBrace))
            {
                var arg = Consume().Expect(TokenType.Identifier);
                if (arguments.Contains(arg.Value))
                    throw new ScripterParsingException($"Imported binding {arg.Value} was declared more than once", arg.Location);
                arguments.Add(arg.Value);
                if (Peek().Match(TokenType.Comma))
                    MoveNext();
            }
            Consume().Expect(TokenType.RightBrace);
            Consume().Expect(TokenType.Keyword, "from");
            var module = Consume().Expect(TokenType.String);
            Consume().Expect(TokenType.SemiColon);
            return new ImportExpression(arguments, module.Value, lexicalContext.GetModuleContext());
        }

        private Expression ParseVariableStatement(ScopeLexicalContext lexicalContext, Token token)
        {
            MoveNext();
            var expression = ParseVariableExpression(lexicalContext, new ScopedVariableAccessor(token.Value, lexicalContext));
            Consume().Expect(TokenType.SemiColon);
            return expression;
        }

        private Expression ParseVariableExpression(ScopeLexicalContext lexicalContext, VariableAccessor accessor)
        {
            while (true)
            {
                var next = Peek();

                switch (next.Type)
                {
                    case TokenType.LeftBracket:
                    {
                        MoveNext();
                        var index = ParseValueStatementExpression(lexicalContext);
                        Consume().Expect(TokenType.RightBracket);
                        accessor = new IndexerAccessor(accessor, index);
                        continue;
                    }
                    case TokenType.LeftParenthesis:
                    {
                        MoveNext();
                        var arguments = ParseArgumentList(lexicalContext, TokenType.RightParenthesis);
                        Consume().Expect(TokenType.RightParenthesis);
                        return new FunctionCallExpression(accessor, arguments, lexicalContext);
                    }
                    case TokenType.Assignment:
                    {
                        MoveNext();
                        var right = ParseValueStatementExpression(lexicalContext);
                        return new AssignmentExpression(accessor, right);
                    }
                    case TokenType.AssignmentOperator:
                    {
                        MoveNext();
                        var right = ParseValueStatementExpression(lexicalContext);
                        return new AssignmentOperatorExpression(accessor, next.Value, right);
                    }
                    case TokenType.IncrementDecrement:
                        MoveNext();
                        return new IncrementDecrementExpression(accessor, next.Value, true);
                    case TokenType.Dot:
                        MoveNext();
                        return ParseDotRightExpression(lexicalContext, accessor);
                    default:
                        return accessor;
                }
            }
        }

        private Expression ParseThrowDeclaration(ScopeLexicalContext lexicalContext)
        {
            MoveNext();
            var message = ParseValueStatementExpression(lexicalContext);
            Consume().Expect(TokenType.SemiColon);
            return new ThrowExpression(message);
        }

        private FunctionDeclarationExpression ParseFunctionDeclaration(ScopeLexicalContext lexicalContext)
        {
            MoveNext();
            var name = Consume().Expect(TokenType.Identifier);
            Consume().Expect(TokenType.LeftParenthesis);
            var functionLexicalContext = new FunctionLexicalContext(lexicalContext);
            var arguments = new List<string>();
            while (!Peek().Match(TokenType.RightParenthesis))
            {
                var arg = Consume().Expect(TokenType.Identifier).Value;
                functionLexicalContext.Declare(arg, name.Location);
                if (arguments.Contains(name.Value))
                    throw new ScripterParsingException($"Argument {name.Value} of function {name.Value} was declared more than once", name.Location);
                arguments.Add(arg);
                if (Peek().Match(TokenType.Comma))
                    MoveNext();
            }
            Consume().Expect(TokenType.RightParenthesis);

            Consume().Expect(TokenType.LeftBrace);
            var expressions = new List<Expression>();

            while (!Peek().Match(TokenType.RightBrace))
            {
                var expression = ParseExpression(functionLexicalContext);
                expressions.Add(expression);
            }
            Consume().Expect(TokenType.RightBrace);
            var body = new CodeBlockExpression(expressions, functionLexicalContext);

            var function = new FunctionDeclarationExpression(name.Value, arguments, body, functionLexicalContext);
            lexicalContext.DeclareHoisted(name.Value, function.Value, name.Location);
            return function;
        }

        private Expression ParseIfStatement(ScopeLexicalContext lexicalContext)
        {
            MoveNext();
            Consume().Expect(TokenType.LeftParenthesis);
            var condition = ParseValueStatementExpression(lexicalContext);
            Consume().Expect(TokenType.RightParenthesis);
            var thenBlock = ParseCodeBlock(lexicalContext);

            if (Peek().Match(TokenType.Keyword, "else"))
            {
                MoveNext();
                Expression elseBlock;
                if (Peek().Match(TokenType.Keyword, "if"))
                {
                    elseBlock = ParseIfStatement(lexicalContext);
                }
                else
                {
                    elseBlock = ParseCodeBlock(lexicalContext);
                }
                return new IfExpression(condition, thenBlock, elseBlock);
            }

            return new IfExpression(condition, thenBlock, null);
        }

        private Expression ParseForStatement(ScopeLexicalContext lexicalContext)
        {
            MoveNext();
            Consume().Expect(TokenType.LeftParenthesis);
            Expression initializer;
            var next = Peek();
            if (next.Match(TokenType.Keyword) && (next.Value == "var" || next.Value == "let"))
                initializer = ParseVariableDeclaration(lexicalContext);
            else
                initializer = ParseValueStatementExpression(lexicalContext);
            var condition = ParseValueStatementExpression(lexicalContext);
            Consume().Expect(TokenType.SemiColon);
            var increment = ParseValueStatementExpression(lexicalContext);
            Consume().Expect(TokenType.RightParenthesis);
            var body = ParseCodeBlock(lexicalContext);

            return new ForExpression(initializer, condition, increment, body);
        }

        private Expression ParseWhileStatement(ScopeLexicalContext lexicalContext)
        {
            MoveNext();
            Consume().Expect(TokenType.LeftParenthesis);
            var condition = ParseValueStatementExpression(lexicalContext);
            Consume().Expect(TokenType.RightParenthesis);
            var body = ParseCodeBlock(lexicalContext);

            return new WhileExpression(condition, body);
        }

        private Expression ParseReturnStatement(ScopeLexicalContext lexicalContext)
        {
            MoveNext();
            var value = !Peek().Match(TokenType.SemiColon) ? ParseValueStatementExpression(lexicalContext) : UndefinedExpression.Instance;
            Consume().Expect(TokenType.SemiColon);
            return new ReturnExpression(value, lexicalContext);
        }

        private VariableDeclarationExpression ParseVariableDeclaration(ScopeLexicalContext lexicalContext)
        {
            MoveNext();
            var nameToken = Consume();
            nameToken.Expect(TokenType.Identifier);
            lexicalContext.Declare(nameToken.Value, nameToken.Location);
            if (Peek().Match(TokenType.Assignment))
            {
                MoveNext();
                var initialValue = ParseValueStatementExpression(lexicalContext);
                Consume().Expect(TokenType.SemiColon);
                return new VariableDeclarationExpression(nameToken.Value, initialValue, lexicalContext);
            }
            else
            {
                Consume().Expect(TokenType.SemiColon);
                return new VariableDeclarationExpression(nameToken.Value, UndefinedExpression.Instance, lexicalContext);
            }
        }

        private Expression ParseValueStatementExpression(ScopeLexicalContext lexicalContext)
        {
            if (Peek().Match(TokenType.LeftBracket))
            {
                MoveNext();
                var values = ParseArgumentList(lexicalContext, TokenType.RightBracket);
                Consume().Expect(TokenType.RightBracket);
                return new ArrayDeclarationExpression(values);
            }
            return ParseValueStatementExpression(lexicalContext, 0);
        }

        private Expression ParseValueStatementExpression(ScopeLexicalContext lexicalContext, int precedence)
        {
            var left = ParsePureValueExpression(lexicalContext);

            while (precedence < GetOperatorPrecedence(Peek().Value)) {
                var operatorToken = Consume();
                if (operatorToken.Type == TokenType.Operator)
                {
                    var right = ParseValueStatementExpression(lexicalContext, GetOperatorPrecedence(operatorToken.Value));
                    left = new BinaryExpression(left, operatorToken.Value, right);
                }
                else if (operatorToken.Type == TokenType.Dot)
                {
                    left = ParseDotRightExpression(lexicalContext, left);
                }
                else
                {
                    throw new ScripterParsingException($"Unexpected token {operatorToken.Value}");
                }
            }

            return left;
        }

        private Expression ParseDotRightExpression(ScopeLexicalContext lexicalContext, Expression left)
        {
            var property = Consume().Expect(TokenType.Identifier);
            var accessor = new PropertyAccessor(left, property.Value);
            return ParseVariableExpression(lexicalContext, accessor);
        }

        private int GetOperatorPrecedence(string op)
        {
            // https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Operators/Operator_Precedence#table
            switch (op)
            {
                case ".":
                    return 17;

                case "*":
                case "/":
                case "%":
                    return 12;

                case "+":
                case "-":
                    return 11;

                case "<=":
                case "<":
                case ">=":
                case ">":
                    return 9;

                case "==":
                case "!=":
                    return 8;

                case "&&":
                    return 4;

                case "||":
                    return 3;

                default:
                    return 0;
            }
        }

        private Expression ParsePureValueExpression(ScopeLexicalContext lexicalContext)
        {
            var token = Consume();
            switch (token.Type)
            {
                case TokenType.Float:
                    return new FloatExpression(float.Parse(token.Value));
                case TokenType.Integer:
                    return new IntegerExpression(int.Parse(token.Value));
                case TokenType.String:
                    return new StringExpression(token.Value);
                case TokenType.Boolean:
                    return new BooleanExpression(bool.Parse(token.Value));
                case TokenType.Undefined:
                    return new UndefinedExpression();
                case TokenType.Negation:
                    return new NegateExpression(ParsePureValueExpression(lexicalContext));
                case TokenType.IncrementDecrement:
                    return ParseIncrementDecrementExpression(lexicalContext, token);
                case TokenType.LeftParenthesis:
                {
                    var expression = ParseValueStatementExpression(lexicalContext);
                    Consume().Expect(TokenType.RightParenthesis);
                    return new ParenthesesExpression(expression);
                }
                case TokenType.Identifier:
                    return ParseVariableExpression(lexicalContext, new ScopedVariableAccessor(token.Value, lexicalContext));
                default:
                    throw new ScripterParsingException("Unexpected token " + token.Value, token.Location);
            }
        }

        private Expression ParseIncrementDecrementExpression(ScopeLexicalContext lexicalContext, Token op)
        {
            var next = Consume();
            VariableAccessor accessor = new ScopedVariableAccessor(next.Value, lexicalContext);
            if (!next.Match(TokenType.Identifier))
                throw new ScripterParsingException($"Unexpected token {next.Value}", next.Location);
            if (Peek().Match(TokenType.LeftBracket))
            {
                MoveNext();
                var index = ParseValueStatementExpression(lexicalContext);
                Consume().Expect(TokenType.RightBracket);
                accessor = new IndexerAccessor(accessor, index);
            }

            return new IncrementDecrementExpression(accessor, op.Value, false);
        }

        private CodeBlockExpression ParseCodeBlock(ScopeLexicalContext parentLexicalContext)
        {
            Consume().Expect(TokenType.LeftBrace);
            var expressions = new List<Expression>();
            var lexicalContext = new ScopeLexicalContext(parentLexicalContext);
            while (!Peek().Match(TokenType.RightBrace))
            {
                var expression = ParseExpression(lexicalContext);
                expressions.Add(expression);
            }
            Consume().Expect(TokenType.RightBrace);
            return new CodeBlockExpression(expressions, lexicalContext);
        }

        private List<Expression> ParseArgumentList(ScopeLexicalContext lexicalContext, int endTokenType)
        {
            var arguments = new List<Expression>();
            while (!Peek().Match(endTokenType))
            {
                var expression = ParseValueStatementExpression(lexicalContext);
                arguments.Add(expression);
                if (Peek().Match(TokenType.Comma))
                    MoveNext();
            }
            return arguments;
        }
    }
}
