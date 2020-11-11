using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Qucs_Data_Exporter
{
    public partial class Form1 : Form
    {

        string fileData;
        string pattern = @"<(dep|indep) (.*) (.*)>\n((.|\n)+?)</\\1>";
        List<QucsSimData> simDatas = new List<QucsSimData>();

        public Form1()
        {
            InitializeComponent();
            Console.WriteLine();
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var sr = new StreamReader(openFileDialog1.FileName);
                    fileData = sr.ReadToEnd();
                    if(Regex.IsMatch(fileData, "<Qucs Dataset [0-9]+\\.[0-9]+\\.[0-9]+>"))
                    {
                        Console.WriteLine("Arquivo do Qucs");
                        simDatas.Clear();
                        checkedListBox1.Items.Clear();
                        Console.WriteLine(Regex.Match(fileData, "<(dep|indep) (.*) (.*)>\\r?\\n").Success);
                        foreach (Match m in Regex.Matches(fileData, "<(dep|indep) (.*) (.*)>\\r?\\n((.|\\n)+?)</\\1>")){
                            simDatas.Add(new QucsSimData(m.Groups));
                            checkedListBox1.Items.Add(m.Groups[2], CheckState.Unchecked);
                        }
                        //Console.WriteLine(simDatas[0].Name);
                    }


                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        private void cSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CultureInfo oldC = CultureInfo.CurrentCulture;
            //CultureInfo.CurrentCulture = new CultureInfo("en-US");
            List<QucsSimData> toExport = new List<QucsSimData>();
            int rows = 0;
            int columns = 0;
            var csv = new StringBuilder();
            foreach(object item in checkedListBox1.CheckedItems)
            {
                QucsSimData data = simDatas.Single(x => x.Name == item.ToString());
                toExport.Add(data);
                columns += 1;
                if (rows < data.Values.Length)
                    rows = data.Values.Length;
            }

            for(int i = 0; i < columns; i++)
            {
                csv.Append(toExport[i].Name);
                if (i != columns - 1)
                    csv.Append(';');
            }
            csv.AppendLine("");

            for(int i = 0; i < rows; i++)
            {
                for(int j = 0; j < columns; j++)
                {
                    csv.Append(toExport[j].Values.Length < i ? "" : toExport[j].Values[i].ToString());
                    if (j != columns - 1)
                        csv.Append(';');
                }
                csv.AppendLine("");
            }
            string outstr = csv.ToString();
            CultureInfo.CurrentCulture = oldC;
            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog1.FileName, outstr);
            }
            Console.WriteLine("DONE");
        }
    }
}
