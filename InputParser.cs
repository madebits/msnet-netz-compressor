using System;
using System.IO;
using System.Collections;
using System.Security.Cryptography;
using System.Resources;
using System.Text;

namespace netz
{
	public class InputParser
	{
		internal static bool printStackStrace = false;
		internal static bool exeFileSet = false; // a single exe must be given

		// -xr
		internal static bool externalResourceMode = false;
		internal static string externalResourceName = null;

		private InputParser()
		{}

		public static ArrayList Parse(ref GenData genData, string[] args)
		{
			ArrayList files = new ArrayList(); // of InputFile
			if(args.Length <= 0)
			{
				Help.ShowHelp();
				return null;
			}
            string outDirPath = null;
			for(int i = 0; i < args.Length; i++)
			{
				if((args[i][0] == '-') || (args[i][0] == '/'))
				{
					string arg = args[i].Substring(1, args[i].Length - 1).ToLower();
					switch(arg)
					{
						case "?":
							Help.ShowHelp();
							return null;
						case "v":
							printStackStrace = true;
							break;
						case "s":
							genData.SingleExe = true;
							break;
						case "c":
							genData.Console = true;
							genData.Auto = false;
							break;
						case "w":
							genData.Console = false;
							genData.Auto = false;
							break;
						case "i":
							genData.IconFile = args[++i];
							genData.UserIconFile = true;
							break;
                        case "o":
                            outDirPath = args[++i];
                            break;
						case "b":
							genData.BatchMode = true;
							break;
						case "p":
							genData.PrivatePath = args[++i];
							break;
						case "z":
							genData.PackZipDll = true;
							break;
						case "l":
							genData.ZipDllName = args[++i];
							break;
						case "r":
							genData.CompressProviderDLL = args[++i];
							break;
						case "n":
							genData.SetNetzVersion = true;
							break;
						case "!":
							PrintInternalVersion(genData.CompressProviderDLL);
							return null;
						case "so":
							genData.Optimize = true;
							break;
						case "a":
							using(StreamReader sr = new StreamReader(args[++i]))
							{
								if(sr == null) break;
								string t = sr.ReadToEnd();
								if(t != null)
                                    genData.UserAssemblyAttributes = t.Trim();
							}
							break;
						case "aw":
							genData.ReportEXEAttributes = true;
							break;
						case "mta":
							genData.MtaAttribute = true;
							break;
						case "kf":
							genData.KeyGetFromAttributes = false;
							genData.KeyFile = args[++i]; //Path.GetFullPath();
							if(!File.Exists(genData.KeyFile)) throw new Exception("Key file not found: " + genData.KeyFile);
							break;
						case "kn":
							genData.KeyGetFromAttributes = false;
							genData.KeyName = args[++i];
							break;
						case "kd":
							genData.KeyDelay = true;
							break;
						case "ka":
							genData.KeyGetFromAttributes = true;
							break;
						case "pl":
							genData.XPlatform = args[++i];
							break;
						case "x86":
							genData.XPlatform = "x86";
							break;
						case "d":
							break;
						case "xr":
							externalResourceMode = true;
							if(i != 0) throw new Exception("-xr must be first");
							if(args.Length < 2) throw new Exception("-xr name required");
							externalResourceName = args[++i];
							if(externalResourceName.IndexOf(' ') >= 0) throw new Exception("-xr name must not contain spaces");
							break;
						case "sr":
							genData.IsService = true;
							break;
						case "csc":
							genData.OtherCompOptions = args[++i];
							break;
						case "srp":
							genData.serviceParams = GetMap(args[++i]);
							break;
						default:
						  if(arg.StartsWith("d:")) break;
							throw new Exception("E1003 Unknown argument: " + args[i]);
					}
				}
				else
				{
					bool mangleName = true;
					string resourceName = null;
					if((i > 0) && (args[i - 1].ToLower().StartsWith("-d") || args[i - 1].ToLower().StartsWith("/d")))
					{
						mangleName = false;
						int k = args[i - 1].IndexOf(':');
						if(k >= 0)
						{
							if((k != 2) || (args[i -1].Length < 4)) throw new Exception("E1003 Unknown argument: " + args[i]);
							resourceName = args[i - 1].ToLower().Substring(k + 1, args[i - 1].Length - k - 1);
						}
					}
					AppendFile(ref files, args[i], ref genData, mangleName, resourceName);
				}
			}
			Logger.Log(".NET Runtime    : " + Environment.Version.ToString());
			ValidateInput(genData);
            if (outDirPath != null)
            {
                OutDirMan.OutDir = outDirPath;
            }

			genData.CompressProvider = new CompressProvider(genData.CompressProviderDLL);
			if(genData.ZipDllName == null)
			{
				genData.ZipDllName = genData.CompressProvider.Provider.GetRedistributableDLLPath();
				if((genData.ZipDllName != null) && Path.GetFileName(genData.ZipDllName).ToLower().Equals(genData.ZipDllName))
				{
					genData.ZipDllName = GenData.GetFullPath(genData.ZipDllName);
				}
			}
			return files;
		}

