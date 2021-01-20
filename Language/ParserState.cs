// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

namespace Language
{
    public class ParserState
    {
        public Ast_Scope Scope = new Ast_Scope("root", null);
        private readonly LexerState State;
        public bool EOF => State.EOF;


        public ParserState(string source)
        {
            State = new LexerState(source);
        }

        public void Reset()
        {
            State.Reset();
            Scope.Clear();
        }

        public Token GetToken()
        {
            return Lexer.GetToken(State);
        }

        public Token PeekToken()
        {
            return Lexer.PeekToken(State);
        }
    }
}
