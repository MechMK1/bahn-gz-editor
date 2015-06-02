using System;
using System.Collections.Generic;
using System.Globalization;
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

		#region Internal Constructors

		internal Animation()
		{
		}

		#endregion Internal Constructors

		#region Public Indexers

		public AnimationProgram this[int elementID, int alternative]
		{
			get
			{
				if (!animationPrograms.ContainsKey(new Tuple<int, int>(elementID, alternative)))
					return null;
				return animationPrograms[new Tuple<int, int>(elementID, alternative)];
			}
		}

		#endregion Public Indexers

		#region Public Methods

		public static Animation Load(string path)
		{
			if (File.Exists(path))
			{
				return Load(File.ReadAllLines(path));
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

		public int AnimationProgramCount
		{
			get
			{
				return this.animationPrograms.Count;
			}
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

		private static Animation Load(string[] lines)
		{
			Animation animation = new Animation();

			for (int i = 0; i < lines.Length; i++)
			{
				if (lines[i] == "END")
					break;
				if (lines[i][0] != ';')
				{
					string line = lines[i].Replace(" ", "");
					if (line.StartsWith("xd=", StringComparison.Ordinal))
					{
						string[] s = line.Split(',');
						int xDiff = int.Parse(s[0].Substring(3), CultureInfo.InvariantCulture);
						int yDiff = int.Parse(s[2].Substring(3), CultureInfo.InvariantCulture);
						int width = int.Parse(s[1].Substring(2), CultureInfo.InvariantCulture);
						int height = int.Parse(s[3].Substring(2), CultureInfo.InvariantCulture);
						int steps = int.Parse(s[4].Substring(3), CultureInfo.InvariantCulture);
						string IDs = s[5].Substring(4);
						int alternative = Constants.NoAlternative;
						int elementID;
						if (Char.IsLetter(IDs, IDs.Length - 1))
						{
							switch (IDs[IDs.Length - 1])
							{
								case 'a':
									alternative = 1;
									break;
								case 'b':
									alternative = 2;
									break;
								case 'c':
									alternative = 3;
									break;
								case 'd':
									alternative = 4;
									break;
								default:
									throw new InvalidDataException("Alternative");
							}
							elementID = int.Parse(IDs.Remove(IDs.Length - 1), CultureInfo.InvariantCulture);
						}
						else
						{
							elementID = int.Parse(IDs, CultureInfo.InvariantCulture);
						}
						AnimationProgram AnimationProgram = new AnimationProgram(xDiff, yDiff, width, height);
						for (int j = 0; j < steps; i++, j++)
						{
							string step = lines[i + 1].Replace(" ", "");
							string[] stepArray = step.Split(',');
							int phase = int.Parse(stepArray[0], CultureInfo.InvariantCulture);
							int minimumTime = int.Parse(stepArray[1], CultureInfo.InvariantCulture);
							int maximumTime = int.Parse(stepArray[2], CultureInfo.InvariantCulture);
							int sound = int.Parse(stepArray[3], CultureInfo.InvariantCulture);
							AnimationStep animationStep = new AnimationStep(phase, minimumTime, maximumTime, sound);
							AnimationProgram.AddAnimationStep(animationStep);
						}
						animation.AddAnimationProgram(AnimationProgram, elementID, alternative);
					}
				}
			}

			return animation;
		}

		private bool Save(FileStream stream)
		{
			using (StreamWriter sw = new StreamWriter(stream, System.Text.Encoding.Unicode))
			{
				foreach (var program in this.animationPrograms)
				{
					if (program.Value.AnimationStepCount > 0)
					{
						string symbol = String.Format(CultureInfo.InvariantCulture, "{0}{1}", program.Key.Item1, program.Key.Item2 == 0 ? "" : (program.Key.Item2 - 1 + 'a').ToString(CultureInfo.InvariantCulture));
						string headLine = String.Format(CultureInfo.InvariantCulture, "xd={0},b={1},yd={2},h={3},st={4},sym={5}", program.Value.XDiff, program.Value.Width, program.Value.YDiff, program.Value.Height, program.Value.AnimationStepCount, symbol);
						sw.WriteLine(headLine);
						for (int i = 0; i < program.Value.AnimationStepCount; i++)
						{
							sw.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3}", program.Value[i].AnimationPhase, program.Value[i].MinimumTime, program.Value[i].MaximumTime, program.Value[i].Sound));
						}
					}
					sw.WriteLine(";");
				}
				sw.WriteLine("END");
			}
			return true;
		}

		#endregion Private Methods
	}
}