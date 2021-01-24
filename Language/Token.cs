// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using static Language.TokenType;

namespace Language
{
    public class Token
    {
        public int Line;
        public int Offset;
        public int Index;
        public dynamic Lexeme;
        public TokenType Type;

        public Token(int line = -1, int offset = -1, int index = -1)
        {
            Line = line;
            Offset = offset;
            Index = index;
        }

        public Token(LexerState state, TokenType type = Unknown)
        {
            Line = state.Line;
            Offset = state.Offset;
            Index = state.Index;
            Type = type;
        }

        public Token Clone()
        {
            return new Token()
            {
                Line = Line,
                Offset = Offset,
                Index = Index,
                Type = Type,
                Lexeme = Lexeme
            };
        }

        public static Token EOFToken => new Token
        {
            Type = EOF
        };

        public static Token ErrorToken => new Token
        {
            Type = Error,
            Lexeme = "error"
        };

        public override string ToString()
        {
            var output = $"Token Type: {Type,-20}, ({Line,5},{Offset,4})@({Index,8}) {Lexeme}";
            return output;
        }
    }
}
