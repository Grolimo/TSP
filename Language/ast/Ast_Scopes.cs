// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System.Collections;
using System.Collections.Generic;

namespace Language
{
    public class Ast_Scopes : ICollection<Ast_Scope>
    {
        public List<Ast_Scope> Items = new List<Ast_Scope>();

        public int Count => ((ICollection<Ast_Scope>)Items).Count;

        public bool IsReadOnly => ((ICollection<Ast_Scope>)Items).IsReadOnly;

        public void Add(Ast_Scope item)
        {
            ((ICollection<Ast_Scope>)Items).Add(item);
        }

        public void Clear()
        {
            ((ICollection<Ast_Scope>)Items).Clear();
        }

        public bool Contains(Ast_Scope item)
        {
            return ((ICollection<Ast_Scope>)Items).Contains(item);
        }

        public void CopyTo(Ast_Scope[] array, int arrayIndex)
        {
            ((ICollection<Ast_Scope>)Items).CopyTo(array, arrayIndex);
        }

        public IEnumerator<Ast_Scope> GetEnumerator()
        {
            return ((IEnumerable<Ast_Scope>)Items).GetEnumerator();
        }

        public bool Remove(Ast_Scope item)
        {
            return ((ICollection<Ast_Scope>)Items).Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Items).GetEnumerator();
        }
    }
}
