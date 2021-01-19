// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System.Collections.Generic;

namespace Language
{
    public abstract class Ast_Base : IAst
    {
        public Token Token;
        public List<Ast_Base> Block { get; set; } = new List<Ast_Base>();
        public AstType Type;

        public Ast_Base(Token token)
        {
            Token = token;
            Type = AstType.Base;
        }
        public override string ToString()
        {
            if (Token != null)
            {
                return Token.Lexeme.ToString();
            }
            return string.Empty;
        }

        public abstract dynamic Execute(Ast_Scope scope, Libraries libraries);

    }
}
