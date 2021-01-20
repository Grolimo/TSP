// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

namespace Language
{
    public class Ast_ElIf : Ast_Base
    {
        public Ast_Expression Expression;

        public Ast_ElIf(Token token) : base(token)
        {
            Type = AstType.Elif;
        }

        public override string ToString()
        {
            return $"elif {Expression}";
        }

        public dynamic ExecuteElif(Ast_Scope scope)
        {
            var child = scope.CreateChild($"{scope.Name}->ExecuteElIf");
            dynamic res = false;
            foreach (Ast_Base fi in Block)
            {
                res = fi.Execute(child);
                if (res is Ast_Terminate || res == true)
                {
                    return true;
                }
            }
            return res;
        }

        public override dynamic Execute(Ast_Scope scope)
        {
            var value = Expression.Execute(scope);
            if (value == true || (value != null && value != false && value != 0))
            {
                return ExecuteElif(scope);
            }
            return false;
        }
    }
}
