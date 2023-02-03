﻿using System;
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
            _position = -1;
        }
        private char Current => _input[_position];
        private char Peek(int c = 1) => _input[_position + c];
        private Location Location => new Location { Line = _line };
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
            while (MoveNext())
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
                        var isFloat = false;
                        while (MoveNext())
                        {
                            if (char.IsDigit(Current)) continue;
                            if (Current == '.')
                            {
                                isFloat = true;
                                continue;
                            }
                            break;
                        }

                        yield return new Token(isFloat ? TokenType.Float : TokenType.Integer, Substr(start, _position - start), Location);
                        break;
                    case '/':
                        if (Peek() == '/')
                        {
                            MoveNext();
                            while (MoveNext() && Current != '\n')
                            {
                            }

                            break;
                        }

                        if (Peek() == '*')
                        {
                            MoveNext();
                            while (MoveNext() && Current != '*' && Peek() != '/')
                            {
                            }

                            MoveNext(2);
                            break;
                        }

                        yield return new Token(TokenType.Operator, Current.ToString(), Location);
                        MoveNext();
                        break;
                    case '+':
                    case '-':
                        if (Current == Peek())
                        {
                            yield return new Token(TokenType.IncrementDecrement, Substr(_position, 2), Location);
                            MoveNext(2);
                            break;
                        }

                        yield return new Token(TokenType.Operator, Current.ToString(), Location);
                        MoveNext();
                        break;
                    case '*':
                    case '%':
                    case '^':
                        yield return new Token(TokenType.Operator, Current.ToString(), Location);
                        MoveNext();
                        break;
                    case '&':
                        MoveNext();
                        if (IsAtEnd() && Current == '&')
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
                        MoveNext();
                        if (IsAtEnd() && Current == '|')
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
                        MoveNext();
                        if (IsAtEnd() && Current == '=')
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
                        MoveNext();
                        if (IsAtEnd() && Current == '=')
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
                        MoveNext();
                        if (IsAtEnd() && Current == '=')
                        {
                            yield return new Token(TokenType.Operator, "==", Location);
                            MoveNext();
                        }
                        else
                        {
                            yield return new Token(TokenType.Assignment, "=", Location);
                        }

                        break;
                    case '!':
                        MoveNext();
                        if (IsAtEnd() && Current == '=')
                        {
                            yield return new Token(TokenType.Operator, "!=", Location);
                            MoveNext();
                        }
                        else
                        {
                            yield return new Token(TokenType.Negation, "!", Location);
                        }

                        break;
                    case '"':
                        var end = Array.IndexOf(_input, '"', _position + 1);
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
                            while (IsAtEnd() && char.IsLetterOrDigit(Current))
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
                                case "var":
                                case "for":
                                case "if":
                                case "else":
                                case "return":
                                case "throw":
                                case "static":
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
