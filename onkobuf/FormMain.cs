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
        DataTable table;
        string filter;

        public FormMain() {
            InitializeComponent();
            cmbStage.DataSource = model.Stages.byDiagnosis(string.Empty);
            cmbStage.DisplayMember = "DiagnosisCode";
            cmbStage.ValueMember = "ID";

            cmbTumor.DataSource = model.Tumors.byDiagnosis(string.Empty);
            cmbTumor.DisplayMember = "DiagnosisCode";
            cmbTumor.ValueMember = "ID";

            cmbNodus.DataSource = model.Nodules.byDiagnosis(string.Empty);
            cmbNodus.DisplayMember = "DiagnosisCode";
            cmbNodus.ValueMember = "ID";

            cmbMetastases.DataSource = model.Metastases.byDiagnosis(string.Empty);
            cmbMetastases.DisplayMember = "DiagnosisCode";
            cmbMetastases.ValueMember = "ID";

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

            table = lib.DataTableHelper.ConvertToDatatable(query);
            sgData.DataSource = table;
            sgData.Columns["Code"].Visible = false;
            lblCaseCode.DataBindings.Add("Text", sgData.DataSource, "Code");

            CultureInfo TypeOfLanguage = CultureInfo.CreateSpecificCulture("en-US");
            System.Threading.Thread.CurrentThread.CurrentCulture = TypeOfLanguage;
            InputLanguage l = InputLanguage.FromCulture(TypeOfLanguage);
            InputLanguage.CurrentInputLanguage = l;

            edICD.Focus();
        }

        void BuildFilter() {
            filter = "(Diagnosis like '%" + edICD.Text.Replace("'", "''") + "%')";

            model.Stage s = (model.Stage)cmbStage.SelectedItem;
            if (s != null) filter += " and (Stage = '" + s.Code + "')";

            model.Tumor t = (model.Tumor)cmbTumor.SelectedItem;
            if (t != null) filter += " and (Tumor = '" + t.Code + "')";

            model.Nodus n = (model.Nodus)cmbNodus.SelectedItem;
            if (n != null) filter += " and (Nodus = '" + n.Code + "')";

            model.Metastasis m = (model.Metastasis)cmbMetastases.SelectedItem;
            if (m != null) filter += " and (Metastasis = '" + m.Code + "')";
        }

        private void UpdateCode() {
            if (table != null) table.DefaultView.RowFilter = filter;
        }

        private void tbtnSearch_Click(object sender, EventArgs e) {
            DataTable t = table;
            table = null;

            cmbStage.DataSource = model.Stages.byDiagnosis(edICD.Text);
            cmbTumor.DataSource = model.Tumors.byDiagnosis(edICD.Text);
            cmbNodus.DataSource = model.Nodules.byDiagnosis(edICD.Text);
            cmbMetastases.DataSource = model.Metastases.byDiagnosis(edICD.Text);

            cmbStage.SelectedItem = null;
            cmbTumor.SelectedItem = null;
            cmbNodus.SelectedItem = null;
            table = t;
            cmbMetastases.SelectedItem = null;
        }

        private void cmbStage_SelectedIndexChanged(object sender, EventArgs e) {
            BuildFilter();
            UpdateCode();
        }

        private void cmbTumor_SelectedIndexChanged(object sender, EventArgs e) {
            model.Tumor t = (model.Tumor)cmbTumor.SelectedItem;
            if (t != null)
                lblTumor.Text = t.Title;
            else
                lblTumor.Text = "нет";
            BuildFilter();
            UpdateCode();
        }

        private void cmbNodus_SelectedIndexChanged(object sender, EventArgs e) {
            model.Nodus n = (model.Nodus)cmbNodus.SelectedItem;
            if (n != null)
                lblNodules.Text = n.Title;
            else
                lblNodules.Text = "нет";
            BuildFilter();
            UpdateCode();
        }

        private void cmbMetastases_SelectedIndexChanged(object sender, EventArgs e) {
            model.Metastasis n = (model.Metastasis)cmbMetastases.SelectedItem;
            if (n != null)
                lblMetastases.Text = n.Title;
            else
                lblMetastases.Text = "нет";
            BuildFilter();
            UpdateCode();
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.F2) {
                tbtnSearch_Click(null, null);
            }
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

        public static string GetCode(int s, int t, int n, int m) {
            return string.Format("x{0}-{1}-{2}-{3}", s, t, n, m);
        }
    };
}
