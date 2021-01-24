// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System.Linq;

namespace Language
{
    public class Ast_Scope
    {
        public Ast_Scope Parent;
        public bool CanSearchUp = true;

        public Ast_Scopes Children = new Ast_Scopes();

        public Ast_Variables Variables = new Ast_Variables();

        public bool InLoop = false;
        public bool IsLoop = false;

        public string Name = string.Empty;

        public Ast_Scope(string name, Ast_Scope parent)
        {
            Parent = parent;
            Name = name;
        }

        public override string ToString()
        {
            var str = Name;
            var curr = this;
            while (curr.Parent != null)
            {
                curr = curr.Parent;
                str = $"{curr.Name} -> {str}";
            }
            return str;
        }

        public void Clear()
        {
            Variables.Clear();
            Children.Clear();
            InLoop = false;
            IsLoop = false;
        }

        public Ast_Scope CreateChild(string name)
        {
            Ast_Scope scope = new Ast_Scope(name, this);
            Children.Add(scope);
            return scope;
        }

        private Ast_Scope FindScopeByVariableName(string name)
        {
            if (name.Contains('.'))
            {
                return GetStructScope(name);
            }
            else
            {
                var lItem = Variables.FirstOrDefault(i => i.Name == name);
                if (lItem != null)
                {
                    return this;
                }

                if (Parent != null && CanSearchUp)
                {
                    return Parent.FindScopeByVariableName(name);
                }

                return null;
            }
        }

        public Ast_Scope GetStructScope(string name)
        {
            var structName = name.Substring(0, name.IndexOf('.'));
            var structVar = Variables.FirstOrDefault(v => v.Name == structName);
            var type = structVar?.Value.Type;
            var value = structVar?.Value.Value;
            Ast_Scope structScope = null;
            if (value != null && type == ValueType.Struct)
            {
                structScope = (Ast_Scope)value.StructScope;
                name = name[(structName.Length + 1)..];
                while (name.Contains('.') && structVar != null)
                {
                    structName = name.Substring(0, name.IndexOf('.') - 1);
                    structVar = structScope.Variables.FirstOrDefault(v => v.Name == structName);
                    type = structVar?.Value.Type;
                    value = structVar?.Value.Value;
                    if (value != null && type == ValueType.Struct)
                    {
                        structScope = (Ast_Scope)value.StructScope;
                        name = name[(structName.Length + 1)..];
                    }
                    else
                    {
                        structScope = null;
                        break;
                    }
                }
            }
            return structScope;
        }

        private static string RemoveStructNames(string name)
        {
            var sn = name;
            while (sn.Contains('.'))
            {
                sn = sn[(sn.IndexOf('.') + 1)..];
            }
            return sn;
        }

        public static Ast_Scope GetIteratorScope(Ast_Scope scope)
        {
            if (scope.IsLoop)
            {
                return scope;
            }
            while (scope.Parent != null)
            {
                scope = scope.Parent;
                if (scope.IsLoop)
                {
                    return scope;
                }
            }
            return scope;
        }
        public bool VariableExists(string name)
        {
            if (name.Contains('.'))
            {
                return GetStructScope(name) != null;
            }
            else
            {
                return FindScopeByVariableName(name) != null;
            }
        }

        public Ast_Variable GetVariable(string name, bool showError = true)
        {
            var scope = FindScopeByVariableName(name);
            if (scope == null && showError)
            {
                throw new SyntaxError($"Variable not found: {name}.");
            }
            if (name.Contains('.'))
            {
                name = RemoveStructNames(name);
            }
            return scope.Variables[name];
        }

    }
}
