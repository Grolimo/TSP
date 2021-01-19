// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

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

        public override dynamic Execute(Ast_Scope scope, Libraries libraries)
        {
            dynamic value = Expression.Execute(scope, libraries);
            if (value is Ast_Procedure || value is Ast_Function)
            {
                value = value.Clone(Variable.Token);
            }
            dynamic index = null;
            if (Variable.Index != null)
            {
                index = Variable.Index;
            }

            if (!scope.VariableExists(Variable.Name))
            {
                scope.Variables.Append(Variable.Name, value);
            }

            var ScopeVar = scope.GetVariable(Variable.Name);

            if (index != null)
            {
                dynamic indexedv = null;
                if (Operand.Type != TokenType.OpAssign)
                {
                    indexedv = ScopeVar.DoGetValue(index, scope, libraries);
                }
                switch (Operand.Type)
                {
                    case TokenType.OpAssign:
                        ScopeVar.DoSetValue(value, index, scope, libraries);
                        break;
                    case TokenType.OpAssignAdd:
                        ScopeVar.DoSetValue(indexedv + value, index, scope, libraries);
                        break;
                    case TokenType.OpAssignDivide:
                        if (value == 0)
                        {
                            throw new RuntimeError(Token, "Division by zero.");
                        }
                        ScopeVar.DoSetValue(indexedv / value, index, scope, libraries);
                        break;
                    case TokenType.OpAssignMultiply:
                        ScopeVar.DoSetValue(indexedv * value, index, scope, libraries);
                        break;
                    case TokenType.OpAssignSubtract:
                        ScopeVar.DoSetValue(indexedv - value, index, scope, libraries);
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
