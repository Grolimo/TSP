// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System.Windows.Forms;

namespace Editor
{
    public static class FindDialog
    {
        private static readonly FrmFind frm = new FrmFind();
        public static string Text { get; set; } = string.Empty;
        public static RichTextBoxFinds Options { get; private set; } = RichTextBoxFinds.None;
        public static DialogResult ShowDialog()
        {
            frm.tbText.Text = Text;
            frm.ActiveControl = frm.tbText;
            var result = frm.ShowDialog();
            if (result == DialogResult.OK)
            {
                Text = frm.tbText.Text;
                if (frm.rbReverse.Checked) Options |= RichTextBoxFinds.Reverse;
                if (frm.cbCaseSensitive.Checked) Options |= RichTextBoxFinds.MatchCase;
            }
            return result;
        }
    }
}
