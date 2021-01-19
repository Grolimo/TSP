// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

namespace SimpleConfiguration
{
    public class ConfigurationSection
    {
        public string Name { get; set; }
        public ConfigurationItems Items { get; set; }
        public ConfigurationItem this[string key] => Items[key];

        public ConfigurationSection()
        {
            Items = new ConfigurationItems();
        }

    }
}
