// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using Language;
using System;
using System.IO;
using System.Windows.Forms;
using ValueType = Language.ValueType;

namespace Editor
{
    public partial class Form1 : Form
    {
        private string fileName = string.Empty;
        private readonly RichEditHelper reh;
        private static readonly string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Simple Language Projects");

        private Interpreter Interpreter;

        public Form1()
        {
            InitializeComponent();
            tbEditor.Select();
            tbEditor.SelectionStart = 0;
            tbEditor.SelectionLength = 0;

            toolStripStatusLabel1.Text = $"line: 0";
            toolStripStatusLabel2.Text = $"col:  0";
            DoubleBuffered = true;

            PersistSourceFolder();

            reh = new RichEditHelper(this, tbEditor, LineNumberTextBox, toolStripStatusLabel1, toolStripStatusLabel2);
            Console.SetOut(new MultiTextWriter(new ControlWriter(tbOutput), Console.Out));

            // For debugging purposes.
            LoadFile(Path.Combine(ConfigurationDlg.SourceFiles, "new.tsl"));
        }

        private static void PersistSourceFolder()
        {
            if (string.IsNullOrEmpty(ConfigurationDlg.SourceFiles))
            {
                if (!Directory.Exists(defaultPath))
                {
                    Directory.CreateDirectory(defaultPath);
                }
                ConfigurationDlg.SourceFiles = defaultPath;
            }
        }

        private void ExecuteDebug()
        {
            Interpreter = new Interpreter();
            Interpreter.Execute(tbEditor.Text);
            ShowDebugData();
            tbOutput.AppendText("\r\n");
            tbOutput.AppendText("Interpreter ran successfully.\r\n");
        }

        private void ShowDebugData()
        {
            ShowLexerOutput();
            tv1.Nodes.Clear();
            tv2.Nodes.Clear();
            tvAppScope.Nodes.Clear();
            if (Interpreter != null)
            {
                if (Interpreter.Application != null)
                {
                    ShowAstTree(Interpreter.Application, null);
                    if (Interpreter.Application.Scope != null)
                    {
                        ShowScopeTree(Interpreter.Application.Scope, null, tvAppScope);
                    }
                }
                if (Interpreter.ParserScope != null)
                {
                    ShowScopeTree(Interpreter.ParserScope, null, tv2);
                }
            }
            tv1.ExpandAll();
            tv2.ExpandAll();
            tvAppScope.ExpandAll();
        }

        private void Debug()
        {
            bool debug = ConfigurationDlg.EnableDebug;
            ClearOutput();
            if (debug)
            {
                ExecuteDebug();
            }
            else
            {
                try
                {
                    ExecuteDebug();
                }
                catch (Exception ex)
                {
                    tv1.Nodes.Clear();
                    ShowDebugData();
                    tbOutput.AppendText($"An error ocurred: {ex.Message}");
                }
            }
        }

        private static void Configure()
        {
            _ = ConfigurationDlg.ShowDialog();
        }

        private void ClearOutput()
        {
            tbOutput.Clear();
            tv1.Nodes.Clear();
            tv2.Nodes.Clear();
            tbLexerOutput.Clear();
        }

        private void ShowLexerOutput()
        {
            LexerState LexerState = new LexerState(tbEditor.Text);
            LexerState.Reset();
            var token = Lexer.GetToken(LexerState);
            while (token.Type != TokenType.EOF)
            {
                tbLexerOutput.AppendText(token.ToString());
                tbLexerOutput.AppendText("\r\n");
                token = Lexer.GetToken(LexerState);
            }
        }

        private void AddNode(TreeNode tn, TreeNode tvn)
        {
            if (tvn == null)
            {
                tv1.Nodes.Add(tn);
            }
            else
            {
                tvn.Nodes.Add(tn);
            }
        }

        private static void AddScopeNode(TreeNode tn, TreeNode tvn, TreeView tv)
        {
            if (tvn == null)
            {
                tv.Nodes.Add(tn);
            }
            else
            {
                tvn.Nodes.Add(tn);
            }
        }

        private void ShowAstTree(IAst node, TreeNode tvn)
        {
            string text = node.ToString();
            TreeNode tn = new TreeNode(text);
            AddNode(tn, tvn);
            if (node.Block.Count > 0 && !(node is Ast_Call))
            {
                ShowAstTreeBlock(node, tn);
            }
        }

