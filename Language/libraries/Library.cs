// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System.Collections.Generic;
using System.Linq;

namespace Language
{
    public interface ILibrary
    {
        bool Contains(string name);
        void Add(Ast_Lambda lambda);
        dynamic Execute(Ast_Scope scope, Libraries libraries);
    }

    public class Library : ILibrary
    {
        public string Name;
        protected readonly List<Ast_Lambda> Items = new List<Ast_Lambda>();
        public bool Contains(string name) => Items.Any(i => i.Token.Lexeme == name);
        public Ast_Lambda GetFunctionOrMethod(string name) => Items.FirstOrDefault(i => i.Token.Lexeme == name);

        public void Add(Ast_Lambda lambda)
        {
            Items.Add(lambda);
        }

        public virtual dynamic Execute(Ast_Scope scope, Libraries libraries)
        {
            throw new RuntimeError(Token.ErrorToken, "Execute function not implemented.");
        }

    }
}
