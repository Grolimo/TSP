// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System.Collections.Generic;

namespace Language
{
    public class Libraries
    {
        private readonly List<Library> Items = new List<Library>();

        public Library GetLibWithMethodOrFunction(string name)
        {
            foreach (var lib in Items)
            {
                if (lib.Contains(name))
                {
                    return lib;
                }
            }
            return null;
        }

        public Ast_Lambda GetMethodOrFunction(string name)
        {
            var lib = GetLibWithMethodOrFunction(name);
            return lib?.GetFunctionOrMethod(name);
        }

        public void Add(Library lib)
        {
            Items.Add(lib);
        }

    }
}
