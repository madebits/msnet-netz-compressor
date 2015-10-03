using System;
using System.IO;

namespace netz
{

	public class Zipper
	{
		private Zipper()
		{}

		public static string MakeZipFileName(string file, bool useOutDir)
		{
			string temp = Path.GetFileNameWithoutExtension(file) + "z"
				+ Path.GetExtension(file);
			if(useOutDir)
				temp = OutDirMan.MakeOutFileName(temp);
			else
				temp = Path.Combine(Path.GetDirectoryName(file), temp);
			return temp;
		}

		public static void ZipFile(string file, string zipFile, GenData genData)
		{
			if(genData.CompressProvider.Provider == null) throw new Exception("E1013 Compress provider is not initialized");
			Netz.LogZipSize(genData.CompressProvider.Provider.Compress(file, zipFile));
		}

	}//EOC
}
