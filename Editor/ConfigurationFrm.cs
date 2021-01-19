// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using SimpleConfiguration;
using System;
using System.Windows.Forms;

namespace Editor
{
    public partial class ConfigurationFrm : Form
    {
        public const string ConfigurationFileName = "config.json";
        public const string SelectFileFolderDescription = "Select source files folder";
        public const string DefaultSourceFileExtension = ".tsl";

        public ConfigurationFrm()
        {
            InitializeComponent();
            Configuration.Load(ConfigurationFileName);
            tbSourceFiles.Text = Configuration.Sections["Locations"]["SourceFiles"].Value;
            CbEnableDebug.Checked = Configuration.Sections["General"]["Debug"].Value == "true";
            TbSourceFileExtension.Text = Configuration.Sections["General"]["FileExtension"].Value;
        }

        public string SourceFiles
        {
            get => tbSourceFiles.Text;
            set
            {
                tbSourceFiles.Text = value;
                Configuration.Save();
            }
        }

        public bool EnableDebug
        {
            get => CbEnableDebug.Checked;
            set
            {
                CbEnableDebug.Checked = value;
                Configuration.Save();
            }
        }

        public string SourceFileExtension
        {
            get => TbSourceFileExtension.Text;
            set
            {
                TbSourceFileExtension.Text = value;
                Configuration.Sections["General"]["FileExtension"].Value = value;
                Configuration.Save();
            }
        }
        public static void Save()
        {
            Configuration.Save();
        }

        private void SetSourceFiles()
        {
            fbd.Description = SelectFileFolderDescription;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                tbSourceFiles.Text = fbd.SelectedPath;
                Configuration.Sections["Locations"]["SourceFiles"].Value = fbd.SelectedPath;
            }
        }

        private void SetDebugMode()
        {
            Configuration.Sections["General"]["Debug"].Value = CbEnableDebug.Checked ? "true" : "false";
        }

        private void SetSourceFileExtension()
        {
            Configuration.Sections["General"]["FileExtension"].Value = TbSourceFileExtension.Text;
        }

        private void SetDefaultSourceFileExtension()
        {
            TbSourceFileExtension.Text = DefaultSourceFileExtension;
            Configuration.Sections["General"]["FileExtension"].Value = DefaultSourceFileExtension;
        }

        private void BtnChangeSourceLocation_Click(object sender, EventArgs e)
        {
            SetSourceFiles();
        }

        private void CbEnableDebug_CheckedChanged(object sender, EventArgs e)
        {
            SetDebugMode();
        }

        private void BtnDefaultExtension_Click(object sender, EventArgs e)
        {
            SetDefaultSourceFileExtension();
        }

        private void TbSourceFileExtension_TextChanged(object sender, EventArgs e)
        {
            SetSourceFileExtension();
        }
    }
}
