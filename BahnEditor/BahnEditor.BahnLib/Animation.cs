using System;
using System.Collections.Generic;
using System.IO;

namespace BahnEditor.BahnLib
{
	public class Animation
	{
		/*private Graphic[] Graphics;

		public Animation(GraphicArchive archive, int elementID, int alternativeID)
		{
			this.Graphics = archive[(element => element.ElementNumber == elementID && element.Alternative == alternativeID)].Select((s=>s.Graphic)).ToArray<Graphic>();
		}
		*/

		#region Private Fields

		private Dictionary<Tuple<int, int>, AnimationProgram> animationPrograms = new Dictionary<Tuple<int, int>, AnimationProgram>();

		#endregion Private Fields


		#region Public Indexers

		public AnimationProgram this[int elementID, int alternative]
		{
			get
			{
				return animationPrograms[new Tuple<int, int>(elementID, alternative)];
			}
		}

		#endregion Public Indexers


		#region Public Methods

		public static Animation Load(string path)
		{
			if (File.Exists(path))
			{
				using (FileStream stream = File.OpenRead(path))
				{
					return Load(stream);
				}
			}
			else throw new FileNotFoundException("File not found", path);
		}

		public void AddAnimationProgram(AnimationProgram program, int elementID, int alternative)
		{
			this.animationPrograms.Add(new Tuple<int, int>(elementID, alternative), program);
		}

		public void RemoveAnimationProgram(int elementID, int alternative)
		{
			this.animationPrograms.Remove(new Tuple<int, int>(elementID, alternative));
		}

		public bool Save(string path, bool overwrite)
		{
			if (path == null)
				throw new ArgumentNullException("path");
			if (File.Exists(path) && !overwrite)
			{
				return false;
			}
			using (FileStream stream = File.OpenWrite(path))
			{
				return Save(stream);
			}
		}

		#endregion Public Methods


		#region Private Methods

		private static Animation Load(FileStream stream)
		{
			return null;
		}

		private bool Save(FileStream stream)
		{
			return false;
		}

		#endregion Private Methods
	}
}