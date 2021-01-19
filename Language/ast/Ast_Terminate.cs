// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

namespace Language
{
    public class Ast_Terminate
    {
        public bool IsBreak { get; private set; } = false;
        public bool IsHalt { get; private set; } = false;

        public Ast_Terminate(bool isBreak, bool isHalt)
        {
            IsBreak = isBreak;
            IsHalt = isHalt;
        }

        public static readonly Ast_Terminate Break = new Ast_Terminate(true, false) { };
        public static readonly Ast_Terminate Halt = new Ast_Terminate(false, true) { };
    }
}
