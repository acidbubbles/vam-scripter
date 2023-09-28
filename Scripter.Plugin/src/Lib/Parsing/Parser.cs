using System.Collections.Generic;
using System.Text;

namespace ScripterLang
{
    public class Parser
    {
        private static readonly StringBuilder _stringBuilder = new StringBuilder();
        private readonly IList<Token> _tokens;
        private int _position;

        public Parser(IList<Token> tokens)
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
            if (IsAtEnd()) return new Token(TokenType.None, "EOF", new Location { line = _tokens[_tokens.Count - 1].location.line + 1 });
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

        public ModuleExpression Parse(GlobalLexicalContext globalLexicalContext, string moduleName)
        {
            var expressions = new List<Expression>();
            var lexicalContext = new ModuleLexicalContext(globalLexicalContext);
            while (!IsAtEnd())
            {
                expressions.Add(ParseRootExpression(lexicalContext));
            }
            var module = new ModuleExpression(expressions, moduleName, lexicalContext);
            module.Bind();
            return module;
        }

        private Expression ParseRootExpression(ModuleLexicalContext lexicalContext)
        {
            var token = Peek();
            if (token.Match(TokenType.Keyword))
            {
                if (token.value == "import") return ParseImportDeclaration(lexicalContext);
                if (token.value == "export") return ParseExportDeclaration(lexicalContext);
            }
            return ParseExpression(lexicalContext);
        }

        private Expression ParseExpression(ScopeLexicalContext lexicalContext)
        {
            var token = Peek();
            if (token.Match(TokenType.Keyword))
            {
                if (token.value == "if") return ParseIfStatement(lexicalContext);
                if (token.value == "for") return ParseForStatement(lexicalContext);
                if (token.value == "while") return ParseWhileStatement(lexicalContext);
                if (token.value == "return") return ParseReturnStatement(lexicalContext);
                if (token.value == "var" || token.value == "let") return ParseVariableDeclaration(lexicalContext, false);
                if (token.value == "const") return ParseVariableDeclaration(lexicalContext, true);
                if (token.value == "throw") return ParseThrowDeclaration(lexicalContext);
                if (token.value == "try") return TryCatchDeclaration(lexicalContext);
                if (token.value == "function") return ParseFunctionDeclaration(lexicalContext, true);
                if (token.value == "break" || token.value == "continue") return ParseLoopControlFlowDeclaration(lexicalContext);
                throw new ScripterParsingException($"Unexpected keyword: {token.value}", token.location);
            }

            if (token.Match(TokenType.Identifier))
            {
                return ParseVariableStatement(lexicalContext, token);
            }

            if (token.Match(TokenType.LeftBrace))
            {
                var blockScope = new ScopeLexicalContext(lexicalContext);
                return ParseCodeBlock(blockScope);
            }

            if (token.Match(TokenType.SemiColon))
            {
                Consume();
                return UndefinedExpression.Instance;
            }

            return ParsePureValueExpression(lexicalContext);
        }

        private Expression TryCatchDeclaration(ScopeLexicalContext lexicalContext)
        {
            Consume().Expect(TokenType.Keyword, "try");
            var tryContext = new ScopeLexicalContext(lexicalContext);
            var tryBlock = ParseCodeBlock(tryContext);

            CodeBlockExpression catchBlock = null;
            VariableReference errorReference = null;
            if (Peek().Match(TokenType.Keyword, "catch"))
            {
                MoveNext();
                var catchContext = new ScopeLexicalContext(lexicalContext);

                if (Peek().Match(TokenType.LeftParenthesis))
                {
                    MoveNext();
                    if (Peek().Match(TokenType.Identifier))
                    {
                        var errorVariable = Consume();
                        errorReference = new VariableReference(errorVariable.value, errorVariable.location) { constant = true, local = true };
                        catchContext.Declare(errorReference);
                    }

                    Consume().Expect(TokenType.RightParenthesis);
                }
                catchBlock = ParseCodeBlock(catchContext);
            }

            CodeBlockExpression finallyBlock = null;
            if (Peek().Match(TokenType.Keyword, "finally"))
            {
                MoveNext();
                var finallyContext = new ScopeLexicalContext(lexicalContext);
                finallyBlock = ParseCodeBlock(finallyContext);
            }

            return new TryCatchExpression(tryBlock, catchBlock, finallyBlock, errorReference);
        }