		public static void PrintInternalVersion(string zipProvider)
		{
			Logger.Log("| User                   : "
				+ Environment.UserName + "@" + Environment.MachineName);
			OperatingSystem os = Environment.OSVersion;
			Logger.Log("| OS Platform            : "
				+ os.Platform.ToString());
			Logger.Log("| OS Version             : "
				+ os.Version.ToString());
			Logger.Log("| .NET Runtime Version   : "
				+ Environment.Version.ToString());
			Logger.Log("| Internal .NETZ Version : "
				+ System.Reflection.Assembly.GetExecutingAssembly().FullName);
			Logger.Log("| Compiled with .NET     : #DOTNETVERSION#");
			try
			{
				Logger.Log(string.Empty);
				Logger.Log(GetInnerTemplatesFingerPrint(zipProvider));
			}
			catch{}
		}

		#region fp

		internal static string GetInnerTemplatesFingerPrint(string zipProvider)
		{
			StringBuilder sb = new StringBuilder();
			SHA1 sha = new SHA1CryptoServiceProvider();
			byte[] hash = null;
			try
			{
				System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
				ResourceManager rm = new ResourceManager("netz.starter.starter", a);
				byte[] data = (byte[])rm.GetObject("data");
				hash = sha.ComputeHash(data);
				sb.Append("Starter ID: ").Append(Byte2HexString(hash)).Append(Environment.NewLine);
			}
			catch(Exception ex01)
			{
				sb.Append("Starter ID: ").Append(ex01.Message).Append(Environment.NewLine);
			}
			try
			{
				CompressProvider cp = new CompressProvider(zipProvider);
				if(cp == null) throw new Exception("No Compression Provider");
				try
				{
					string temp = cp.Provider.GetHeadTemplate();
					if(temp != null)
					{
						hash = sha.ComputeHash(System.Text.Encoding.ASCII.GetBytes(temp));
						sb.Append(zipProvider).Append(".head ID: ").Append(Byte2HexString(hash)).Append(Environment.NewLine);
					}
				}
				catch(Exception ex02)
				{
					sb.Append(zipProvider).Append(".head ID: ").Append(ex02.Message).Append(Environment.NewLine);
				}
				try
				{
					string temp = cp.Provider.GetBodyTemplate();
					if(temp != null)
					{
						hash = sha.ComputeHash(System.Text.Encoding.ASCII.GetBytes(temp));
						sb.Append(zipProvider).Append(".body ID: ").Append(Byte2HexString(hash)).Append(Environment.NewLine);
					}
				}
				catch(Exception ex03)
				{
					sb.Append(zipProvider).Append(".body ID: ").Append(ex03.Message).Append(Environment.NewLine);
				}
			}
			catch(Exception ex04)
			{
				sb.Append(zipProvider).Append(" ID: ").Append(ex04.Message).Append(Environment.NewLine);
			}
			return sb.ToString();
		}

//Logger.Log2("| " + name + " ID: ");

