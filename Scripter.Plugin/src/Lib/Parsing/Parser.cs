using System.Collections.Generic;

namespace ScripterLang
{
    public class Parser
    {
        public static Expression Parse(string source, GlobalLexicalContext globalLexicalContext)
        {
            var tokens = new List<Token>(Tokenizer.Tokenize(source));
            return Parse(tokens, globalLexicalContext);
        }

        public static Expression Parse(IList<Token> tokens, GlobalLexicalContext globalLexicalContext)
        {
            return new Parser(tokens).Parse(globalLexicalContext);
        }

        private readonly IList<Token> _tokens;
        private int _position;

        private Parser(IList<Token> tokens)
        {
            _tokens = tokens;
            _position = 0;
        }

        private void MoveNext()
        {
            _position++;
        }

        private Token Peek()
        {
            if (IsAtEnd()) return Token.None;
            return _tokens[_position];
        }

        private Token PeekNext()
        {
            if (_position >= _tokens.Count - 2) throw new ScripterParsingException("Unexpected end of script");
            return _tokens[_position + 1];
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

        private Expression Parse(GlobalLexicalContext globalLexicalContext)
        {
            var expressions = new List<Expression>();
            var lexicalContext = new ScopeLexicalContext(globalLexicalContext);
            while (!IsAtEnd())
            {
                expressions.Add(ParseExpression(lexicalContext));
            }
            return new FunctionBlockExpression(expressions, lexicalContext);
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
                if (token.Value == "static") return ParseStaticDeclaration(lexicalContext);
                if (token.Value == "var") return ParseVariableDeclaration(lexicalContext);
                if (token.Value == "let") return ParseVariableDeclaration(lexicalContext);
                if (token.Value == "throw") return ParseThrowDeclaration(lexicalContext);
                if (token.Value == "function") return ParseFunctionDeclaration(lexicalContext);
                throw new ScripterParsingException($"Unexpected keyword: {token.Value}", token.Location);
            }

            if (token.Match(TokenType.Identifier))
            {
                var next = PeekNext();
                if (next.Match(TokenType.LeftParenthesis))
                {
                    MoveNext();
                    MoveNext();
                    var arguments = ParseArgumentList(lexicalContext);
                    Consume().Expect(TokenType.RightParenthesis);
                    return new FunctionCallExpression(token.Value, arguments, lexicalContext);
                }
                else
                {
                    return ParseAssignmentExpression(lexicalContext);
                }
            }

            if (token.Match(TokenType.SemiColon))
            {
                Consume();
                return UndefinedExpression.Instance;
            }
            if (token.Match(TokenType.LeftBrace)) return ParseCodeBlock(lexicalContext);
            throw new ScripterParsingException($"Unexpected token: '{token.Value}'", token.Location);
        }

        private Expression ParseThrowDeclaration(ScopeLexicalContext lexicalContext)
        {
            MoveNext();
            var message = ParseValueStatementExpression(lexicalContext);
            return new ThrowExpression(message);
        }

        private Expression ParseFunctionDeclaration(ScopeLexicalContext lexicalContext)
        {
            MoveNext();
            var name = Consume().Expect(TokenType.Identifier);
            Consume().Expect(TokenType.LeftParenthesis);
            var functionLexicalContext = new ScopeLexicalContext(lexicalContext);
            var arguments = new List<string>();
            while (!Peek().Match(TokenType.RightParenthesis))
            {
                var arg = Consume().Expect(TokenType.Identifier).Value;
                functionLexicalContext.Declare(name.Value, name.Location);
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
            var body = new FunctionBlockExpression(expressions, functionLexicalContext);
            if (lexicalContext.Root.Functions.ContainsKey(name.Value))
                throw new ScripterParsingException($"A function with the name {name.Value} was already declared", name.Location);
            lexicalContext.Root.Functions.Add(name.Value, new FunctionDeclaration(name.Value, arguments, body).Invoke);
            return new UndefinedExpression();
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

        private Expression ParseReturnStatement(LexicalContext lexicalContext)
        {
            MoveNext();
            var value = ParseValueStatementExpression(lexicalContext);
            Consume().Expect(TokenType.SemiColon);
            return new ReturnExpression(value);
        }

        private Expression ParseVariableDeclaration(ScopeLexicalContext lexicalContext)
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
                return new VariableDeclarationExpression(nameToken.Value, initialValue);
            }
            else
            {
                Consume().Expect(TokenType.SemiColon);
                return new VariableDeclarationExpression(nameToken.Value, UndefinedExpression.Instance);
            }
        }

