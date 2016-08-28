using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace BahnEditor.WinForms
{
	public class ToolStripRadioButton : ToolStripButton
	{

		#region Private Fields

		private int radioButtonGroupID = 0;
		private bool updateButtonGroup = true;

		#endregion Private Fields


		#region Public Properties

		[Category("Behavior")]
		public int RadioButtonGroupID
		{
			get
			{
				return this.radioButtonGroupID;
			}
			set
			{
				this.radioButtonGroupID = value;
				this.UpdateGroup();
			}
		}

		#endregion Public Properties


		#region Public Constructors

		public ToolStripRadioButton()
		{
			this.CheckOnClick = true;
		}

		#endregion Public Constructors


		#region Protected Methods

		protected override void OnCheckedChanged(EventArgs e)
		{
			if (this.Parent != null && updateButtonGroup)
			{
				if (!this.Checked)
				{
					this.SetCheckValue(true);
				}
				else
				{
					foreach (ToolStripRadioButton radioButton in this.Parent.Items.OfType<ToolStripRadioButton>())
					{
						if (radioButton != this && radioButton.RadioButtonGroupID == this.RadioButtonGroupID)
						{
							radioButton.SetCheckValue(false);
						}
					}
				}
			}
		}

		#endregion Protected Methods


		#region Private Methods

		private void SetCheckValue(bool checkValue)
		{
			updateButtonGroup = false;
			this.Checked = checkValue;
			updateButtonGroup = true;
		}

		private void UpdateGroup()
		{
			if (this.Parent != null)
			{
				int checkedCount = this.Parent.Items.OfType<ToolStripRadioButton>().Count(x => x.RadioButtonGroupID == RadioButtonGroupID && x.Checked);

				if (checkedCount > 1)
				{
					this.Checked = false;
				}
			}
		}

		#endregion Private Methods
	}
}