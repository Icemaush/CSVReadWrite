using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using LumenWorks.Framework.IO.Csv;


namespace CSVReadWrite
{
    public partial class CSVReadWrite : Form
    {
        private string[] headers;
        private string filePath;

        public CSVReadWrite()
        {
            InitializeComponent();
        }

        // Load CSV file
        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = Directory.GetCurrentDirectory();
            openFile.Filter = "csv files (*.csv)|*.csv";


            if (openFile.ShowDialog() == DialogResult.OK)
            {
                filePath = openFile.FileName;
                ReadCSV(openFile.FileName);
            }

        }

        // Save CSV file
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFile(filePath);
        }

        // Save CSV file as...
        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.InitialDirectory = Directory.GetCurrentDirectory();
            saveFile.Filter = "csv files (*.csv)|*.csv";
            saveFile.ShowDialog();

            if (saveFile.FileName != "")
            {
                SaveFile(saveFile.FileName);
            }
        }

        // Save file
        private void SaveFile(string filePath)
        {
            using (StreamWriter outputFile = new StreamWriter(filePath))
            {
                outputFile.WriteLine(string.Join(",", headers));

                foreach (ListViewItem item in lvOutput.Items)
                {
                    string[] fields = new string[item.SubItems.Count];
                    for (int i = 0; i < item.SubItems.Count; i++)
                    {
                        fields[i] = item.SubItems[i].Text;
                    }
                    outputFile.WriteLine(string.Join(",", fields));
                }
            }

            UpdateStatus("File saved.", Color.Red);
        }

        // Modify CSV record
        private void btnModify_Click(object sender, EventArgs e)
        {
            ModifyRecord();
        }

        // Exit program
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Read CSV file
        private void ReadCSV(string filename)
        {
            using (CsvReader csv = new CsvReader(new StreamReader(filename), true))
            {
                
                int fieldCount = csv.FieldCount;
                headers = csv.GetFieldHeaders();

                // Add column headers to listview
                foreach (string header in headers)
                {
                    lvOutput.Columns.Add(header);
                }

                while (csv.ReadNextRecord())
                {
                    var record = new ListViewItem();
                    record.Text = csv[0];
                    for (int i = 1; i < fieldCount; i++)
                    {
                        record.SubItems.Add(csv[i]);
                    }
                    lvOutput.Items.Add(record);
                }
            }
            lvOutput.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            UpdateFileInfo();
        }

        // Update file info labels
        private void UpdateFileInfo()
        {
            char[] separator = { '\\' };
            string[] nameArray = filePath.Split(separator);
            lblFile.Text = nameArray[nameArray.Length - 1];
            lblRows.Text = lvOutput.Items.Count.ToString();
        }

        // Update modify table when an item is selected
        private void lvOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateModifyTable();
            ClearModifyTable();
            UpdateStatus("", Color.Black);
        }

        // Add record to modify table
        private void UpdateModifyTable()
        {
            if (lvOutput.SelectedItems.Count != 0)
            {
                tblMod.Controls.Clear();
                tblMod.ColumnCount = headers.Length;
                tblMod.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                tblMod.RowStyles.Add(new RowStyle(SizeType.AutoSize));


                for (int i = 0; i < headers.Length; i++)
                {
                    tblMod.Controls.Add(new Label() { Text = headers[i] }, i, 0);

                }

                for (int i = 0; i < lvOutput.SelectedItems[0].SubItems.Count; i++)
                {
                    tblMod.Controls.Add(new TextBox() { Text = lvOutput.SelectedItems[0].SubItems[i].Text }, i, 1);

                    if (lvOutput.SelectedItems[0].SubItems[i].Text.Length > headers[i].Length)
                    {
                        lvOutput.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    }
                }
            }
        }

        // Modify record
        private void ModifyRecord()
        {

            if (lvOutput.SelectedItems.Count != 0)
            {
                // Get collection of text boxes from modify table
                List<Control> tblControls = new List<Control>();
                foreach (Control ctrl in tblMod.Controls)
                {
                    if (ctrl is TextBox)
                    {
                        tblControls.Add(ctrl);
                    }
                }

                // Update selected records in list view
                int count = 1;
                foreach (Control ctrl in tblControls)
                {
                    if (tblControls.IndexOf(ctrl) == 0)
                    {
                        lvOutput.SelectedItems[0].Text = ctrl.Text;
                    }
                    else
                    {
                        lvOutput.SelectedItems[0].SubItems[count].Text = ctrl.Text;
                        count++;
                    }
                }

                UpdateStatus("Record updated!", Color.Green);
            }
        }

        // Clear modify table
        private void ClearModifyTable()
        {
            if (lvOutput.SelectedItems.Count == 0)
            {
                foreach (Control ctrl in tblMod.Controls)
                {
                    if (ctrl is TextBox)
                    {
                        ctrl.Text = "";
                    }
                }
            }
        }

        // Update status message
        private void UpdateStatus(string message, Color color)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = color;
        }
    }
}
