// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

namespace Language
{
    public class Ast_Procedure : Ast_Lambda
    {
        private Ast_Scope procScope = null;

        public Ast_Procedure(Token token) : base(token)
        {
            Type = AstType.Procedure;
        }

        public Ast_Procedure Clone(Token token)
        {
            Ast_Procedure proc = new Ast_Procedure(token);
            foreach (var instruction in Block)
            {
                proc.Block.Add(instruction);
            }
            foreach (var arg in Args)
            {
                proc.Args.Add(new Ast_Variable(arg.Token));
            }
            return proc;
        }

        public override string ToString()
        {
            return $"procedure {base.ToString()}";
        }

        public override dynamic Execute(Ast_Scope scope, Libraries libraries)
        {
            scope.Variables.Append(Token.Lexeme, this);
            return false;
        }

        public override dynamic ExecuteCall(Ast_Scope scope, Libraries libraries)
        {
            if (procScope == null)
            {
                procScope = scope.CreateChild($"{Token.Lexeme}");
                procScope.CanSearchUp = false;
            }

            foreach (var arg in Args)
            {
                procScope.Variables.Append(arg.Token.Lexeme, arg.Value.Value);
            }
            foreach (var instruction in Block)
            {
                instruction.Execute(procScope, libraries);
            }
            return false;
        }
    }
}