        private Expression ParseStaticDeclaration(ScopeLexicalContext lexicalContext)
        {
            MoveNext();
            Consume().Expect(TokenType.Keyword, "var");
            var nameToken = Consume().Expect(TokenType.Identifier);
            Consume().Expect(TokenType.Assignment);
            var initialValueExpression = ParseValueStatementExpression(lexicalContext);
            Consume().Expect(TokenType.SemiColon);

            return new StaticVariableDeclarationExpression(nameToken.Value, initialValueExpression);
        }

        private Expression ParseAssignmentExpression(LexicalContext lexicalContext)
        {
            var nameToken = Consume().Expect(TokenType.Identifier);

            var nextToken = Peek();

            if (nextToken.Match(TokenType.Assignment))
            {
                MoveNext();
                var right = ParseValueStatementExpression(lexicalContext);
                Consume().Expect(TokenType.SemiColon);
                return new AssignmentExpression(nameToken.Value, right);
            }

            if (nextToken.Match(TokenType.AssignmentOperator))
            {
                MoveNext();
                var right = ParseValueStatementExpression(lexicalContext);
                Consume().Expect(TokenType.SemiColon);
                return new AssignmentOperatorExpression(nameToken.Value,  nextToken.Value, right);
            }

            if (nextToken.Match(TokenType.IncrementDecrement))
            {
                MoveNext();
                return new IncrementDecrementExpression(nameToken.Value, nextToken.Value, true);
            }

            MoveNext();
            return new VariableExpression(nameToken.Value);
        }

        private Expression ParseValueStatementExpression(LexicalContext lexicalContext, int precedence = 0)
        {
            var left = ParsePureValueExpression(lexicalContext);

            while (precedence < GetOperatorPrecedence(Peek().Value)) {
                var operatorToken = Consume();
                var right = ParseValueStatementExpression(lexicalContext, GetOperatorPrecedence(operatorToken.Value));
                left = new BinaryExpression(left, operatorToken.Value, right);
            }

            return left;
        }

        private int GetOperatorPrecedence(string op)
        {
            // https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Operators/Operator_Precedence#table
            switch (op)
            {
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

        private Expression ParsePureValueExpression(LexicalContext lexicalContext)
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
                {
                    var op = token.Value;
                    var name = Consume().Value;
                    return new IncrementDecrementExpression(name, op, false);
                }
                case TokenType.LeftParenthesis:
                {
                    var expression = ParseValueStatementExpression(lexicalContext);
                    Consume().Expect(TokenType.RightParenthesis);
                    return new ParenthesesExpression(expression);
                }
                case TokenType.Identifier:
                {
                    var name = token.Value;
                    var next = Peek();
                    if (next.Match(TokenType.LeftParenthesis))
                    {
                        MoveNext();
                        var arguments = ParseArgumentList(lexicalContext);
                        Consume().Expect(TokenType.RightParenthesis);
                        return new FunctionCallExpression(name, arguments, lexicalContext);
                    }
                    else if (next.Match(TokenType.IncrementDecrement))
                    {
                        MoveNext();
                        return new IncrementDecrementExpression(name, next.Value, true);
                    }
                    else
                    {
                        return new VariableExpression(name);
                    }
                }
                default:
                    throw new ScripterParsingException("Unexpected token " + token.Value, token.Location);
            }
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

        private List<Expression> ParseArgumentList(LexicalContext lexicalContext)
        {
            var arguments = new List<Expression>();
            while (!Peek().Match(TokenType.RightParenthesis))
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
