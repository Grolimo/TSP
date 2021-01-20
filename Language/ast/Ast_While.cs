// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

namespace Language
{
    public class Ast_While : Ast_Base
    {
        public Ast_Expression Expression;

        public Ast_While(Token token) : base(token)
        {
            Type = AstType.While;
        }

        public override string ToString()
        {
            return $"while {Expression}";
        }

        public override dynamic Execute(Ast_Scope scope)
        {
            var child = scope.CreateChild("while.execute");
            child.IsLoop = true;
            child.InLoop = true;
            while (child.InLoop)
            {
                var value = Expression.Execute(scope);
                if (value == true || (value != null && value != false && value != 0))
                {
                    foreach (Ast_Base fi in Block)
                    {
                        var res = fi.Execute(child);
                        if (res == true || res is Ast_Terminate)
                        {
                            child.InLoop = false;
                            return true;
                        }
                    }
                }
                else
                {
                    child.InLoop = false;
                }
            }
            return false;
        }
    }
}
