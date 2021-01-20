// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

namespace Language
{
    public class Ast_Halt : Ast_Base
    {
        public Ast_Expression Exitcode;

        public Ast_Halt(Token token) : base(token)
        {
            Type = AstType.Halt;
        }

        public override string ToString()
        {
            return $"halt {Exitcode}";
        }

        public override dynamic Execute(Ast_Scope scope)
        {
            var value = Exitcode.Execute(scope);
            var estr = $"Program terminated at {Token.Line}, {Token.Offset}. Exit code: {value}.";
            var v = scope.Variables["exitcode"];
            if (v == null)
            {
                v = new Ast_Variable(null) { Name = "exitcode" };
                scope.Variables.Add(v);
            }
            v.DoSetValue(estr);
            return Ast_Terminate.Halt;
        }

    }
}
