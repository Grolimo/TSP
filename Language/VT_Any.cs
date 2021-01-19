// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

namespace Language
{
    public class VT_Any : IVT
    {
        public dynamic Value { get; set; }
        public ValueType Type { get; set; }
        public VT_Any()
        {
            Type = ValueType.Any;
        }
    }
}
