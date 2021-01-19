// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

namespace Language
{
    public class Ast_Break : Ast_Base
    {
        public Ast_Break(Token token) : base(token)
        {
            Type = AstType.Break;
        }

        public override dynamic Execute(Ast_Scope scope, Libraries libraries)
        {
            if (!scope.IsLoop)
            {
                var lscope = Ast_Scope.GetIteratorScope(scope);
                if (lscope == null)
                {
                    throw new RuntimeError(Token, "Break used outside loop.");
                }
            }
            return Ast_Terminate.Break;
        }
    }
}
