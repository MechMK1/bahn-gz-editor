using BahnEditor.BahnLib;
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace BahnEditor.Winforms
{
	public partial class AnimationForm : Form
	{
		private Editor editor;
		private Rectangle dragBoxFromMouseDown;
		private int rowIndexFromMouseDown;
		private int rowIndexOfItemUnderMouseToDrop;
		private bool cellValueCodeChanged = false;
		private bool numericCodeChanged = false;

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
			this.dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			ChangeAnimationProgram();
		}

		internal bool ChangeAnimationProgram()
		{
			this.dataGridView.Rows.Clear();
			if (this.editor.Zoom1Archive != null && this.editor.Zoom1Archive.Animation != null && this.editor.Zoom1Archive.Animation[this.editor.ActualGraphicID, this.editor.ActualAlternativeID] != null)
			{
				this.dataGridView.Visible = true;
				this.addAnimationStepButton.Visible = true;
				this.deleteAnimationStepButton.Visible = true;
				this.upAnimationStepButton.Visible = true;
				this.downAnimationStepButton.Visible = true;
				this.deleteAnimationButton.Visible = true;
				this.numericCodeChanged = true;
				this.xNumericUpDown.Value = this.editor.Zoom1Archive.Animation[this.editor.ActualGraphicID, this.editor.ActualAlternativeID].XDiff;
				this.yNumericUpDown.Value = this.editor.Zoom1Archive.Animation[this.editor.ActualGraphicID, this.editor.ActualAlternativeID].YDiff;
				this.widthNumericUpDown.Value = this.editor.Zoom1Archive.Animation[this.editor.ActualGraphicID, this.editor.ActualAlternativeID].Width;
				this.heightNumericUpDown.Value = this.editor.Zoom1Archive.Animation[this.editor.ActualGraphicID, this.editor.ActualAlternativeID].Height;
				this.numericCodeChanged = false;
				this.xLabel.Visible = true;
				this.xNumericUpDown.Visible = true;
				this.yLabel.Visible = true;
				this.yNumericUpDown.Visible = true;
				this.widthLabel.Visible = true;
				this.widthNumericUpDown.Visible = true;
				this.heightLabel.Visible = true;
				this.heightNumericUpDown.Visible = true;
				this.noAnimationLabel.Visible = false;
				this.createAnimationProgramButton.Visible = false;
				AnimationProgram program = this.editor.Zoom1Archive.Animation[this.editor.ActualGraphicID, this.editor.ActualAlternativeID];
				for (int i = 0; i < program.AnimationStepCount; i++)
				{
					this.dataGridView.Rows.Add(program[i].AnimationPhase.ToString(CultureInfo.InvariantCulture), program[i].MinimumTime.ToString(CultureInfo.InvariantCulture), program[i].MaximumTime.ToString(CultureInfo.InvariantCulture), program[i].Sound.ToString(CultureInfo.InvariantCulture));
				}
				editor.UpdateAnimation(true);
				return true;
			}
			this.dataGridView.Visible = false;
			this.addAnimationStepButton.Visible = false;
			this.deleteAnimationStepButton.Visible = false;
			this.upAnimationStepButton.Visible = false;
			this.downAnimationStepButton.Visible = false;
			this.deleteAnimationButton.Visible = false;
			this.xLabel.Visible = false;
			this.xNumericUpDown.Visible = false;
			this.yLabel.Visible = false;
			this.yNumericUpDown.Visible = false;
			this.widthLabel.Visible = false;
			this.widthNumericUpDown.Visible = false;
			this.heightLabel.Visible = false;
			this.heightNumericUpDown.Visible = false;
			this.noAnimationLabel.Visible = true;
			this.createAnimationProgramButton.Visible = true;
			editor.UpdateAnimation(false);
			return false;
		}

		private void addAnimationStepButton_Click(object sender, EventArgs e)
		{
			this.dataGridView.Rows.Add("0", "0", "0", "0");
			this.editor.Zoom1Archive.Animation[this.editor.ActualGraphicID, this.editor.ActualAlternativeID].AddAnimationStep(new AnimationStep(0, 0, 0, 0));
			this.editor.UserMadeChanges(true);
		}

		private void deleteAnimationStepButton_Click(object sender, EventArgs e)
		{
			if (this.dataGridView.SelectedCells.Count <= 0)
				return;
			if (MessageBox.Show("Do you really want to delete the entry?", "Delete Animationstep", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			else
			{
				this.editor.Zoom1Archive.Animation[this.editor.ActualGraphicID, this.editor.ActualAlternativeID].RemoveAnimationStep(this.dataGridView.CurrentCell.RowIndex);
				this.dataGridView.Rows.RemoveAt(this.dataGridView.CurrentCell.RowIndex);
				this.editor.UserMadeChanges(true);
			}
		}

		private void upAnimationStepButton_Click(object sender, EventArgs e)
		{
			this.MoveStep(this.dataGridView.CurrentCell.RowIndex, this.dataGridView.CurrentCell.RowIndex - 1);
		}

		private void downAnimationStepButton_Click(object sender, EventArgs e)
		{
			this.MoveStep(this.dataGridView.CurrentCell.RowIndex, this.dataGridView.CurrentCell.RowIndex + 1);
		}

		private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
		{
			if (MessageBox.Show("Do you really want to delete the entry?", "Delete Animationstep", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				e.Cancel = true;
			}
			else
			{
				this.editor.Zoom1Archive.Animation[this.editor.ActualGraphicID, this.editor.ActualAlternativeID].RemoveAnimationStep(e.Row.Index);
				this.editor.UserMadeChanges(true);
			}
		}

		private void dataGridView_MouseMove(object sender, MouseEventArgs e)
		{
			if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
			{
				// If the mouse moves outside the rectangle, start the drag.
				if (dragBoxFromMouseDown != Rectangle.Empty &&
					!dragBoxFromMouseDown.Contains(e.X, e.Y))
				{
					// Proceed with the drag and drop, passing in the list item.
					DragDropEffects dropEffect = dataGridView.DoDragDrop(
					dataGridView.Rows[rowIndexFromMouseDown],
					DragDropEffects.Move);
				}
			}
		}

		private void dataGridView_MouseDown(object sender, MouseEventArgs e)
		{
			// Get the index of the item the mouse is below.
			rowIndexFromMouseDown = dataGridView.HitTest(e.X, e.Y).RowIndex;
			if (rowIndexFromMouseDown != -1)
			{
				// Remember the point where the mouse down occurred.
				// The DragSize indicates the size that the mouse can move
				// before a drag event should be started.
				Size dragSize = SystemInformation.DragSize;

				// Create a rectangle using the DragSize, with the mouse position being
				// at the center of the rectangle.
				dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2),
															   e.Y - (dragSize.Height / 2)),
																dragSize);
			}
			else
				// Reset the rectangle if the mouse is not over an item in the ListBox.
				dragBoxFromMouseDown = Rectangle.Empty;
		}

		private void dataGridView_DragOver(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Move;
		}

		private void dataGridView_DragDrop(object sender, DragEventArgs e)
		{
			// The mouse locations are relative to the screen, so they must be
			// converted to client coordinates.
			Point clientPoint = dataGridView.PointToClient(new Point(e.X, e.Y));

			// Get the row index of the item the mouse is below.
			rowIndexOfItemUnderMouseToDrop =
				dataGridView.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

			// If the drag operation was a move then remove and insert the row.
			if (e.Effect == DragDropEffects.Move)
			{
				//DataGridViewRow rowToMove = e.Data.GetData(
				//	typeof(DataGridViewRow)) as DataGridViewRow;
				//dataGridView.Rows.RemoveAt(rowIndexFromMouseDown);
				//dataGridView.Rows.Insert(rowIndexOfItemUnderMouseToDrop, rowToMove);
				this.MoveStep(rowIndexFromMouseDown, rowIndexOfItemUnderMouseToDrop);
			}
		}

		private void MoveStep(int actualRow, int targetRow)
		{
			if (actualRow == -1 || targetRow == -1 || targetRow >= dataGridView.Rows.Count || targetRow >= editor.Zoom1Archive.Animation[editor.ActualGraphicID, editor.ActualAlternativeID].AnimationStepCount)
				return;
			DataGridViewRow rowToMove = dataGridView.Rows[actualRow];
			dataGridView.Rows.Remove(rowToMove);
			this.cellValueCodeChanged = true;
			dataGridView.Rows.Insert(targetRow, rowToMove);
			AnimationStep step = this.editor.Zoom1Archive.Animation[editor.ActualGraphicID, editor.ActualAlternativeID][actualRow];
			this.editor.Zoom1Archive.Animation[editor.ActualGraphicID, editor.ActualAlternativeID].RemoveAnimationStep(actualRow);
			this.editor.Zoom1Archive.Animation[editor.ActualGraphicID, editor.ActualAlternativeID].InsertAnimationStep(step, targetRow);
			this.dataGridView.CurrentCell = this.dataGridView.Rows[targetRow].Cells[0];
			this.editor.UserMadeChanges(true);
		}

		private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
		{
			if (this.cellValueCodeChanged)
			{
				this.cellValueCodeChanged = false;
				return;
			}
			if (this.dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == e.FormattedValue || editor.Zoom1Archive.Animation == null || editor.Zoom1Archive.Animation[editor.ActualGraphicID, editor.ActualAlternativeID] == null)
				return;

			if (e.RowIndex >= 0)
			{
				int value = 0;
				bool result = int.TryParse(e.FormattedValue as String, NumberStyles.Integer, CultureInfo.CurrentCulture, out value);
				if (!result)
				{
					MessageBox.Show("Value is not a number", "Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					e.Cancel = true;
					return;
				}
				switch (e.ColumnIndex)
				{
					case 0:
						editor.Zoom1Archive.Animation[editor.ActualGraphicID, editor.ActualAlternativeID][e.RowIndex].AnimationPhase = value;
						break;

					case 1:
						editor.Zoom1Archive.Animation[editor.ActualGraphicID, editor.ActualAlternativeID][e.RowIndex].MinimumTime = value;
						break;

					case 2:
						editor.Zoom1Archive.Animation[editor.ActualGraphicID, editor.ActualAlternativeID][e.RowIndex].MaximumTime = value;
						break;

					case 3:
						editor.Zoom1Archive.Animation[editor.ActualGraphicID, editor.ActualAlternativeID][e.RowIndex].Sound = value;
						break;

					default:
						MessageBox.Show("Unknown Column", "Unknown Column", MessageBoxButtons.OK, MessageBoxIcon.Error);
						break;
				}
				this.editor.UserMadeChanges(true);
			}
		}

		private void createAnimationProgramButton_Click(object sender, EventArgs e)
		{
			if (editor.Zoom1Archive.Animation == null)
				editor.Zoom1Archive.AddAnimation();
			if (editor.Zoom1Archive.Animation[editor.ActualGraphicID, editor.ActualAlternativeID] == null)
				editor.Zoom1Archive.Animation.AddAnimationProgram(new AnimationProgram(0, 0, 1, 1), editor.ActualGraphicID, editor.ActualAlternativeID);
			ChangeAnimationProgram();
			this.editor.UserMadeChanges(true);
			editor.UpdateAnimation(true);
		}

		private void deleteAnimationButton_Click(object sender, EventArgs e)
		{
			if (editor.Zoom1Archive.Animation != null && editor.Zoom1Archive.Animation[editor.ActualGraphicID, editor.ActualAlternativeID] != null)
			{
				DialogResult result = MessageBox.Show("Do you really want to delete the animation?", "Delete animation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (result == DialogResult.Yes)
				{
					editor.Zoom1Archive.Animation.RemoveAnimationProgram(editor.ActualGraphicID, editor.ActualAlternativeID);
					if (editor.Zoom1Archive.Animation.AnimationProgramCount <= 0)
					{
						editor.Zoom1Archive.RemoveAnimation();
					}
					if (MessageBox.Show("Do you also want to delete the graphics for the animationsteps?", "Delete graphics for animationsteps", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						for (int i = 1; i <= Constants.MaxAnimationPhase; i++)
						{
							if (editor.Zoom1Archive[editor.ActualGraphicID, i, editor.ActualAlternativeID] != null) editor.Zoom1Archive.RemoveGraphic(editor.ActualGraphicID, i, editor.ActualAlternativeID);
						}
					}
					this.ChangeAnimationProgram();
					this.editor.UserMadeChanges(true);
					editor.ResetAnimationNumericUpDown();
					editor.UpdateAnimation(false);
				}
			}
		}

		private void xNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (!this.numericCodeChanged && editor.Zoom1Archive.Animation != null && editor.Zoom1Archive.Animation[editor.ActualGraphicID, editor.ActualAlternativeID] != null)
			{
				if (widthNumericUpDown.Value + xNumericUpDown.Value > 2)
				{
					this.widthNumericUpDown.Value = 2 - (int)this.xNumericUpDown.Value;
				}
				editor.Zoom1Archive.Animation[editor.ActualGraphicID, editor.ActualAlternativeID].XDiff = (int)this.xNumericUpDown.Value;
				this.editor.UserMadeChanges(true);
				editor.graphicPanel.Invalidate();
			}
		}

		private void yNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (!this.numericCodeChanged && editor.Zoom1Archive.Animation != null && editor.Zoom1Archive.Animation[editor.ActualGraphicID, editor.ActualAlternativeID] != null)
			{
				if (heightNumericUpDown.Value + yNumericUpDown.Value > 7)
				{
					this.heightNumericUpDown.Value = 7 - (int)this.yNumericUpDown.Value;
				}
				editor.Zoom1Archive.Animation[editor.ActualGraphicID, editor.ActualAlternativeID].YDiff = (int)this.yNumericUpDown.Value;
				this.editor.UserMadeChanges(true);
				editor.graphicPanel.Invalidate();
			}
		}

		private void widthNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (!this.numericCodeChanged && editor.Zoom1Archive.Animation != null && editor.Zoom1Archive.Animation[editor.ActualGraphicID, editor.ActualAlternativeID] != null)
			{
				if (widthNumericUpDown.Value + xNumericUpDown.Value > 2)
				{
					int result = 2 - (int)xNumericUpDown.Value;
					if (result == widthNumericUpDown.Value)
						return;
					this.widthNumericUpDown.Value = result;
				}
				editor.Zoom1Archive.Animation[editor.ActualGraphicID, editor.ActualAlternativeID].Width = (int)this.widthNumericUpDown.Value;
				this.editor.UserMadeChanges(true);
				editor.graphicPanel.Invalidate();
			}
		}

		private void heightNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (!this.numericCodeChanged && editor.Zoom1Archive.Animation != null && editor.Zoom1Archive.Animation[editor.ActualGraphicID, editor.ActualAlternativeID] != null)
			{
				if (heightNumericUpDown.Value + yNumericUpDown.Value > 7)
				{
					int result = 7 - (int)yNumericUpDown.Value;
					if (result == heightNumericUpDown.Value)
						return;
					this.heightNumericUpDown.Value = result;
				}
				editor.Zoom1Archive.Animation[editor.ActualGraphicID, editor.ActualAlternativeID].Height = (int)this.heightNumericUpDown.Value;
				this.editor.UserMadeChanges(true);
				editor.graphicPanel.Invalidate();
			}
		}

		private void AnimationForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				this.Hide();
			}
		}
	}
}