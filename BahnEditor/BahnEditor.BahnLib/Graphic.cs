using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BahnEditor.BahnLib
{
	public class Graphic
	{
		public Graphic Load(string path) {
			if (File.Exists(path))
			{
				return Load(File.OpenRead(path));
			}
			else throw new FileNotFoundException("File not found", path);
		}

		private Graphic Load(FileStream path)
		{
			//TODO Implement Load(FileStream)

			throw new NotImplementedException();
		}

		public bool Save(string path, bool overwrite)
		{
			if (File.Exists(path) && !overwrite)
			{
				return false;
			}
			else
			{
				return Save(File.OpenWrite(path));
			}
		}

		private bool Save(FileStream path)
		{
			try
			{
				//TODO Implement Save(FileStream)

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
