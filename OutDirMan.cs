using System;
using System.IO;

namespace netz
{

	public class OutDirMan
	{
		private static string outDir = null;
		
		private OutDirMan()
		{}

		public static string OutDir
		{
			get { return outDir; }
			set { if(value != null)
					  outDir = Path.GetFullPath(value);
			}
		}

		public static void Make()
		{
			if(OutDir == null) return;
			Directory.CreateDirectory(OutDir);
			Logger.Log("Output directory: " + OutDirMan.OutDir);
		}

		public static string MakeOutFileName(string file)
		{
			return Path.Combine(OutDir, Path.GetFileName(file));
		}

	}//EOC
}
