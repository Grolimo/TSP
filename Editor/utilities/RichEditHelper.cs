// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Editor
{
    public class RichEditHelper
    {
        private readonly ToolStripStatusLabel statusLine;
        private readonly ToolStripStatusLabel statusOffset;
        private readonly RichTextBox LineNumberTextBox;
        private readonly RichTextBox TbInput;
        private readonly Form LocalForm;
        public bool Changed = false;

        public RichEditHelper(Form frm, RichTextBox tbinput, RichTextBox linenumbers, ToolStripStatusLabel l1, ToolStripStatusLabel l2)
        {
            LocalForm = frm;
            LocalForm.Resize += Resize;

            TbInput = tbinput;
            TbInput.KeyPress += KeyPress;
            TbInput.KeyUp += KeyUp;
            TbInput.SelectionChanged += SelectionChanged;
            TbInput.Click += Click;
            TbInput.MouseClick += MouseClick;
            TbInput.VScroll += VScroll;
            TbInput.TextChanged += TextChanged;
            TbInput.FontChanged += FontChanged;

            LineNumberTextBox = linenumbers;
            LineNumberTextBox.MouseDown += MouseDown;

            statusLine = l1;
            statusOffset = l2;
        }

        public void Refresh()
        {
            KeyUp(TbInput, null);
            AddLineNumbers();
        }

        public int GetWidth()
        {
            // get total lines of richTextBox1    
            int line = TbInput.Lines.Length;

            int w;
            if (line <= 99)
            {
                w = 20 + (int)TbInput.Font.Size;
            }
            else if (line <= 999)
            {
                w = 30 + (int)TbInput.Font.Size;
            }
            else
            {
                w = 50 + (int)TbInput.Font.Size;
            }

            return w;
        }

        public void AddLineNumbers()
        {
            LocalForm.SuspendLayout();
            TbInput.SuspendDrawing();
            LineNumberTextBox.SuspendDrawing();
            // create & set Point pt to (0,0)    
            Point pt = new Point(0, 0);
            // get First Index & First Line from richTextBox1    
            int First_Index = TbInput.GetCharIndexFromPosition(pt);
            int First_Line = TbInput.GetLineFromCharIndex(First_Index);
            // set X & Y coordinates of Point pt to ClientRectangle Width & Height respectively    
            pt.X = LocalForm.ClientRectangle.Width;
            pt.Y = LocalForm.ClientRectangle.Height;
            // get Last Index & Last Line from richTextBox1    
            int Last_Index = TbInput.GetCharIndexFromPosition(pt);
            int Last_Line = TbInput.GetLineFromCharIndex(Last_Index);
            // set Center alignment to LineNumberTextBox    
            LineNumberTextBox.SelectionAlignment = HorizontalAlignment.Center;
            // set LineNumberTextBox text to null & width to getWidth() function value    
            TbInput.Left = 37;
            LineNumberTextBox.Text = "";
            LineNumberTextBox.Width = GetWidth();
            if ((LineNumberTextBox.Left + LineNumberTextBox.Width - 1) > TbInput.Left)
            {
                TbInput.Left = LineNumberTextBox.Left + LineNumberTextBox.Width + 1;
            }
            TbInput.Width = LocalForm.ClientRectangle.Width - TbInput.Left;
            // now add each line number to LineNumberTextBox upto last line    
            for (int i = First_Line; i <= Last_Line + 2; i++)
            {
                LineNumberTextBox.Text += i + 1 + "\n";
            }
            LineNumberTextBox.ResumeDrawing();
            TbInput.ResumeDrawing();
            LocalForm.ResumeLayout();
        }

        private void KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Tab)
            {
                var rtb = ((RichTextBox)sender);
                e.Handled = true;
                rtb.SelectionLength = 0;
                int numSpaces = 4 - ((rtb.SelectionStart - rtb.GetFirstCharIndexOfCurrentLine()) % 4);
                rtb.SelectedText = new string(' ', numSpaces);
            }
        }

        private void KeyUp(object sender, KeyEventArgs e)
        {
            if (e?.KeyCode == Keys.F3)
            {
                FindNext();
            }
            if ((e != null) && (e.KeyCode == Keys.F) && (e.Control))
            {
                Find();
            }
            var rtb = ((RichTextBox)sender);
            if (rtb == null) return;
            // Get the line.
            int index = rtb.SelectionStart;
            int line = rtb.GetLineFromCharIndex(index);

            // Get the column.
            int firstChar = rtb.GetFirstCharIndexFromLine(line);
            int column = index - firstChar;

            statusLine.Text = $"line: {line + 1}";
            statusOffset.Text = $"col:  {column + 1}";
        }

        private void SelectionChanged(object sender, EventArgs e)
        {
            Point pt = TbInput.GetPositionFromCharIndex(TbInput.SelectionStart);
            if (pt.X == 1)
            {
                AddLineNumbers();
            }
        }

        private void Click(object sender, EventArgs e)
        {
            KeyUp(sender, null);
        }

        private void MouseClick(object sender, MouseEventArgs e)
        {
            KeyUp(sender, null);
        }

        private void Resize(object sender, EventArgs e)
        {
            AddLineNumbers();
        }

        private void VScroll(object sender, EventArgs e)
        {
            LineNumberTextBox.Text = "";
            AddLineNumbers();
            LineNumberTextBox.Invalidate();
        }

        private void TextChanged(object sender, EventArgs e)
        {
            Changed = true;
            if (TbInput.Text == "")
            {
                AddLineNumbers();
            }
        }

        private void FontChanged(object sender, EventArgs e)
        {
            LineNumberTextBox.Font = TbInput.Font;
            TbInput.Select();
            AddLineNumbers();
        }

        private void MouseDown(object sender, MouseEventArgs e)
        {
            TbInput.Select();
            LineNumberTextBox.DeselectAll();
        }

        public void Find()
        {
            if (TbInput.SelectionLength > 0)
            {
                FindDialog.Text = TbInput.SelectedText;
            }
            if (FindDialog.ShowDialog() == DialogResult.OK)
            {
                TbInput.Find(FindDialog.Text, FindDialog.Options);
            }
            LocalForm.ActiveControl = TbInput;
        }

        public void FindNext()
        {
            if (FindDialog.Text != string.Empty)
            {
                TbInput.Find(FindDialog.Text, TbInput.SelectionStart + 1, FindDialog.Options);
            }
            LocalForm.ActiveControl = TbInput;
        }

        public void CopyToClip()
        {
            if (TbInput.SelectionLength > 0)
            {
                TbInput.Copy();
            }
        }

        public void CutToClip()
        {
            if (TbInput.SelectionLength > 0)
            {
                TbInput.Cut();
            }
        }

        public void PasteFromClip()
        {
            TbInput.Paste();
        }

        public void Delete()
        {
            if (TbInput.SelectionLength > 0)
            {
                TbInput.SelectedText = "";
            }
        }
    }

    public static class RichTextBoxExtensions
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        private const int WM_SETREDRAW = 0x0b;

        public static void SuspendDrawing(this RichTextBox richTextBox)
        {
            SendMessage(richTextBox.Handle, WM_SETREDRAW, (IntPtr)0, IntPtr.Zero);
        }

        public static void ResumeDrawing(this RichTextBox richTextBox)
        {
            SendMessage(richTextBox.Handle, WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);
            richTextBox.Invalidate();
        }
    }

}
