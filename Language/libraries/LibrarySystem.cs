// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System;
using System.Collections;
using System.Text;

namespace Language
{
    public class LibrarySystem : Library
    {
        public LibrarySystem()
        {
            Ast_Procedure proc = new Ast_Console(new Token { Lexeme = "console" });
            Add(proc);

            Ast_Function func = new Ast_Length(new Token { Lexeme = "length" });
            Add(func);
        }

        public class Ast_Console : Ast_Procedure
        {
            public Ast_Console(Token token) : base(token)
            {
                var variable = new Ast_Variable(new Token { Lexeme = "args" });
                variable.DoSetValue(Ast_Variable.NewParamsValue);
                Args.Add(variable);
            }

            public override dynamic Execute(Ast_Scope scope, Libraries libraries)
            {
                Args[0]?.Value?.Value?.Clear();
                return false;
            }

            public override dynamic ExecuteCall(Ast_Scope scope, Libraries libraries)
            {
                var argLst = Args[0].Value.Value;
                var sb = new StringBuilder();
                foreach (var v in argLst)
                {
                    if (v is Ast_Variable)
                    {
                        Ast_Variable variable = v as Ast_Variable;
                        // ToDo: Perhaps verify if i need to get the var from the scope to actually check!
                        dynamic value;
                        if (variable.Index != null)
                        {
                            value = variable.DoGetValue(variable.Index, scope, libraries);
                        }
                        else
                        {
                            value = variable.DoGetValue();
                        }
                        sb.Append(value.ToString());
                    }
                    else
                    {
                        sb.Append(v.Value);
                    }
                    sb.Append(' ');
                }
                if (sb.Length > 1)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                Console.WriteLine(sb.ToString());
                return false;
            }
        }

        public class Ast_Length : Ast_Function
        {
            public Ast_Length(Token token) : base(token)
            {
                Args.Add(new Ast_Variable(new Token { Lexeme = "arg" }));
            }

            public override dynamic Execute(Ast_Scope scope, Libraries libraries)
            {
                return false;
            }

            public override dynamic ExecuteCall(Ast_Scope scope, Libraries libraries)
            {
                if (Args?.Count != 1)
                {
                    throw new RuntimeError(Token, "Length requires 1 iterable variable as argument.");
                }
                var value = Args[0].DoGetValue();
                if (value is string)
                {
                    return value.Length;
                }
                else if (value is IList || value is ICollection)
                {
                    return value.Count;
                }
                else
                {
                    throw new RuntimeError(Token, $"Invalid function call: Length argument is invalid type ({value.GetType()}).");
                }
            }
        }

    }
}
