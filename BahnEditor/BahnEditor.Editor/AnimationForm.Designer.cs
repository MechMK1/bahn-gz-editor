namespace BahnEditor.Editor
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
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGridView
			// 
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
			this.dataGridView.Location = new System.Drawing.Point(13, 51);
			this.dataGridView.MultiSelect = false;
			this.dataGridView.Name = "dataGridView";
			this.dataGridView.Size = new System.Drawing.Size(460, 307);
			this.dataGridView.TabIndex = 0;
			// 
			// phaseColumn
			// 
			this.phaseColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.phaseColumn.HeaderText = "Animationphase";
			this.phaseColumn.Name = "phaseColumn";
			// 
			// minTimeColumn
			// 
			this.minTimeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.minTimeColumn.HeaderText = "Minimum Time";
			this.minTimeColumn.Name = "minTimeColumn";
			// 
			// maxTimeColumn
			// 
			this.maxTimeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.maxTimeColumn.HeaderText = "Maximum Time";
			this.maxTimeColumn.Name = "maxTimeColumn";
			// 
			// soundColumn
			// 
			this.soundColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.soundColumn.FillWeight = 50F;
			this.soundColumn.HeaderText = "Sound";
			this.soundColumn.Name = "soundColumn";
			// 
			// AnimationForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(485, 370);
			this.Controls.Add(this.dataGridView);
			this.MaximizeBox = false;
			this.Name = "AnimationForm";
			this.Text = "Animations";
			this.Load += new System.EventHandler(this.AnimationForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView dataGridView;
		private System.Windows.Forms.DataGridViewTextBoxColumn phaseColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn minTimeColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn maxTimeColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn soundColumn;
	}
}