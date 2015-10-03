using System;
using System.Runtime.InteropServices;

namespace netz.subsys
{
	public class Win32PESubSystem
	{

		[DllImport("subsys")]
		public static extern int GetSubSystem(string file);

		public static readonly int IMAGE_SUBSYSTEM_ERROR = -1;
		public static readonly int IMAGE_SUBSYSTEM_NATIVE = 1;
		public static readonly int IMAGE_SUBSYSTEM_WINDOWS_GUI = 2;
		public static readonly int IMAGE_SUBSYSTEM_WINDOWS_CUI = 3;
		public static readonly int IMAGE_SUBSYSTEM_OS2_CUI = 5;
		public static readonly int IMAGE_SUBSYSTEM_POSIX_CUI = 7;
		public static readonly int IMAGE_SUBSYSTEM_NATIVE_WINDOWS = 8;
		public static readonly int IMAGE_SUBSYSTEM_WINDOWS_CE_GUI = 9;

	}//EOC
}