        private LoopControlFlowExpression ParseLoopControlFlowDeclaration(ScopeLexicalContext lexicalContext)
        {
            var token = Consume();
            return new LoopControlFlowExpression(token.value, lexicalContext);
        }

        private Expression ParseExportDeclaration(ModuleLexicalContext lexicalContext)
        {
            MoveNext();
            var token = Peek();
            DeclarationExpression expression;
            if (!token.Match(TokenType.Keyword))
                throw new ScripterParsingException("Expected var or function after export");
            if (token.value == "var" || token.value == "let" || token.value == "const")
                expression = ParseVariableDeclaration(lexicalContext, true);
            else if (token.value == "function")
                expression = ParseFunctionDeclaration(lexicalContext, true);
            else
                throw new ScripterParsingException("Expected var or function after export");
            return new ExportExpression(expression, lexicalContext);
        }

        private Expression ParseImportDeclaration(ModuleLexicalContext lexicalContext)
        {
            MoveNext();
            var arguments = new List<string>();
            if (Peek().Match(TokenType.LeftBrace))
            {
                MoveNext();
                while (!Peek().Match(TokenType.RightBrace))
                {
                    var arg = Consume().Expect(TokenType.Identifier);
                    if (arguments.Contains(arg.value))
                        throw new ScripterParsingException($"Imported binding {arg.value} was declared more than once", arg.location);
                    arguments.Add(arg.value);
                    lexicalContext.Declare(new VariableReference(arg.value, arg.location) { constant = true });
                    if (Peek().Match(TokenType.Comma))
                        MoveNext();
                }

                Consume().Expect(TokenType.RightBrace);
                Consume().Expect(TokenType.Keyword, "from");
            }
            var module = Consume().Expect(TokenType.String);
            Consume().Expect(TokenType.SemiColon);
            return new ImportExpression(arguments, module.value, lexicalContext);
        }

        private Expression ParseVariableStatement(ScopeLexicalContext lexicalContext, Token token)
        {
            MoveNext();
            var expression = ParseVariableExpression(lexicalContext, new ScopedVariableAccessor(token.value, lexicalContext));
            Consume().Expect(TokenType.SemiColon);
            return expression;
        }

