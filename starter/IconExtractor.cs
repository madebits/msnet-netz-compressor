using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

using vbAccelerator.Components.Win32;

namespace netz.starter
{

	public class IconExtractor
	{
		private IconExtractor()
		{}

		public static void SaveExeIcon(string exeFile, string iconFile)
		{
			IconEx iEx = null;
			try
			{
				iEx = new IconEx(exeFile, IDI_APPLICATION);
				iEx.Save(iconFile);
			}
			finally
			{
				if(iEx != null) iEx.Dispose();
			}
		}
			
		private static readonly int IDI_APPLICATION = 32512;
	}//EOC
}
