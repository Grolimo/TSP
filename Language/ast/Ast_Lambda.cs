// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System;
using System.Text;

namespace Language
{
    public class Ast_Lambda : Ast_Base
    {
        public Ast_Arguments Args = new Ast_Arguments();
        public Ast_Lambda(Token token) : base(token)
        {
            Type = AstType.Lambda;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append('(');
            foreach (var arg in Args)
            {
                sb.Append($"{arg}, ");
            }
            if (sb.Length > 2 && Args.Count > 0)
            {
                sb.Remove(sb.Length - 2, 2);
            }
            sb.Append(')');
            return sb.ToString();
        }

        public override dynamic Execute(Ast_Scope scope)
        {
            throw new NotImplementedException();
        }

        public virtual dynamic ExecuteCall(Ast_Scope scope)
        {
            throw new NotImplementedException();
        }
    }
}
