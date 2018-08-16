namespace onkobuf {
    partial class FormMain {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tbtnSearch = new System.Windows.Forms.ToolStripButton();
            this.tbtnHelp = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.stbrMain = new System.Windows.Forms.ToolStripStatusLabel();
            this.edICD = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblCaseCode = new System.Windows.Forms.Label();
            this.sgData = new System.Windows.Forms.DataGridView();
            this.label6 = new System.Windows.Forms.Label();
            this.pcMain = new System.Windows.Forms.TabControl();
            this.tcDiagnosis = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.label2 = new System.Windows.Forms.Label();
            this.lblParsed = new System.Windows.Forms.Label();
            this.tcSuspect = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lblDirection = new System.Windows.Forms.Label();
            this.edFilter = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.sgDirections = new System.Windows.Forms.DataGridView();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sgData)).BeginInit();
            this.pcMain.SuspendLayout();
            this.tcDiagnosis.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tcSuspect.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sgDirections)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbtnSearch,
            this.tbtnHelp,
            this.toolStripSeparator1,
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(913, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tbtnSearch
            // 
            this.tbtnSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbtnSearch.Image = ((System.Drawing.Image)(resources.GetObject("tbtnSearch.Image")));
            this.tbtnSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnSearch.Name = "tbtnSearch";
            this.tbtnSearch.Size = new System.Drawing.Size(23, 22);
            this.tbtnSearch.Text = "Обновить";
            this.tbtnSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tbtnSearch.ToolTipText = "Обновить списки кодов по диагнозу (F2)";
            this.tbtnSearch.Click += new System.EventHandler(this.tbtnSearch_Click);
            // 
            // tbtnHelp
            // 
            this.tbtnHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbtnHelp.Image = ((System.Drawing.Image)(resources.GetObject("tbtnHelp.Image")));
            this.tbtnHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnHelp.Name = "tbtnHelp";
            this.tbtnHelp.Size = new System.Drawing.Size(23, 22);
            this.tbtnHelp.Text = "toolStripButton1";
            this.tbtnHelp.ToolTipText = "Показать справку";
            this.tbtnHelp.Click += new System.EventHandler(this.OnHelp);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stbrMain});
            this.statusStrip1.Location = new System.Drawing.Point(0, 533);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(913, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // stbrMain
            // 
            this.stbrMain.AutoToolTip = true;
            this.stbrMain.Name = "stbrMain";
            this.stbrMain.Size = new System.Drawing.Size(0, 17);
            // 
            // edICD
            // 
            this.edICD.Location = new System.Drawing.Point(49, 27);
            this.edICD.Name = "edICD";
            this.edICD.Size = new System.Drawing.Size(100, 20);
            this.edICD.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Поиск";
            // 
            // lblCaseCode
            // 
            this.lblCaseCode.AutoSize = true;
            this.lblCaseCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblCaseCode.ForeColor = System.Drawing.Color.DarkMagenta;
            this.lblCaseCode.Location = new System.Drawing.Point(108, 51);
            this.lblCaseCode.Name = "lblCaseCode";
            this.lblCaseCode.Size = new System.Drawing.Size(118, 24);
            this.lblCaseCode.TabIndex = 6;
            this.lblCaseCode.Text = "lblCaseCode";
            // 
            // sgData
            // 
            this.sgData.AllowUserToAddRows = false;
            this.sgData.AllowUserToDeleteRows = false;
            this.sgData.AllowUserToResizeRows = false;
            this.sgData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.sgData.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.sgData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sgData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sgData.Location = new System.Drawing.Point(0, 0);
            this.sgData.MultiSelect = false;
            this.sgData.Name = "sgData";
            this.sgData.ReadOnly = true;
            this.sgData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.sgData.ShowCellErrors = false;
            this.sgData.ShowCellToolTips = false;
            this.sgData.ShowEditingIcon = false;
            this.sgData.ShowRowErrors = false;
            this.sgData.Size = new System.Drawing.Size(899, 393);
            this.sgData.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 59);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Занести МЭС1";
            // 
            // pcMain
            // 
            this.pcMain.Controls.Add(this.tcDiagnosis);
            this.pcMain.Controls.Add(this.tcSuspect);
            this.pcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pcMain.Location = new System.Drawing.Point(0, 25);
            this.pcMain.Name = "pcMain";
            this.pcMain.SelectedIndex = 0;
            this.pcMain.Size = new System.Drawing.Size(913, 508);
            this.pcMain.TabIndex = 18;
            this.pcMain.SelectedIndexChanged += new System.EventHandler(this.pcMain_SelectedIndexChanged);
            // 
            // tcDiagnosis
            // 
            this.tcDiagnosis.Controls.Add(this.splitContainer2);
            this.tcDiagnosis.Location = new System.Drawing.Point(4, 22);
            this.tcDiagnosis.Name = "tcDiagnosis";
            this.tcDiagnosis.Padding = new System.Windows.Forms.Padding(3);
            this.tcDiagnosis.Size = new System.Drawing.Size(905, 482);
            this.tcDiagnosis.TabIndex = 0;
            this.tcDiagnosis.Text = "C00";
            this.tcDiagnosis.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.label2);
            this.splitContainer2.Panel1.Controls.Add(this.lblParsed);
            this.splitContainer2.Panel1.Controls.Add(this.label1);
            this.splitContainer2.Panel1.Controls.Add(this.label6);
            this.splitContainer2.Panel1.Controls.Add(this.edICD);
            this.splitContainer2.Panel1.Controls.Add(this.lblCaseCode);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.sgData);
            this.splitContainer2.Size = new System.Drawing.Size(899, 476);
            this.splitContainer2.SplitterDistance = 79;
            this.splitContainer2.TabIndex = 18;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(10, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(534, 17);
            this.label2.TabIndex = 20;
            this.label2.Text = "В поле поиска через пробел: код МКБ, клиническая стадия, значения T, N и M";
            // 
            // lblParsed
            // 
            this.lblParsed.AutoSize = true;
            this.lblParsed.Location = new System.Drawing.Point(165, 30);
            this.lblParsed.Name = "lblParsed";
            this.lblParsed.Size = new System.Drawing.Size(0, 13);
            this.lblParsed.TabIndex = 19;
            // 
            // tcSuspect
            // 
            this.tcSuspect.Controls.Add(this.splitContainer1);
            this.tcSuspect.Location = new System.Drawing.Point(4, 22);
            this.tcSuspect.Name = "tcSuspect";
            this.tcSuspect.Padding = new System.Windows.Forms.Padding(3);
            this.tcSuspect.Size = new System.Drawing.Size(905, 482);
            this.tcSuspect.TabIndex = 1;
            this.tcSuspect.Text = "Z03.1";
            this.tcSuspect.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lblDirection);
            this.splitContainer1.Panel1.Controls.Add(this.edFilter);
            this.splitContainer1.Panel1.Controls.Add(this.label7);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.sgDirections);
            this.splitContainer1.Size = new System.Drawing.Size(899, 476);
            this.splitContainer1.SplitterDistance = 61;
            this.splitContainer1.TabIndex = 3;
            // 
            // lblDirection
            // 
            this.lblDirection.AutoEllipsis = true;
            this.lblDirection.AutoSize = true;
            this.lblDirection.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblDirection.ForeColor = System.Drawing.Color.DarkMagenta;
            this.lblDirection.Location = new System.Drawing.Point(51, 26);
            this.lblDirection.Name = "lblDirection";
            this.lblDirection.Size = new System.Drawing.Size(92, 31);
            this.lblDirection.TabIndex = 3;
            this.lblDirection.Text = "label8";
            // 
            // edFilter
            // 
            this.edFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.edFilter.Location = new System.Drawing.Point(58, 3);
            this.edFilter.Name = "edFilter";
            this.edFilter.Size = new System.Drawing.Size(836, 20);
            this.edFilter.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Фильтр";
            // 
            // sgDirections
            // 
            this.sgDirections.AllowUserToAddRows = false;
            this.sgDirections.AllowUserToOrderColumns = true;
            this.sgDirections.AllowUserToResizeRows = false;
            this.sgDirections.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.sgDirections.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.sgDirections.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sgDirections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sgDirections.Location = new System.Drawing.Point(0, 0);
            this.sgDirections.MultiSelect = false;
            this.sgDirections.Name = "sgDirections";
            this.sgDirections.ReadOnly = true;
            this.sgDirections.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.sgDirections.ShowEditingIcon = false;
            this.sgDirections.Size = new System.Drawing.Size(899, 411);
            this.sgDirections.TabIndex = 1;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "Выйти из программы";
            this.toolStripButton1.Click += new System.EventHandler(this.OnExit);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(913, 555);
            this.Controls.Add(this.pcMain);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Онкология: коды";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormMain_KeyDown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sgData)).EndInit();
            this.pcMain.ResumeLayout(false);
            this.tcDiagnosis.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tcSuspect.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sgDirections)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tbtnSearch;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel stbrMain;
        private System.Windows.Forms.TextBox edICD;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblCaseCode;
        private System.Windows.Forms.DataGridView sgData;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TabControl pcMain;
        private System.Windows.Forms.TabPage tcDiagnosis;
        private System.Windows.Forms.TabPage tcSuspect;
        private System.Windows.Forms.TextBox edFilter;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DataGridView sgDirections;
        private System.Windows.Forms.Label lblDirection;
        private System.Windows.Forms.Label lblParsed;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripButton tbtnHelp;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
    }
}

