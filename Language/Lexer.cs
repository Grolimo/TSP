// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using static Language.TokenType;

namespace Language
{
    public class Lexer
    {
        private static void SkipWhitespace(LexerState state)
        {
            while (!state.EOF && char.IsWhiteSpace(state.Current))
            {
                var chr = state.Current;
                _ = state.Next();
                if (chr == '\n')
                {
                    state.NewLine();
                }
            }
        }

        private static Token GetIdentifier(LexerState state)
        {
            var start = state.Index;
            var result = new Token(state, Identifier);

            if (char.IsLetter(state.Current))
            {
                _ = state.Next();
            }
            else
            {
                throw new TokenError(result, $"Invalid starting character for identifier: {state.Current}.");
            }
            if (state.EOF)
            {
                throw new TokenError(result, "Unexpected end of file.");
            }
            while (char.IsLetterOrDigit(state.Current) || state.Current == '_')
            {
                _ = state.Next();
                if (state.EOF)
                {
                    throw new TokenError(result, "Unexpected end of file.");
                }
            }

            result.Lexeme = state.Source[start..state.Index];
            if (result.Lexeme == "true" || result.Lexeme == "false")
            {
                result.Lexeme = result.Lexeme == "true";
                result.Type = TokenType.ConstantBool;
            }
            return result;
        }

        private static Token GetNumber(LexerState state)
        {
            var result = new Token(state, ConstantNumber);
            var value = 0;
            var chr = state.Current;

            while (!state.EOF && char.IsDigit(chr))
            {
                value = (value * 10) + (byte)chr - 48;
                chr = state.Next();
                if (state.EOF)
                {
                    throw new TokenError(result, "Unexpected end of file.");
                }
            }
            var mulf = 0.1;
            double dvalue = value;
            if (chr == '.')
            {
                chr = state.Next();
                if (!char.IsDigit(chr))
                {
                    throw new TokenError(result, "Unexpected trailing decimal point in number.");
                }
                while (!state.EOF && char.IsDigit(chr))
                {
                    dvalue += (((byte)chr - 48) * mulf);
                    mulf /= 10;
                    chr = state.Next();
                }
                if (chr == '.')
                {
                    throw new TokenError(result, "Unexpected trailing decimal point in number.");
                }
            }
            if (mulf < 0.1)
            {
                result.Lexeme = dvalue;
            }
            else
            {
                result.Lexeme = value;
            }

            return result;
        }

        private static Token GetString(LexerState state)
        {
            var result = new Token(state, ConstantString);
            var terminator = state.Current;
            state.Next();
            var start = state.Index;

            var chr = state.Current;
            while (!state.EOF && chr != terminator)
            {
                if (chr == '\n')
                {
                    chr = state.Next();
                    state.NewLine();
                    continue;
                }
                if (chr == '\t')
                {
                    state.Offset += 3;
                    chr = state.Next();
                    continue;
                }
                chr = state.Next();
            }
            result.Lexeme = state.Source[start..state.Index];
            if (state.Current != terminator)
            {
                throw new TokenError(result, $"Unexpected character found: {state.Current}");
            }
            _ = state.Next();
            return result;
        }

        private static Token GetShortComment(LexerState state)
        {
            var result = new Token(state, Comment);
            var start = state.Index;
            while (!state.EOF && state.Current != '\n')
            {
                _ = state.Next();
            }
            result.Lexeme = state.Source.Substring(start, state.Index - start - 1);
            if (!state.EOF)
            {
                state.Next();
                state.NewLine();
            }
            return result;
        }

        private static Token GetShortToken(LexerState state, TokenType type)
        {
            var result = new Token(state, type)
            {
                Lexeme = state.Current.ToString()
            };
            _ = state.Next();
            return result;
        }

        private static Token GetDToken(LexerState state, TokenType type)
        {
            var result = new Token(state, type);
            var start = state.Index;
            _ = state.Next();
            _ = state.Next();
            result.Lexeme = state.Source.Substring(start, 2);
            return result;
        }

