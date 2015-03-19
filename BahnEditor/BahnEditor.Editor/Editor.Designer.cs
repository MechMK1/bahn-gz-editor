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
			this.controlPanel = new System.Windows.Forms.Panel();
			this.leftComboBox = new System.Windows.Forms.ComboBox();
			this.rightLabel = new System.Windows.Forms.Label();
			this.leftLabel = new System.Windows.Forms.Label();
			this.layerComboBox = new System.Windows.Forms.ComboBox();
			this.rightComboBox = new System.Windows.Forms.ComboBox();
			this.zoomTrackBar = new System.Windows.Forms.TrackBar();
			this.rightColorButton = new System.Windows.Forms.Button();
			this.leftColorButton = new System.Windows.Forms.Button();
			this.loadFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.saveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.colorDialog = new System.Windows.Forms.ColorDialog();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.zoom1Tab = new System.Windows.Forms.TabPage();
			this.zoom2Tab = new System.Windows.Forms.TabPage();
			this.zoom4Tab = new System.Windows.Forms.TabPage();
			this.drawPanel = new BahnEditor.Editor.DrawPanel();
			this.overviewPanel = new BahnEditor.Editor.DrawPanel();
			this.overviewDownButton = new System.Windows.Forms.Button();
			this.overviewUpButton = new System.Windows.Forms.Button();
			this.overviewLeftRightButton = new System.Windows.Forms.Button();
			this.toolStrip.SuspendLayout();
			this.controlPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.zoomTrackBar)).BeginInit();
			this.menuStrip.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.overviewPanel.SuspendLayout();
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
			this.toolStrip.Size = new System.Drawing.Size(1008, 25);
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
			// controlPanel
			// 
			this.controlPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.controlPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.controlPanel.Controls.Add(this.leftComboBox);
			this.controlPanel.Controls.Add(this.rightLabel);
			this.controlPanel.Controls.Add(this.leftLabel);
			this.controlPanel.Controls.Add(this.layerComboBox);
			this.controlPanel.Controls.Add(this.rightComboBox);
			this.controlPanel.Controls.Add(this.zoomTrackBar);
			this.controlPanel.Controls.Add(this.rightColorButton);
			this.controlPanel.Controls.Add(this.leftColorButton);
			this.controlPanel.Location = new System.Drawing.Point(831, 52);
			this.controlPanel.Name = "controlPanel";
			this.controlPanel.Size = new System.Drawing.Size(170, 666);
			this.controlPanel.TabIndex = 2;
			// 
			// leftComboBox
			// 
			this.leftComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.leftComboBox.FormattingEnabled = true;
			this.leftComboBox.Items.AddRange(new object[] {
            "Normal",
            "Transparent",
            "Halb Transparent (Hinter Glass)",
            "Licht (nachts hell)",
            "Lampe (warmweiß)",
            "Lampe (kaltweiß)",
            "Lampe (rot)",
            "Lampe (gelb, Glühlampe)",
            "Lampe (gelb, Gaslicht)",
            "Fenster (nachts gelb)",
            "Fenster (nachts neon)",
            "wie Hintergrund",
            "wie Schwellen 0",
            "wie Schwellen 1",
            "wie Schwellen 3",
            "wie Schienen Str 0",
            "wie Schienen Str 1",
            "wie Schienen Str 2",
            "wie Schienen Str 3",
            "wie Schienen Gleisbett 0",
            "wie Schienen Gleisbett 1",
            "wie Schienen Gleisbett 2",
            "wie Schienen Gleisbett 3",
            "wie Punkte Bus 0",
            "wie Punkte Bus 1",
            "wie Punkte Bus 2",
            "wie Punkte Bus 3",
            "wie Punkte Wasserweg",
            "wie Schottersteine",
            "wie Kies",
            "wie Rasengleis (Gras)",
            "wie Feldweg (Hintergrund)",
            "wie Feldweg (Fahrspur)",
            "wie Text"});
			this.leftComboBox.Location = new System.Drawing.Point(5, 55);
			this.leftComboBox.Name = "leftComboBox";
			this.leftComboBox.Size = new System.Drawing.Size(160, 21);
			this.leftComboBox.TabIndex = 4;
			this.leftComboBox.TabStop = false;
			this.leftComboBox.SelectedIndexChanged += new System.EventHandler(this.leftComboBox_SelectedIndexChanged);
			// 
			// rightLabel
			// 
			this.rightLabel.AutoSize = true;
			this.rightLabel.Location = new System.Drawing.Point(38, 104);
			this.rightLabel.Name = "rightLabel";
			this.rightLabel.Size = new System.Drawing.Size(94, 13);
			this.rightLabel.TabIndex = 8;
			this.rightLabel.Text = "Rechte Maustaste";
			// 
			// leftLabel
			// 
			this.leftLabel.AutoSize = true;
			this.leftLabel.Location = new System.Drawing.Point(42, 10);
			this.leftLabel.Name = "leftLabel";
			this.leftLabel.Size = new System.Drawing.Size(85, 13);
			this.leftLabel.TabIndex = 7;
			this.leftLabel.Text = "Linke Maustaste";
			// 
			// layerComboBox
			// 
			this.layerComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.layerComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.layerComboBox.FormattingEnabled = true;
			this.layerComboBox.Items.AddRange(new object[] {
            "Vordergrund",
            "Hintergrund und flach nach vorn",
            "Flach nach hinten",
            "Vordergrund, oben",
            "Vorn, auf Brücken"});
			this.layerComboBox.Location = new System.Drawing.Point(3, 589);
			this.layerComboBox.Name = "layerComboBox";
			this.layerComboBox.Size = new System.Drawing.Size(160, 21);
			this.layerComboBox.TabIndex = 6;
			this.layerComboBox.TabStop = false;
			this.layerComboBox.SelectedIndexChanged += new System.EventHandler(this.layerComboBox_SelectedIndexChanged);
			// 
			// rightComboBox
			// 
			this.rightComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.rightComboBox.FormattingEnabled = true;
			this.rightComboBox.Items.AddRange(new object[] {
            "Normal",
            "Transparent",
            "Halb Transparent (Hinter Glass)",
            "Licht (nachts hell)",
            "Lampe (warmweiß)",
            "Lampe (kaltweiß)",
            "Lampe (rot)",
            "Lampe (gelb, Glühlampe)",
            "Lampe (gelb, Gaslicht)",
            "Fenster (nachts gelb)",
            "Fenster (nachts neon)",
            "wie Hintergrund",
            "wie Schwellen 0",
            "wie Schwellen 1",
            "wie Schwellen 3",
            "wie Schienen Str 0",
            "wie Schienen Str 1",
            "wie Schienen Str 2",
            "wie Schienen Str 3",
            "wie Schienen Gleisbett 0",
            "wie Schienen Gleisbett 1",
            "wie Schienen Gleisbett 2",
            "wie Schienen Gleisbett 3",
            "wie Punkte Bus 0",
            "wie Punkte Bus 1",
            "wie Punkte Bus 2",
            "wie Punkte Bus 3",
            "wie Punkte Wasserweg",
            "wie Schottersteine",
            "wie Kies",
            "wie Rasengleis (Gras)",
            "wie Feldweg (Hintergrund)",
            "wie Feldweg (Fahrspur)",
            "wie Text"});
			this.rightComboBox.Location = new System.Drawing.Point(5, 149);
			this.rightComboBox.Name = "rightComboBox";
			this.rightComboBox.Size = new System.Drawing.Size(160, 21);
			this.rightComboBox.TabIndex = 5;
			this.rightComboBox.TabStop = false;
			this.rightComboBox.SelectedIndexChanged += new System.EventHandler(this.rightComboBox_SelectedIndexChanged);
			// 
			// zoomTrackBar
			// 
			this.zoomTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.zoomTrackBar.LargeChange = 1;
			this.zoomTrackBar.Location = new System.Drawing.Point(3, 616);
			this.zoomTrackBar.Maximum = 5;
			this.zoomTrackBar.Minimum = 2;
			this.zoomTrackBar.Name = "zoomTrackBar";
			this.zoomTrackBar.Size = new System.Drawing.Size(160, 45);
			this.zoomTrackBar.TabIndex = 2;
			this.zoomTrackBar.TabStop = false;
			this.zoomTrackBar.Value = 3;
			this.zoomTrackBar.Scroll += new System.EventHandler(this.zoomTrackBar_Scroll);
			// 
			// rightColorButton
			// 
			this.rightColorButton.BackColor = System.Drawing.Color.White;
			this.rightColorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.rightColorButton.Location = new System.Drawing.Point(40, 120);
			this.rightColorButton.Name = "rightColorButton";
			this.rightColorButton.Size = new System.Drawing.Size(90, 23);
			this.rightColorButton.TabIndex = 1;
			this.rightColorButton.TabStop = false;
			this.rightColorButton.UseVisualStyleBackColor = false;
			this.rightColorButton.Click += new System.EventHandler(this.rightColorButton_Click);
			// 
			// leftColorButton
			// 
			this.leftColorButton.BackColor = System.Drawing.Color.Black;
			this.leftColorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.leftColorButton.Location = new System.Drawing.Point(40, 26);
			this.leftColorButton.Name = "leftColorButton";
			this.leftColorButton.Size = new System.Drawing.Size(90, 23);
			this.leftColorButton.TabIndex = 0;
			this.leftColorButton.TabStop = false;
			this.leftColorButton.UseVisualStyleBackColor = false;
			this.leftColorButton.Click += new System.EventHandler(this.leftColorButton_Click);
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
			this.menuStrip.Size = new System.Drawing.Size(1008, 24);
			this.menuStrip.TabIndex = 3;
			this.menuStrip.Text = "menuStrip1";
			// 
			// fileMenuItem
			// 
			this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newMenuItem,
            this.openMenuItem,
            this.toolStripSeparator1,
            this.saveMenuItem,
            this.saveAsMenuItem,
            this.toolStripSeparator2,
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
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(163, 6);
			// 
			// saveMenuItem
			// 
			this.saveMenuItem.Name = "saveMenuItem";
			this.saveMenuItem.Size = new System.Drawing.Size(166, 22);
			this.saveMenuItem.Text = "Speichern";
			this.saveMenuItem.Click += new System.EventHandler(this.saveMenuItem_Click);
			// 
			// saveAsMenuItem
			// 
			this.saveAsMenuItem.Name = "saveAsMenuItem";
			this.saveAsMenuItem.Size = new System.Drawing.Size(166, 22);
			this.saveAsMenuItem.Text = "Speichern unter...";
			this.saveAsMenuItem.Click += new System.EventHandler(this.saveAsMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(163, 6);
			// 
			// exitMenuItem
			// 
			this.exitMenuItem.Name = "exitMenuItem";
			this.exitMenuItem.Size = new System.Drawing.Size(166, 22);
			this.exitMenuItem.Text = "Beenden";
			this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog_FileOk);
			// 
			// colorDialog
			// 
			this.colorDialog.FullOpen = true;
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.zoom1Tab);
			this.tabControl.Controls.Add(this.zoom2Tab);
			this.tabControl.Controls.Add(this.zoom4Tab);
			this.tabControl.Location = new System.Drawing.Point(13, 120);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(812, 20);
			this.tabControl.TabIndex = 6;
			this.tabControl.TabStop = false;
			this.tabControl.Visible = false;
			this.tabControl.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl_Selecting);
			// 
			// zoom1Tab
			// 
			this.zoom1Tab.Location = new System.Drawing.Point(4, 22);
			this.zoom1Tab.Name = "zoom1Tab";
			this.zoom1Tab.Padding = new System.Windows.Forms.Padding(3);
			this.zoom1Tab.Size = new System.Drawing.Size(804, 0);
			this.zoom1Tab.TabIndex = 0;
			this.zoom1Tab.Text = "Zoom 1";
			this.zoom1Tab.UseVisualStyleBackColor = true;
			// 
			// zoom2Tab
			// 
			this.zoom2Tab.Location = new System.Drawing.Point(4, 22);
			this.zoom2Tab.Name = "zoom2Tab";
			this.zoom2Tab.Padding = new System.Windows.Forms.Padding(3);
			this.zoom2Tab.Size = new System.Drawing.Size(804, 0);
			this.zoom2Tab.TabIndex = 1;
			this.zoom2Tab.Text = "Zoom 2";
			this.zoom2Tab.UseVisualStyleBackColor = true;
			// 
			// zoom4Tab
			// 
			this.zoom4Tab.Location = new System.Drawing.Point(4, 22);
			this.zoom4Tab.Name = "zoom4Tab";
			this.zoom4Tab.Size = new System.Drawing.Size(804, 0);
			this.zoom4Tab.TabIndex = 2;
			this.zoom4Tab.Text = "Zoom 4";
			this.zoom4Tab.UseVisualStyleBackColor = true;
			// 
			// drawPanel
			// 
			this.drawPanel.AutoScroll = true;
			this.drawPanel.BackColor = System.Drawing.Color.Transparent;
			this.drawPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.drawPanel.Location = new System.Drawing.Point(13, 142);
			this.drawPanel.Name = "drawPanel";
			this.drawPanel.Size = new System.Drawing.Size(812, 576);
			this.drawPanel.TabIndex = 4;
			this.drawPanel.Visible = false;
			this.drawPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.drawPanel_Paint);
			this.drawPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.drawPanel_MouseClick);
			this.drawPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.drawPanel_MouseMove);
			this.drawPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.drawPanel_MouseUp);
			// 
			// overviewPanel
			// 
			this.overviewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.overviewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.overviewPanel.Controls.Add(this.overviewDownButton);
			this.overviewPanel.Controls.Add(this.overviewUpButton);
			this.overviewPanel.Controls.Add(this.overviewLeftRightButton);
			this.overviewPanel.Location = new System.Drawing.Point(13, 52);
			this.overviewPanel.Name = "overviewPanel";
			this.overviewPanel.Size = new System.Drawing.Size(812, 62);
			this.overviewPanel.TabIndex = 5;
			this.overviewPanel.Visible = false;
			this.overviewPanel.Click += new System.EventHandler(this.overviewPanel_Click);
			this.overviewPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.overviewPanel_Paint);
			// 
			// overviewDownButton
			// 
			this.overviewDownButton.BackgroundImage = global::BahnEditor.Editor.Properties.Resources.downarrow;
			this.overviewDownButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.overviewDownButton.Location = new System.Drawing.Point(20, 16);
			this.overviewDownButton.Name = "overviewDownButton";
			this.overviewDownButton.Size = new System.Drawing.Size(17, 17);
			this.overviewDownButton.TabIndex = 3;
			this.overviewDownButton.UseVisualStyleBackColor = true;
			this.overviewDownButton.Click += new System.EventHandler(this.overviewDownButton_Click);
			// 
			// overviewUpButton
			// 
			this.overviewUpButton.BackgroundImage = global::BahnEditor.Editor.Properties.Resources.uparrow;
			this.overviewUpButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.overviewUpButton.Location = new System.Drawing.Point(20, 0);
			this.overviewUpButton.Name = "overviewUpButton";
			this.overviewUpButton.Size = new System.Drawing.Size(17, 17);
			this.overviewUpButton.TabIndex = 2;
			this.overviewUpButton.UseVisualStyleBackColor = true;
			this.overviewUpButton.Click += new System.EventHandler(this.overviewUpButton_Click);
			// 
			// overviewLeftRightButton
			// 
			this.overviewLeftRightButton.Image = global::BahnEditor.Editor.Properties.Resources.leftrightarrow;
			this.overviewLeftRightButton.Location = new System.Drawing.Point(10, 37);
			this.overviewLeftRightButton.Name = "overviewLeftRightButton";
			this.overviewLeftRightButton.Size = new System.Drawing.Size(27, 17);
			this.overviewLeftRightButton.TabIndex = 1;
			this.overviewLeftRightButton.TabStop = false;
			this.overviewLeftRightButton.UseVisualStyleBackColor = true;
			this.overviewLeftRightButton.Click += new System.EventHandler(this.overviewLeftRightButton_Click);
			// 
			// Editor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1008, 730);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.drawPanel);
			this.Controls.Add(this.overviewPanel);
			this.Controls.Add(this.controlPanel);
			this.Controls.Add(this.toolStrip);
			this.Controls.Add(this.menuStrip);
			this.MainMenuStrip = this.menuStrip;
			this.Name = "Editor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Bahn Editor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Editor_FormClosing);
			this.Load += new System.EventHandler(this.Editor_Load);
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.controlPanel.ResumeLayout(false);
			this.controlPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.zoomTrackBar)).EndInit();
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.tabControl.ResumeLayout(false);
			this.overviewPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip;
		private System.Windows.Forms.ToolStripButton newButton;
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
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.TrackBar zoomTrackBar;
		private DrawPanel drawPanel;
		private System.Windows.Forms.ComboBox rightComboBox;
		private System.Windows.Forms.ComboBox leftComboBox;
		private System.Windows.Forms.ComboBox layerComboBox;
		private System.Windows.Forms.Label rightLabel;
		private System.Windows.Forms.Label leftLabel;
		private DrawPanel overviewPanel;
		private System.Windows.Forms.Button overviewLeftRightButton;
		private System.Windows.Forms.Button overviewUpButton;
		private System.Windows.Forms.Button overviewDownButton;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage zoom1Tab;
		private System.Windows.Forms.TabPage zoom2Tab;
		private System.Windows.Forms.TabPage zoom4Tab;

	}
}

