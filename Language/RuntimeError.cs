// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System;

namespace Language
{
    [Serializable]
    public class RuntimeError : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    https://msdn.microsoft.com/en-us/library/ms229064(v=vs.100).aspx
        //

        public RuntimeError(string message) : base($"Runtime Error: {message}")
        {
        }

        public RuntimeError(Token token, string message) : base($"Runtime Error: {message} ({token.Line}, {token.Offset}))")
        {
        }
    }
}
