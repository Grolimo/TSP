// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System.Collections.Generic;

namespace Language
{
    public class Ast_Application : IAst
    {
        public List<Ast_Base> Block { get; set; } = new List<Ast_Base>();
        public Ast_Scope Scope = new Ast_Scope("Application Scope", null);
        public Libraries Libraries;

        public void Execute()
        {
            foreach (var instruction in Block)
            {
                var result = instruction.Execute(Scope, Libraries);
                if (result is Ast_Terminate)
                {
                    break;
                }
            }
        }
    }
}
