// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

namespace Language
{
    public class Interpreter
    {
        private Libraries libs;
        private Parser Parser;
        private ParserState State;

        public Ast_Scope ParserScope => State?.Scope;
        public Ast_Application Application => Parser?.Root;

        public void Execute(string sourceCode)
        {
            libs = new Libraries();
            libs.Add(new LibrarySystem());

            Parser = new Parser(libs);
            State = new ParserState(libs, sourceCode);

            Parser.GetAst(Parser.Root, State);
            Parser.Root.Execute();
        }
    }
}
