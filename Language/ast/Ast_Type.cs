// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System;

namespace Language
{
    public class Ast_Type : Ast_Base
    {
        public Ast_Type(Token token) : base(token)
        {
            Type = AstType.Type;
        }

        public override dynamic Execute(Ast_Scope scope, Libraries libraries)
        {
            throw new NotImplementedException();
        }
    }
}