        public static Token GetToken(LexerState state)
        {
            if (state.EOF)
            {
                return Token.EOFToken;
            }
            SkipWhitespace(state);
            if (state.EOF)
            {
                return Token.EOFToken;
            }

            if (char.IsLetter(state.Current))
            {
                return GetIdentifier(state);
            }
            else if (char.IsDigit(state.Current))
            {
                return GetNumber(state);
            }
            else if (state.Current == '\'' || state.Current == '"')
            {
                return GetString(state);
            }
            else if (state.Current == '\\' && state.Peek() == '\\')
            {
                return GetShortComment(state);
            }
            else if (state.Current == ',')
            {
                return GetShortToken(state, Comma);
            }
            else if (state.Current == ';')
            {
                return GetShortToken(state, Semicolon);
            }
            else if (state.Current == '{')
            {
                return GetShortToken(state, BlockLeft);
            }
            else if (state.Current == '}')
            {
                return GetShortToken(state, BlockRight);
            }
            else if (state.Current == '[')
            {
                return GetShortToken(state, IndexLeft);
            }
            else if (state.Current == ']')
            {
                return GetShortToken(state, IndexRight);
            }
            else if (state.Current == '$')
            {
                return GetShortToken(state, FormatString);
            }
            else if (state.Current == '&')
            {
                return GetShortToken(state, Ref);
            }
            else if (state.Current == '(')
            {
                return GetShortToken(state, BracketLeft);
            }
            else if (state.Current == ')')
            {
                return GetShortToken(state, BracketRight);
            }
            else if (state.Current == '^')
            {
                return GetShortToken(state, OpPower);
            }
            else if (state.Current == ':')
            {
                if (state.Peek() == ':')
                {
                    return GetDToken(state, OpAssignLambda);
                }
                else
                {
                    return GetShortToken(state, OpAssignReturnVar);
                }
            }
            else if (state.Current == '*')
            {
                if (state.Peek() == '=')
                {
                    return GetDToken(state, OpAssignMultiply);
                }
                else
                {
                    return GetShortToken(state, OpMultiply);
                }
            }
            else if (state.Current == '/')
            {
                if (state.Peek() == '=')
                {
                    return GetDToken(state, OpAssignDivide);
                }
                else
                {
                    return GetShortToken(state, OpDivide);
                }
            }
            else if (state.Current == '+')
            {
                if (state.Peek() == '=')
                {
                    return GetDToken(state, OpAssignAdd);
                }
                else
                {
                    return GetShortToken(state, OpAdd);
                }
            }
            else if (state.Current == '-')
            {
                if (state.Peek() == '=')
                {
                    return GetDToken(state, OpAssignSubtract);
                }
                else
                {
                    return GetShortToken(state, OpSubtract);
                }
            }
            else if (state.Current == '=')
            {
                if (state.Peek() == '=')
                {
                    return GetDToken(state, OpEqual);
                }
                else
                {
                    return GetShortToken(state, OpAssign);
                }
            }
            else if (state.Current == '>')
            {
                if (state.Peek() == '=')
                {
                    return GetDToken(state, OpGTE);
                }
                else
                {
                    return GetShortToken(state, OpGT);
                }
            }
            else if (state.Current == '<')
            {
                if (state.Peek() == '=')
                {
                    return GetDToken(state, OpLTE);
                }
                else
                {
                    return GetShortToken(state, OpLT);
                }
            }

            else if (state.Current == '!')
            {
                if (state.Peek() == '=')
                {
                    return GetDToken(state, OpNE);
                }
                else
                {
                    return GetShortToken(state, OpNot);
                }
            }
            else
            {
                return GetShortToken(state, Unknown);
            }
        }

        public static Token PeekToken(LexerState state)
        {
            var peekState = state.Clone();
            return GetToken(peekState);
        }
    }
}
