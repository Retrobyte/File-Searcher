using File_Searcher.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace File_Searcher.Forms
{
    public partial class Main : Form
    {
        private Searcher _searcher;

        public Main()
        {
            InitializeComponent();

            _searcher = new Searcher(completeSearch);
        }

        private void folderButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                    folderTextBox.Text = fbd.SelectedPath;
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(folderTextBox.Text)) return;

            searchButton.Enabled = false;
            _searcher.Search(folderTextBox.Text, searchTextBox.Text, caseSensitiveCheckBox.Checked);
        }

        private void completeSearch(string[] files)
        {
            Invoke(new MethodInvoker(() =>
                {
                    using (ItemList i = new ItemList(files))
                        i.ShowDialog();

                    searchButton.Enabled = true;
                }));
        }
    }
}
