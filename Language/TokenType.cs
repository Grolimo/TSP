// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

namespace Language
{
    public enum TokenType
    {
        BlockLeft,
        BlockRight,
        BracketLeft,
        BracketRight,
        Comma,
        Comment,
        ConstantArray,
        ConstantBool,
        ConstantNil,
        ConstantNumber,
        ConstantParams,
        ConstantRecord,
        ConstantString,
        Error,
        EOF,
        FormatString,
        Identifier,
        IndexLeft,
        IndexRight,
        OpAdd,
        OpAssign,
        OpAssignAdd,
        OpAssignDivide,
        OpAssignLambda,
        OpAssignMultiply,
        OpAssignReturnVar,
        OpAssignSubtract,
        OpDivide,
        OpEqual,
        OpGT,
        OpGTE,
        OpLT,
        OpLTE,
        OpMultiply,
        OpNE,
        OpNot,
        OpPower,
        OpSubtract,
        Ref,
        Semicolon,
        Simple,
        TypeArray,
        TypeRecord,
        Unknown
    }
}
