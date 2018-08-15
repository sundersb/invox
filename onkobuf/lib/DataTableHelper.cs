using System.Collections.Generic;
using System.Data;

namespace onkobuf.lib {
    /// <summary>
    /// List of something to DataTable convertion helper
    /// </summary>
    class DataTableHelper {
        /// <summary>
        /// Convert list of ClassesRecord to DataTable representation
        /// </summary>
        public static DataTable ConvertToDatatable(IEnumerable<ClassesRecord> list) {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Diagnosis", typeof(string));
            dt.Columns.Add("Stage", typeof(string));
            dt.Columns.Add("Tumor", typeof(string));
            dt.Columns.Add("Nodus", typeof(string));
            dt.Columns.Add("Metastasis", typeof(string));
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("Rating", typeof(int));

            foreach (var item in list) {
                var row = dt.NewRow();

                row["ID"] = item.ID;
                row["Diagnosis"] = item.Diagnosis;
                row["Stage"] = item.Stage;
                row["Tumor"] = item.Tumor;
                row["Nodus"] = item.Nodus;
                row["Metastasis"] = item.Metastasis;
                row["Code"] = item.Code;
                row["Rating"] = item.Rating;

                dt.Rows.Add(row);
            }

            return dt;
        }

        /// <summary>
        /// Convert list of directions to DataTable
        /// </summary>
        public static DataTable ConvertToDatatable(IEnumerable<model.Direction> list) {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Title", typeof(string));

            foreach (var item in list) {
                var row = dt.NewRow();

                row["ID"] = item.ID;
                row["Title"] = item.Title;

                dt.Rows.Add(row);
            }

            return dt;
        }
    }
}
