// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System.Collections.Generic;

namespace Language
{
    public class Ast_Index : Ast_Base
    {
        public Ast_Index(Token token) : base(token)
        {
            Type = AstType.Index;
        }

        public override dynamic Execute(Ast_Scope scope)
        {
            var res = new List<dynamic>();
            foreach (var expr in Block)
            {
                Ast_Expression expression = expr as Ast_Expression;
                res.Add(expression.Execute(scope));
            }
            return res;
        }
    }
}
