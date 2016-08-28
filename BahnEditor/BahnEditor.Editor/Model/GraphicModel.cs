using BahnEditor.BahnLib;
using BahnEditor.Editor.Util;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace BahnEditor.Editor
{
	public class GraphicModel : BindableBase
	{
		private int selectedGraphic;
		private int selectedAlternative;
		private int selectedAnimationPhase;
		private GraphicMode mode;
		private bool hasAlternatives;

		private ZoomFactor zoomFactor;
		private LayerID selectedLayer;
		private int zoomLevelZoom1;
		private int zoomLevelZoom2;
		private int zoomLevelZoom4;
		private GraphicArchive zoom1Archive;
		private GraphicArchive zoom2Archive;
		private GraphicArchive zoom4Archive;
		private Graphic zoom1Graphic;
		private Graphic zoom2Graphic;
		private Graphic zoom4Graphic;

		public GraphicModel()
		{
			zoomFactor = ZoomFactor.Zoom1;
			zoomLevelZoom1 = 6;
			zoomLevelZoom2 = 6;
			zoomLevelZoom4 = 8;
		}

		public uint[,] CurrentGraphic
		{
			get
			{
				if (SelectedGraphic == null)
				{
					return null;
				}
				return SelectedGraphic[selectedLayer];
			}
		}

		public GraphicArchive Zoom1Archive
		{
			get
			{
				return zoom1Archive;
			}
			set
			{
				zoom1Archive = value;
				OnPropertyChanged();
				OnPropertyChanged(() => CurrentGraphic);
			}
		}
		public GraphicArchive Zoom2Archive
		{
			get
			{
				return zoom2Archive;
			}
			set
			{
				zoom2Archive = value;
				OnPropertyChanged();
				OnPropertyChanged(() => CurrentGraphic);
			}
		}
		public GraphicArchive Zoom4Archive
		{
			get
			{
				return zoom4Archive;
			}
			set
			{
				zoom4Archive = value;
				OnPropertyChanged();
				OnPropertyChanged(() => CurrentGraphic);
			}
		}

		public Graphic Zoom1Graphic
		{
			get
			{
				return zoom1Graphic;
			}
			set
			{
				zoom1Graphic = value;
				OnPropertyChanged();
				OnPropertyChanged(() => CurrentGraphic);
			}
		}
		public Graphic Zoom2Graphic
		{
			get
			{
				return zoom2Graphic;
			}
			set
			{
				zoom2Graphic = value;
				OnPropertyChanged();
				OnPropertyChanged(() => CurrentGraphic);
			}
		}
		public Graphic Zoom4Graphic
		{
			get
			{
				return zoom4Graphic;
			}
			set
			{
				zoom4Graphic = value;
				OnPropertyChanged();
				OnPropertyChanged(() => CurrentGraphic);
			}
		}

		public ZoomFactor ZoomFactor
		{
			get
			{
				return zoomFactor;
			}
			set
			{
				zoomFactor = value;
				OnPropertyChanged();
				OnPropertyChanged(() => ZoomFactorIndex);
				OnPropertyChanged(() => ZoomLevel);
				OnPropertyChanged(() => CurrentGraphic);
				OnPropertyChanged(() => SelectedGraphic);
				OnPropertyChanged(() => CanvasHeight);
				OnPropertyChanged(() => CanvasWidth);
			}
		}

		public int ZoomFactorIndex
		{
			get
			{
				switch (ZoomFactor)
				{
					case BahnLib.ZoomFactor.Zoom1:
						return 0;
					case BahnLib.ZoomFactor.Zoom2:
						return 1;
					case BahnLib.ZoomFactor.Zoom4:
						return 2;
					default:
						return -1;
				}
			}
			set
			{
				switch (value)
				{
					case 1:

						ZoomFactor = BahnLib.ZoomFactor.Zoom2;
						break;
					case 2:
						ZoomFactor = BahnLib.ZoomFactor.Zoom4;
						break;
					case 0:
					default:
						ZoomFactor = BahnLib.ZoomFactor.Zoom1;
						break;
				}
				OnPropertyChanged();
			}
		}

		public int ZoomLevel
		{
			get
			{
				switch (this.ZoomFactor)
				{
					case ZoomFactor.Zoom1:
						return this.zoomLevelZoom1;

					case ZoomFactor.Zoom2:
						return this.zoomLevelZoom2;

					case ZoomFactor.Zoom4:
						return this.zoomLevelZoom4;

					default:
						return 6;
				}
			}
			set
			{
				switch (this.ZoomFactor)
				{
					case ZoomFactor.Zoom1:
						this.zoomLevelZoom1 = value;
						break;

					case ZoomFactor.Zoom2:
						this.zoomLevelZoom2 = value;
						break;

					case ZoomFactor.Zoom4:
						this.zoomLevelZoom4 = value;
						break;

					default:
						throw new ArgumentException("Unknown zoomFactor");
				}
				OnPropertyChanged();
				OnPropertyChanged(() => CanvasHeight);
				OnPropertyChanged(() => CanvasWidth);
			}
		}

		public LayerID SelectedLayer
		{
			get
			{
				return selectedLayer;
			}
			set
			{
				selectedLayer = value;
				OnPropertyChanged();
			}
		}

		public bool HasAlternatives
		{
			get
			{
				return hasAlternatives;
			}
			set
			{
				hasAlternatives = value;
				OnPropertyChanged();
			}
		}

		public int CanvasWidth
		{
			get
			{
				return (int)((this.ZoomLevel) * Constants.GraphicWidth) + 0;
			}
		}

		public int CanvasHeight
		{
			get
			{
				return (int)((this.ZoomLevel) * Constants.GraphicHeight) + 0;
			}
		}

		public int SelectedGraphicId
		{
			get
			{
				return selectedGraphic;
			}
			set
			{
				selectedGraphic = value;
				OnPropertyChanged();
				OnPropertyChanged(() => CurrentGraphic);
			}
		}

		public int SelectedAlternative
		{
			get
			{
				return selectedAlternative;
			}
			set
			{
				selectedAlternative = value;
				OnPropertyChanged();
				OnPropertyChanged(() => CurrentGraphic);
			}
		}

		public int SelectedAnimationPhase
		{
			get
			{
				return selectedAnimationPhase;
			}
			set
			{
				selectedAnimationPhase = value;
				OnPropertyChanged();
				OnPropertyChanged(() => CurrentGraphic);
			}
		}

		public GraphicMode Mode
		{
			get
			{
				return mode;
			}
			set
			{
				mode = value;
				OnPropertyChanged();
			}
		}

		private Graphic SelectedGraphic
		{
			get
			{
				switch (ZoomFactor)
				{
					case ZoomFactor.Zoom1:
						if (this.ActualZoom1Graphic != null)
						{
							return this.ActualZoom1Graphic;
						}
						break;

					case ZoomFactor.Zoom2:
						if (this.ActualZoom2Graphic != null)
						{
							return this.ActualZoom2Graphic;
						}
						break;

					case ZoomFactor.Zoom4:
						if (this.ActualZoom4Graphic != null)
						{
							return this.ActualZoom4Graphic;
						}
						break;

					default:
						break;
				}
				return null;
			}
		}

		private Graphic ActualZoom1Graphic
		{
			get
			{
				return (this.mode == GraphicMode.GraphicSingle || this.mode == GraphicMode.DrivingWaySingle) ? Zoom1Graphic : Zoom1Archive[SelectedGraphicId, SelectedAnimationPhase, SelectedAlternative];
			}
		}

		private Graphic ActualZoom2Graphic
		{
			get
			{
				return (this.mode == GraphicMode.GraphicSingle || this.mode == GraphicMode.DrivingWaySingle) ? Zoom2Graphic : Zoom2Archive[SelectedGraphicId, SelectedAnimationPhase, SelectedAlternative];
			}
		}

		private Graphic ActualZoom4Graphic
		{
			get
			{
				return (this.mode == GraphicMode.GraphicSingle || this.mode == GraphicMode.DrivingWaySingle) ? Zoom4Graphic : Zoom4Archive[SelectedGraphicId, SelectedAnimationPhase, SelectedAlternative];
			}
		}
		#region old
		//private void RecalculateElements()
		//{
		//	CanvasWidth = (int)((this.ZoomLevel) * Constants.GraphicWidth) + 1;
		//	CanvasHeight = (int)((this.ZoomLevel) * Constants.GraphicHeight) + 1;
		//	foreach (var item in Elements)
		//	{
		//		item.Update();
		//	}
		//}

		//private void CreateElements()
		//{
		//	Elements = new ObservableCollection<ElementModel>();
		//	for (int x = 0; x < Constants.GraphicWidth * (int)ZoomFactor; x++)
		//	{
		//		for (int y = 0; y < Constants.GraphicHeight * (int)ZoomFactor; y++)
		//		{
		//			Elements.Add(new ElementModel(this) { X = x, Y = y, FillColor = Brushes.Black});
		//		}
		//	}
		//}
	}

	//public class ElementModel : BindableBase
	//{
	//	private GraphicModel model;
	//	private uint color;

	//	public ElementModel(GraphicModel model)
	//	{
	//		this.model = model;
	//	}

	//	public int X { get; set; }
	//	public int Y { get; set; }

	//	public float RectX
	//	{
	//		get
	//		{
	//			return (X * model.ZoomLevel) / (int)model.ZoomFactor;
	//		}
	//	}

	//	public float RectY
	//	{
	//		get
	//		{
	//			return ((model.ZoomLevel * Constants.ElementHeight) - (model.ZoomLevel * (Y + 1))) / (int)model.ZoomFactor;
	//		}
	//	}

	//	public float RectWidth
	//	{
	//		get
	//		{
	//			return model.ZoomLevel / (float)model.ZoomFactor;
	//		}
	//	}

	//	public float RectHeight
	//	{
	//		get
	//		{
	//			return model.ZoomLevel / (float)model.ZoomFactor;
	//		}
	//	}

	//	public Brush FillColor
	//	{
	//		get
	//		{
	//			return new SolidColorBrush(PixelHelper.PixelToColor(color));
	//		}
	//		set
	//		{
	//			SolidColorBrush brush = value as SolidColorBrush;
	//			if (brush != null)
	//			{
	//				color = Pixel.Create(brush.Color.R, brush.Color.G, brush.Color.B);
	//				OnPropertyChanged();
	//			}
	//		}
	//	}

	//	public void Update()
	//	{
	//		OnPropertyChanged(() => RectX);
	//		OnPropertyChanged(() => RectY);
	//		OnPropertyChanged(() => RectWidth);
	//		OnPropertyChanged(() => RectHeight);
	//	}
	//}
		#endregion
}