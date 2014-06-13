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
		public static Graphic Load(string path) {
			if (File.Exists(path))
			{
				using (FileStream stream = File.OpenRead(path))
				{
					return Load(stream);
				}
			}
			else throw new FileNotFoundException("File not found", path);
		}

		private static Graphic Load(FileStream path)
		{
			using (BinaryReader br = new BinaryReader(path))
			{
				//TODO Implement Load(FileStream)

				throw new NotImplementedException();
			}
		}

		public bool Save(string path, bool overwrite)
		{
			if (File.Exists(path) && !overwrite)
			{
				return false;
			}
			else
			{
				using (FileStream stream = File.OpenWrite(path))
				{
					return Save(stream);
				}
			}
		}

		private bool Save(FileStream path)
		{
			try
			{
				using (BinaryWriter bw = new BinaryWriter(path, Encoding.Unicode))
				{
					//TODO Implement Save(FileStream)
					
					return true;
				}
				
			}
			catch (Exception) //TODO Exchange general exceptions with specific ones
			{
				return false;
			}
		}
	}
}
