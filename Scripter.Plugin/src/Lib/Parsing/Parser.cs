using System.Collections.Generic;
using System.Linq.Expressions;

namespace ScripterLang
{
    public class Parser
    {
        public static Expression Parse(IList<Token> tokens)
        {
            return new Parser(tokens).Parse();
        }

        private readonly IList<Token> _tokens;
        private int _position;

        private Parser(IList<Token> tokens)
        {
            _tokens = tokens;
            _position = 0;
        }

        private bool MoveNext()
        {
            _position++;
            return !IsAtEnd();
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

        private Expression Parse()
        {
            var expressions = new List<Expression>();
            while (!IsAtEnd())
            {
                expressions.Add(ParseExpression());
            }
            return new CodeBlockExpression(expressions);
        }

        private Expression ParseExpression()
        {
            var token = Peek();
            if (token.Match(TokenType.Keyword))
            {
                if (token.Value == "if") return ParseIfStatement();
                if (token.Value == "for") return ParseForStatement();
                if (token.Value == "while") return ParseWhileStatement();
                if (token.Value == "return") return ParseReturnStatement();
                if (token.Value == "var") return ParseVariableDeclaration();
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
            if (token.Match(TokenType.LeftBrace)) return ParseCodeBlock();
            throw new ScripterParsingException($"Unexpected token: '{token.Value}'");
        }

        private Expression ParseIfStatement()
        {
            MoveNext();
            Consume().Expect(TokenType.LeftParenthesis);
            var condition = ParseValueStatementExpression();
            Consume().Expect(TokenType.RightParenthesis);
            var thenBlock = ParseCodeBlock();

            if (Peek().Match(TokenType.Keyword, "else"))
            {
                MoveNext();
                var elseBlock = ParseCodeBlock();
                return new IfExpression(condition, thenBlock, elseBlock);
            }

            return new IfExpression(condition, thenBlock, null);
        }

        private Expression ParseForStatement()
        {
            MoveNext();
            Consume().Expect(TokenType.LeftParenthesis);
            var initializer = ParseExpression();
            var condition = ParseExpression();
            var increment = ParseExpression();
            Consume().Expect(TokenType.RightParenthesis);
            var body = ParseCodeBlock();

            return new ForExpression(initializer, condition, increment, body);
        }

        private Expression ParseWhileStatement()
        {
            MoveNext();
            Consume().Expect(TokenType.LeftParenthesis);
            var condition = ParseExpression();
            Consume().Expect(TokenType.RightParenthesis);
            var body = ParseCodeBlock();

            return new WhileExpression(condition, body);
        }

        private Expression ParseReturnStatement()
        {
            MoveNext();
            var value = ParseValueStatementExpression();
            Consume().Expect(TokenType.SemiColon);
            return new ReturnExpression(value);
        }

        private Expression ParseVariableDeclaration()
        {
            MoveNext();
            var nameToken = Consume();
            nameToken.Expect(TokenType.Identifier);
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
                case TokenType.Number:
                    return new NumberExpression(double.Parse(token.Value));
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

        private CodeBlockExpression ParseCodeBlock()
        {
            var expressions = new List<Expression>();
            Consume().Expect(TokenType.LeftBrace);
            while (!Peek().Match(TokenType.RightBrace))
            {
                var expression = ParseExpression();
                expressions.Add(expression);
            }
            Consume().Expect(TokenType.RightBrace);
            return new CodeBlockExpression(expressions);
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
