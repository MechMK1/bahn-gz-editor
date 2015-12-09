namespace BahnEditor.Editor
{
	partial class DrivingWaySettingsForm
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
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.typeComboBox = new System.Windows.Forms.ComboBox();
			this.arrivalComboBox = new System.Windows.Forms.ComboBox();
			this.departureComboBox = new System.Windows.Forms.ComboBox();
			this.additionalFunctionComboBox = new System.Windows.Forms.ComboBox();
			this.typeLabel = new System.Windows.Forms.Label();
			this.arrivalLabel = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.additionalFunctionLabel = new System.Windows.Forms.Label();
			this.crossingCheckBox = new System.Windows.Forms.CheckBox();
			this.infoLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(111, 115);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 0;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(232, 115);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// typeComboBox
			// 
			this.typeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.typeComboBox.FormattingEnabled = true;
			this.typeComboBox.Items.AddRange(new object[] {
            "---",
            "Rail",
            "Road",
            "Rail+Road",
            "Water"});
			this.typeComboBox.Location = new System.Drawing.Point(12, 59);
			this.typeComboBox.Name = "typeComboBox";
			this.typeComboBox.Size = new System.Drawing.Size(99, 21);
			this.typeComboBox.TabIndex = 2;
			this.typeComboBox.SelectedIndexChanged += new System.EventHandler(this.typeComboBox_SelectedIndexChanged);
			// 
			// arrivalComboBox
			// 
			this.arrivalComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.arrivalComboBox.FormattingEnabled = true;
			this.arrivalComboBox.Items.AddRange(new object[] {
            "↑",
            "↓",
            "←",
            "→",
            "↘",
            "↖",
            "↙",
            "↗"});
			this.arrivalComboBox.Location = new System.Drawing.Point(142, 59);
			this.arrivalComboBox.Name = "arrivalComboBox";
			this.arrivalComboBox.Size = new System.Drawing.Size(34, 21);
			this.arrivalComboBox.TabIndex = 10;
			this.arrivalComboBox.SelectedIndexChanged += new System.EventHandler(this.arrivalComboBox_SelectedIndexChanged);
			// 
			// departureComboBox
			// 
			this.departureComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.departureComboBox.FormattingEnabled = true;
			this.departureComboBox.Items.AddRange(new object[] {
            "↑",
            "↓",
            "←",
            "→",
            "↘",
            "↖",
            "↙",
            "↗"});
			this.departureComboBox.Location = new System.Drawing.Point(206, 59);
			this.departureComboBox.Name = "departureComboBox";
			this.departureComboBox.Size = new System.Drawing.Size(34, 21);
			this.departureComboBox.TabIndex = 11;
			this.departureComboBox.SelectedIndexChanged += new System.EventHandler(this.departureComboBox_SelectedIndexChanged);
			// 
			// additionalFunctionComboBox
			// 
			this.additionalFunctionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.additionalFunctionComboBox.FormattingEnabled = true;
			this.additionalFunctionComboBox.Items.AddRange(new object[] {
            "None",
            "Tunnel entrance (layer -1)",
            "Tunnel exit (layer +1)",
            "Ramp (layer +1)"});
			this.additionalFunctionComboBox.Location = new System.Drawing.Point(268, 60);
			this.additionalFunctionComboBox.Name = "additionalFunctionComboBox";
			this.additionalFunctionComboBox.Size = new System.Drawing.Size(139, 21);
			this.additionalFunctionComboBox.TabIndex = 12;
			this.additionalFunctionComboBox.SelectedIndexChanged += new System.EventHandler(this.additionalFunctionComboBox_SelectedIndexChanged);
			// 
			// typeLabel
			// 
			this.typeLabel.AutoSize = true;
			this.typeLabel.Location = new System.Drawing.Point(11, 44);
			this.typeLabel.Name = "typeLabel";
			this.typeLabel.Size = new System.Drawing.Size(99, 13);
			this.typeLabel.TabIndex = 13;
			this.typeLabel.Text = "Type of driving way";
			// 
			// arrivalLabel
			// 
			this.arrivalLabel.AutoSize = true;
			this.arrivalLabel.Location = new System.Drawing.Point(134, 29);
			this.arrivalLabel.Name = "arrivalLabel";
			this.arrivalLabel.Size = new System.Drawing.Size(62, 26);
			this.arrivalLabel.TabIndex = 14;
			this.arrivalLabel.Text = "Direction\r\nupon arrival";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(199, 29);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(67, 26);
			this.label1.TabIndex = 15;
			this.label1.Text = "Direction\r\non departure";
			// 
			// additionalFunctionLabel
			// 
			this.additionalFunctionLabel.AutoSize = true;
			this.additionalFunctionLabel.Location = new System.Drawing.Point(285, 44);
			this.additionalFunctionLabel.Name = "additionalFunctionLabel";
			this.additionalFunctionLabel.Size = new System.Drawing.Size(99, 13);
			this.additionalFunctionLabel.TabIndex = 16;
			this.additionalFunctionLabel.Text = "Additional functions";
			// 
			// crossingCheckBox
			// 
			this.crossingCheckBox.AutoSize = true;
			this.crossingCheckBox.Location = new System.Drawing.Point(164, 92);
			this.crossingCheckBox.Name = "crossingCheckBox";
			this.crossingCheckBox.Size = new System.Drawing.Size(66, 17);
			this.crossingCheckBox.TabIndex = 17;
			this.crossingCheckBox.Text = "Crossing";
			this.crossingCheckBox.UseVisualStyleBackColor = true;
			this.crossingCheckBox.CheckedChanged += new System.EventHandler(this.crossingCheckBox_CheckedChanged);
			// 
			// infoLabel
			// 
			this.infoLabel.AutoSize = true;
			this.infoLabel.Location = new System.Drawing.Point(7, 8);
			this.infoLabel.Name = "infoLabel";
			this.infoLabel.Size = new System.Drawing.Size(183, 13);
			this.infoLabel.TabIndex = 18;
			this.infoLabel.Text = "Select settings (type = \"---\" for delete)";
			// 
			// DrivingWaySettingsForm
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(420, 150);
			this.Controls.Add(this.infoLabel);
			this.Controls.Add(this.crossingCheckBox);
			this.Controls.Add(this.additionalFunctionLabel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.arrivalLabel);
			this.Controls.Add(this.typeLabel);
			this.Controls.Add(this.additionalFunctionComboBox);
			this.Controls.Add(this.departureComboBox);
			this.Controls.Add(this.arrivalComboBox);
			this.Controls.Add(this.typeComboBox);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "DrivingWaySettingsForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Driving Way Settings";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.ComboBox typeComboBox;
		private System.Windows.Forms.ComboBox arrivalComboBox;
		private System.Windows.Forms.ComboBox departureComboBox;
		private System.Windows.Forms.ComboBox additionalFunctionComboBox;
		private System.Windows.Forms.Label typeLabel;
		private System.Windows.Forms.Label arrivalLabel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label additionalFunctionLabel;
		private System.Windows.Forms.CheckBox crossingCheckBox;
		private System.Windows.Forms.Label infoLabel;
	}
}