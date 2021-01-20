// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System.Collections.Generic;

namespace Language
{
    public class Ast_Foreach : Ast_Base
    {
        public Ast_Base Container;
        public Ast_Foreach(Token token) : base(token)
        {
            Type = AstType.Foreach;
        }

        private dynamic GetContainerValue(Ast_Scope scope)
        {
            if (Container.Token.Type == TokenType.ConstantString)
            {
                return Container.Token.Lexeme;
            }
            if (Container.Type == AstType.Variable)
            {
                var variable = (Ast_Variable)Container;
                var v = scope.GetVariable(variable.Name);
                return v;
            }
            throw new RuntimeError(Container.Token, $"Invalid foreach container type {Container.Token.Type}");
        }

        private static bool IsValueDictionaryType(dynamic value)
        {
            return (value is Dictionary<string, KeyValuePair<string, dynamic>>);
        }

        public override dynamic Execute(Ast_Scope scope)
        {
            if (Block.Count == 0) return false; // Gatekeeper, don't run if there are no instructions in the block.
            dynamic containerValue = GetContainerValue(scope);
            Ast_Scope childScope = scope.CreateChild("foreach");
            childScope.InLoop = true;
            childScope.IsLoop = true;
            Ast_Variable it = new Ast_Variable(null) { Name = "it" };
            childScope.Variables.Add(it);
            if (containerValue is string)
            {
                ExecuteForeachLoop(containerValue, childScope, it);
            }
            else if (containerValue.Type == AstType.Variable && containerValue.Value.Type == ValueType.Record)
            {
                ExecuteForeachLoop(containerValue.Value.Value.Keys, childScope, it);
            }
            else if (containerValue.Type == AstType.Variable && containerValue.Value.Type == ValueType.Array)
            {
                ExecuteForeachLoop(containerValue.Value.Value, childScope, it);
            }
            else if (containerValue.Type == AstType.Variable && containerValue.Value.Type == ValueType.String)
            {
                ExecuteForeachLoop(containerValue.Value.Value, childScope, it);
            }
            else
            {
                throw new RuntimeError(Token, "Invalid container value type");
            }
            childScope.InLoop = false;
            childScope.Variables.Remove(it);
            return false;
        }
        private void ExecuteForeachLoop(dynamic container, Ast_Scope currentScope, Ast_Variable it)
        {
            foreach (var fo in container)
            {
                if (fo is VT_Any)
                {
                    it.DoSetValue(fo.Value);
                }
                else
                {
                    it.DoSetValue(fo);
                }
                foreach (var fi in Block)
                {
                    var res = fi.Execute(currentScope);
                    if (res is Ast_Terminate || res == true)
                    {
                        break;
                    }
                }
            }
        }
    }
}
