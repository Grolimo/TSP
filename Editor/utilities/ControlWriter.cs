// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System.IO;
using System.Text;
using System.Windows.Forms;

/// <summary>
/// Code by Servy: https://stackoverflow.com/users/1159478/servy
/// https://stackoverflow.com/questions/18726852/redirecting-console-writeline-to-textbox
/// </summary>

namespace Editor
{
    public class ControlWriter : TextWriter
    {
        private readonly Control textbox;
        public ControlWriter(Control textbox)
        {
            this.textbox = textbox;
        }

        public override void Write(char value)
        {
            textbox.Text += value;
        }

        public override void Write(string value)
        {
            textbox.Text += value;
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}
