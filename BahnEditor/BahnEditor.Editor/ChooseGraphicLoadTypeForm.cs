using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BahnEditor.Editor
{
	public partial class ChooseGraphicLoadTypeForm : Form
	{
		public bool AddToArchive { get; set; }

		public int Position { get; set; }

		public int Alternative { get; set; }

		public int AnimationPhase { get; set; }

		public ChooseGraphicLoadTypeForm()
		{
			InitializeComponent();
		}

		private void openFileRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			this.okButton.Enabled = true;
			this.positionPanel.Visible = false;
		}

		private void openInArchiveRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			this.okButton.Enabled = true;
			this.positionPanel.Visible = true;
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			if(this.openFileRadioButton.Checked)
			{
				this.AddToArchive = false;
			}
			else if(this.openInArchiveRadioButton.Checked)
			{
				this.AddToArchive = true;
				this.Position = (int)this.positionNumericUpDown.Value;
				this.Alternative = (int)this.alternativeNumericUpDown.Value;
				this.AnimationPhase = (int)this.animationPhaseNumericUpDown.Value;
			}
		}
	}
}
