using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace onkobuf {
    public partial class FormMain : Form {
        const string README_FILENAME = "readme.txt";

        DataTable tableDirections;

        public FormMain() {
            InitializeComponent();

            var query =
                from cs in model.Classifier.All
                    join ss in model.Stages.All on cs.Stage equals ss.ID
                    join ts in model.Tumors.All on cs.Tumor equals ts.ID
                    join ns in model.Nodules.All on cs.Nodus equals ns.ID
                    join ms in model.Metastases.All on cs.Metastasis equals ms.ID
                select new ClassesRecord {
                    ID = cs.ID,
                    Diagnosis = cs.Diagnosis,
                    Stage = ss.Code,
                    Tumor = ts.Code,
                    Nodus = ns.Code,
                    Metastasis = ms.Code,
                    Code = ClassesRecord.GetCode(cs.Stage, cs.Tumor, cs.Nodus, cs.Metastasis)
                };

            DataTable table = lib.DataTableHelper.ConvertToDatatable(query);
            sgData.DataSource = table;
            sgData.Columns["Code"].Visible = false;
            lblCaseCode.DataBindings.Add("Text", sgData.DataSource, "Code");

            tableDirections = lib.DataTableHelper.ConvertToDatatable(model.Directions.All);
            tableDirections.DefaultView.Sort = "Title";
            sgDirections.DataSource = tableDirections;
            lblDirection.DataBindings.Add("Text", sgDirections.DataSource, "ID");

            CultureInfo TypeOfLanguage = CultureInfo.CreateSpecificCulture("en-US");
            System.Threading.Thread.CurrentThread.CurrentCulture = TypeOfLanguage;
            InputLanguage l = InputLanguage.FromCulture(TypeOfLanguage);
            InputLanguage.CurrentInputLanguage = l;

            pcMain.SelectedIndex = 0;
            edICD.Select();
        }

        private void tbtnSearch_Click(object sender, EventArgs e) {
            if (pcMain.SelectedIndex == 0) {
                // Cxx.x
                lib.StageParser parsed = new lib.StageParser(edICD.Text);
                lblParsed.Text = parsed.ToString();

                DataTable table = lib.DataTableHelper.ConvertToDatatable(parsed.GetDataset());
                sgData.DataSource = table;
                lblCaseCode.DataBindings.Clear();
                lblCaseCode.DataBindings.Add("Text", sgData.DataSource, "Code");
            } else {
                // Z03.1
                string title = "(Title like '%" + edFilter.Text.Replace("'", "''") + "%')";
                if (tableDirections != null) tableDirections.DefaultView.RowFilter = title;
                edFilter.Focus();
                edFilter.SelectAll();
            }
        }
        
        private void FormMain_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.F2) {
                tbtnSearch_Click(null, null);
            }
        }

        private void pcMain_SelectedIndexChanged(object sender, EventArgs e) {
            switch (pcMain.SelectedIndex) {
                case 0:
                    edICD.Focus();
                    edICD.SelectAll();
                    break;

                case 1:
                    edFilter.Focus();
                    edFilter.SelectAll();
                    break;
            }
        }

        private void OnHelp(object sender, EventArgs e) {
            System.Diagnostics.Process.Start(Options.ResourceDirectory + README_FILENAME);
        }
    }

    class ClassesRecord {
        public int ID { get; set; }
        public string Diagnosis { get; set; }
        public string Stage { get; set; }
        public string Tumor { get; set; }
        public string Nodus { get; set; }
        public string Metastasis { get; set; }
        public string Code { get; set; }
        public int Rating { get; set; }

        public static string GetCode(int s, int t, int n, int m) {
            return string.Format("x{0}-{1}-{2}-{3}", s, t, n, m);
        }
    };
}
