using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace File_Searcher.Forms
{
    public partial class ItemList : Form
    {
        public ItemList(string[] files)
        {
            InitializeComponent();

            foreach (string file in files)
                itemsListView.Items.Add(new ListViewItem(new string[] { Path.GetFileName(file), Path.GetDirectoryName(file) }));
        }

        private void filenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (itemsListView.SelectedItems.Count != 1) return;

            Clipboard.SetText(itemsListView.SelectedItems[0].Text);
        }

        private void locationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (itemsListView.SelectedItems.Count != 1) return;

            Clipboard.SetText(itemsListView.SelectedItems[0].SubItems[1].Text);
        }
    }
}
