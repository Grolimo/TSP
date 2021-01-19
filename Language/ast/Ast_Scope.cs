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

        public void Clear()
        {
            Variables.Clear();
            Children.Clear();
            InLoop = false;
            IsLoop = false;
        }

        public Ast_Scope CreateChild(string name)
        {
            var scope = new Ast_Scope($"{Name} -> {name}", this);
            Children.Add(scope);
            return scope;
        }

        private Ast_Scope FindScopeByVariableName(string name)
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
            return FindScopeByVariableName(name) != null;
        }

        public Ast_Variable GetVariable(string name, bool showError = true)
        {
            var scope = FindScopeByVariableName(name);
            if (scope == null && showError)
            {
                throw new SyntaxError($"Variable not found: {name}.");
            }
            return scope.Variables[name];
        }
    }
}
