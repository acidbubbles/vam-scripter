using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ScripterLang
{
    public class Parser
    {
        public static Expression Parse(string source, GlobalLexicalContext globalLexicalContext)
        {
            var tokens = Tokenizer.Tokenize(source).ToList();
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
            return new CodeBlockExpression(expressions, lexicalContext);
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
                if (token.Value == "throw") return ParseThrowDeclaration(lexicalContext);
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
                return EmptyExpression.Instance;
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
            if (Peek().Match(TokenType.Keyword, "var"))
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
            if (lexicalContext.Declarations.Contains(nameToken.Value))
                throw new ScripterRuntimeException($"Variable '{nameToken.Value}' was already declared");
            lexicalContext.Declarations.Add(nameToken.Value);
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
                return new VariableDeclarationExpression(nameToken.Value, EmptyExpression.Instance);
            }
        }

        private Expression ParseStaticDeclaration(ScopeLexicalContext lexicalContext)
        {
            MoveNext();
            Consume().Expect(TokenType.Keyword, "var");
            var nameToken = Consume().Expect(TokenType.Identifier);
            Consume().Expect(TokenType.Assignment);
            var initialValueToken = Consume();
            Value initialValue;
            switch (initialValueToken.Type)
            {
                case TokenType.Float:
                    initialValue = float.Parse(initialValueToken.Value);
                    break;
                case TokenType.Integer:
                    initialValue = int.Parse(initialValueToken.Value);
                    break;
                case TokenType.String:
                    initialValue = initialValueToken.Value;
                    break;
                case TokenType.Boolean:
                    initialValue = bool.Parse(initialValueToken.Value);
                    break;
                default:
                    throw new ScripterParsingException("Static declarations must be a value (float, integer, string or boolean)", initialValueToken.Location);
            }
            Consume().Expect(TokenType.SemiColon);

            if (!lexicalContext.Root.StaticDeclarations.ContainsKey(nameToken.Value))
                lexicalContext.Root.StaticDeclarations.Add(nameToken.Value, initialValue);

            return EmptyExpression.Instance;
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

        private Expression ParseValueStatementExpression(LexicalContext lexicalContext)
        {
            var left = ParseValueExpression(lexicalContext);
            while (true)
            {
                var token = Peek();
                if (token.Match(TokenType.SemiColon) || token.Match(TokenType.Comma) || token.Match(TokenType.RightParenthesis))
                    break;

                if (token.Match(TokenType.Operator))
                {
                    MoveNext();
                    var right = ParseValueExpression(lexicalContext);
                    left = new BinaryExpression(left, token.Value, right);
                    continue;
                }

                throw new ScripterParsingException($"Unexpected token in value statement: {token.Value}", token.Location);
            }
            return left;
        }

        private Expression ParseValueExpression(LexicalContext lexicalContext)
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
                case TokenType.Negation:
                    return new NegateExpression(ParseValueExpression(lexicalContext));
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
                var cur = Peek();
                if (cur.Match(TokenType.Comma))
                    MoveNext();
            }
            return arguments;
        }
    }
}
