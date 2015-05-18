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
			this.noAnimationLabel = new System.Windows.Forms.Label();
			this.addAnimationStepButton = new System.Windows.Forms.Button();
			this.deleteAnimationStepButton = new System.Windows.Forms.Button();
			this.upAnimationStepButton = new System.Windows.Forms.Button();
			this.downAnimationStepButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
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
			this.dataGridView.Location = new System.Drawing.Point(12, 12);
			this.dataGridView.MultiSelect = false;
			this.dataGridView.Name = "dataGridView";
			this.dataGridView.RowHeadersWidth = 20;
			this.dataGridView.Size = new System.Drawing.Size(371, 308);
			this.dataGridView.TabIndex = 0;
			this.dataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView_CellValidating);
			this.dataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellValueChanged);
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
			this.addAnimationStepButton.Location = new System.Drawing.Point(389, 12);
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
			this.deleteAnimationStepButton.Location = new System.Drawing.Point(389, 41);
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
			this.upAnimationStepButton.BackgroundImage = global::BahnEditor.Editor.Properties.Resources.uparrow;
			this.upAnimationStepButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.upAnimationStepButton.Location = new System.Drawing.Point(389, 70);
			this.upAnimationStepButton.Name = "upAnimationStepButton";
			this.upAnimationStepButton.Size = new System.Drawing.Size(23, 23);
			this.upAnimationStepButton.TabIndex = 5;
			this.upAnimationStepButton.UseVisualStyleBackColor = true;
			this.upAnimationStepButton.Click += new System.EventHandler(this.upAnimationStepButton_Click);
			// 
			// downAnimationStepButton
			// 
			this.downAnimationStepButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.downAnimationStepButton.BackgroundImage = global::BahnEditor.Editor.Properties.Resources.downarrow;
			this.downAnimationStepButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.downAnimationStepButton.Location = new System.Drawing.Point(389, 99);
			this.downAnimationStepButton.Name = "downAnimationStepButton";
			this.downAnimationStepButton.Size = new System.Drawing.Size(23, 23);
			this.downAnimationStepButton.TabIndex = 6;
			this.downAnimationStepButton.UseVisualStyleBackColor = true;
			this.downAnimationStepButton.Click += new System.EventHandler(this.downAnimationStepButton_Click);
			// 
			// AnimationForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(424, 332);
			this.Controls.Add(this.downAnimationStepButton);
			this.Controls.Add(this.upAnimationStepButton);
			this.Controls.Add(this.deleteAnimationStepButton);
			this.Controls.Add(this.addAnimationStepButton);
			this.Controls.Add(this.noAnimationLabel);
			this.Controls.Add(this.dataGridView);
			this.MaximizeBox = false;
			this.MinimumSize = new System.Drawing.Size(440, 172);
			this.Name = "AnimationForm";
			this.Text = "Animations";
			this.Load += new System.EventHandler(this.AnimationForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView dataGridView;
		private System.Windows.Forms.DataGridViewTextBoxColumn phaseColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn minTimeColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn maxTimeColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn soundColumn;
		private System.Windows.Forms.Label noAnimationLabel;
		private System.Windows.Forms.Button addAnimationStepButton;
		private System.Windows.Forms.Button deleteAnimationStepButton;
		private System.Windows.Forms.Button upAnimationStepButton;
		private System.Windows.Forms.Button downAnimationStepButton;
	}
}