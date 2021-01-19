// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimpleConfiguration
{
    public class ConfigurationSections : ICollection<ConfigurationSection>
    {
        public IList<ConfigurationSection> Items { get; set; } = new List<ConfigurationSection>();

        public ConfigurationSection this[string name] => GetSection(name);

        public int Count => Items.Count;

        public bool IsReadOnly => Items.IsReadOnly;

        public void Add(ConfigurationSection item)
        {
            Items.Add(item);
        }

        public ConfigurationSection Append(string name)
        {
            return GetSection(name);
        }

        public ConfigurationSection GetSection(string name)
        {
            var item = Items.FirstOrDefault(i => i.Name == name);
            if (item == null)
            {
                item = new ConfigurationSection { Name = name };
                Items.Add(item);
            }
            return item;
        }

        public void Clear()
        {
            Items.Clear();
        }

        public bool Contains(ConfigurationSection item)
        {
            return Items.Contains(item);
        }

        public void CopyTo(ConfigurationSection[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ConfigurationSection> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public bool Remove(ConfigurationSection item)
        {
            return Items.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Items).GetEnumerator();
        }
    }
}
