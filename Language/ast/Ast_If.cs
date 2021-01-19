// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

namespace Language
{
    public class Ast_If : Ast_Base
    {
        public Ast_Expression Expression;
        public Ast_ElIf ElIf;
        public Ast_Else Else;

        public Ast_If(Token token) : base(token)
        {
            Type = AstType.If;
        }

        public override string ToString()
        {
            return $"if {Expression}";
        }

        public dynamic ExecuteIf(Ast_Scope scope, Libraries libraries)
        {
            var child = scope.CreateChild("if.executeif");
            dynamic res = false;
            foreach (Ast_Base fi in Block)
            {
                res = fi.Execute(child, libraries);
                if (res is Ast_Terminate)
                {
                    return true;
                }
            }
            return res;
        }

        public bool ExecuteElifBlock(Ast_Scope scope, Libraries libraries)
        {
            if (ElIf == null) return false;
            foreach (Ast_Base elif in ElIf.Block)
            {
                if (elif.Execute(scope, libraries))
                    return true;
            }
            return false;
        }

        public override dynamic Execute(Ast_Scope scope, Libraries libraries)
        {
            var value = Expression.Execute(scope, libraries);
            if (value == true || (value != null && value != false && value != 0))
            {
                return ExecuteIf(scope, libraries);

            }
            if (ElIf?.Block.Count > 0 && ExecuteElifBlock(scope, libraries))
            {
                return false;
            }
            if (Else?.Block.Count > 0)
            {
                Else.Execute(scope, libraries);
            }
            return false;
        }
    }
}
