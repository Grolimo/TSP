// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimpleConfiguration
{
    public class ConfigurationItems : ICollection<ConfigurationItem>
    {
        public IList<ConfigurationItem> Items { get; set; } = new List<ConfigurationItem>();

        public int Count => Items.Count;

        public bool IsReadOnly => Items.IsReadOnly;

        public ConfigurationItem this[string key]
        {
            get
            {
                return GetItem(key);
            }
        }

        public void Add(ConfigurationItem item)
        {
            Items.Add(item);
        }

        public ConfigurationItem Append(string key)
        {
            return GetItem(key);
        }

        public ConfigurationItem Append(string key, dynamic value)
        {
            var item = GetItem(key);
            item.Value = value;
            return item;
        }

        public ConfigurationItem GetItem(string key)
        {
            var item = Items.FirstOrDefault(i => i.Key == key);
            if (item == null)
            {
                item = new ConfigurationItem { Key = key };
                Items.Add(item);
            }
            return item;
        }

        public void Clear()
        {
            Items.Clear();
        }

        public bool Contains(ConfigurationItem item)
        {
            return Items.Contains(item);
        }

        public void CopyTo(ConfigurationItem[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ConfigurationItem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public bool Remove(ConfigurationItem item)
        {
            return Items.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Items).GetEnumerator();
        }
    }
}