        private Expression ParseVariableExpression(ScopeLexicalContext lexicalContext, VariableAccessor accessor)
        {
            while (true)
            {
                var next = Peek();

                switch (next.type)
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
                        var call = new FunctionCallExpression(accessor, arguments, lexicalContext);
                        if (!Peek().Match(TokenType.SemiColon))
                        {
                            return ParseVariableExpression(lexicalContext, new ExpressionAccessor(call));
                        }
                        return call;
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
                        return new AssignmentOperatorExpression(accessor, next.value, right);
                    }
                    case TokenType.IncrementDecrement:
                        MoveNext();
                        return new IncrementDecrementExpression(accessor, next.value, true);
                    case TokenType.Dot:
                        MoveNext();
                        return ParseDotRightExpression(lexicalContext, accessor);
                    case TokenType.Ternary:
                        MoveNext();
                        return ParseTernaryRightExpression(lexicalContext, accessor);
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

        private FunctionDeclarationExpression ParseFunctionDeclaration(ScopeLexicalContext lexicalContext, bool requiresIdentifier)
        {
            MoveNext();
            Token name;
            if(requiresIdentifier)
                name = Consume().Expect(TokenType.Identifier);
            else
                name = Peek().Match(TokenType.Identifier) ? Consume() : Token.None;
            var functionLexicalContext = new FunctionLexicalContext(lexicalContext);
            Consume().Expect(TokenType.LeftParenthesis);
            var arguments = ParseFunctionArguments(name.value, functionLexicalContext);
            Consume().Expect(TokenType.RightParenthesis);
            var body = ParseFunctionBody(functionLexicalContext);
            var function = new FunctionDeclarationExpression(name.value, arguments, body, functionLexicalContext);
            if (name.type == TokenType.Identifier)
                lexicalContext.Declare(new VariableReference(name.value, function.functionValue, name.location) { constant = true, bound = true});
            return function;
        }

        private CodeBlockExpression ParseFunctionBody(FunctionLexicalContext functionLexicalContext)
        {
            Consume().Expect(TokenType.LeftBrace);
            var expressions = new List<Expression>();

            while (!Peek().Match(TokenType.RightBrace))
            {
                var expression = ParseExpression(functionLexicalContext);
                expressions.Add(expression);
            }

            Consume().Expect(TokenType.RightBrace);
            var body = new CodeBlockExpression(expressions, functionLexicalContext);
            return body;
        }

        private List<string> ParseFunctionArguments(string name, FunctionLexicalContext functionLexicalContext)
        {
            var arguments = new List<string>();
            while (!Peek().Match(TokenType.RightParenthesis))
            {
                var arg = Consume().Expect(TokenType.Identifier);
                functionLexicalContext.Declare(new VariableReference(arg.value, arg.location) { local = true });
                if (arguments.Contains(arg.value))
                    throw new ScripterParsingException($"Argument {arg.value} of function {name} was declared more than once", arg.location);
                arguments.Add(arg.value);
                if (Peek().Match(TokenType.Comma))
                    MoveNext();
            }
            return arguments;
        }

        private Expression ParseIfStatement(ScopeLexicalContext lexicalContext)
        {
            MoveNext();
            Consume().Expect(TokenType.LeftParenthesis);
            var condition = ParseValueStatementExpression(lexicalContext);
            Consume().Expect(TokenType.RightParenthesis);
            var thenBlock = ParseCodeBlock(new ScopeLexicalContext(lexicalContext));

            if (Peek().Match(TokenType.Keyword, "else"))
            {
                MoveNext();
                Expression elseBlock;
                if (Peek().Match(TokenType.Keyword, "if"))
                {
                    elseBlock = ParseIfStatement(new ScopeLexicalContext(lexicalContext));
                }
                else
                {
                    elseBlock = ParseCodeBlock(new ScopeLexicalContext(lexicalContext));
                }
                return new IfExpression(condition, thenBlock, elseBlock);
            }

            return new IfExpression(condition, thenBlock, null);
        }

        private Expression ParseForStatement(ScopeLexicalContext lexicalContext)
        {
            MoveNext();
            var loopLexicalContext = new LoopLexicalContext(lexicalContext);
            Consume().Expect(TokenType.LeftParenthesis);
            Expression initializer;
            var next = Peek();
            if (next.Match(TokenType.Keyword) && (next.value == "var" || next.value == "let"))
                initializer = ParseVariableDeclaration(loopLexicalContext, false);
            else
                initializer = ParseValueStatementExpression(loopLexicalContext);
            var condition = ParseValueStatementExpression(loopLexicalContext);
            Consume().Expect(TokenType.SemiColon);
            var increment = ParseValueStatementExpression(loopLexicalContext);
            Consume().Expect(TokenType.RightParenthesis);
            var body = ParseCodeBlock(new ScopeLexicalContext(loopLexicalContext));

            return new ForExpression(initializer, condition, increment, body, loopLexicalContext);
        }

        private Expression ParseWhileStatement(ScopeLexicalContext lexicalContext)
        {
            MoveNext();
            var loopLexicalContext = new LoopLexicalContext(lexicalContext);
            Consume().Expect(TokenType.LeftParenthesis);
            var condition = ParseValueStatementExpression(loopLexicalContext);
            Consume().Expect(TokenType.RightParenthesis);
            var body = ParseCodeBlock(new ScopeLexicalContext(loopLexicalContext));

            return new WhileExpression(condition, body, loopLexicalContext);
        }

        private Expression ParseReturnStatement(ScopeLexicalContext lexicalContext)
        {
            MoveNext();
            var value = !Peek().Match(TokenType.SemiColon) ? ParseValueStatementExpression(lexicalContext) : UndefinedExpression.Instance;
            Consume().Expect(TokenType.SemiColon);
            return new ReturnExpression(value, lexicalContext);
        }

        private VariableDeclarationExpression ParseVariableDeclaration(ScopeLexicalContext lexicalContext, bool isConstant)
        {
            MoveNext();
            var nameToken = Consume();
            nameToken.Expect(TokenType.Identifier);
            lexicalContext.Declare(new VariableReference(nameToken.value, nameToken.location) { local = true, constant = isConstant });
            var next = Peek();

            if (next.Match(TokenType.Assignment))
            {
                MoveNext();
                var initialValue = ParseValueStatementExpression(lexicalContext);
                if (Peek().Match(TokenType.Ternary))
                {
                    MoveNext();
                    initialValue = ParseTernaryRightExpression(lexicalContext, initialValue);
                }
                Consume().Expect(TokenType.SemiColon);
                return new VariableDeclarationExpression(nameToken.value, initialValue, lexicalContext);
            }

            Consume().Expect(TokenType.SemiColon);
            return new VariableDeclarationExpression(nameToken.value, UndefinedExpression.Instance, lexicalContext);
        }

        private Expression ParseValueStatementExpression(ScopeLexicalContext lexicalContext, int precedence = 0)
        {
            var left = ParsePureValueExpression(lexicalContext);

            while (precedence < GetOperatorPrecedence(Peek().value)) {
                var operatorToken = Consume();
                if (operatorToken.type == TokenType.Operator)
                {
                    var right = ParseValueStatementExpression(lexicalContext, GetOperatorPrecedence(operatorToken.value));
                    left = new BinaryExpression(left, operatorToken.value, right);
                }
                else if (operatorToken.type == TokenType.Dot)
                {
                    left = ParseDotRightExpression(lexicalContext, left);
                }
                else
                {
                    throw new ScripterParsingException($"Unexpected operator '{operatorToken.value}'");
                }
            }

            return left;
        }

        private Expression ParseDotRightExpression(ScopeLexicalContext lexicalContext, Expression left)
        {
            var property = Consume().Expect(TokenType.Identifier);
            var accessor = new PropertyAccessor(left, property.value);
            return ParseVariableExpression(lexicalContext, accessor);
        }

        private Expression ParseTernaryRightExpression(ScopeLexicalContext lexicalContext, Expression condition)
        {
            var thenBlock = ParseValueStatementExpression(lexicalContext);
            Consume().Expect(TokenType.Colon);
            var elseBlock = ParseValueStatementExpression(lexicalContext);
            return new IfExpression(condition, thenBlock, elseBlock);
        }

        private static int GetOperatorPrecedence(string op)
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

        private bool IsArrowFunction()
        {
            if (_position > _tokens.Count - 3) return false;
            var next = Peek();

            if (next.Match(TokenType.LeftParenthesis))
            {
                var i = _position + 1;
                while (i < _tokens.Count && _tokens[i].type != TokenType.RightParenthesis)
                {
                    i++;
                }
                if (i == _tokens.Count - 1) return false;
                i++;
                return _tokens[i].Match(TokenType.Arrow);
            }

            if (next.Match(TokenType.Identifier))
            {
                return _tokens[_position + 1].Match(TokenType.Arrow);
            }

            return false;
        }

        private Expression ParsePureValueExpression(ScopeLexicalContext lexicalContext)
        {
            var token = Peek();
            switch (token.type)
            {
                case TokenType.Float:
                    return new ValueExpression(float.Parse(Consume().value));
                case TokenType.Integer:
                    return new ValueExpression(int.Parse(Consume().value));
                case TokenType.String:
                    return new ValueExpression(ParseString(Consume()));
                case TokenType.Template:
                    throw new ScripterParsingException("Template strings are not supported", Consume().location);
                case TokenType.Boolean:
                    return new ValueExpression(bool.Parse(Consume().value));
                case TokenType.Undefined:
                    MoveNext();
                    return new UndefinedExpression();
                case TokenType.Negation:
                    MoveNext();
                    return new NegateExpression(ParsePureValueExpression(lexicalContext));
                case TokenType.Operator:
                    var op = Consume();
                    if (op.value == "-")
                        return new UnaryOperatorExpression(op.value, ParsePureValueExpression(lexicalContext));
                    throw new ScripterParsingException($"Unexpected unary operator token '{token.value}'", token.location);
                case TokenType.IncrementDecrement:
                    return ParseIncrementDecrementExpression(lexicalContext, Consume());
                case TokenType.LeftParenthesis:
                {
                    if (IsArrowFunction())
                        return ParseArrowFunctionExpression(lexicalContext);

                    MoveNext();
                    var expression = ParseValueStatementExpression(lexicalContext);
                    Consume().Expect(TokenType.RightParenthesis);
                    return new ParenthesesExpression(expression);
                }
                case TokenType.Identifier:
                    if (IsArrowFunction())
                        return ParseArrowFunctionExpression(lexicalContext);

                    return ParseVariableExpression(lexicalContext, new ScopedVariableAccessor(Consume().value, lexicalContext));
                case TokenType.Keyword:
                    if (token.value == "function")
                        return ParseFunctionDeclaration(lexicalContext, false);
                    throw new ScripterParsingException($"Unexpected keyword '{token.value}'", token.location);
                case TokenType.LeftBracket:
                    MoveNext();
                    var values = ParseArgumentList(lexicalContext, TokenType.RightBracket);
                    Consume().Expect(TokenType.RightBracket);
                    return new ArrayDeclarationExpression(values);
                case TokenType.LeftBrace:
                    return ParseObjectDeclarationExpression(lexicalContext);
                default:
                    throw new ScripterParsingException($"Unexpected token '{token.value}' in value expression", token.location);
            }
        }

        private static string ParseString(Token token)
        {
            _stringBuilder.Length = 0;
            var value = token.value;
            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];
                if (c == '\\')
                {
                    if (i + 1 >= value.Length)
                        throw new ScripterParsingException("Unexpected end of string", token.location);
                    var next = value[i + 1];
                    switch (next)
                    {
                        case 'n':
                            _stringBuilder.Append('\n');
                            break;
                        case 'r':
                            break;
                        case 't':
                            _stringBuilder.Append('\t');
                            break;
                        case '\\':
                            _stringBuilder.Append('\\');
                            break;
                        case '\'':
                            _stringBuilder.Append('\'');
                            break;
                        case '"':
                            _stringBuilder.Append('"');
                            break;
                        default:
                            throw new ScripterParsingException($"Unexpected escape sequence '\\{next}'", token.location);
                    }
                    i++;
                }
                else
                {
                    _stringBuilder.Append(c);
                }
            }
            var result = _stringBuilder.ToString();
            _stringBuilder.Length = 0;
            return result;
        }

