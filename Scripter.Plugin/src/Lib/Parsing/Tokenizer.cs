using System;
using System.Collections.Generic;

namespace ScripterLang
{
    public class Tokenizer
    {
        public static IEnumerable<Token> Tokenize(string input)
        {
            return Tokenize(input.ToCharArray());
        }

        private static IEnumerable<Token> Tokenize(char[] input)
        {
            return new Tokenizer(input).Tokenize();
        }

        private readonly char[] _input;
        private readonly int _length;
        private int _line;
        private int _position;

        private Tokenizer(char[] input)
        {
            _input = input;
            _length = _input.Length;
            _line = 1;
            _position = 0;
        }
        private char Current => _input[_position];
        private char Peek(int c = 1) => _input[_position + c];
        private Location Location => new Location { line = _line };
        private bool IsAtEnd() => _position == _length;

        private bool MoveNext(int c = 1)
        {
            _position += c;
            return _position < _length;
        }

        private string Substr(int start, int length)
        {
            return new string(_input, start, length);
        }

        private IEnumerable<Token> Tokenize()
        {
            while (!IsAtEnd())
            {
                switch (Current)
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
                        var start = _position;
                        var numberType = TokenType.Integer;
                        while (MoveNext())
                        {
                            if (char.IsDigit(Current)) continue;
                            if (Current == '.')
                            {
                                numberType = TokenType.Float;
                                continue;
                            }
                            break;
                        }

                        if (Current == 'f')
                        {
                            MoveNext();
                            numberType = TokenType.Float;
                        }

                        yield return new Token(numberType, Substr(start, _position - start), Location);
                        break;
                    case '/':
                    {
                        var next = Peek();
                        if (next == '/')
                        {
                            MoveNext();
                            while (MoveNext() && Current != '\n')
                            {
                            }

                            break;
                        }

                        if (next == '*')
                        {
                            MoveNext();
                            while (MoveNext() && Current != '*' && Peek() != '/')
                            {
                            }

                            MoveNext(2);
                            break;
                        }

                        if (next == '=')
                        {
                            yield return new Token(TokenType.AssignmentOperator, "/=", Location);
                            MoveNext(2);
                            break;
                        }

                        yield return new Token(TokenType.Operator, "/", Location);
                        MoveNext();
                        break;
                    }
                    case '+':
                    case '-':
                    {
                        var c = Current;
                        var next = Peek();
                        if (c == next)
                        {
                            yield return new Token(TokenType.IncrementDecrement, Substr(_position, 2), Location);
                            MoveNext(2);
                            break;
                        }

                        if (next == '=')
                        {
                            yield return new Token(TokenType.AssignmentOperator, c + "=", Location);
                            MoveNext(2);
                            break;
                        }

                        yield return new Token(TokenType.Operator, Current.ToString(), Location);
                        MoveNext();
                        break;
                    }
                    case '*':
                        if (MoveNext() && Current == '=')
                        {
                            yield return new Token(TokenType.AssignmentOperator, "*=", Location);
                            MoveNext();
                        }
                        else
                        {
                            yield return new Token(TokenType.Operator, "*", Location);
                        }
                        break;
                    case '%':
                        yield return new Token(TokenType.Operator, Current.ToString(), Location);
                        MoveNext();
                        break;
                    case '^':
                        yield return new Token(TokenType.Operator, Current.ToString(), Location);
                        MoveNext();
                        break;
                    case '&':
                        if (MoveNext() && Current == '&')
                        {
                            yield return new Token(TokenType.Operator, "&&", Location);
                            MoveNext();
                        }
                        else
                        {
                            yield return new Token(TokenType.Operator, "&", Location);
                        }
                        break;
                    case '|':
                        if (MoveNext() && Current == '|')
                        {
                            yield return new Token(TokenType.Operator, "||", Location);
                            MoveNext();
                        }
                        else
                        {
                            yield return new Token(TokenType.Operator, "|", Location);
                        }
                        break;
                    case '<':
                        if (MoveNext() && Current == '=')
                        {
                            yield return new Token(TokenType.Operator, "<=", Location);
                            MoveNext();
                        }
                        else
                        {
                            yield return new Token(TokenType.Operator, "<", Location);
                        }

                        break;
                    case '>':
                        if (MoveNext() && Current == '=')
                        {
                            yield return new Token(TokenType.Operator, ">=", Location);
                            MoveNext();
                        }
                        else
                        {
                            yield return new Token(TokenType.Operator, ">", Location);
                        }

                        break;
                    case '=':
                        if (MoveNext() && Current == '=')
                        {
                            if (Peek() == '=') MoveNext();
                            yield return new Token(TokenType.Operator, "==", Location);
                            MoveNext();
                        }
                        else if (Current == '>')
                        {
                            yield return new Token(TokenType.Arrow, "=>", Location);
                            MoveNext();
                        }
                        else
                        {
                            yield return new Token(TokenType.Assignment, "=", Location);
                        }

                        break;
                    case '!':
                        if (MoveNext() && Current == '=')
                        {
                            if (Peek() == '=') MoveNext();
                            yield return new Token(TokenType.Operator, "!=", Location);
                            MoveNext();
                        }
                        else
                        {
                            yield return new Token(TokenType.Negation, "!", Location);
                        }

                        break;
                    case '"':
                    case '\'':
                        var end = Array.IndexOf(_input, Current, _position + 1);
                        if (end == -1)
                            throw new ScripterParsingException("Unterminated string");

                        yield return new Token(TokenType.String, Substr(_position + 1, end - _position - 1), Location);
                        _position = end + 1;
                        break;
                    case '(':
                        yield return new Token(TokenType.LeftParenthesis, "(", Location);
                        MoveNext();
                        break;
                    case ')':
                        yield return new Token(TokenType.RightParenthesis, ")", Location);
                        MoveNext();
                        break;
                    case ',':
                        yield return new Token(TokenType.Comma, ",", Location);
                        MoveNext();
                        break;
                    case ':':
                        yield return new Token(TokenType.Colon, ":", Location);
                        MoveNext();
                        break;
                    case ';':
                        yield return new Token(TokenType.SemiColon, ";", Location);
                        MoveNext();
                        break;
                    case '{':
                        yield return new Token(TokenType.LeftBrace, "{", Location);
                        MoveNext();
                        break;
                    case '}':
                        yield return new Token(TokenType.RightBrace, "}", Location);
                        MoveNext();
                        break;
                    case '[':
                        yield return new Token(TokenType.LeftBracket, "[", Location);
                        MoveNext();
                        break;
                    case ']':
                        yield return new Token(TokenType.RightBracket, "]", Location);
                        MoveNext();
                        break;
                    case '.':
                        yield return new Token(TokenType.Dot, ".", Location);
                        MoveNext();
                        break;
                    case '\n':
                        _line++;
                        MoveNext();
                        break;
                    case ' ':
                    case '\t':
                    case '\r':
                        MoveNext();
                        break;
                    default:
                        if (char.IsLetter(Current))
                        {
                            var nameStart = _position;
                            while (!IsAtEnd() && (char.IsLetterOrDigit(Current) || Current == '_'))
                            {
                                MoveNext();
                            }

                            var name = Substr(nameStart, _position - nameStart);
                            switch (name)
                            {
                                case "true":
                                case "false":
                                    yield return new Token(TokenType.Boolean, name, Location);
                                    break;
                                case "undefined":
                                case "null":
                                    yield return new Token(TokenType.Undefined, name, Location);
                                    break;
                                case "var":
                                case "let":
                                case "const":
                                case "for":
                                case "while":
                                case "break":
                                case "continue":
                                case "if":
                                case "else":
                                case "return":
                                case "throw":
                                case "function":
                                case "export":
                                case "import":
                                case "from":
                                case "try":
                                case "catch":
                                case "finally":
                                    yield return new Token(TokenType.Keyword, name, Location);
                                    break;
                                default:
                                    yield return new Token(TokenType.Identifier, name, Location);
                                    break;
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException("Unexpected character: " + Current);
                        }

                        break;
                }
            }
        }
    }
}
