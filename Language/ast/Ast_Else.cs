// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

namespace Language
{
    public class Ast_Else : Ast_Base
    {
        public Ast_Else(Token token) : base(token)
        {
            Type = AstType.Else;
        }

        public override dynamic Execute(Ast_Scope scope, Libraries libraries)
        {
            foreach (Ast_Base fi in Block)
            {
                var res = fi.Execute(scope, libraries);
                if (res is Ast_Terminate || res == true)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
