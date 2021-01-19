// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System.Text;

namespace Language
{

    public enum CallType { Function, Procedure, Lambda };
    public class Ast_Call : Ast_Base
    {
        public CallType CallType;
        public string Name
        {
            get
            {
                return Token?.Lexeme;
            }
        }
        public Ast_Call(Token token) : base(token)
        {
            Type = AstType.Call;
        }
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"call {Name}(");
            if (Block?.Count > 0)
            {
                foreach (Ast_Base arg in Block)
                {
                    sb.Append(arg.ToString().Trim());
                    sb.Append(',');
                }
                if (sb.Length > 1)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
            }
            sb.Append(')');
            return sb.ToString();
        }

        public override dynamic Execute(Ast_Scope scope, Libraries libraries)
        {
            Ast_Lambda exec;
            if (scope.VariableExists(Name))
            {
                exec = scope.GetVariable(Name).Value.Value;
            }
            else
            {
                exec = libraries.GetMethodOrFunction(Name);
                exec.Execute(scope, libraries); // run the prepwork.
            }
            if (exec == null)
            {
                throw new SyntaxError(Token, $"Function or procedure not found ({Name}).");
            }

            if (Block?.Count != exec.Args.Count)
            {
                if (!exec.Args.ContainsParams())
                {
                    throw new SyntaxError(Token, "Invalid number of parameters.");
                }
            }

            var i = 0;
            foreach (Ast_Expression expr in Block)
            {
                var expressionValue = expr.Execute(scope, libraries);

                if (exec.Args[i].Value.Type == ValueType.Params)
                {
                    exec.Args[i].Value.Value.Add
                        (new VT_Any { Value = expressionValue });
                }
                else
                {
                    exec.Args[i].DoSetValue(expressionValue);
                    i++;
                }
            }

            if (exec.Type == AstType.Function || exec.Type == AstType.Procedure)
            {
                return exec.ExecuteCall(scope, libraries);
            }
            return exec.Execute(scope, libraries);
        }
    }
}
