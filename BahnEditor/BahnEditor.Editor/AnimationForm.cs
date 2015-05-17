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
	public partial class AnimationForm : Form
	{
		private Editor editor;

		public AnimationForm()
		{
			InitializeComponent();
		}

		public AnimationForm(Editor editor)
		{
			this.editor = editor;
			InitializeComponent();
		}

		private void AnimationForm_Load(object sender, EventArgs e)
		{
			ChangeAnimationProgram();
		}

		internal bool ChangeAnimationProgram()
		{
			this.dataGridView.Rows.Clear();
			if(this.editor.Zoom1Archive != null && this.editor.Zoom1Archive.Animation != null && this.editor.Zoom1Archive.Animation[this.editor.ActualGraphicID, this.editor.ActualAlternativeID] != null)
			{
				BahnLib.AnimationProgram program = this.editor.Zoom1Archive.Animation[this.editor.ActualGraphicID, this.editor.ActualAlternativeID];
				for (int i = 0; i < program.AnimationStepCount; i++)
				{
					this.dataGridView.Rows.Add(program[i].AnimationPhase, program[i].MinimumTime, program[i].MaximumTime, program[i].Sound);
				}
				return true;
			}
			return false;
		}
	}
}
