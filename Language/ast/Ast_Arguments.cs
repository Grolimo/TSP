// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Language
{
    public class Ast_Arguments : ICollection<Ast_Variable>
    {
        public List<Ast_Variable> Items = new List<Ast_Variable>();

        public Ast_Variable this[string index] => Items.FirstOrDefault(v => v.Name == index);
        public Ast_Variable this[int index] => Items[index];
        public int Count => ((ICollection<Ast_Variable>)Items).Count;

        public bool IsReadOnly => ((ICollection<Ast_Variable>)Items).IsReadOnly;

        public void Add(Ast_Variable item)
        {
            ((ICollection<Ast_Variable>)Items).Add(item);
        }

        public void Clear()
        {
            ((ICollection<Ast_Variable>)Items).Clear();
        }

        public bool Contains(Ast_Variable item)
        {
            return ((ICollection<Ast_Variable>)Items).Contains(item);
        }

        public bool ContainsParams()
        {
            foreach (Ast_Variable v in Items)
            {
                if (v.Value.Type == ValueType.Params)
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(Ast_Variable[] array, int arrayIndex)
        {
            ((ICollection<Ast_Variable>)Items).CopyTo(array, arrayIndex);
        }

        public IEnumerator<Ast_Variable> GetEnumerator()
        {
            return ((IEnumerable<Ast_Variable>)Items).GetEnumerator();
        }

        public bool Remove(Ast_Variable item)
        {
            return ((ICollection<Ast_Variable>)Items).Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Items).GetEnumerator();
        }
    }
}
