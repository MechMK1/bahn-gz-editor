using BahnEditor.BahnLib;
using System;
using System.Windows.Forms;

namespace BahnEditor.Editor
{
	public partial class DrivingWaySettingsForm : Form
	{
		public DrivingWay DrivingWay { get; set; }

		public Direction DirectionArrival { get; set; }

		public Direction DirectionDeparture { get; set; }

		public DrivingWayFunction DrivingWayFunction { get; set; }

		public bool Delete { get; set; }

		public DrivingWaySettingsForm()
		{
			InitializeComponent();
			this.typeComboBox.SelectedIndex = 0;
			this.additionalFunctionComboBox.SelectedIndex = 0;
		}

		public DrivingWaySettingsForm(DrivingWayElement drivingWayElement)
		{
			InitializeComponent();
			this.DrivingWay = drivingWayElement.DrivingWay;
			this.DrivingWayFunction = drivingWayElement.Function;
			this.DirectionArrival = drivingWayElement.Arrival;
			this.DirectionDeparture = drivingWayElement.Departure;

			this.typeComboBox.SelectedIndex = (int)DrivingWay;
			this.arrivalComboBox.SelectedIndex = (int)DirectionArrival;
			this.departureComboBox.SelectedIndex = (int)DirectionDeparture;
			this.additionalFunctionComboBox.SelectedIndex = ((int)DrivingWayFunction) & (int)~BahnLib.DrivingWayFunction.Crossing;
			this.crossingCheckBox.Checked = ((int)DrivingWayFunction & (1 << 7)) != 0;
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			if (typeComboBox.SelectedIndex <= 0 || typeComboBox.SelectedIndex > 4)
			{
				this.Delete = true;
				return;
			}
			else
			{
				this.Delete = false;
				this.DrivingWay = (DrivingWay)typeComboBox.SelectedIndex;
			}

			if (this.crossingCheckBox.Checked)
			{
				this.DrivingWayFunction |= BahnLib.DrivingWayFunction.Crossing;
			}
			else
			{
				this.DrivingWayFunction &= ~BahnLib.DrivingWayFunction.Crossing;
			}

			switch (this.additionalFunctionComboBox.SelectedIndex)
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

			this.DirectionArrival = (Direction)this.arrivalComboBox.SelectedIndex;
			this.DirectionDeparture = (Direction)this.departureComboBox.SelectedIndex;
		}
	}
}