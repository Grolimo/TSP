// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System.Collections.Generic;

namespace Language
{
    public class LexerState
    {
        public string Source;
        public int Index;
        public int Line;
        public int Offset;
        public int Length;
        public Dictionary<int, int> LineIndexDict = new Dictionary<int, int>();

        public bool EOF => Index >= Length;

        public LexerState(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new StateError("Nil source not allowed.");
            }
            Source = source;
            Length = source.Length;
            Index = 0;
            Line = 0;
            Offset = 0;
            NewLine();
        }

        public void Reset()
        {
            if (string.IsNullOrEmpty(Source))
            {
                throw new StateError("Nil source not allowed.");
            }
            Index = 0;
            Line = 0;
            Offset = 0;
            LineIndexDict.Clear();
            NewLine();
        }

        public char Current
        {
            get
            {
                if (Index < 0 || Index >= Source.Length)
                {
                    throw new StateError("Index out of range.");
                }
                return Source[Index];
            }
        }

        public char Peek(int offset = 1)
        {
            if (Index + offset < 0 || Index + offset >= Source.Length)
            {
                throw new StateError("Index out of range.");
            }
            return Source[Index + offset];
        }

        public char Next()
        {
            if (Index < 0 || Index >= Source.Length)
            {
                throw new StateError("Index out of range.");
            }
            if (Index + 1 == Source.Length)
            {
                Index += 1;
                return (char)0;
            }
            Index += 1;
            Offset += 1;
            return Current;
        }

        public void NewLine()
        {
            Line += 1;
            Offset = 1;
            LineIndexDict.Add(Line, Index);
        }

        public string GetLine(int line)
        {
            if (!LineIndexDict.ContainsKey(line))
                throw new StateError("Index out of range.");
            var start = LineIndexDict[line];
            if (LineIndexDict.ContainsKey(line + 1))
            {
                var lens = LineIndexDict[line + 1] - start;
                return Source.Substring(start, lens);
            }
            var i = start;
            while (i < Length && Source[i] != '\r')
            {
                i += 1;
            }
            var len = i - start;
            return Source.Substring(start, len);
        }
        public LexerState Clone() => new LexerState(Source)
        {
            Index = Index,
            Line = Line,
            Offset = Offset,
            Length = Length,
            LineIndexDict = new Dictionary<int, int>(LineIndexDict)
        };
    }
}
