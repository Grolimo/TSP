// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

namespace Language
{
    public class Ast_Block : Ast_Base
    {
        public Ast_Block(Token token) : base(token)
        {
            Type = AstType.Block;
        }

        public override dynamic Execute(Ast_Scope scope)
        {
            Ast_Scope blockScope = scope.CreateChild("block");
            dynamic result = false;
            foreach (var instruction in Block)
            {
                result = instruction.Execute(blockScope);
                if (result is Ast_Terminate)
                {
                    break;
                }
            }
            return result;
        }
    }
}
