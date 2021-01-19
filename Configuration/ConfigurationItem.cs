// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

namespace SimpleConfiguration
{
    public class ConfigurationItem
    {
        public string Key { get; set; }
        private string _Value = string.Empty;
        public string Value
        {
            get => _Value;
            set => SetValue(value);
        }

        protected virtual void SetValue(string value)
        {
            _Value = value;
        }
    }
}