		private static string Byte2HexString(byte[] hash)
		{
			if(hash == null) return string.Empty;
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("X"));
				if(i < (hash.Length - 1)) sb.Append(" ");
			}
			return sb.ToString();
		}

		#endregion fp

		private static void ValidateInput(GenData genData)
		{
			if(!exeFileSet)
			{
				string err = "an EXE file must be specified for the -";
				bool throwit = false;
				if(genData.SingleExe)
				{
					throwit = true;
					err += "s";
				}
				else if(genData.PackZipDll)
				{
					throwit = true;
					err += "z";
				}
				if(throwit) throw new Exception("E1004 " + err + " option");
			}
			if(genData.UserIconFile && !File.Exists(genData.IconFile))
				throw new Exception("icon file not found: " + genData.IconFile);
		}

		#region files

		private static void AppendFile(ref ArrayList files, string file, ref GenData genData, bool mangleName, string resourceName)
		{
			string[] f = null;
			if(mangleName)
			{
				f = ExpandFiles(file);
			}
			else
			{
				f = new string[]{ file };	
			}
			if((f == null) || (f.Length <= 0))
			{
				throw new Exception("E1006 No such file(s): " + file);
			}
			for(int i = 0; i < f.Length; i++)
			{
				if(f[i].ToLower().EndsWith(".exe"))
				{
					SetExeOutPath(f[i], ref genData);
				}
				AddFile(ref files, f[i], mangleName, resourceName);
			}
		}

		private static void AddFile(ref ArrayList files, string file, bool mangleName, string resourceName)
		{
			if(!File.Exists(file))
				throw new Exception("E1006 File not found: " + file);
			string pfile = Path.GetFullPath(file);
			InputFile ipfile = new InputFile(pfile, mangleName);
			ipfile.ResourceName = resourceName;
			if(!files.Contains(ipfile)) files.Add(ipfile);
		}

		private static string[] ExpandFiles(string filepat)
		{
			string dir = Path.GetDirectoryName(filepat);
			string pat = Path.GetFileName(filepat);
			if(pat.IndexOfAny(new char[]{'*', '?'}) < 0)
			{
				if(!File.Exists(filepat))
				{
					if(!pat.ToUpper().EndsWith(".DLL")
						|| !pat.ToUpper().EndsWith(".EXE"))
						pat += ".exe";
				}
			}
			if(dir.Trim().Equals(string.Empty))
			{
				dir = ".";
			}
			return Directory.GetFiles(dir, pat);
		}

		private static void SetExeOutPath(string file, ref GenData genData)
		{
			if(exeFileSet)
				throw new Exception("E1007 A single EXE must be specified");
			exeFileSet = true;
			string pfile = Path.GetFullPath(file);
			if(genData.Auto)
			{
				try
				{
					int ss = subsys.Win32PESubSystem.GetSubSystem(pfile);
					if(ss == subsys.Win32PESubSystem.IMAGE_SUBSYSTEM_WINDOWS_CUI)
					{
						genData.Console = true;
					}
					else if(ss == subsys.Win32PESubSystem.IMAGE_SUBSYSTEM_WINDOWS_GUI)
					{
						genData.Console = false;
					}
					else throw new Exception("E1008 Unsupported PE subsystem (KB Q90493): " + ss);
					Logger.Log("PE subsystem    : " + (genData.Console ? "CUI" : "GUI") );
				}
				catch(Exception ex)
				{
					netz.Netz.PrintWarning("1004 Cannot determine EXE's subsystem (default is GUI windows EXE) The packed application will fail!: ", ex);
				}
			}
			OutDirMan.OutDir = pfile + ".netz";
		}

		#endregion files

		private static Hashtable GetMap(string file)
		{
			System.Collections.Hashtable h = new System.Collections.Hashtable();
			if(File.Exists(file))
			{
				using(StreamReader sr = new StreamReader(file))
				{
					for(string line = sr.ReadLine(); line != null; line = sr.ReadLine())
					{
						line = line.Trim();
						if(line.Length <= 0) continue;
						if(line.StartsWith("#")) continue;
						int i = line.IndexOf('=');
						if(i <= 0) continue;
						h[line.Substring(0, i).ToLower()] = line.Substring(i + 1, line.Length - i - 1);
					}
				}
			}
			return h;
		}

	}//EOC
}
