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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.stbrMain = new System.Windows.Forms.ToolStripStatusLabel();
            this.edICD = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbStage = new System.Windows.Forms.ComboBox();
            this.lblCaseCode = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbTumor = new System.Windows.Forms.ComboBox();
            this.lblTumor = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbNodus = new System.Windows.Forms.ComboBox();
            this.lblNodules = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbMetastases = new System.Windows.Forms.ComboBox();
            this.lblMetastases = new System.Windows.Forms.Label();
            this.sgData = new System.Windows.Forms.DataGridView();
            this.label6 = new System.Windows.Forms.Label();
            this.pcMain = new System.Windows.Forms.TabControl();
            this.tcDiagnosis = new System.Windows.Forms.TabPage();
            this.tcSuspect = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.edFilter = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.sgDirections = new System.Windows.Forms.DataGridView();
            this.lblDirection = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sgData)).BeginInit();
            this.pcMain.SuspendLayout();
            this.tcDiagnosis.SuspendLayout();
            this.tcSuspect.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sgDirections)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbtnSearch});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(507, 25);
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
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stbrMain});
            this.statusStrip1.Location = new System.Drawing.Point(0, 495);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(507, 22);
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
            this.edICD.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.edICD.Location = new System.Drawing.Point(61, 11);
            this.edICD.Name = "edICD";
            this.edICD.Size = new System.Drawing.Size(100, 20);
            this.edICD.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "К&од МКБ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Стадия";
            // 
            // cmbStage
            // 
            this.cmbStage.AllowDrop = true;
            this.cmbStage.FormattingEnabled = true;
            this.cmbStage.Location = new System.Drawing.Point(61, 35);
            this.cmbStage.Name = "cmbStage";
            this.cmbStage.Size = new System.Drawing.Size(100, 21);
            this.cmbStage.TabIndex = 5;
            this.cmbStage.SelectedIndexChanged += new System.EventHandler(this.cmbStage_SelectedIndexChanged);
            // 
            // lblCaseCode
            // 
            this.lblCaseCode.AutoSize = true;
            this.lblCaseCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblCaseCode.ForeColor = System.Drawing.Color.DarkMagenta;
            this.lblCaseCode.Location = new System.Drawing.Point(84, 136);
            this.lblCaseCode.Name = "lblCaseCode";
            this.lblCaseCode.Size = new System.Drawing.Size(118, 24);
            this.lblCaseCode.TabIndex = 6;
            this.lblCaseCode.Text = "lblCaseCode";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Tumor";
            // 
            // cmbTumor
            // 
            this.cmbTumor.FormattingEnabled = true;
            this.cmbTumor.Location = new System.Drawing.Point(61, 60);
            this.cmbTumor.Name = "cmbTumor";
            this.cmbTumor.Size = new System.Drawing.Size(100, 21);
            this.cmbTumor.TabIndex = 8;
            this.cmbTumor.SelectedIndexChanged += new System.EventHandler(this.cmbTumor_SelectedIndexChanged);
            // 
            // lblTumor
            // 
            this.lblTumor.AutoSize = true;
            this.lblTumor.Location = new System.Drawing.Point(167, 63);
            this.lblTumor.Name = "lblTumor";
            this.lblTumor.Size = new System.Drawing.Size(35, 13);
            this.lblTumor.TabIndex = 9;
            this.lblTumor.Text = "label4";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Nodus";
            // 
            // cmbNodus
            // 
            this.cmbNodus.FormattingEnabled = true;
            this.cmbNodus.Location = new System.Drawing.Point(61, 85);
            this.cmbNodus.Name = "cmbNodus";
            this.cmbNodus.Size = new System.Drawing.Size(100, 21);
            this.cmbNodus.TabIndex = 11;
            this.cmbNodus.SelectedIndexChanged += new System.EventHandler(this.cmbNodus_SelectedIndexChanged);
            // 
            // lblNodules
            // 
            this.lblNodules.AutoSize = true;
            this.lblNodules.Location = new System.Drawing.Point(167, 88);
            this.lblNodules.Name = "lblNodules";
            this.lblNodules.Size = new System.Drawing.Size(35, 13);
            this.lblNodules.TabIndex = 12;
            this.lblNodules.Text = "label5";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Metastases";
            // 
            // cmbMetastases
            // 
            this.cmbMetastases.FormattingEnabled = true;
            this.cmbMetastases.Location = new System.Drawing.Point(61, 110);
            this.cmbMetastases.Name = "cmbMetastases";
            this.cmbMetastases.Size = new System.Drawing.Size(100, 21);
            this.cmbMetastases.TabIndex = 14;
            this.cmbMetastases.SelectedIndexChanged += new System.EventHandler(this.cmbMetastases_SelectedIndexChanged);
            // 
            // lblMetastases
            // 
            this.lblMetastases.AutoSize = true;
            this.lblMetastases.Location = new System.Drawing.Point(167, 113);
            this.lblMetastases.Name = "lblMetastases";
            this.lblMetastases.Size = new System.Drawing.Size(35, 13);
            this.lblMetastases.TabIndex = 15;
            this.lblMetastases.Text = "label6";
            // 
            // sgData
            // 
            this.sgData.AllowUserToAddRows = false;
            this.sgData.AllowUserToDeleteRows = false;
            this.sgData.AllowUserToResizeRows = false;
            this.sgData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
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
            this.sgData.Size = new System.Drawing.Size(493, 269);
            this.sgData.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 144);
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
            this.pcMain.Size = new System.Drawing.Size(507, 470);
            this.pcMain.TabIndex = 18;
            this.pcMain.SelectedIndexChanged += new System.EventHandler(this.pcMain_SelectedIndexChanged);
            // 
            // tcDiagnosis
            // 
            this.tcDiagnosis.Controls.Add(this.splitContainer2);
            this.tcDiagnosis.Location = new System.Drawing.Point(4, 22);
            this.tcDiagnosis.Name = "tcDiagnosis";
            this.tcDiagnosis.Padding = new System.Windows.Forms.Padding(3);
            this.tcDiagnosis.Size = new System.Drawing.Size(499, 444);
            this.tcDiagnosis.TabIndex = 0;
            this.tcDiagnosis.Text = "C00";
            this.tcDiagnosis.UseVisualStyleBackColor = true;
            // 
            // tcSuspect
            // 
            this.tcSuspect.Controls.Add(this.splitContainer1);
            this.tcSuspect.Location = new System.Drawing.Point(4, 22);
            this.tcSuspect.Name = "tcSuspect";
            this.tcSuspect.Padding = new System.Windows.Forms.Padding(3);
            this.tcSuspect.Size = new System.Drawing.Size(499, 444);
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
            this.splitContainer1.Size = new System.Drawing.Size(493, 438);
            this.splitContainer1.SplitterDistance = 61;
            this.splitContainer1.TabIndex = 3;
            // 
            // edFilter
            // 
            this.edFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.edFilter.Location = new System.Drawing.Point(58, 3);
            this.edFilter.Name = "edFilter";
            this.edFilter.Size = new System.Drawing.Size(430, 20);
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
            this.splitContainer2.Panel1.Controls.Add(this.label1);
            this.splitContainer2.Panel1.Controls.Add(this.lblTumor);
            this.splitContainer2.Panel1.Controls.Add(this.label6);
            this.splitContainer2.Panel1.Controls.Add(this.cmbTumor);
            this.splitContainer2.Panel1.Controls.Add(this.edICD);
            this.splitContainer2.Panel1.Controls.Add(this.label4);
            this.splitContainer2.Panel1.Controls.Add(this.lblMetastases);
            this.splitContainer2.Panel1.Controls.Add(this.label3);
            this.splitContainer2.Panel1.Controls.Add(this.cmbNodus);
            this.splitContainer2.Panel1.Controls.Add(this.cmbMetastases);
            this.splitContainer2.Panel1.Controls.Add(this.lblCaseCode);
            this.splitContainer2.Panel1.Controls.Add(this.label2);
            this.splitContainer2.Panel1.Controls.Add(this.lblNodules);
            this.splitContainer2.Panel1.Controls.Add(this.label5);
            this.splitContainer2.Panel1.Controls.Add(this.cmbStage);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.sgData);
            this.splitContainer2.Size = new System.Drawing.Size(493, 438);
            this.splitContainer2.SplitterDistance = 165;
            this.splitContainer2.TabIndex = 18;
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
            this.sgDirections.Size = new System.Drawing.Size(493, 373);
            this.sgDirections.TabIndex = 1;
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
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(507, 517);
            this.Controls.Add(this.pcMain);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Онкология: коды";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormMain_KeyDown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sgData)).EndInit();
            this.pcMain.ResumeLayout(false);
            this.tcDiagnosis.ResumeLayout(false);
            this.tcSuspect.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbStage;
        private System.Windows.Forms.Label lblCaseCode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbTumor;
        private System.Windows.Forms.Label lblTumor;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbNodus;
        private System.Windows.Forms.Label lblNodules;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbMetastases;
        private System.Windows.Forms.Label lblMetastases;
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
    }
}

