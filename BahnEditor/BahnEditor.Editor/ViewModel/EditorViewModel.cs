using BahnEditor.BahnLib;
using BahnEditor.Editor.Util;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BahnEditor.Editor
{
	public class EditorViewModel : BindableBase
	{
		private string statusBarText;
		private string fileName;

		private bool gridVisible;

		public GraphicModel Model { get; set; }

		public bool GridVisible
		{
			get
			{
				return gridVisible;
			}
			set
			{
				gridVisible = value;
				OnPropertyChanged();
			}
		}

		public string StatusBarText
		{
			get
			{
				return statusBarText;
			}
			set
			{
				statusBarText = value;
				OnPropertyChanged();
			}
		}

		public DelegateCommand<object> NewCommand { get; set; }
		public DelegateCommand OpenCommand { get; set; }
		public DelegateCommand SaveCommand { get; set; }
		public DelegateCommand ExitCommand { get; set; }
		public DelegateCommand ZoomInCommand { get; set; }
		public DelegateCommand ZoomOutCommand { get; set; }
		public DelegateCommand OverviewUpCommand { get; set; }
		public DelegateCommand OverviewDownCommand { get; set; }
		public DelegateCommand OverviewLeftRightCommand { get; set; }

		public string FileName
		{
			get
			{
				return fileName;
			}
			set
			{
				fileName = value;
				OnPropertyChanged();
			}
		}

		public EditorViewModel()
		{
			Model = new GraphicModel();
			StatusBarText = String.Format("Zoomlevel: {0}  Zoomfactor: {1}", Model.ZoomLevel, Model.ZoomFactor.ToString());
			NewCommand = new DelegateCommand<object>(NewAction);
			OpenCommand = new DelegateCommand(OpenAction);
			SaveCommand = new DelegateCommand(SaveAction);
			ExitCommand = new DelegateCommand(ExitAction);
			ZoomInCommand = new DelegateCommand(ZoomInAction, CanZoomIn);
			ZoomOutCommand = new DelegateCommand(ZoomOutAction, CanZoomOut);
			NewGraphic(GraphicMode.GraphicArchive);
		}


		#region Command Actions

		private void NewAction(object graphicMode)
		{
			NewGraphic((GraphicMode)graphicMode);
		}

		private void OpenAction()
		{
			LoadGraphic();
		}

		private void SaveAction()
		{
			MessageBox.Show("Saved");
		}

		private void ExitAction()
		{
			Application.Current.MainWindow.Close();
		}

		private void ZoomInAction()
		{
			Model.ZoomLevel += (int)Model.ZoomFactor;
			UpdateZoomButtons();
		}

		private void ZoomOutAction()
		{
			Model.ZoomLevel -= (int)Model.ZoomFactor;
			UpdateZoomButtons();
		}

		private bool CanZoomIn()
		{
			return !(Model.ZoomLevel > 7 * (int)Model.ZoomFactor);
		}

		private bool CanZoomOut()
		{
			return !(Model.ZoomLevel < 1 * (int)Model.ZoomFactor + 1);
		}

		#endregion Command Actions

		private void NewGraphic(GraphicMode mode)
		{
			if (mode == GraphicMode.DrivingWaySingle || mode == GraphicMode.GraphicSingle)
			{
				Model.Zoom1Graphic = new Graphic("", ZoomFactor.Zoom1);
				Model.Zoom2Graphic = new Graphic("", ZoomFactor.Zoom2);
				Model.Zoom4Graphic = new Graphic("", ZoomFactor.Zoom4);
			}
			else
			{
				Model.Zoom1Archive = new GraphicArchive(ZoomFactor.Zoom1);
				Model.Zoom2Archive = new GraphicArchive(ZoomFactor.Zoom2);
				Model.Zoom4Archive = new GraphicArchive(ZoomFactor.Zoom4);
			}
			Model.SelectedLayer = LayerID.Foreground;
			Model.Mode = mode;
		}

		private void LoadGraphic()
		{
			if (FileName.EndsWith(".uz1"))
			{
				LoadGraphicArchive();
			}
			else if (FileName.EndsWith(".gz1"))
			{
				LoadGraphicSingle();
			}
			else
			{

			}
		}

		private void LoadGraphicArchive()
		{
			try
			{
				Model.Zoom1Archive = GraphicArchive.Load(FileName);
				if (Model.Zoom1Archive.ZoomFactor != ZoomFactor.Zoom1)
				{
					throw new InvalidDataException("Zoomfactor of archive not 1");
				}
				try
				{
					Model.Zoom2Archive = GraphicArchive.Load(FileName.Remove(FileName.Length - 3) + "uz2");
					if (Model.Zoom2Archive.ZoomFactor != ZoomFactor.Zoom2)
					{
						throw new InvalidDataException("Zoom2-file found, internal zoomfactor not 2");
					}
				}
				catch (FileNotFoundException)
				{
					Model.Zoom2Archive = new GraphicArchive(ZoomFactor.Zoom2);
				}
				try
				{
					Model.Zoom4Archive = GraphicArchive.Load(FileName.Remove(FileName.Length - 3) + "uz4");
					if (Model.Zoom4Archive.ZoomFactor != ZoomFactor.Zoom4)
					{
						throw new InvalidDataException("Zoom4-file found, internal zoomfactor not 4");
					}
				}
				catch (FileNotFoundException)
				{
					Model.Zoom4Archive = new GraphicArchive(ZoomFactor.Zoom4);
				}
				for (int i = 0; i < 90; i++)
				{
					if (Model.Zoom1Archive[i] != null)
					{
						if (Model.Zoom1Archive[i].Properties.RawData.HasFlag(GraphicProperties.Properties.DrivingWay))
						{
							Model.Mode = GraphicMode.DrivingWayArchive;
						}
						else
						{
							Model.Mode = GraphicMode.GraphicArchive;
						}
						break;
					}
				}
				Model.SelectedAlternative = 0;
				Model.SelectedAnimationPhase = 0;
				Model.SelectedGraphicId = 0;
				Model.SelectedLayer = LayerID.Foreground;
				Model.ZoomFactor = ZoomFactor.Zoom1;
			}
			catch (Exception ex)
			{
				MessageBox.Show(String.Format("There was an error while loading: {0}!", ex.ToString()), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				this.NewGraphic(GraphicMode.GraphicArchive);
			}
		}

		private void LoadGraphicSingle()
		{
			try
			{
				Model.Zoom1Graphic = Graphic.Load(FileName);
				if (Model.Zoom1Graphic.ZoomFactor != ZoomFactor.Zoom1)
				{
					throw new InvalidDataException("Zoomfactor of graphic not 1");
				}
				try
				{
					Model.Zoom2Graphic = Graphic.Load(FileName.Remove(FileName.Length - 3) + "gz2");
					if (Model.Zoom2Graphic.ZoomFactor != ZoomFactor.Zoom2)
					{
						throw new InvalidDataException("Zoom2-file found, internal zoomfactor not 2");
					}
				}
				catch (FileNotFoundException)
				{
					Model.Zoom2Graphic = null;
				}
				try
				{
					Model.Zoom4Graphic = Graphic.Load(FileName.Remove(FileName.Length - 3) + "gz4");
					if (Model.Zoom4Graphic.ZoomFactor != ZoomFactor.Zoom4)
					{
						throw new InvalidDataException("Zoom4-file found, internal zoomfactor not 4");
					}
				}
				catch (FileNotFoundException)
				{
					Model.Zoom4Graphic = null;
				}
				if (Model.Zoom1Graphic.Properties.RawData.HasFlag(GraphicProperties.Properties.DrivingWay))
				{
					Model.Mode = GraphicMode.DrivingWaySingle;
				}
				else
				{
					Model.Mode = GraphicMode.GraphicSingle;
				}
				Model.SelectedAlternative = 0;
				Model.SelectedAnimationPhase = 0;
				Model.SelectedGraphicId = 0;
				Model.SelectedLayer = LayerID.Foreground;
				Model.ZoomFactor = ZoomFactor.Zoom1;
			}
			catch (Exception ex)
			{
				MessageBox.Show(String.Format("There was an error while loading: {0}!", ex.ToString()), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				this.NewGraphic(GraphicMode.GraphicArchive);
			}
		}

		private void UpdateZoomButtons()
		{
			ZoomInCommand.RaiseCanExecuteChanged();
			ZoomOutCommand.RaiseCanExecuteChanged();
			StatusBarText = String.Format("Zoomlevel: {0}  Zoomfactor: {1}", Model.ZoomLevel, Model.ZoomFactor.ToString());
		}
	}
}