        private Expression ParseObjectDeclarationExpression(ScopeLexicalContext lexicalContext)
        {
            Consume().Expect(TokenType.LeftBrace);
            var values = new Dictionary<string, Expression>();
            while (!Peek().Match(TokenType.RightBrace))
            {
                var token = Consume();
                if (!token.Match(TokenType.Identifier) && !token.Match(TokenType.String))
                    throw new ScripterParsingException($"Unexpected token '{token.value}' in object declaration key", token.location);
                var key = token.value;
                Consume().Expect(TokenType.Colon);
                var value = ParseValueStatementExpression(lexicalContext);
                values.Add(key, value);
                if (Peek().Match(TokenType.Comma))
                    MoveNext();
            }
            Consume().Expect(TokenType.RightBrace);
            return new ObjectDeclarationExpression(values);
        }

        private Expression ParseArrowFunctionExpression(ScopeLexicalContext lexicalContext)
        {
            const string name = "";
            var functionLexicalContext = new FunctionLexicalContext(lexicalContext);
            List<string> arguments;
            if (Peek().Match(TokenType.LeftParenthesis))
            {
                Consume().Expect(TokenType.LeftParenthesis);
                arguments = ParseFunctionArguments(name, functionLexicalContext);
                Consume().Expect(TokenType.RightParenthesis);
            }
            else
            {
                var arg = Consume();
                functionLexicalContext.Declare(new VariableReference(arg.value, arg.location) { local = true });
                arguments = new List<string> { arg.value };
            }
            Consume().Expect(TokenType.Arrow);
            CodeBlockExpression body;
            if (Peek().Match(TokenType.LeftBrace))
                body = ParseFunctionBody(functionLexicalContext);
            else
                body = new CodeBlockExpression(new List<Expression> { ParseValueStatementExpression(functionLexicalContext) }, lexicalContext);
            var function = new FunctionDeclarationExpression(name, arguments, body, functionLexicalContext);
            return function;
        }

        private Expression ParseIncrementDecrementExpression(ScopeLexicalContext lexicalContext, Token op)
        {
            var next = Consume();
            VariableAccessor accessor = new ScopedVariableAccessor(next.value, lexicalContext);
            if (!next.Match(TokenType.Identifier))
                throw new ScripterParsingException($"Unexpected token {next.value}", next.location);
            if (Peek().Match(TokenType.LeftBracket))
            {
                MoveNext();
                var index = ParseValueStatementExpression(lexicalContext);
                Consume().Expect(TokenType.RightBracket);
                accessor = new IndexerAccessor(accessor, index);
            }

            return new IncrementDecrementExpression(accessor, op.value, false);
        }

        private CodeBlockExpression ParseCodeBlock(ScopeLexicalContext lexicalContext)
        {
            var expressions = new List<Expression>();
            if (Peek().Match(TokenType.LeftBrace))
            {
                Consume().Expect(TokenType.LeftBrace);
                while (!Peek().Match(TokenType.RightBrace))
                {
                    var expression = ParseExpression(lexicalContext);
                    expressions.Add(expression);
                }

                Consume().Expect(TokenType.RightBrace);
            }
            else
            {
                expressions = new List<Expression> { ParseExpression(lexicalContext) };
            }

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
