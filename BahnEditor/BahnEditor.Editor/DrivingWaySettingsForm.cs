using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BahnEditor.BahnLib;

namespace BahnEditor.Editor
{
	public partial class DrivingWaySettingsForm : Form
	{
		private bool typeComboBoxCC = false;
		private bool arrivalComboBoxCC = false;
		private bool departureComboBoxCC = false;
		private bool additionalFunctionComboBoxCC = false;
		private bool crossingCheckBoxCC = false;

		public DrivingWay DrivingWay { get; set; }

		public Direction DirectionArrival { get; set; }

		public Direction DirectionDeparture { get; set; }

		public DrivingWayFunction DrivingWayFunction { get; set; }

		public bool Delete { get; set; }

		public DrivingWaySettingsForm()
		{
			InitializeComponent();
			this.typeComboBoxCC = true;
			this.additionalFunctionComboBoxCC = true;
			this.typeComboBox.SelectedIndex = 0;
			this.additionalFunctionComboBox.SelectedIndex = 0;
			this.typeComboBoxCC = false;
			this.additionalFunctionComboBoxCC = false;
		}

		public DrivingWaySettingsForm(DrivingWayElement drivingWayElement)
		{
			InitializeComponent();
			this.DrivingWay = drivingWayElement.DrivingWay;
			this.DrivingWayFunction = drivingWayElement.Function;
			this.DirectionArrival = drivingWayElement.Arrival;
			this.DirectionDeparture = drivingWayElement.Departure;

			this.typeComboBoxCC = true;
			this.arrivalComboBoxCC = true;
			this.departureComboBoxCC = true;
			this.additionalFunctionComboBoxCC = true;
			this.crossingCheckBoxCC = true;
			
			this.typeComboBox.SelectedIndex = (int)DrivingWay;
			this.arrivalComboBox.SelectedIndex = (int)DirectionArrival;
			this.departureComboBox.SelectedIndex = (int)DirectionDeparture;
			this.additionalFunctionComboBox.SelectedIndex = ((int)DrivingWayFunction) & (int)~BahnLib.DrivingWayFunction.Crossing;
			this.crossingCheckBox.Checked = ((int)DrivingWayFunction & (1 << 7)) != 0;

			this.typeComboBoxCC = false;
			this.arrivalComboBoxCC = false;
			this.departureComboBoxCC = false;
			this.additionalFunctionComboBoxCC = false;
			this.crossingCheckBoxCC = false;
		}

		private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.typeComboBoxCC)
				return;

			if (typeComboBox.SelectedIndex == 0 || typeComboBox.SelectedIndex > 4)
			{
				this.Delete = true;
			}
			else
			{
				this.Delete = false;
				this.DrivingWay = (DrivingWay)typeComboBox.SelectedIndex;
			}
		}

		private void arrivalComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.arrivalComboBoxCC)
				return;

			this.DirectionArrival = (Direction)this.arrivalComboBox.SelectedIndex;
		}

		private void departureComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.departureComboBoxCC)
				return;

			this.DirectionDeparture = (Direction)this.departureComboBox.SelectedIndex;
		}

		private void additionalFunctionComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.additionalFunctionComboBoxCC)
				return;

			switch(this.additionalFunctionComboBox.SelectedIndex)
			{
				case 0:
					this.DrivingWayFunction &= BahnLib.DrivingWayFunction.Crossing;
					break;
				case 1:
					this.DrivingWayFunction &= BahnLib.DrivingWayFunction.Crossing;
					this.DrivingWayFunction |= BahnLib.DrivingWayFunction.TunnelIn;
					break;
				case 2:
					this.DrivingWayFunction &= BahnLib.DrivingWayFunction.Crossing;
					this.DrivingWayFunction |= BahnLib.DrivingWayFunction.TunnelOut;
					break;
				case 3:
					this.DrivingWayFunction &= BahnLib.DrivingWayFunction.Crossing;
					this.DrivingWayFunction |= BahnLib.DrivingWayFunction.Ramp;
					break;
				default:
					break;
			}
		}

		private void crossingCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (this.crossingCheckBoxCC)
				return;

			if(this.crossingCheckBox.Checked)
			{
				this.DrivingWayFunction |= BahnLib.DrivingWayFunction.Crossing;
			}
			else
			{
				this.DrivingWayFunction &= ~BahnLib.DrivingWayFunction.Crossing;
			}
		}
	}
}
