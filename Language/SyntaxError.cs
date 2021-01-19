// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System;
using System.Runtime.Serialization;

namespace Language
{
    [Serializable]
    public class SyntaxError : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    https://msdn.microsoft.com/en-us/library/ms229064(v=vs.100).aspx
        //

        public SyntaxError()
        {
        }

        public SyntaxError(string message) : base($"Syntax Error: {message}")
        {
        }

        public SyntaxError(Token token, string message) : base($"Syntax Error: {message} ({token.Line}, {token.Offset}))")
        {
        }

        public SyntaxError(string message, Exception inner) : base($"Syntax Error: {message}", inner)
        {
        }

        protected SyntaxError(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
