using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace onkobuf {
    public partial class FormMain : Form {
        const string README_FILENAME = "readme.txt";

        FormNag nagging = null;
        DataTable tableDirections;

        public FormMain() {
            InitializeComponent();

            nagging = new FormNag();
            nagging.Show(this);

            var query =
                from ss in model.Stages.All
                join ts in model.Tumors.All on ss.Diagnosis equals ts.Diagnosis
                join ns in model.Nodules.All on ss.Diagnosis equals ns.Diagnosis
                join ms in model.Metastases.All on ss.Diagnosis equals ms.Diagnosis
                select new ClassesRecord {
                    ID = 0,
                    Diagnosis = ts.Diagnosis,
                    Stage = ss.Code,
                    StageArabic = ss.CodeArabic,
                    Tumor = ts.Code,
                    Nodus = ns.Code,
                    Metastasis = ms.Code,
                    Code = ClassesRecord.GetCode(ss.ID, ts.ID, ns.ID, ms.ID)
                };

            // Load directions' table with options
            DataTable table = lib.DataTableHelper.ConvertToDatatable(query.Take(100));
            sgData.DataSource = table;
            sgData.Columns["Code"].Visible = false;
            lblCaseCode.DataBindings.Add("Text", sgData.DataSource, "Code");

            // Preload stage-coding table
            tableDirections = lib.DataTableHelper.ConvertToDatatable(model.Directions.All);
            tableDirections.DefaultView.Sort = "Title";
            sgDirections.DataSource = tableDirections;
            lblDirection.DataBindings.Add("Text", sgDirections.DataSource, "ID");

            // Keyboard to latin:
            CultureInfo TypeOfLanguage = CultureInfo.CreateSpecificCulture("en-US");
            System.Threading.Thread.CurrentThread.CurrentCulture = TypeOfLanguage;
            InputLanguage l = InputLanguage.FromCulture(TypeOfLanguage);
            InputLanguage.CurrentInputLanguage = l;

            // Select first tab
            pcMain.SelectedIndex = 0;
            edICD.Select();
        }

        private void tbtnSearch_Click(object sender, EventArgs e) {
            // Make search
            if (pcMain.SelectedIndex == 0) {
                // Cxx.x
                lib.StageParser parsed = new lib.StageParser(edICD.Text);
                lblParsed.Text = parsed.ToString();

                DataTable table = lib.DataTableHelper.ConvertToDatatable(parsed.GetDataset());
                sgData.DataSource = table;
                lblCaseCode.DataBindings.Clear();
                lblCaseCode.DataBindings.Add("Text", sgData.DataSource, "Code");

                foreach (DataGridViewRow row in sgData.Rows) {
                    if ((int)row.Cells["Rating"].Value == parsed.MaximumRating)
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.PaleGreen;
                }
            } else {
                // Z03.1
                string title = "(Title like '%" + edFilter.Text.Replace("'", "''") + "%')";
                if (tableDirections != null) tableDirections.DefaultView.RowFilter = title;
                edFilter.Focus();
                edFilter.SelectAll();
            }
        }
        
        // Hotkey for search
        private void FormMain_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.F2) {
                tbtnSearch_Click(null, null);
            }
        }

        // Bring focus to search field on page control's tab change
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

        // Show readme
        private void OnHelp(object sender, EventArgs e) {
            System.Diagnostics.Process.Start(Options.ResourceDirectory + README_FILENAME);
        }

        private void FormMain_Load(object sender, EventArgs e) {
            if (nagging != null) {
                nagging.Close();
                nagging = null;
            }
        }

        private void OnExit(object sender, EventArgs e) {
            Close();
        }
    }

    /// <summary>
    /// POCO model for classification records
    /// </summary>
    class ClassesRecord {
        public int ID { get; set; }
        public string Diagnosis { get; set; }
        public string Stage { get; set; }
        public string StageArabic { get; set; }
        public string Tumor { get; set; }
        public string Nodus { get; set; }
        public string Metastasis { get; set; }
        public string Code { get; set; }
        public int Rating { get; set; }

        /// <summary>
        /// Compose RELAX-compatible code out of dictionary ID's
        /// </summary>
        /// <param name="s">ID of the clinical stage</param>
        /// <param name="t">ID of tumor</param>
        /// <param name="n">ID of nodulus</param>
        /// <param name="m">ID of metastases</param>
        /// <returns>Code for pasting to RELAXes KSG field</returns>
        public static string GetCode(int s, int t, int n, int m) {
            return string.Format("x{0}-{1}-{2}-{3}", s, t, n, m);
        }
    };
}
