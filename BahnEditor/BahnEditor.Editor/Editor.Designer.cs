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
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.newButton = new System.Windows.Forms.ToolStripButton();
			this.loadButton = new System.Windows.Forms.ToolStripButton();
			this.saveButton = new System.Windows.Forms.ToolStripButton();
			this.drawPanel = new System.Windows.Forms.Panel();
			this.controlPanel = new System.Windows.Forms.Panel();
			this.loadFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.saveAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.colorDialog = new System.Windows.Forms.ColorDialog();
			this.leftColorButton = new System.Windows.Forms.Button();
			this.rightColorButton = new System.Windows.Forms.Button();
			this.toolStrip.SuspendLayout();
			this.controlPanel.SuspendLayout();
			this.menuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip
			// 
			this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newButton,
            this.loadButton,
            this.saveButton});
			this.toolStrip.Location = new System.Drawing.Point(0, 24);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Size = new System.Drawing.Size(577, 25);
			this.toolStrip.TabIndex = 0;
			this.toolStrip.Text = "toolStrip1";
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
			// saveButton
			// 
			this.saveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.saveButton.Image = ((System.Drawing.Image)(resources.GetObject("saveButton.Image")));
			this.saveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(23, 22);
			this.saveButton.Text = "Save";
			this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
			// 
			// drawPanel
			// 
			this.drawPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.drawPanel.Location = new System.Drawing.Point(13, 52);
			this.drawPanel.Name = "drawPanel";
			this.drawPanel.Size = new System.Drawing.Size(403, 317);
			this.drawPanel.TabIndex = 1;
			this.drawPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.drawPanel_Paint);
			// 
			// controlPanel
			// 
			this.controlPanel.Controls.Add(this.rightColorButton);
			this.controlPanel.Controls.Add(this.leftColorButton);
			this.controlPanel.Location = new System.Drawing.Point(423, 52);
			this.controlPanel.Name = "controlPanel";
			this.controlPanel.Size = new System.Drawing.Size(142, 317);
			this.controlPanel.TabIndex = 2;
			// 
			// loadFileDialog
			// 
			this.loadFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.loadFileDialog_FileOk);
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(577, 24);
			this.menuStrip.TabIndex = 3;
			this.menuStrip.Text = "menuStrip1";
			// 
			// fileMenuItem
			// 
			this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newMenuItem,
            this.openMenuItem,
            this.saveMenuItem,
            this.saveAsMenuItem,
            this.exitMenuItem});
			this.fileMenuItem.Name = "fileMenuItem";
			this.fileMenuItem.Size = new System.Drawing.Size(46, 20);
			this.fileMenuItem.Text = "Datei";
			// 
			// newMenuItem
			// 
			this.newMenuItem.Name = "newMenuItem";
			this.newMenuItem.Size = new System.Drawing.Size(166, 22);
			this.newMenuItem.Text = "Neu";
			this.newMenuItem.Click += new System.EventHandler(this.newMenuItem_Click);
			// 
			// openMenuItem
			// 
			this.openMenuItem.Name = "openMenuItem";
			this.openMenuItem.Size = new System.Drawing.Size(166, 22);
			this.openMenuItem.Text = "Öffnen";
			this.openMenuItem.Click += new System.EventHandler(this.openMenuItem_Click);
			// 
			// saveMenuItem
			// 
			this.saveMenuItem.Name = "saveMenuItem";
			this.saveMenuItem.Size = new System.Drawing.Size(166, 22);
			this.saveMenuItem.Text = "Speichern";
			this.saveMenuItem.Click += new System.EventHandler(this.saveMenuItem_Click);
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog_FileOk);
			// 
			// saveAsMenuItem
			// 
			this.saveAsMenuItem.Name = "saveAsMenuItem";
			this.saveAsMenuItem.Size = new System.Drawing.Size(166, 22);
			this.saveAsMenuItem.Text = "Speichern unter...";
			this.saveAsMenuItem.Click += new System.EventHandler(this.saveAsMenuItem_Click);
			// 
			// exitMenuItem
			// 
			this.exitMenuItem.Name = "exitMenuItem";
			this.exitMenuItem.Size = new System.Drawing.Size(166, 22);
			this.exitMenuItem.Text = "Beenden";
			this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
			// 
			// colorDialog
			// 
			this.colorDialog.FullOpen = true;
			// 
			// leftColorButton
			// 
			this.leftColorButton.BackColor = System.Drawing.Color.Black;
			this.leftColorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.leftColorButton.Location = new System.Drawing.Point(4, 4);
			this.leftColorButton.Name = "leftColorButton";
			this.leftColorButton.Size = new System.Drawing.Size(60, 23);
			this.leftColorButton.TabIndex = 0;
			this.leftColorButton.TabStop = false;
			this.leftColorButton.UseVisualStyleBackColor = false;
			this.leftColorButton.Click += new System.EventHandler(this.leftColorButton_Click);
			// 
			// rightColorButton
			// 
			this.rightColorButton.BackColor = System.Drawing.Color.White;
			this.rightColorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.rightColorButton.Location = new System.Drawing.Point(79, 4);
			this.rightColorButton.Name = "rightColorButton";
			this.rightColorButton.Size = new System.Drawing.Size(60, 23);
			this.rightColorButton.TabIndex = 1;
			this.rightColorButton.TabStop = false;
			this.rightColorButton.UseVisualStyleBackColor = false;
			this.rightColorButton.Click += new System.EventHandler(this.rightColorButton_Click);
			// 
			// Editor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(577, 381);
			this.Controls.Add(this.controlPanel);
			this.Controls.Add(this.drawPanel);
			this.Controls.Add(this.toolStrip);
			this.Controls.Add(this.menuStrip);
			this.MainMenuStrip = this.menuStrip;
			this.Name = "Editor";
			this.Text = "Bahn Editor";
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.controlPanel.ResumeLayout(false);
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip;
		private System.Windows.Forms.ToolStripButton newButton;
		private System.Windows.Forms.Panel drawPanel;
		private System.Windows.Forms.Panel controlPanel;
		private System.Windows.Forms.ToolStripButton loadButton;
		private System.Windows.Forms.OpenFileDialog loadFileDialog;
		private System.Windows.Forms.ToolStripButton saveButton;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveMenuItem;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.ToolStripMenuItem saveAsMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
		private System.Windows.Forms.ColorDialog colorDialog;
		private System.Windows.Forms.Button leftColorButton;
		private System.Windows.Forms.Button rightColorButton;

	}
}

