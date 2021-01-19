// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System.Windows.Forms;

namespace Editor
{
    public static class ConfigurationDlg
    {
        private static readonly ConfigurationFrm Frm = new ConfigurationFrm();
        public static string SourceFiles
        {
            get => Frm.SourceFiles;
            set { Frm.SourceFiles = value; }
        }
        public static bool EnableDebug
        {
            get => Frm.EnableDebug;
            set { Frm.EnableDebug = value; }
        }

        public static string SourceFileExtension
        {
            get => Frm.SourceFileExtension;
            set { Frm.SourceFileExtension = value; }
        }

        public static DialogResult ShowDialog()
        {
            var result = Frm.ShowDialog();
            ConfigurationFrm.Save();
            return result;
        }
    }
}
