// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System;

namespace Language
{
    public class Ast_Op : Ast_Base
    {
        public Ast_Op(Token token) : base(token)
        {
            Type = AstType.Operand;
        }

        public override dynamic Execute(Ast_Scope scope)
        {
            throw new NotImplementedException();
        }
    }
}
