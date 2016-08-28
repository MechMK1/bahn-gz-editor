namespace BahnEditor.WinForms
{
	partial class ChooseGraphicLoadTypeForm
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
			this.captionLabel = new System.Windows.Forms.Label();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.openFileRadioButton = new System.Windows.Forms.RadioButton();
			this.openInArchiveRadioButton = new System.Windows.Forms.RadioButton();
			this.positionNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.positionLabel = new System.Windows.Forms.Label();
			this.alternativeLabel = new System.Windows.Forms.Label();
			this.alternativeNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.animationPhaseLabel = new System.Windows.Forms.Label();
			this.animationPhaseNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.positionPanel = new System.Windows.Forms.Panel();
			this.noAlternativeLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.positionNumericUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.alternativeNumericUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.animationPhaseNumericUpDown)).BeginInit();
			this.positionPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// captionLabel
			// 
			this.captionLabel.AutoSize = true;
			this.captionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.captionLabel.Location = new System.Drawing.Point(13, 13);
			this.captionLabel.Name = "captionLabel";
			this.captionLabel.Size = new System.Drawing.Size(265, 16);
			this.captionLabel.TabIndex = 0;
			this.captionLabel.Text = "Choose how the graphic should get loaded.";
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Enabled = false;
			this.okButton.Location = new System.Drawing.Point(12, 226);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(100, 23);
			this.okButton.TabIndex = 1;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(176, 226);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(100, 23);
			this.cancelButton.TabIndex = 2;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// openFileRadioButton
			// 
			this.openFileRadioButton.AutoSize = true;
			this.openFileRadioButton.Location = new System.Drawing.Point(16, 44);
			this.openFileRadioButton.Name = "openFileRadioButton";
			this.openFileRadioButton.Size = new System.Drawing.Size(194, 17);
			this.openFileRadioButton.TabIndex = 3;
			this.openFileRadioButton.Text = "Open the file (closes the current file)";
			this.openFileRadioButton.UseVisualStyleBackColor = true;
			this.openFileRadioButton.CheckedChanged += new System.EventHandler(this.openFileRadioButton_CheckedChanged);
			// 
			// openInArchiveRadioButton
			// 
			this.openInArchiveRadioButton.AutoSize = true;
			this.openInArchiveRadioButton.Location = new System.Drawing.Point(16, 68);
			this.openInArchiveRadioButton.Name = "openInArchiveRadioButton";
			this.openInArchiveRadioButton.Size = new System.Drawing.Size(156, 17);
			this.openInArchiveRadioButton.TabIndex = 4;
			this.openInArchiveRadioButton.Text = "Import the file to the archive";
			this.openInArchiveRadioButton.UseVisualStyleBackColor = true;
			this.openInArchiveRadioButton.CheckedChanged += new System.EventHandler(this.openInArchiveRadioButton_CheckedChanged);
			// 
			// positionNumericUpDown
			// 
			this.positionNumericUpDown.Location = new System.Drawing.Point(116, 2);
			this.positionNumericUpDown.Maximum = new decimal(new int[] {
            89,
            0,
            0,
            0});
			this.positionNumericUpDown.Name = "positionNumericUpDown";
			this.positionNumericUpDown.Size = new System.Drawing.Size(33, 20);
			this.positionNumericUpDown.TabIndex = 5;
			// 
			// positionLabel
			// 
			this.positionLabel.AutoSize = true;
			this.positionLabel.Location = new System.Drawing.Point(3, 4);
			this.positionLabel.Name = "positionLabel";
			this.positionLabel.Size = new System.Drawing.Size(47, 13);
			this.positionLabel.TabIndex = 6;
			this.positionLabel.Text = "Position:";
			// 
			// alternativeLabel
			// 
			this.alternativeLabel.AutoSize = true;
			this.alternativeLabel.Location = new System.Drawing.Point(3, 33);
			this.alternativeLabel.Name = "alternativeLabel";
			this.alternativeLabel.Size = new System.Drawing.Size(60, 13);
			this.alternativeLabel.TabIndex = 8;
			this.alternativeLabel.Text = "Alternative:";
			// 
			// alternativeNumericUpDown
			// 
			this.alternativeNumericUpDown.Location = new System.Drawing.Point(116, 31);
			this.alternativeNumericUpDown.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
			this.alternativeNumericUpDown.Name = "alternativeNumericUpDown";
			this.alternativeNumericUpDown.Size = new System.Drawing.Size(33, 20);
			this.alternativeNumericUpDown.TabIndex = 7;
			// 
			// animationPhaseLabel
			// 
			this.animationPhaseLabel.AutoSize = true;
			this.animationPhaseLabel.Location = new System.Drawing.Point(3, 64);
			this.animationPhaseLabel.Name = "animationPhaseLabel";
			this.animationPhaseLabel.Size = new System.Drawing.Size(85, 13);
			this.animationPhaseLabel.TabIndex = 10;
			this.animationPhaseLabel.Text = "Animationphase:";
			// 
			// animationPhaseNumericUpDown
			// 
			this.animationPhaseNumericUpDown.Location = new System.Drawing.Point(116, 62);
			this.animationPhaseNumericUpDown.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this.animationPhaseNumericUpDown.Name = "animationPhaseNumericUpDown";
			this.animationPhaseNumericUpDown.Size = new System.Drawing.Size(33, 20);
			this.animationPhaseNumericUpDown.TabIndex = 9;
			// 
			// positionPanel
			// 
			this.positionPanel.Controls.Add(this.noAlternativeLabel);
			this.positionPanel.Controls.Add(this.positionLabel);
			this.positionPanel.Controls.Add(this.animationPhaseLabel);
			this.positionPanel.Controls.Add(this.positionNumericUpDown);
			this.positionPanel.Controls.Add(this.animationPhaseNumericUpDown);
			this.positionPanel.Controls.Add(this.alternativeNumericUpDown);
			this.positionPanel.Controls.Add(this.alternativeLabel);
			this.positionPanel.Location = new System.Drawing.Point(16, 102);
			this.positionPanel.Name = "positionPanel";
			this.positionPanel.Size = new System.Drawing.Size(260, 84);
			this.positionPanel.TabIndex = 11;
			this.positionPanel.Visible = false;
			// 
			// noAlternativeLabel
			// 
			this.noAlternativeLabel.AutoSize = true;
			this.noAlternativeLabel.Location = new System.Drawing.Point(155, 33);
			this.noAlternativeLabel.Name = "noAlternativeLabel";
			this.noAlternativeLabel.Size = new System.Drawing.Size(91, 13);
			this.noAlternativeLabel.TabIndex = 11;
			this.noAlternativeLabel.Text = "0 = No alternative";
			// 
			// ChooseGraphicLoadTypeForm
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(288, 261);
			this.Controls.Add(this.positionPanel);
			this.Controls.Add(this.openInArchiveRadioButton);
			this.Controls.Add(this.openFileRadioButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.captionLabel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChooseGraphicLoadTypeForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Load Graphic";
			((System.ComponentModel.ISupportInitialize)(this.positionNumericUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.alternativeNumericUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.animationPhaseNumericUpDown)).EndInit();
			this.positionPanel.ResumeLayout(false);
			this.positionPanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label captionLabel;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.RadioButton openFileRadioButton;
		private System.Windows.Forms.RadioButton openInArchiveRadioButton;
		private System.Windows.Forms.NumericUpDown positionNumericUpDown;
		private System.Windows.Forms.Label positionLabel;
		private System.Windows.Forms.Label alternativeLabel;
		private System.Windows.Forms.NumericUpDown alternativeNumericUpDown;
		private System.Windows.Forms.Label animationPhaseLabel;
		private System.Windows.Forms.NumericUpDown animationPhaseNumericUpDown;
		private System.Windows.Forms.Panel positionPanel;
		private System.Windows.Forms.Label noAlternativeLabel;
	}
}