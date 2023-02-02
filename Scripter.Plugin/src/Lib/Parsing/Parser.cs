using System.Collections.Generic;

namespace ScripterLang
{
    public class Parser
    {
        public static IEnumerable<Expression> ParseExpressions(IList<Token> tokens)
        {
            var i = 0;
            while (i < tokens.Count)
            {
                yield return ParseExpression(tokens, ref i);
                if (i >= tokens.Count) continue;
                if (tokens[i].Type != TokenType.SemiColon)
                    throw new ScripterParsingException("Expected line to end with ';'");
                i++;
            }
        }

        private static Expression ParseExpression(IList<Token> tokens, ref int i)
        {
            var token = tokens[i];
            switch (token.Type)
            {
                case TokenType.Number:
                    i++;
                    return new NumberExpression(double.Parse(token.Value));
                case TokenType.String:
                    i++;
                    return new StringExpression(token.Value);
                case TokenType.Boolean:
                    i++;
                    return new BooleanExpression(bool.Parse(token.Value));
                case TokenType.Identifier:
                {
                    i++;
                    if (i < tokens.Count && tokens[i].Type == TokenType.LeftParenthesis)
                    {
                        i++;
                        var args = new List<Expression>();
                        while (tokens[i].Type != TokenType.RightParenthesis)
                        {
                            args.Add(ParseExpression(tokens, ref i));
                            if (tokens[i].Type == TokenType.Comma)
                            {
                                i++;
                            }
                        }

                        i++;
                        return new FunctionCallExpression(token.Value, args);
                    }
                    else
                    {
                        return new VariableExpression(token.Value);
                    }
                }
                case TokenType.Operator:
                {
                    i++;
                    var left = ParseExpression(tokens, ref i);
                    var right = ParseExpression(tokens, ref i);
                    return new BinaryExpression(left, token.Value, right);
                }
                case TokenType.Keyword:
                {
                    switch (token.Value)
                    {
                        case "return":
                        {
                            i++;
                            var right = ParseExpression(tokens, ref i);
                            return new ReturnExpression(right);
                        }
                        case "if":
                        {
                            i++;
                            var condition = ParseExpression(tokens, ref i);
                            var trueBranch = ParseExpression(tokens, ref i);
                            Expression falseBranch = null;
                            if (i < tokens.Count && tokens[i].Type == TokenType.Keyword && tokens[i].Value == "else")
                            {
                                i++;
                                falseBranch = ParseExpression(tokens, ref i);
                            }

                            return new IfExpression(condition, trueBranch, falseBranch);
                        }
                        case "for":
                        {
                            i++;
                            var start = ParseExpression(tokens, ref i);
                            var end = ParseExpression(tokens, ref i);
                            var body = ParseExpression(tokens, ref i);
                            return new ForExpression(start, end, body);
                        }
                        case "var":
                        {
                            i++;
                            if (i >= tokens.Count || tokens[i].Type != TokenType.Identifier)
                                throw new ScripterParsingException("Expected an identifier after var");
                            var identifier = tokens[i].Value;
                            i++;
                            Expression right;
                            if (i < tokens.Count && tokens[i].Type == TokenType.Assignment)
                            {
                                i++;
                                right = ParseExpression(tokens, ref i);
                            }
                            else
                            {
                                right = new EmptyExpression();
                            }

                            return new DeclareExpression(identifier, right);
                        }
                        default:
                            throw new ScripterParsingException("Unexpected keyword: " + token.Value);
                    }
                }
                case TokenType.SemiColon:
                    i++;
                    return new EmptyExpression();
                default:
                    throw new ScripterParsingException("Unexpected token: " + token.Value);
            }
        }
    }
}
