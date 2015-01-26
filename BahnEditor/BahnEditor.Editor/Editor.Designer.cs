namespace BahnEditor.Editor
{
	partial class Editor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Editor));
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.newButton = new System.Windows.Forms.ToolStripButton();
			this.drawPanel = new System.Windows.Forms.Panel();
			this.controlPanel = new System.Windows.Forms.Panel();
			this.loadButton = new System.Windows.Forms.ToolStripButton();
			this.loadFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newButton,
            this.loadButton});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(577, 25);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// newButton
			// 
			this.newButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.newButton.Image = ((System.Drawing.Image)(resources.GetObject("newButton.Image")));
			this.newButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.newButton.Name = "newButton";
			this.newButton.Size = new System.Drawing.Size(23, 22);
			this.newButton.Text = "New";
			this.newButton.Click += new System.EventHandler(this.newButton_Click);
			// 
			// drawPanel
			// 
			this.drawPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.drawPanel.Location = new System.Drawing.Point(13, 29);
			this.drawPanel.Name = "drawPanel";
			this.drawPanel.Size = new System.Drawing.Size(403, 340);
			this.drawPanel.TabIndex = 1;
			this.drawPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.drawPanel_Paint);
			// 
			// controlPanel
			// 
			this.controlPanel.Location = new System.Drawing.Point(423, 29);
			this.controlPanel.Name = "controlPanel";
			this.controlPanel.Size = new System.Drawing.Size(142, 340);
			this.controlPanel.TabIndex = 2;
			// 
			// loadButton
			// 
			this.loadButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.loadButton.Image = ((System.Drawing.Image)(resources.GetObject("loadButton.Image")));
			this.loadButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.loadButton.Name = "loadButton";
			this.loadButton.Size = new System.Drawing.Size(23, 22);
			this.loadButton.Text = "Load";
			this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
			// 
			// loadFileDialog
			// 
			this.loadFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.loadFileDialog_FileOk);
			// 
			// Editor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(577, 381);
			this.Controls.Add(this.controlPanel);
			this.Controls.Add(this.drawPanel);
			this.Controls.Add(this.toolStrip1);
			this.Name = "Editor";
			this.Text = "Bahn Editor";
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton newButton;
		private System.Windows.Forms.Panel drawPanel;
		private System.Windows.Forms.Panel controlPanel;
		private System.Windows.Forms.ToolStripButton loadButton;
		private System.Windows.Forms.OpenFileDialog loadFileDialog;

	}
}

