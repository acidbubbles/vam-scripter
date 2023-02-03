using System;
using System.Collections.Generic;

namespace ScripterLang
{
    public static class Tokenizer
    {
        public static IEnumerable<Token> Tokenize(string input)
        {
            return Tokenize(input.ToCharArray());
        }

        private static IEnumerable<Token> Tokenize(char[] input)
        {
            var length = input.Length;
            var position = 0;
            var line = 1;
            while (position < length)
            {
                switch (input[position])
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        var start = position;
                        var isFloat = false;
                        while (++position < length)
                        {
                            if (char.IsDigit(input[position])) continue;
                            if (input[position] == '.')
                            {
                                isFloat = true;
                                continue;
                            }
                            break;
                        }

                        yield return new Token(isFloat ? TokenType.Float : TokenType.Integer, new string(input, start, position - start), line);
                        break;
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                    case '%':
                    case '^':
                        if (input[position] == '/')
                        {
                            if (input[position + 1] == '/')
                            {
                                position++;
                                while (++position < length && input[position] != '\n')
                                {
                                }
                                break;
                            }

                            if (input[position + 1] == '*')
                            {
                                position++;
                                while (++position < length && input[position] != '*' && input[position + 1] != '/')
                                {
                                }
                                position += 2;
                                break;
                            }
                        }
                        yield return new Token(TokenType.Operator, input[position].ToString(), line);
                        position++;
                        break;
                    case '&':
                        position++;
                        if (position < length && input[position] == '&')
                        {
                            yield return new Token(TokenType.Operator, "&&", line);
                            position++;
                        }
                        else
                        {
                            yield return new Token(TokenType.Operator, "&", line);
                        }
                        break;
                    case '|':
                        position++;
                        if (position < length && input[position] == '|')
                        {
                            yield return new Token(TokenType.Operator, "||", line);
                            position++;
                        }
                        else
                        {
                            yield return new Token(TokenType.Operator, "|", line);
                        }
                        break;
                    case '<':
                        position++;
                        if (position < length && input[position] == '=')
                        {
                            yield return new Token(TokenType.Operator, "<=", line);
                            position++;
                        }
                        else
                        {
                            yield return new Token(TokenType.Operator, "<", line);
                        }

                        break;
                    case '>':
                        position++;
                        if (position < length && input[position] == '=')
                        {
                            yield return new Token(TokenType.Operator, ">=", line);
                            position++;
                        }
                        else
                        {
                            yield return new Token(TokenType.Operator, ">", line);
                        }

                        break;
                    case '=':
                        position++;
                        if (position < length && input[position] == '=')
                        {
                            yield return new Token(TokenType.Operator, "==", line);
                            position++;
                        }
                        else
                        {
                            yield return new Token(TokenType.Assignment, "=", line);
                        }

                        break;
                    case '!':
                        position++;
                        if (position < length && input[position] == '=')
                        {
                            yield return new Token(TokenType.Operator, "!=", line);
                            position++;
                        }
                        else
                        {
                            yield return new Token(TokenType.Negation, "!", line);
                        }

                        break;
                    case '"':
                        var end = Array.IndexOf(input, '"', position + 1);
                        if (end == -1)
                        {
                            throw new InvalidOperationException("Unterminated string");
                        }

                        yield return new Token(TokenType.String, new string(input, position + 1, end - position - 1), line);
                        position = end + 1;
                        break;
                    case '(':
                        yield return new Token(TokenType.LeftParenthesis, "(", line);
                        position++;
                        break;
                    case ')':
                        yield return new Token(TokenType.RightParenthesis, ")", line);
                        position++;
                        break;
                    case ',':
                        yield return new Token(TokenType.Comma, ",", line);
                        position++;
                        break;
                    case ';':
                        yield return new Token(TokenType.SemiColon, ";", line);
                        position++;
                        break;
                    case '{':
                        yield return new Token(TokenType.LeftBrace, "{", line);
                        position++;
                        break;
                    case '}':
                        yield return new Token(TokenType.RightBrace, "}", line);
                        position++;
                        break;
                    case '\n':
                        line++;
                        position++;
                        break;
                    case ' ':
                    case '\t':
                    case '\r':
                        position++;
                        break;
                    default:
                        if (char.IsLetter(input[position]))
                        {
                            var nameStart = position;
                            while (position < length && char.IsLetterOrDigit(input[position]))
                            {
                                position++;
                            }

                            var name = new string(input, nameStart, position - nameStart);
                            switch (name)
                            {
                                case "true":
                                case "false":
                                    yield return new Token(TokenType.Boolean, name, line);
                                    break;
                                case "var":
                                case "for":
                                case "if":
                                case "else":
                                case "return":
                                case "throw":
                                    yield return new Token(TokenType.Keyword, name, line);
                                    break;
                                default:
                                    yield return new Token(TokenType.Identifier, name, line);
                                    break;
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException("Unexpected character: " + input[position]);
                        }

                        break;
                }
            }
        }
    }
}
