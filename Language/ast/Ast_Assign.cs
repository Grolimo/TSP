// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System.Collections.Generic;
using System.Text;

namespace Language
{
    public class Ast_Assign : Ast_Base
    {
        public Ast_Assign(Token token) : base(token)
        {
            Type = AstType.Assign;
        }

        public Ast_Variable Variable;
        public Token Operand;
        public Ast_Expression Expression;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Variable.Name);
            if (Variable.Index != null)
            {
                if (Variable.Index.Block.Count == 1)
                {
                    sb.Append("[]");
                }
                else
                {
                    foreach (var expr in Variable.Index.Block)
                    {
                        sb.Append('[');
                        sb.Append(expr);
                        sb.Append(']');
                    }
                }
            }
            sb.Append(Operand.Lexeme.ToString());
            sb.Append(Expression);
            sb.Append(';');
            return sb.ToString();
        }

        private static List<VT_Any> CloneArray(List<VT_Any> value)
        {
            var result = new List<VT_Any>();
            foreach(var v in value)
            {
                result.Add(new VT_Any() { Type = v.Type, Value = v.Value });
            }
            return result;
        }

        private static Dictionary<string, VT_Any> CloneRecord(Dictionary<string, VT_Any> value)
        {
            var result = new Dictionary<string, VT_Any>();
            foreach(var i in value.Keys)
            {
                result.Add(i, new VT_Any() { Type = value[i].Type, Value = value[i].Value } );
            }
            return result;
        }

        public override dynamic Execute(Ast_Scope scope)
        {
            dynamic value = Expression.Execute(scope);
            if (value is Ast_Procedure procedure)
            {
                value = procedure.Clone(Variable.Token);
            }
            else if (value is Ast_Function function)
            {
                value = function.Clone(Variable.Token);
            }
            else if (value is Ast_Struct strct)
            {
                value = strct.Clone(Variable.Token);
            }
            else if (value is List<VT_Any>)
            {
                value = CloneArray(value);
            }
            else if (value is Dictionary<string, VT_Any>)
            {
                value = CloneRecord(value);
            }

            dynamic index = null;
            if (Variable.Index != null)
            {
                index = Variable.Index;
            }

            Ast_Variable ScopeVar;
            var ve = scope.VariableExists(Variable.Name);
            if (!ve)
            {
                ScopeVar = scope.Variables.Append(Variable.Name, value);
            }
            else
            {
                ScopeVar = scope.GetVariable(Variable.Name);
            }

            if (index != null)
            {
                dynamic indexedv = null;
                if (Operand.Type != TokenType.OpAssign)
                {
                    indexedv = ScopeVar.DoGetValue(index, scope);
                }
                switch (Operand.Type)
                {
                    case TokenType.OpAssign:
                        ScopeVar.DoSetValue(value, index, scope);
                        break;
                    case TokenType.OpAssignAdd:
                        ScopeVar.DoSetValue(indexedv + value, index, scope);
                        break;
                    case TokenType.OpAssignDivide:
                        if (value == 0)
                        {
                            throw new RuntimeError(Token, "Division by zero.");
                        }
                        ScopeVar.DoSetValue(indexedv / value, index, scope);
                        break;
                    case TokenType.OpAssignMultiply:
                        ScopeVar.DoSetValue(indexedv * value, index, scope);
                        break;
                    case TokenType.OpAssignSubtract:
                        ScopeVar.DoSetValue(indexedv - value, index, scope);
                        break;
                }
            }
            else
            {
                dynamic v = null;
                if (Operand.Type != TokenType.OpAssign)
                {
                    v = ScopeVar.DoGetValue();
                }
                switch (Operand.Type)
                {
                    case TokenType.OpAssign:
                        ScopeVar.DoSetValue(value);
                        break;
                    case TokenType.OpAssignAdd:
                        ScopeVar.DoSetValue(v + value);
                        break;
                    case TokenType.OpAssignDivide:
                        if (value == 0)
                        {
                            throw new RuntimeError(Token, "Division by zero.");
                        }
                        ScopeVar.DoSetValue(v / value);
                        break;
                    case TokenType.OpAssignMultiply:
                        ScopeVar.DoSetValue(v * value);
                        break;
                    case TokenType.OpAssignSubtract:
                        ScopeVar.DoSetValue(v - value);
                        break;
                }
            }
            return false;
        }
    }
}
