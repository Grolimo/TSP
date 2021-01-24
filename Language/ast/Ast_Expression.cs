// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System;
using System.Collections.Generic;
using System.Text;

namespace Language
{
    public class Ast_Expression : Ast_Base
    {
        public static readonly Dictionary<string, KeyValuePair<int, string>> precedenceTable =
            new Dictionary<string, KeyValuePair<int, string>>
        {
            {"(", new KeyValuePair<int, string>(10, "left")},
            {"<", new KeyValuePair<int,string>(9, "left")},
            {">", new KeyValuePair<int,string>(9, "left")},
            {"<=", new KeyValuePair<int,string>(9, "left")},
            {">=", new KeyValuePair<int,string>(9, "left")},
            {"==", new KeyValuePair<int,string>(8, "left")},
            {"!=", new KeyValuePair<int,string>(8, "left")},
            {"^", new KeyValuePair<int,string>(7, "right")},
            {"/", new KeyValuePair<int,string>(6, "left")},
            {"*", new KeyValuePair<int,string>(6, "left")},
            {"+", new KeyValuePair<int,string>(5, "left")},
            {"-", new KeyValuePair<int,string>(5, "left")},
            {"not", new KeyValuePair<int,string>(4, "unary")},
            {"and", new KeyValuePair<int,string>(1, "left")},
            {"xor", new KeyValuePair<int,string>(1, "left")},
            {"or", new KeyValuePair<int,string>(1, "left")},
            {")", new KeyValuePair<int,string>(0, "left")},
        };

        public static readonly List<TokenType> Operands = new List<TokenType> {
            TokenType.OpPower,
            TokenType.OpMultiply,
            TokenType.OpDivide,
            TokenType.OpAdd,
            TokenType.OpSubtract,
            TokenType.OpLT,
            TokenType.OpLTE,
            TokenType.OpGT,
            TokenType.OpGTE,
            TokenType.OpEqual,
            TokenType.OpNE,
            TokenType.OpNot };

        public static readonly List<TokenType> Constants = new List<TokenType> {
            TokenType.ConstantArray,
            TokenType.ConstantBool,
            TokenType.ConstantNil,
            TokenType.ConstantNumber,
            TokenType.ConstantParams,
            TokenType.ConstantRecord,
            TokenType.ConstantString,
            TokenType.FormatString };

        public Ast_Expression(Token token) : base(token)
        {
            Type = AstType.Expression;
        }

        private static dynamic GetValue(Ast_Base ast, Ast_Scope scope)
        {
            if (ast.Type == AstType.Constant)
            {
                return ast.Token.Lexeme;
            }

            if (ast.Type == AstType.Variable)
            {
                Ast_Variable variable = ast as Ast_Variable;
                Ast_Variable v = scope.GetVariable(ast.Token.Lexeme);
                dynamic value;
                if (variable.Index != null)
                {
                    value = v.DoGetValue(variable.Index, scope);
                }
                else
                {
                    value = v.DoGetValue();
                }

                if (variable.Index != null)
                {
                    v.Index = null;
                }
                return value;
            }

            if (ast.Type == AstType.Type)
            {
                if (ast.Token.Lexeme == Ast_Variable.ArrayValue)
                {
                    return Ast_Variable.NewArrayValue;
                }
                else if (ast.Token.Lexeme == Ast_Variable.RecordValue)
                {
                    return Ast_Variable.NewRecordValue;
                }
                else if (ast.Token.Lexeme == Ast_Variable.ParamsValue)
                {
                    return Ast_Variable.NewParamsValue;
                }
                else
                {
                    throw new RuntimeError(ast.Token, $"Unknown ast Type: {ast.Token.Lexeme}");
                }
            }
            if (ast.Type == AstType.Call)
            {
                if (ast is Ast_Call)
                {
                    var call = ast as Ast_Call;
                    if (call?.CallType == CallType.Function)
                    {
                        return call.Execute(scope);
                    }
                    else
                    {
                        throw new RuntimeError(ast.Token, $"Procedure can not be used as a function. ({call.Name})");
                    }
                }
                else
                {
                    throw new RuntimeError(ast.Token, "Invalid call type");
                }
            }
            throw new RuntimeError(ast.Token, $"Unknown value type: {ast.Type}.");
        }

        private static void PushConst(Stack<Ast_Base> stack, dynamic value)
        {
            var token = new Token();
            if (value is string)
            {
                if (value == Ast_Variable.NilValue)
                {
                    token.Type = TokenType.ConstantNil;
                }
                else if (value == Ast_Variable.RecordValue)
                {
                    token.Type = TokenType.ConstantRecord;
                }
                else if (value == Ast_Variable.ArrayValue)
                {
                    token.Type = TokenType.ConstantArray;
                }
                else if (value == Ast_Variable.ParamsValue)
                {
                    token.Type = TokenType.ConstantParams;
                }
                else
                {
                    token.Type = TokenType.ConstantString;
                }
            }
            else if (value is bool)
            {
                token.Type = TokenType.ConstantBool;
            }
            else
            {
                token.Type = TokenType.ConstantNumber;
            }
            token.Lexeme = value;
            var cnst = new Ast_Constant(token);
            stack.Push(cnst);
        }

