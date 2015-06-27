using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace FileTimeChanger
{
    public partial class FileTimeChanger : Form
    {
        public FileTimeChanger()
        {
            InitializeComponent();

            btnChange.Enabled = false;
            btnRecovery.Enabled = false;
            btnReset.Enabled = false;
            btnRemove.Enabled = false;
        }

        private void listView_DragDrop(object sender, DragEventArgs e)
        {
            e.Data.GetData(DataFormats.FileDrop);
            int count = ((string[])e.Data.GetData(DataFormats.FileDrop)).GetLength(0);

            progressBar.Value = 0;
            progressBar.Minimum = 0;
            progressBar.Maximum = count;
            progressBar.Step = 1;

            int imageIndex = 0;

            foreach (string fileName in ((string[])e.Data.GetData(DataFormats.FileDrop)))
            {
                FileInfo fi = new FileInfo(fileName);

                if (File.GetAttributes(fileName).ToString().StartsWith("Archive") == true)
                {
                    imageIndex = 0;
                }
                else if (File.GetAttributes(fileName).ToString().StartsWith("Directory") == true)
                {
                    imageIndex = 1;
                }
                else
                {
                    continue;
                }

                string[] strItem = new string[]
                                    { 
                                        fi.Name, 
                                        fi.CreationTime.ToString(), 
                                        fi.LastWriteTime.ToString(), 
                                        fi.LastAccessTime.ToString(),
                                        fi.FullName, 
                                        string.Format("{0}!{1}!{2}", fi.CreationTime.ToString(), fi.LastWriteTime.ToString(), fi.LastAccessTime.ToString())
                                    };

                ListViewItem item = new ListViewItem(strItem, imageIndex);
                item.Name = fi.FullName;

                if (!listView.Items.ContainsKey(fi.FullName))
                {
                    listView.Items.Add(item);
                    progressBar.PerformStep();
                }
            }

            progressBar.Value = progressBar.Maximum;

            if (listView.Items.Count > 0)
            {
                btnChange.Enabled = true;
                btnRecovery.Enabled = true;
                btnReset.Enabled = true;
                btnRemove.Enabled = true;
            }
        }

        private void listView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void ChangeHalfDatetime()
        {
            FileSystemInfo fsi = null;
            foreach (ListViewItem item in listView.Items)
            {
                fsi = null;
                if (item.ImageIndex == 0)
                {
                    fsi = new FileInfo(item.SubItems[4].Text);
                }
                else if (item.ImageIndex == 1)
                {
                    fsi = new DirectoryInfo(item.SubItems[4].Text);
                }

                try
                {

                    fsi.CreationTime = new DateTime(dateTimePicker.Value.Year, dateTimePicker.Value.Month, dateTimePicker.Value.Day, fsi.CreationTime.Hour, fsi.CreationTime.Minute, fsi.CreationTime.Second);
                    fsi.LastWriteTime = new DateTime(dateTimePicker.Value.Year, dateTimePicker.Value.Month, dateTimePicker.Value.Day, fsi.LastWriteTime.Hour, fsi.LastWriteTime.Minute, fsi.LastWriteTime.Second);
                    fsi.LastAccessTime = new DateTime(dateTimePicker.Value.Year, dateTimePicker.Value.Month, dateTimePicker.Value.Day, fsi.LastAccessTime.Hour, fsi.LastAccessTime.Minute, fsi.LastAccessTime.Second);

                    item.SubItems[1].Text = fsi.CreationTime.ToString();
                    item.SubItems[2].Text = fsi.LastWriteTime.ToString();
                    item.SubItems[3].Text = fsi.LastAccessTime.ToString();
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                progressBar.PerformStep();
            }
        }

        private void ChangeFullDatetime()
        {
            FileSystemInfo fsi = null;
            foreach (ListViewItem item in listView.Items)
            {
                fsi = null;
                if (item.ImageIndex == 0)
                {
                    fsi = new FileInfo(item.SubItems[4].Text);
                }
                else if (item.ImageIndex == 1)
                {
                    fsi = new DirectoryInfo(item.SubItems[4].Text);
                }

                try
                {
                    fsi.CreationTime = dateTimePicker.Value;
                    fsi.LastWriteTime = dateTimePicker.Value;
                    fsi.LastAccessTime = dateTimePicker.Value;

                    item.SubItems[1].Text = fsi.CreationTime.ToString();
                    item.SubItems[2].Text = fsi.LastWriteTime.ToString();
                    item.SubItems[3].Text = fsi.LastAccessTime.ToString();
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                progressBar.PerformStep();
            }
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            progressBar.Value = 0;
            progressBar.Maximum = listView.Items.Count;

            if (checkTime.Checked)
            {
                ChangeHalfDatetime();
            }
            else
            {
                ChangeFullDatetime();
            }

            progressBar.Value = progressBar.Maximum;
        }

        private void btnRecovery_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "원래 날짜로 복구하시겠습니까?", "복구", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                progressBar.Value = 0;
                progressBar.Maximum = listView.Items.Count;

                FileSystemInfo fsi = null;
                foreach (ListViewItem item in listView.Items)
                {
                    fsi = null;
                    if (item.ImageIndex == 0)
                    {
                        fsi = new FileInfo(item.SubItems[4].Text);
                    }
                    else if (item.ImageIndex == 1)
                    {
                        fsi = new DirectoryInfo(item.SubItems[4].Text);
                    }

                    string[] Times = item.SubItems[5].Text.Split('!');

                    try
                    {
                        fsi.CreationTime = DateTime.Parse(Times[0]);
                        fsi.LastWriteTime = DateTime.Parse(Times[1]);
                        fsi.LastAccessTime = DateTime.Parse(Times[2]);

                        item.SubItems[1].Text = fsi.CreationTime.ToString();
                        item.SubItems[2].Text = fsi.LastWriteTime.ToString();
                        item.SubItems[3].Text = fsi.LastAccessTime.ToString();

                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    progressBar.PerformStep();
                }
                progressBar.Value = progressBar.Maximum;
            }

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            listView.Items.Clear();
            dateTimePicker.Value = DateTime.Now;
            progressBar.Value = 0;
            checkTime.Checked = false;

            btnChange.Enabled = false;
            btnRecovery.Enabled = false;
            btnReset.Enabled = false;
            btnRemove.Enabled = false;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "선택된 항목들을 목록에서 제거합니다.\r\n제거하시겠습니까?", "목록제거", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                foreach (ListViewItem item in listView.SelectedItems)
                {
                    listView.Items.Remove(item);
                }
            }

            if (listView.Items.Count <= 0)
            {
                btnChange.Enabled = false;
                btnRecovery.Enabled = false;
                btnReset.Enabled = false;
                btnRemove.Enabled = false;
            }
        }
    }
}