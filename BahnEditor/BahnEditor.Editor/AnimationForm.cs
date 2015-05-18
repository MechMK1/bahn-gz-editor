using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BahnEditor.BahnLib;

namespace BahnEditor.Editor
{
	public partial class AnimationForm : Form
	{
		private Editor editor;
		private Rectangle dragBoxFromMouseDown;
		private int rowIndexFromMouseDown;
		private int rowIndexOfItemUnderMouseToDrop;

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
			if (this.editor.Zoom1Archive != null && this.editor.Zoom1Archive.Animation != null && this.editor.Zoom1Archive.Animation[this.editor.ActualGraphicID, this.editor.ActualAlternativeID] != null)
			{
				this.dataGridView.Visible = true;
				this.addAnimationStepButton.Visible = true;
				this.deleteAnimationStepButton.Visible = true;
				this.upAnimationStepButton.Visible = true;
				this.downAnimationStepButton.Visible = true;
				this.noAnimationLabel.Visible = false;
				AnimationProgram program = this.editor.Zoom1Archive.Animation[this.editor.ActualGraphicID, this.editor.ActualAlternativeID];
				for (int i = 0; i < program.AnimationStepCount; i++)
				{
					this.dataGridView.Rows.Add(program[i].AnimationPhase.ToString(CultureInfo.InvariantCulture), program[i].MinimumTime.ToString(CultureInfo.InvariantCulture), program[i].MaximumTime.ToString(CultureInfo.InvariantCulture), program[i].Sound.ToString(CultureInfo.InvariantCulture));
				}
				return true;
			}
			this.dataGridView.Visible = false;
			this.addAnimationStepButton.Visible = false;
			this.deleteAnimationStepButton.Visible = false;
			this.upAnimationStepButton.Visible = false;
			this.downAnimationStepButton.Visible = false;
			this.noAnimationLabel.Visible = true;
			return false;
		}

		private void addAnimationStepButton_Click(object sender, EventArgs e)
		{
			this.dataGridView.Rows.Add("0", "0", "0", "0");
			this.editor.Zoom1Archive.Animation[this.editor.ActualGraphicID, this.editor.ActualAlternativeID].AddAnimationStep(new AnimationStep(0, 0, 0, 0));
		}

		private void deleteAnimationStepButton_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Should the entry be deleted?", "Delete Animationstep", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			else
			{
				this.editor.Zoom1Archive.Animation[this.editor.ActualGraphicID, this.editor.ActualAlternativeID].RemoveAnimationStep(this.dataGridView.CurrentCell.RowIndex);
				this.dataGridView.Rows.RemoveAt(this.dataGridView.CurrentCell.RowIndex);
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

		private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			
		}

		private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
		{
			if (MessageBox.Show("Should the entry be deleted?", "Delete Animationstep", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				e.Cancel = true;
			}
			else
			{
				this.editor.Zoom1Archive.Animation[this.editor.ActualGraphicID, this.editor.ActualAlternativeID].RemoveAnimationStep(e.Row.Index);
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
			dataGridView.Rows.RemoveAt(actualRow);
			dataGridView.Rows.Insert(targetRow, rowToMove);
			AnimationStep step = this.editor.Zoom1Archive.Animation[editor.ActualGraphicID, editor.ActualAlternativeID][actualRow];
			this.editor.Zoom1Archive.Animation[editor.ActualGraphicID, editor.ActualAlternativeID].RemoveAnimationStep(actualRow);
			this.editor.Zoom1Archive.Animation[editor.ActualGraphicID, editor.ActualAlternativeID].InsertAnimationStep(step, targetRow);
			this.dataGridView.CurrentCell = this.dataGridView.Rows[targetRow].Cells[0];
		}

		private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
		{
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
			}
		}
	}
}