        private static dynamic ProcessOp(dynamic left, Ast_Base op, dynamic right)
        {
            dynamic value;
            if (op.Token.Lexeme == "==")
            {
                value = (left == right) ? true : false;
            }
            else if (op.Token.Lexeme == "!=")
            {
                value = (left != right) ? true : false;
            }
            else if (op.Token.Lexeme == ">=")
            {
                value = (left >= right) ? true : false;
            }
            else if (op.Token.Lexeme == "<=")
            {
                value = (left <= right) ? true : false;
            }
            else if (op.Token.Lexeme == ">")
            {
                value = (left > right) ? true : false;
            }
            else if (op.Token.Lexeme == "<")
            {
                value = (left < right) ? true : false;
            }
            else if (op.Token.Lexeme == "or")
            {
                value = (left | right) ? true : false;
            }
            else if (op.Token.Lexeme == "and")
            {
                value = (left & right) ? true : false;
            }
            else if (op.Token.Lexeme == "xor")
            {
                value = ((bool)left ^ (bool)right) ? 1 : 0;
            }
            else if (op.Token.Lexeme == "+")
            {
                value = (left + right);
            }
            else if (op.Token.Lexeme == "-")
            {
                value = (left - right);
            }
            else if (op.Token.Lexeme == "*")
            {
                value = (left * right);
            }
            else if (op.Token.Lexeme == "/")
            {
                if (right != 0)
                {
                    value = (left / right);
                }
                else
                {
                    throw new RuntimeError(left.Token, "Division by zero!");
                }
            }
            else if (op.Token.Lexeme == "^")
            {
                value = Math.Pow(left, right);
            }
            else
            {
                throw new RuntimeError(op.Token, $"Unknown operand type {op.Type}.");
            }
            return value;
        }

        public override dynamic Execute(Ast_Scope scope)
        {
            var s = new Stack<Ast_Base>();

            if (Type == AstType.NewArrayIndex)
            {
                return Ast_Variable.NewArrayIndex;
            }

            foreach (var ast in Block)
            {
                dynamic Value = null;
                if (ast.Type == AstType.Type)
                {
                    PushConst(s, Ast_Expression.GetValue(ast, scope));
                }
                else if (ast.Type == AstType.Variable || ast.Type == AstType.Call || ast.Type == AstType.Constant)
                {
                    PushConst(s, Ast_Expression.GetValue(ast, scope));
                }
                else if (ast.Type == AstType.BoolOp)
                {
                    if (ast.Token.Lexeme == "not" && s.Count > 0)
                    {
                        var left = Ast_Expression.GetValue(s.Pop(), scope);
                        Value = !left;
                    }
                    else if (s.Count >= 2)
                    {
                        var right = Ast_Expression.GetValue(s.Pop(), scope);
                        var left = Ast_Expression.GetValue(s.Pop(), scope);
                        Value = ProcessOp(left, ast, right);
                    }
                    else
                    {
                        throw new RuntimeError(ast.Token, $"Unable to handle boolop {ast.Token.Lexeme}, not enough arguments on stack.");
                    }
                }
                else if (Operands.Contains(ast.Token.Type) && s.Count >= 2)
                {
                    var right = Ast_Expression.GetValue(s.Pop(), scope);
                    var left = Ast_Expression.GetValue(s.Pop(), scope);
                    Value = ProcessOp(left, ast, right);
                }
                else if (ast.Type == AstType.Procedure || ast.Type == AstType.Function)
                {
                    s.Push(ast);
                }
                else
                {
                    throw new RuntimeError(ast.Token, $"Unknown token type for postfix eval: {ast.Type}.");
                }
                if (Value != null)
                {
                    PushConst(s, Value);
                }
            }

            Ast_Base token = null;
            if (s.Count > 0)
            {
                token = s.Pop();
                if (token.Type == AstType.Constant ||
                    token.Type == AstType.Variable ||
                    token.Type == AstType.Call)
                {
                    var value = Ast_Expression.GetValue(token, scope);
                    return value;
                }
            }
            return token;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var ast in Block)
            {
                if (ast.Type == AstType.Constant && ast.Token.Type == TokenType.ConstantString)
                {
                    sb.Append($"\"{ast}\"");
                }
                else
                {
                    sb.Append(ast.ToString());
                }

                sb.Append(' ');
            }
            return sb.ToString().Trim();
        }
    }
}
