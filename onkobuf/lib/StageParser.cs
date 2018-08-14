using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace onkobuf.lib {
    class StageParser {
        string icd;
        string stage;
        string tumor;
        string nodus;
        string mts;

        public string Diagnosis { get { return icd; } }
        public string Stage { get { return stage; } }
        public string Tumor { get { return tumor; } }
        public string Nodus { get { return nodus; } }
        public string Metastasis { get { return mts; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line">A string to extract stage and TNM classification from</param>
        /// <value>
        /// Instances:
        /// C50.0 4a t1a1 n1a m0
        /// C16 IIb T1N0MX
        /// etc.
        /// 
        /// 1. Space(s) after ICD code is (are) required
        /// 2. Stage, tumor, nodules, metastases fields may be separated by spaces or not
        /// 3. T, N, M letters in the classification are mandatory (if correspondent part is present) unless spaces are used to
        ///   tell one part from another
        /// 4. Any part of the classification (and diagnosis also) may be omited
        /// 5. Stage number may be given either in roman or arabic digits
        /// </value>
        public StageParser(string line) {
            // Line could be regexped as
            // c?\d\d(\.\d)?\s+([^\s]+)t([0-4](a[12]?|b|c)|a|is|x)\s*n([0123][abc]?|x)\s*m([01][abc]?|x)
            Parse(line.ToUpper());
        }

        string Secure(string value) {
            if (string.IsNullOrEmpty(value)) return value;

            return new string (value.Where(c => c != '\'').ToArray());
        }

        void SecureFields() {
            icd = Secure(icd);
            stage = Secure(stage);
            tumor = Secure(tumor);
            nodus = Secure(nodus);
            mts = Secure(mts);
        }

        // Helper to form StageParser fields from a string
        void Parse(string line) {
            // Separate ICD code
            int pos = line.IndexOf(' ');
            if (pos < 0) return;

            icd = line.Substring(0, pos);
            line = line.Substring(pos + 1).TrimStart();

            // Make ICD code like Cxx
            if (!string.IsNullOrEmpty(icd) && char.IsDigit(icd.First()))
                icd = 'C' + icd;

            pos = icd.IndexOf('.');
            if (pos > 0) icd = icd.Substring(0, pos);

            // Look for stage fields
            int t = line.IndexOf('T');
            int n = line.IndexOf('N');
            int m = line.IndexOf('M');

            if (t < 0 && n < 0 && m < 0) {
                // No fields found, supposing they are separated with spaces
                string[] parts = line.Split(' ').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToArray();
                int len = parts.Count();
                if (len > 0) stage = parts[0];
                if (len > 1) tumor = parts[1];
                if (len > 2) nodus = parts[2];
                if (len > 3) mts = parts[3];
            } else {
                // Extract class fields
                if (t >= 0) {
                    stage = line.Substring(0, t).Trim();

                    pos = n < 0 ? m : n;
                    if (pos < 0)
                        tumor = line.Substring(t).Trim();
                    else
                        tumor = line.Substring(t, pos - t).Trim();
                }

                if (n >= 0) {
                    if (t < 0)
                        stage = line.Substring(0, n).Trim();

                    if (m < 0)
                        nodus = line.Substring(n).Trim();
                    else
                        nodus = line.Substring(n, m - n).Trim();
                }

                if (m >= 0) {
                    if (t < 0 && n < 0)
                        stage = line.Substring(0, m).Trim();

                    mts = line.Substring(m).Trim();
                }
            }

            // Clinical stage to roman digits:
            if (!string.IsNullOrEmpty(stage) && char.IsDigit(stage.First())) {
                switch (stage.First()) {
                    case '0':
                        break;

                    case '1':
                        stage = 'I' + stage.Substring(1);
                        break;

                    case '2':
                        stage = "II" + stage.Substring(1);
                        break;

                    case '3':
                        stage = "III" + stage.Substring(1);
                        break;

                    case '4':
                        stage = "IV" + stage.Substring(1);
                        break;

                    default:
                        // Wrong stage
                        stage = string.Empty;
                        break;
                }
            }

            // Prettying up TNM fields
            if (!string.IsNullOrEmpty(tumor)) {
                if (tumor.First() != 'T')
                    tumor = 'T' + tumor.ToLower();
                else
                    tumor = 'T' + tumor.Substring(1).ToLower();
            }

            if (!string.IsNullOrEmpty(nodus)) {
                if (nodus.First() != 'N')
                    nodus = 'N' + nodus.ToLower();
                else
                    nodus = 'N' + nodus.Substring(1).ToLower();
            }

            if (!string.IsNullOrEmpty(mts)) {
                if (mts.First() != 'M')
                    mts = 'M' + mts;
                else
                    mts = 'M' + mts.Substring(1).ToLower();
            }

            SecureFields();
        }

        public override string ToString() {
            return string.Format("Диагноз {0}, стадия {1}, {2} {3} {4}", Diagnosis, Stage, Tumor, Nodus, Metastasis);
        }

        public string GetFilter(DataTable table) {
            if (table == null) return string.Empty;

            string filter = string.Format("(Diagnosis = '{0}')", icd);

            int count = table.Select(filter).Count();
            if (count == 0)
                filter = "(Diagnosis = '')";

            var query =
                from cs in model.Classifier.All
                join ss in model.Stages.All on cs.Stage equals ss.ID
                join ts in model.Tumors.All on cs.Tumor equals ts.ID
                join ns in model.Nodules.All on cs.Nodus equals ns.ID
                join ms in model.Metastases.All on cs.Metastasis equals ms.ID
                where cs.Diagnosis.ToUpper() == icd
                select new ClassesRecord {
                    ID = cs.ID,
                    Diagnosis = cs.Diagnosis,
                    Stage = ss.Code,
                    Tumor = ts.Code,
                    Nodus = ns.Code,
                    Metastasis = ms.Code,
                    // TODODODO
                    Code = ClassesRecord.GetCode(cs.Stage, cs.Tumor, cs.Nodus, cs.Metastasis)
                };

            //rows[0]["Stage"]

            //model.Stage s = (model.Stage)cmbStage.SelectedItem;
            //if (s != null) filter += " and (Stage = '" + s.Code + "')";

            //model.Tumor t = (model.Tumor)cmbTumor.SelectedItem;
            //if (t != null) filter += " and (Tumor = '" + t.Code + "')";

            //model.Nodus n = (model.Nodus)cmbNodus.SelectedItem;
            //if (n != null) filter += " and (Nodus = '" + n.Code + "')";

            //model.Metastasis m = (model.Metastasis)cmbMetastases.SelectedItem;
            //if (m != null) filter += " and (Metastasis = '" + m.Code + "')";

            return filter;
        }
    }
}
