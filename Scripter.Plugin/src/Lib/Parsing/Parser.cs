using System.Collections.Generic;
using System.Linq;

namespace ScripterLang
{
    public class Parser
    {
        public static Expression Parse(string source, GlobalLexicalContext globalLexicalContext)
        {
            var tokens = Tokenizer.Tokenize(source).ToList();
            return new Parser(tokens).Parse(globalLexicalContext);
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
                if (token.Value == "return") return ParseReturnStatement();
                if (token.Value == "var") return ParseVariableDeclaration(lexicalContext);
                throw new ScripterParsingException($"Unexpected keyword: {token.Value}");
            }

            if (token.Match(TokenType.Identifier))
            {
                if (PeekNext().Match(TokenType.LeftParenthesis))
                {
                    MoveNext();
                    MoveNext();
                    var arguments = ParseArgumentList();
                    Consume().Expect(TokenType.RightParenthesis);
                    return new FunctionCallExpression(token.Value, arguments);
                }
                else
                {
                    return ParseAssignmentExpression();
                }
            }

            if (token.Match(TokenType.SemiColon))
            {
                Consume();
                return new EmptyExpression();
            }
            if (token.Match(TokenType.LeftBrace)) return ParseCodeBlock(lexicalContext);
            throw new ScripterParsingException($"Unexpected token: '{token.Value}'");
        }

        private Expression ParseIfStatement(ScopeLexicalContext lexicalContext)
        {
            MoveNext();
            Consume().Expect(TokenType.LeftParenthesis);
            var condition = ParseValueStatementExpression();
            Consume().Expect(TokenType.RightParenthesis);
            var thenBlock = ParseCodeBlock(lexicalContext);

            if (Peek().Match(TokenType.Keyword, "else"))
            {
                MoveNext();
                var elseBlock = ParseCodeBlock(lexicalContext);
                return new IfExpression(condition, thenBlock, elseBlock);
            }

            return new IfExpression(condition, thenBlock, null);
        }

        private Expression ParseForStatement(ScopeLexicalContext lexicalContext)
        {
            MoveNext();
            Consume().Expect(TokenType.LeftParenthesis);
            var initializer = ParseValueStatementExpression();
            var condition = ParseValueStatementExpression();
            var increment = ParseValueStatementExpression();
            Consume().Expect(TokenType.RightParenthesis);
            var body = ParseCodeBlock(lexicalContext);

            return new ForExpression(initializer, condition, increment, body);
        }

        private Expression ParseWhileStatement(ScopeLexicalContext lexicalContext)
        {
            MoveNext();
            Consume().Expect(TokenType.LeftParenthesis);
            var condition = ParseValueExpression();
            Consume().Expect(TokenType.RightParenthesis);
            var body = ParseCodeBlock(lexicalContext);

            return new WhileExpression(condition, body);
        }

        private Expression ParseReturnStatement()
        {
            MoveNext();
            var value = ParseValueStatementExpression();
            Consume().Expect(TokenType.SemiColon);
            return new ReturnExpression(value);
        }

        private Expression ParseVariableDeclaration(ScopeLexicalContext lexicalContext)
        {
            MoveNext();
            var nameToken = Consume();
            nameToken.Expect(TokenType.Identifier);
            lexicalContext.Declarations.Add(nameToken.Value, Value.Uninitialized);
            if (Peek().Match(TokenType.Assignment))
            {
                MoveNext();
                var initialValue = ParseValueStatementExpression();
                Consume().Expect(TokenType.SemiColon);
                return new VariableDeclarationExpression(nameToken.Value, initialValue);
            }
            else
            {
                Consume().Expect(TokenType.SemiColon);
                return new VariableDeclarationExpression(nameToken.Value, new EmptyExpression());
            }
        }

        private Expression ParseAssignmentExpression()
        {
            var nameToken = Consume();
            nameToken.Expect(TokenType.Identifier);

            if (Peek().Match(TokenType.Assignment))
            {
                MoveNext();
                var right = ParseValueStatementExpression();
                Consume().Expect(TokenType.SemiColon);
                return new AssignmentExpression(nameToken.Value, right);
            }

            // TODO: ++, --

            MoveNext();
            return new VariableExpression(nameToken.Value);
        }

        private Expression ParseValueStatementExpression()
        {
            var left = ParseValueExpression();
            while (true)
            {
                var token = Peek();
                if (token.Match(TokenType.SemiColon) || token.Match(TokenType.Comma) || token.Match(TokenType.RightParenthesis))
                    break;

                if (token.Match(TokenType.Operator))
                {
                    MoveNext();
                    var right = ParseValueExpression();
                    left = new BinaryExpression(left, token.Value, right);
                    continue;
                }

                throw new ScripterParsingException($"Unexpected token in value statement: {token.Value}");
            }
            return left;
        }

        private Expression ParseValueExpression()
        {
            var token = Consume();
            switch (token.Type)
            {
                case TokenType.Float:
                    return new FloatExpression(float.Parse(token.Value));
                case TokenType.Integer:
                    return new FloatExpression(float.Parse(token.Value));
                case TokenType.String:
                    return new StringExpression(token.Value);
                case TokenType.Boolean:
                    return new BooleanExpression(bool.Parse(token.Value));
                case TokenType.Identifier:
                    var name = token.Value;
                    if (Peek().Match(TokenType.LeftParenthesis))
                    {
                        MoveNext();
                        var arguments = ParseArgumentList();
                        Consume().Expect(TokenType.RightParenthesis);
                        return new FunctionCallExpression(name, arguments);
                    }
                    else
                    {
                        return new VariableExpression(name);
                    }
                default:
                    throw new ScripterParsingException("Unexpected token " + token.Value);
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

        private List<Expression> ParseArgumentList()
        {
            var arguments = new List<Expression>();
            while (!Peek().Match(TokenType.RightParenthesis))
            {
                var expression = ParseValueStatementExpression();
                arguments.Add(expression);
                var cur = Peek();
                if (cur.Match(TokenType.Comma))
                    MoveNext();
            }
            return arguments;
        }
    }
}
