using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace onkobuf.lib {
    class DataTableHelper {
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
