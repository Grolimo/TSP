// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System.Collections.Generic;

namespace Language
{
    public class Ast_Struct : Ast_Base
    {
        public Ast_Scope StructScope;
        public string Name { get => Token.Lexeme; }
        public string StructType { get; private set; }

        public Ast_Struct(Token token) : base(token)
        {
            Type = AstType.Struct;
            StructType = token.Lexeme;
            StructScope = new Ast_Scope(token.Lexeme, null)
            {
                CanSearchUp = false
            };
        }

        public Ast_Struct Clone(Token token)
        {
            var str = new Ast_Struct(token)
            {
                StructType = Name,
                StructScope = StructScope.Parent.CreateChild(token.Lexeme)
            };
            StructScope.CanSearchUp = false;
            foreach (var item in StructScope?.Variables)
            {
                if (item.Value.Type is ValueType.Function or ValueType.Procedure)
                {
                    str.StructScope.Variables.Add(item);
                }
                else if (item.Type == AstType.Variable)
                {
                    var value = new VT_Any();
                    if (item.Value.Type == ValueType.Array)
                    {
                        value.Value = new List<VT_Any>();
                        value.Type = ValueType.Array;
                    }
                    else if (item.Value.Type == ValueType.Record)
                    {
                        value.Value = new Dictionary<string, VT_Any>();
                        value.Type = ValueType.Record;
                    }
                    else if (item.Value.Type == ValueType.Params)
                    {
                        value.Value = new List<VT_Any>();
                        value.Type = ValueType.Params;
                    }
                    else
                    {
                        value.Value = item.Value.Value;
                        value.Type = item.Value.Type;
                    }
                    _ = str.StructScope.Variables.Append(item.Name, value.Value);
                }
                else
                {
                    throw new RuntimeError(token, $"Unknown type discovered: {item.Type}");
                }
            }
            return str;
        }

        public override string ToString()
        {
            return $"struct {Name}({StructType})";
        }

        public override dynamic Execute(Ast_Scope scope)
        {
            Ast_Variable structVar = new Ast_Variable(Token);
            structVar.DoSetValue(this);
            scope.Variables.Add(structVar);
            StructScope = scope.CreateChild(Name);
            StructScope.CanSearchUp = false;
            foreach (var ast in Block)
            {
                if (ast.Type == AstType.Variable)
                {
                    StructScope.Variables.Add(ast as Ast_Variable);
                }
                else if (ast.Type == AstType.Assign || ast.Type == AstType.Function || ast.Type == AstType.Procedure)
                {
                    ast.Execute(StructScope);
                }
                else
                {
                    throw new RuntimeError(ast.Token, $"Invalid struct item type: {ast.Type}.");
                }
            }
            return false;
        }
    }
}
