using Microsoft.Win32;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BahnEditor.Editor.Util
{

	public class FileDialogHelper : DependencyObject
	{
		public string FileName
		{
			get { return (string)GetValue(FileNameProperty); }
			set { SetValue(FileNameProperty, value); }
		}

		public string Filter
		{
			get { return (string)GetValue(FilterProperty); }
			set { SetValue(FilterProperty, value); }
		}

		public ICommand FileOpenCommand
		{
			get { return (ICommand)GetValue(FileOpenCommandProperty); }
			set { SetValue(FileOpenCommandProperty, value); }
		}

		private static readonly DependencyProperty FileNameProperty =
			DependencyProperty.Register("FileName", typeof(string), typeof(FileDialogHelper));

		private static readonly DependencyProperty FilterProperty =
			DependencyProperty.Register("Filter", typeof(string), typeof(FileDialogHelper));

		private static readonly DependencyProperty FileOpenCommandProperty =
			DependencyProperty.Register("FileOpenCommand", typeof(ICommand), typeof(FileDialogHelper));

		public ICommand OpenFileDialog { get; set; }

		public FileDialogHelper()
		{
			OpenFileDialog = new DelegateCommand(OpenFileAction);
		}

		private void OpenFileAction()
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = Filter;
			if (dialog.ShowDialog() == true)
			{
				FileName = dialog.FileName;
				FileOpenCommand.Execute(null);
			}
		}
	}
}
