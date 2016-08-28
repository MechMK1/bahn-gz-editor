namespace BahnEditor.WinForms
{
	partial class AnimationForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dataGridView = new System.Windows.Forms.DataGridView();
			this.phaseColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.minTimeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.maxTimeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.soundColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.noAnimationLabel = new System.Windows.Forms.Label();
			this.addAnimationStepButton = new System.Windows.Forms.Button();
			this.deleteAnimationStepButton = new System.Windows.Forms.Button();
			this.upAnimationStepButton = new System.Windows.Forms.Button();
			this.downAnimationStepButton = new System.Windows.Forms.Button();
			this.createAnimationProgramButton = new System.Windows.Forms.Button();
			this.xNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.xLabel = new System.Windows.Forms.Label();
			this.yLabel = new System.Windows.Forms.Label();
			this.yNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.widthLabel = new System.Windows.Forms.Label();
			this.widthNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.heightLabel = new System.Windows.Forms.Label();
			this.heightNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.deleteAnimationButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.xNumericUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.yNumericUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.widthNumericUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.heightNumericUpDown)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGridView
			// 
			this.dataGridView.AllowDrop = true;
			this.dataGridView.AllowUserToAddRows = false;
			this.dataGridView.AllowUserToResizeColumns = false;
			this.dataGridView.AllowUserToResizeRows = false;
			this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.phaseColumn,
            this.minTimeColumn,
            this.maxTimeColumn,
            this.soundColumn});
			this.dataGridView.Location = new System.Drawing.Point(12, 41);
			this.dataGridView.MultiSelect = false;
			this.dataGridView.Name = "dataGridView";
			this.dataGridView.RowHeadersWidth = 20;
			this.dataGridView.Size = new System.Drawing.Size(371, 279);
			this.dataGridView.TabIndex = 0;
			this.dataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView_CellValidating);
			this.dataGridView.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dataGridView_UserDeletingRow);
			this.dataGridView.DragDrop += new System.Windows.Forms.DragEventHandler(this.dataGridView_DragDrop);
			this.dataGridView.DragOver += new System.Windows.Forms.DragEventHandler(this.dataGridView_DragOver);
			this.dataGridView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView_MouseDown);
			this.dataGridView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dataGridView_MouseMove);
			// 
			// phaseColumn
			// 
			this.phaseColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.phaseColumn.HeaderText = "Animationphase";
			this.phaseColumn.Name = "phaseColumn";
			this.phaseColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// minTimeColumn
			// 
			this.minTimeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.minTimeColumn.HeaderText = "Minimum Time";
			this.minTimeColumn.Name = "minTimeColumn";
			this.minTimeColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// maxTimeColumn
			// 
			this.maxTimeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.maxTimeColumn.HeaderText = "Maximum Time";
			this.maxTimeColumn.Name = "maxTimeColumn";
			this.maxTimeColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// soundColumn
			// 
			this.soundColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.soundColumn.FillWeight = 50F;
			this.soundColumn.HeaderText = "Sound";
			this.soundColumn.Name = "soundColumn";
			this.soundColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// noAnimationLabel
			// 
			this.noAnimationLabel.AutoSize = true;
			this.noAnimationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.noAnimationLabel.Location = new System.Drawing.Point(13, 13);
			this.noAnimationLabel.Name = "noAnimationLabel";
			this.noAnimationLabel.Size = new System.Drawing.Size(308, 18);
			this.noAnimationLabel.TabIndex = 1;
			this.noAnimationLabel.Text = "No animation defined for the selected graphic.";
			// 
			// addAnimationStepButton
			// 
			this.addAnimationStepButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.addAnimationStepButton.Location = new System.Drawing.Point(391, 41);
			this.addAnimationStepButton.Name = "addAnimationStepButton";
			this.addAnimationStepButton.Size = new System.Drawing.Size(23, 23);
			this.addAnimationStepButton.TabIndex = 3;
			this.addAnimationStepButton.Text = "+";
			this.addAnimationStepButton.UseVisualStyleBackColor = true;
			this.addAnimationStepButton.Click += new System.EventHandler(this.addAnimationStepButton_Click);
			// 
			// deleteAnimationStepButton
			// 
			this.deleteAnimationStepButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.deleteAnimationStepButton.Location = new System.Drawing.Point(391, 70);
			this.deleteAnimationStepButton.Name = "deleteAnimationStepButton";
			this.deleteAnimationStepButton.Size = new System.Drawing.Size(23, 23);
			this.deleteAnimationStepButton.TabIndex = 4;
			this.deleteAnimationStepButton.Text = "-";
			this.deleteAnimationStepButton.UseVisualStyleBackColor = true;
			this.deleteAnimationStepButton.Click += new System.EventHandler(this.deleteAnimationStepButton_Click);
			// 
			// upAnimationStepButton
			// 
			this.upAnimationStepButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.upAnimationStepButton.BackgroundImage = global::BahnEditor.WinForms.Properties.Resources.uparrow;
			this.upAnimationStepButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.upAnimationStepButton.Location = new System.Drawing.Point(391, 99);
			this.upAnimationStepButton.Name = "upAnimationStepButton";
			this.upAnimationStepButton.Size = new System.Drawing.Size(23, 23);
			this.upAnimationStepButton.TabIndex = 5;
			this.upAnimationStepButton.UseVisualStyleBackColor = true;
			this.upAnimationStepButton.Click += new System.EventHandler(this.upAnimationStepButton_Click);
			// 
			// downAnimationStepButton
			// 
			this.downAnimationStepButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.downAnimationStepButton.BackgroundImage = global::BahnEditor.WinForms.Properties.Resources.downarrow;
			this.downAnimationStepButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.downAnimationStepButton.Location = new System.Drawing.Point(391, 128);
			this.downAnimationStepButton.Name = "downAnimationStepButton";
			this.downAnimationStepButton.Size = new System.Drawing.Size(23, 23);
			this.downAnimationStepButton.TabIndex = 6;
			this.downAnimationStepButton.UseVisualStyleBackColor = true;
			this.downAnimationStepButton.Click += new System.EventHandler(this.downAnimationStepButton_Click);
			// 
			// createAnimationProgramButton
			// 
			this.createAnimationProgramButton.Location = new System.Drawing.Point(16, 41);
			this.createAnimationProgramButton.Name = "createAnimationProgramButton";
			this.createAnimationProgramButton.Size = new System.Drawing.Size(241, 23);
			this.createAnimationProgramButton.TabIndex = 7;
			this.createAnimationProgramButton.Text = "Create new animationprogram for this layer";
			this.createAnimationProgramButton.UseVisualStyleBackColor = true;
			this.createAnimationProgramButton.Click += new System.EventHandler(this.createAnimationProgramButton_Click);
			// 
			// xNumericUpDown
			// 
			this.xNumericUpDown.Location = new System.Drawing.Point(31, 13);
			this.xNumericUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.xNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.xNumericUpDown.Name = "xNumericUpDown";
			this.xNumericUpDown.Size = new System.Drawing.Size(33, 20);
			this.xNumericUpDown.TabIndex = 8;
			this.xNumericUpDown.ValueChanged += new System.EventHandler(this.xNumericUpDown_ValueChanged);
			// 
			// xLabel
			// 
			this.xLabel.AutoSize = true;
			this.xLabel.Location = new System.Drawing.Point(13, 15);
			this.xLabel.Name = "xLabel";
			this.xLabel.Size = new System.Drawing.Size(15, 13);
			this.xLabel.TabIndex = 9;
			this.xLabel.Text = "x:";
			// 
			// yLabel
			// 
			this.yLabel.AutoSize = true;
			this.yLabel.Location = new System.Drawing.Point(73, 15);
			this.yLabel.Name = "yLabel";
			this.yLabel.Size = new System.Drawing.Size(15, 13);
			this.yLabel.TabIndex = 11;
			this.yLabel.Text = "y:";
			// 
			// yNumericUpDown
			// 
			this.yNumericUpDown.Location = new System.Drawing.Point(89, 13);
			this.yNumericUpDown.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
			this.yNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.yNumericUpDown.Name = "yNumericUpDown";
			this.yNumericUpDown.Size = new System.Drawing.Size(33, 20);
			this.yNumericUpDown.TabIndex = 10;
			this.yNumericUpDown.ValueChanged += new System.EventHandler(this.yNumericUpDown_ValueChanged);
			// 
			// widthLabel
			// 
			this.widthLabel.AutoSize = true;
			this.widthLabel.Location = new System.Drawing.Point(131, 15);
			this.widthLabel.Name = "widthLabel";
			this.widthLabel.Size = new System.Drawing.Size(38, 13);
			this.widthLabel.TabIndex = 13;
			this.widthLabel.Text = "Width:";
			// 
			// widthNumericUpDown
			// 
			this.widthNumericUpDown.Location = new System.Drawing.Point(170, 13);
			this.widthNumericUpDown.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
			this.widthNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.widthNumericUpDown.Name = "widthNumericUpDown";
			this.widthNumericUpDown.Size = new System.Drawing.Size(33, 20);
			this.widthNumericUpDown.TabIndex = 12;
			this.widthNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.widthNumericUpDown.ValueChanged += new System.EventHandler(this.widthNumericUpDown_ValueChanged);
			// 
			// heightLabel
			// 
			this.heightLabel.AutoSize = true;
			this.heightLabel.Location = new System.Drawing.Point(212, 15);
			this.heightLabel.Name = "heightLabel";
			this.heightLabel.Size = new System.Drawing.Size(41, 13);
			this.heightLabel.TabIndex = 15;
			this.heightLabel.Text = "Height:";
			// 
			// heightNumericUpDown
			// 
			this.heightNumericUpDown.Location = new System.Drawing.Point(254, 13);
			this.heightNumericUpDown.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
			this.heightNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.heightNumericUpDown.Name = "heightNumericUpDown";
			this.heightNumericUpDown.Size = new System.Drawing.Size(33, 20);
			this.heightNumericUpDown.TabIndex = 14;
			this.heightNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.heightNumericUpDown.ValueChanged += new System.EventHandler(this.heightNumericUpDown_ValueChanged);
			// 
			// deleteAnimationButton
			// 
			this.deleteAnimationButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.deleteAnimationButton.Location = new System.Drawing.Point(318, 12);
			this.deleteAnimationButton.Name = "deleteAnimationButton";
			this.deleteAnimationButton.Size = new System.Drawing.Size(96, 23);
			this.deleteAnimationButton.TabIndex = 16;
			this.deleteAnimationButton.Text = "Delete animation";
			this.deleteAnimationButton.UseVisualStyleBackColor = true;
			this.deleteAnimationButton.Click += new System.EventHandler(this.deleteAnimationButton_Click);
			// 
			// AnimationForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(424, 332);
			this.Controls.Add(this.deleteAnimationButton);
			this.Controls.Add(this.heightLabel);
			this.Controls.Add(this.heightNumericUpDown);
			this.Controls.Add(this.widthLabel);
			this.Controls.Add(this.widthNumericUpDown);
			this.Controls.Add(this.yLabel);
			this.Controls.Add(this.yNumericUpDown);
			this.Controls.Add(this.xLabel);
			this.Controls.Add(this.xNumericUpDown);
			this.Controls.Add(this.createAnimationProgramButton);
			this.Controls.Add(this.downAnimationStepButton);
			this.Controls.Add(this.upAnimationStepButton);
			this.Controls.Add(this.deleteAnimationStepButton);
			this.Controls.Add(this.addAnimationStepButton);
			this.Controls.Add(this.noAnimationLabel);
			this.Controls.Add(this.dataGridView);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MaximizeBox = false;
			this.MinimumSize = new System.Drawing.Size(440, 205);
			this.Name = "AnimationForm";
			this.Text = "Animations";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AnimationForm_FormClosing);
			this.Load += new System.EventHandler(this.AnimationForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.xNumericUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.yNumericUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.widthNumericUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.heightNumericUpDown)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView dataGridView;
		private System.Windows.Forms.Label noAnimationLabel;
		private System.Windows.Forms.Button addAnimationStepButton;
		private System.Windows.Forms.Button deleteAnimationStepButton;
		private System.Windows.Forms.Button upAnimationStepButton;
		private System.Windows.Forms.Button downAnimationStepButton;
		private System.Windows.Forms.Button createAnimationProgramButton;
		private System.Windows.Forms.NumericUpDown xNumericUpDown;
		private System.Windows.Forms.Label xLabel;
		private System.Windows.Forms.Label yLabel;
		private System.Windows.Forms.NumericUpDown yNumericUpDown;
		private System.Windows.Forms.Label widthLabel;
		private System.Windows.Forms.NumericUpDown widthNumericUpDown;
		private System.Windows.Forms.Label heightLabel;
		private System.Windows.Forms.NumericUpDown heightNumericUpDown;
		private System.Windows.Forms.Button deleteAnimationButton;
		private System.Windows.Forms.DataGridViewTextBoxColumn phaseColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn minTimeColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn maxTimeColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn soundColumn;
	}
}