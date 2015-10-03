using System;

namespace netz
{
	public class Help
	{
		internal static readonly int COL_NETZ = ColorConsole.FOREGROUND_RED
			| ColorConsole.FOREGROUND_GREEN
			| ColorConsole.FOREGROUND_INTENSITY;
		
		private Help()
		{}

		internal static readonly string EMAIL = "http://madebits.com";

		private static int GetYear()
		{
			int year = DateTime.Now.Year;
			if(year < 2004) year = 2004;
			return year;
		}

		internal static void PrintLogo()
		{
			ColorConsole c = null;
			try
			{
				c = new ColorConsole();
				c.SetColor(COL_NETZ);
			}
			catch{}
		
			Puts(" __________________________________________ ");
			Puts("|                                          |");
			Puts("| .NETZ - .NET Executables Compressor      |");
			Puts("| Copyright (C) 2004-" + GetYear() + " Vasian Cepa      |");
			Puts2("| [v" + Utils.NetzVersion);
			Puts("]  " + EMAIL +  "            |");
			Puts("|__________________________________________|");
			Puts(string.Empty);

			try
			{
				if(c != null) c.Reset();
			}
			catch{}
		}

		internal static void ShowHelp()
		{
			Puts("Usage: netz [-s] [-so] [exe file] [[-d] dll file]* [-a assemblyAttributes file]");
            Puts("            [-o outPutFolder]");
			Puts("            [-aw] [-p privatePath] [-i win32icon] [-n] [-c | -w] [-mta]");
			Puts("            [-pl platform] [-x86]");
			Puts("            ([-z] [-r compressLib] [-l redistributableCompressLib])");
			Puts("            ([-kf] [-kn] [-kd] | [-ka])");
			Puts("            [-sr] [-srp file]");
			Puts("            [-v] [-b] [-!]");
			Puts("            [-xr] name [[-d] dll file]*");
			Puts("            [-csc] string");
			Puts(string.Empty);
			Puts(" Where:");
			Puts(string.Empty);
			Puts("\t-s   single exe, pack dll-s as resources      |");
			Puts("\t-so  optimize single exe (valid with -s only) |");
			Puts("\t-a   assemblyAttributes file, custom EXE assembly attributes");
			Puts("\t     in the Visual Studio C# format");
			Puts("\t-o   output folder, will be created if not exists, default exename.netz");
			Puts("\t-aw  warn about unhandled EXE assembly attributes, default ignore");
			Puts("\t-p   privatePath, optional private application domain path");
			Puts("\t-i   win32icon, optional icon file");
			Puts("\t-n   add version info to starter, default no info");
			Puts("\t-c   console exe CUI, default is autodetect |");
			Puts("\t-w   windows exe GUI, default is autodetect |");
			Puts("\t-pl  supports /platform cross-compilation in 64 bit systems |");
			Puts("\t-x86 shortcut for -pl x86                                   |");
			Puts("\t-mta set MTAThread attribute to starter main (default STAThread)");
			Puts(string.Empty);
			Puts(" Compress options:");
			Puts(string.Empty);
			Puts("\t-r   compressLib, compress provider dll, default defcomp.dll");
			Puts("\t-z   pack redistributable compress DLL as resource, ignored if no");
			Puts("\t     redistributable compress DLL");
			Puts("\t-l   redistributableCompressLib, name of the redistributable");
			Puts("\t     compress DLL, overwrites the one given by the provider");
			Puts(string.Empty);
			Puts(" Strong name (sign) options: (default no sign)");
			Puts(string.Empty);
			Puts("\t-kf  keyFile, to use for signing the packed assembly |");
			Puts("\t-kn  keyName, to use for signing the packed assembly |");
			Puts("\t-kd  set delay sign true, default false              |");
			Puts("\t-ka  get keyFile, keyName, delay sign, and algorithmId from EXE");
			Puts("\t     attributes. The -kf, -kn and -kd are ignored when ka is specified");
			Puts(string.Empty);
			Puts(" Service options:");
			Puts("\t-sr  creates a basic NT service from the input exe and dll files |");
			Puts("\t-srp file, parameters file for -sr option |");
			Puts(string.Empty);
			Puts(" Debug options:");
			Puts(string.Empty);
			Puts("\t-b   batch mode, generates a batch file and source code");
			Puts("\t-v   print stack trace if error");
			Puts("\t-!   print internal version");
			Puts(string.Empty);
			Puts(" The -xr option:");
			Puts("\t-xr  the -xr should be used alone to create external DLL resources");
			Puts(string.Empty);
			Puts(" Other options:");
			Puts("\t-csc string passes the string to csc compiler");
			Puts(string.Empty);
			Puts(" Input files:");
			Puts("\t     At most one EXE file must be specified at [exe file].");
			Puts("\t     The DLL files can be specified alone or with wildcards.     |");
			Puts("\t-d   If use before a DLL file, this option tells .NETZ that      |");
			Puts("\t     the next DLL will be loaded dynamically by the application. |");
		}

		private static void Puts(string s)
		{
			Logger.Log(s);
		}

		private static void Puts2(string s)
		{
			Logger.Log2(s);
		}

	}//EOC
}
