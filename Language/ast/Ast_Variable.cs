// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System.Collections.Generic;
using System.Text;

namespace Language
{
    public class Ast_Variable : Ast_Base
    {
        public const string NewArrayValue = "[[---NEWarray/NEW---]]";
        public const string NewRecordValue = "[[---NEWrecord/NEW---]]";
        public const string NewParamsValue = "[[---NEWparams/NEW---]]";
        public const string NewArrayIndex = "{{::new_array_index::}}";

        public const string NilValue = "nil";
        public const string RecordValue = "record";
        public const string ArrayValue = "array";
        public const string ParamsValue = "params";

        public string Name;
        public Ast_Scope Scope;
        public Libraries Libraries;
        public Ast_Index Index = null;
        public bool Ref;

        protected IVT _Value = new VT_Any();
        public IVT Value { get => _Value; }
        public static ValueType GetType(dynamic value)
        {
            if (value == null)
            {
                throw new RuntimeError("Null is not a valid value.");
            }
            else if (value is string)
            {
                if (value == NewArrayValue)
                {
                    return ValueType.Array;
                }
                else if (value == NewRecordValue)
                {
                    return ValueType.Record;
                }
                else if (value == NewParamsValue)
                {
                    return ValueType.Params;
                }
                else if (value == NilValue)
                {
                    return ValueType.Nil;
                }
                return ValueType.String;
            }
            else if (value is char)
            {
                return ValueType.Char;
            }
            else if (value is int || value is long)
            {
                return ValueType.Int;
            }
            else if (value is double || value is float)
            {
                return ValueType.Double;
            }
            else if (value is bool)
            {
                return ValueType.Bool;
            }
            else if (value is Dictionary<string, VT_Any>)
            {
                return ValueType.Record;
            }
            else if (value is List<VT_Any>) // cannot distinguish between array and params, assuming array.
            {
                return ValueType.Array;
            }
            else if (value is Ast_Procedure) // must be before lambda
            {
                return ValueType.Procedure;
            }
            else if (value is Ast_Function) // must be before lambda
            {
                return ValueType.Function;
            }
            else if (value is Ast_Lambda) // must be after function/procedure
            {
                return ValueType.Lambda;
            }
            else if (value is Ast_Struct)
            {
                return ValueType.Struct;
            }
            throw new RuntimeError($"Unknown value type ({value.GetType()}) passed.");

        }
        public virtual void DoSetValue(dynamic value)
        {
            if (value == null)
            {
                value = NilValue;
            }
            ValueType type = GetType(value);
            if (value is string)
            {
                if (value == NewArrayValue || value == NewParamsValue)
                {
                    value = new List<VT_Any>();
                }
                else if (value == NewRecordValue)
                {
                    value = new Dictionary<string, VT_Any>();
                }
            }
            _Value.Value = value;
            _Value.Type = type;
        }

        public virtual void DoSetValue(dynamic value, Ast_Index index, Ast_Scope scope)
        {
            dynamic idx = ValidateIndex(index, scope);
            ValueType type = ValidateType(ref value);

            var ar = _Value.Value;
            foreach (var i in idx)
            {
                if (ar is Dictionary<string, VT_Any> && i is string && !ar.ContainsKey(i))
                {
                    ar.Add(i, new VT_Any());
                    ar = ar[i];
                    break;
                }
                if (ar is List<VT_Any> && i is string)
                {
                    if (i == NewArrayIndex)
                    {
                        ar.Add(new VT_Any());
                        ar = ar[ar.Count - 1];
                        break;
                    }
                    else
                    {
                        throw new RuntimeError(Token, "Invalid indexer type.");
                    }
                }

                if (
                        (ar is List<VT_Any> && i >= ar.Count)
                        || (ar is string && (i >= ar.Length || i < 0))
                   )
                {
                    throw new RuntimeError(Token, "E: Index out of range.");
                }

                if (ar is VT_Any && ar.Value is string)
                {
                    ar.Value[index] = value;
                    return;
                }
                ar = ar[i];
            }
            ar.Value = value;
            ar.Type = type;
        }

        private ValueType ValidateType(ref dynamic value)
        {
            if (value == null)
            {
                value = NilValue;
            }

            ValueType type = GetType(value);
            if (value is string)
            {
                if (value == NewArrayValue || value == NewParamsValue)
                {
                    value = new List<VT_Any>();
                }
                else if (value == NewRecordValue)
                {
                    value = new Dictionary<string, VT_Any>();
                }
            }

            return type;
        }

        private dynamic ValidateIndex(Ast_Index index, Ast_Scope scope)
        {
            if (index == null)
            {
                throw new RuntimeError(Token, "Nil index provided.");
            }
            if (_Value.Type != ValueType.Array && _Value.Type != ValueType.Record &&
                _Value.Type != ValueType.String && Value.Type != ValueType.Params)
            {
                throw new RuntimeError(Token, "Cannot iterate over non iterable variable.");
            }
            var idx = index.Execute(scope);
            return idx;
        }

        public Ast_Variable(Token token) : base(token)
        {
            Type = AstType.Variable;
            Name = token?.Lexeme?.ToString();
            _Value.Value = NilValue;
        }

        public override string ToString()
        {
            if (Token == null) return string.Empty;
            var sb = new StringBuilder();
            sb.Append(Token.Lexeme);
            if (Value != null)
            {
                sb.Append($":{Value.Type} ");
            }
            if (Index != null)
            {
                foreach (var idx in Index.Block)
                {
                    sb.Append($"[{idx}] ");
                }
            }
            if ((Value != null)
                    && (Value.Type != ValueType.Array && Value.Type != ValueType.Record && Value.Type != ValueType.Nil && Value.Value != null))
            {
                if (Value.Type == ValueType.String)
                {
                    sb.Append($"= \"{Value.Value.ToString()}\"");
                }
                else
                {
                    sb.Append($"= {Value.Value.ToString()}");
                }

            }
            return sb.ToString().Trim();
        }

        public dynamic DoGetValue()
        {
            return _Value.Value;
        }
        public dynamic DoGetValue(Ast_Index index, Ast_Scope scope)
        {
            var idx = ValidateIndex(index, scope);
            var ar = _Value.Value;
            foreach (var i in idx)
            {
                if (ar is Dictionary<string, VT_Any> && i is string && !ar.ContainsKey(i))
                {
                    ar.Add(i, new VT_Any());
                    ar = ar[i];
                    break;
                }
                if (
                        (ar is List<VT_Any> && i >= ar.Count)
                        || (ar is string && (i >= ar.Length || i < 0))
                   )
                {
                    throw new RuntimeError(Token, "E: Index out of range.");
                }
                if (ar is string)
                {
                    return ar[i];
                }
                ar = ar[i];
            }
            return ar.Value;
        }

        public override dynamic Execute(Ast_Scope scope)
        {
            throw new System.NotImplementedException();
        }
    }
}