        private void ShowAstTreeBlock(IAst node, TreeNode tn)
        {
            foreach (Ast_Base n in node.Block)
            {
                ShowAstTree(n, tn);
            }
        }

        private void ShowScopeTree(Ast_Scope scope, TreeNode tvn, TreeView tv)
        {
            TreeNode st = new TreeNode($"scope {scope}");
            AddScopeNode(st, tvn, tv);
            var nv = st.Nodes.Add("Variables");
            foreach (Ast_Variable v in scope.Variables)
            {
                var varnode = nv.Nodes.Add(v.ToString());
                if (v.Value.Type == ValueType.Array || v.Value.Type == ValueType.Record || v.Value.Type == ValueType.Params)
                {
                    ShowVarArrayTree(v, varnode);
                }
            }
            var cn = st.Nodes.Add("Scope");
            ShowScopeChildren(scope, cn, tv);
        }

        private static void ShowVarArrayTree(Ast_Variable v, TreeNode tn)
        {
            if (v.Value.Type == ValueType.Record)
            {
                foreach (var key in v.Value.Value.Keys)
                {
                    tn.Nodes.Add($"{v.ToString(key)}");
                }
            }
            else
            {
                for (var i = 0; i < v.Value.Value.Count; i++)
                {
                    tn.Nodes.Add($"{v.ToString(i)}");
                }
            }
        }

        private void ShowScopeChildren(Ast_Scope scope, TreeNode st, TreeView tv)
        {
            foreach (var child in scope.Children)
            {
                ShowScopeTree(child, st, tv);
            }
        }

        private void Open()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                Filter = $"TSL File|*{ConfigurationDlg.SourceFileExtension}",
                Title = "Open code file",
                InitialDirectory = ConfigurationDlg.SourceFiles
            };
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ClearOutput();
                LoadFile(openFileDialog1);
            }
        }

        private void LoadFile(OpenFileDialog openFileDialog1)
        {
            fileName = openFileDialog1.FileName;
            LoadFile(fileName);
        }

        private void LoadFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                fileName = filePath;
                Text = $"The Simple Language Editor - {Path.GetFileName(fileName)}";
                using (var sr = new StreamReader(fileName))
                {
                    tbEditor.Text = sr.ReadToEnd();
                }
                RefreshEditor();
            }
        }

        private void RefreshEditor()
        {
            tbEditor.Select();
            tbEditor.SelectionStart = 0;
            tbEditor.SelectionLength = 0;
            reh.Refresh();
        }

        private void Save()
        {
            if (fileName == string.Empty)
            {
                SaveAs();
            }
            if (fileName != string.Empty)
            {
                Text = $"The Simple Language Editor - {Path.GetFileName(fileName)}";
                using var sw = new StreamWriter(new FileStream(fileName, FileMode.Create));
                sw.Write(tbEditor.Text);
            }
        }

        private void SaveAs()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog
            {
                Filter = $"Simple Code File|*{ConfigurationDlg.SourceFileExtension}",
                Title = "Save TSL code file as:",
                InitialDirectory = ConfigurationDlg.SourceFiles
            };
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = saveFileDialog1.FileName;
                Save();
            }
        }

        private void New()
        {
            ClearOutput();
            fileName = Path.Combine(ConfigurationDlg.SourceFiles, $"new{ConfigurationDlg.SourceFileExtension}");
            Text = $"The Simple Language Editor - new{ConfigurationDlg.SourceFileExtension}";
            tbEditor.Text = string.Empty;
            RefreshEditor();
        }

        private void DebugToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Debug();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Open();
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            New();
        }

        private void FindToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reh.Find();
        }

        private void FindNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reh.FindNext();
        }

        private void OptionsMenuItem_Click(object sender, EventArgs e)
        {
            Configure();
        }

        private void CmCopy_Click(object sender, EventArgs e)
        {
            reh.CopyToClip();
        }

        private void CmPaste_Click(object sender, EventArgs e)
        {
            reh.PasteFromClip();
        }

        private void CmCut_Click(object sender, EventArgs e)
        {
            reh.CutToClip();
        }

        private void CmDelete_Click(object sender, EventArgs e)
        {
            reh.Delete();
        }

        private void CmFind_Click(object sender, EventArgs e)
        {
            reh.Find();
        }

        private void CmFindNext_Click(object sender, EventArgs e)
        {
            reh.FindNext();
        }
    }
}
