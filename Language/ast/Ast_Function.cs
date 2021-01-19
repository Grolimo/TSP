// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

namespace Language
{
    public class Ast_Function : Ast_Lambda
    {
        public Ast_Variable ReturnVariable;

        private Ast_Scope funcScope = null;

        public Ast_Function(Token token) : base(token)
        {
            Type = AstType.Function;
        }

        public Ast_Function Clone(Token token)
        {
            Ast_Function func = new Ast_Function(token);
            foreach (var instruction in Block)
            {
                func.Block.Add(instruction);
            }
            foreach (var arg in Args)
            {
                func.Args.Add(new Ast_Variable(arg.Token));
            }
            func.ReturnVariable = new Ast_Variable(ReturnVariable.Token);
            return func;
        }
        public override string ToString()
        {
            return $"function {base.ToString()}";
        }

        public override dynamic Execute(Ast_Scope scope, Libraries libraries)
        {
            scope.Variables.Append(Token.Lexeme, this);
            return false;
        }

        public override dynamic ExecuteCall(Ast_Scope scope, Libraries libraries)
        {
            if (funcScope == null)
            {
                funcScope = scope.CreateChild($"{Token.Lexeme}");
                funcScope.CanSearchUp = false;
            }
            foreach (var arg in Args)
            {
                funcScope.Variables.Append(arg.Token.Lexeme, arg.Value.Value);
            }
            funcScope.Variables.Append(ReturnVariable.Token.Lexeme, null);
            foreach (var instruction in Block)
            {
                instruction.Execute(funcScope, libraries);
            }
            return funcScope.Variables[ReturnVariable.Token.Lexeme].DoGetValue();
        }
    }
}
