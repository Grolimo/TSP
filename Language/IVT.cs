// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

namespace Language
{
    public interface IVT
    {
        dynamic Value { get; set; }
        ValueType Type { get; set; }
    }
}
