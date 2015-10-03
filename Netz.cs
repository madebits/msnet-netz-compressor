using System;
using System.IO;
using System.Collections;
using System.Resources;
using System.Text;

namespace netz
{
	public class Netz
	{
		#region vars
		
		private static ResMan rm = null;
		private static GenData genData = null;
		internal static readonly string LOGPREFIX = "     ";
				
		#endregion vars

		[STAThread]
		static int Main(string[] args)
		{
			Help.PrintLogo();
			try
			{
				genData = new GenData();
				ArrayList files = InputParser.Parse(ref genData, args);
				if((files != null) && (files.Count >= 0))
				{
					if(InputParser.externalResourceMode)
					{
						ProcessCreateXRes(files);
					}
					else
					{
						Proceed(files);
					}
				}
			}
			catch(Exception ex)
			{
				PrintErr(null, ex);
				return 1;
			}
			return 0;
		}

		#region xr

		private static void ProcessCreateXRes(ArrayList files)
		{
			//TODO implement a custom resource format
			if(files.Count <= 0) throw new Exception("No DLL files!");
			string fileName = Path.GetFullPath(InputParser.externalResourceName + "-netz.resources");
			OutDirMan.OutDir = Path.GetDirectoryName(fileName);
			Logger.Log("XR Creating external resource file: " + fileName);
			Logger.Log(string.Empty);
			rm = new ResMan();
			rm.ResourceFilePath = fileName;
			for(int i = 0; i < files.Count; i++)
			{
				InputFile inf = (InputFile)files[i];
				Logger.Log("[" + (i + 1)+ " of " + files.Count + "] XR adding: " + inf.File);
				ProcessXRDllFile(inf);
			}
			rm.Save();
			Logger.Log("Done!");
		}

		private static void ProcessXRDllFile(InputFile file)
		{
			TestLoad(file.File);
			string zipFile = Zipper.MakeZipFileName(file.File, false);
			Zipper.ZipFile(file.File, zipFile, genData);
			rm.AddResource(GetDLLName(file), Utils.ReadFile(zipFile));
			File.Delete(zipFile);
		}

		#endregion xr

		private static void PrintErr(string err, Exception ex)
		{
			bool showTrace = InputParser.printStackStrace;
			if(ex.Message.ToLower().StartsWith("object reference not set"))
			{
				showTrace = true;
			}
			Logger.LogErr(((err != null) ? err + " -> " : string.Empty)
				+ ex.Message + 
				(showTrace ? "\r\n" + ex.StackTrace : string.Empty));
			if(showTrace)
			{
				Logger.Log(string.Empty);
				Logger.Log("--------------------------");
				InputParser.PrintInternalVersion(genData.CompressProviderDLL);
			}
		}

		internal static void PrintWarning(string err, Exception ex)
		{
			Logger.LogWarn(LOGPREFIX + "! Warning: " + ((err != null) ? err : "9999")
				+ (InputParser.printStackStrace ?
				 (ex != null ? " -> " + ex.Message + "\r\n" + ex.StackTrace : string.Empty)
				 : string.Empty));
		}


		private static void Proceed(ArrayList files)
		{
			if(files.Count <= 0) throw new Exception("E1009 No files");
			long start = DateTime.Now.Ticks;
			OutDirMan.Make();
			InitResMan();
			Logger.Log("Processing      : " + files.Count + " file(s)\r\n");
			ProcessFiles(files);
			rm.Save();
			genData.MakeReadOnly();
			if(InputParser.exeFileSet)
			{
				MakeStarterApp();
			}
			start = DateTime.Now.Ticks - start;
			Logger.Log("\r\nDone [" + ElapsedTime(start) + "]");
		}

		private static string ElapsedTime(long delta)
		{
			TimeSpan ts = new TimeSpan(delta);
			StringBuilder sb = new StringBuilder();
			sb.Append(ts.Hours.ToString("00")).Append(":");
			sb.Append(ts.Minutes.ToString("00")).Append(":");
			sb.Append(ts.Seconds.ToString("00")).Append(".");
			sb.Append(ts.Milliseconds);
			return sb.ToString();
		}

		private static void InitResMan()
		{
			rm = new ResMan();
			if(genData.PackZipDll && (genData.ZipDllName != null))
			{
				rm.AddResource(Path.GetFileNameWithoutExtension(genData.ZipDllName).ToLower() + ".dll", Utils.ReadFile(genData.ZipDllName));
				FileInfo fi = new FileInfo(genData.ZipDllName);
				Logger.Log("Added           : " + genData.ZipDllName + " " + Utils.FormatFileSize(fi.Length));
				fi = null;
			}
		}

		#region processfiles

		private static void ProcessFiles(ArrayList files)
		{
			ColorConsole c = null;
			try
			{
				c = new ColorConsole();
			}
			catch{}

			for(int i = 0; i < files.Count; ++i)
			{
				InputFile file = (InputFile)files[i];
				try
				{
					ProcessFile(i + 1, file, c);
				}
				catch(Exception ex)
				{
					PrintErr(file.File, ex);
				}
			}
		}

		private static long lastLength = 0L;
		private static void ProcessFile(int i, InputFile file, ColorConsole c)
		{
			FileInfo fi = new FileInfo(file.File);
			lastLength = fi.Length;
			if(c != null) c.SetColor(Help.COL_NETZ);
			Logger.Log2((i < 10 ? " " : string.Empty) + i.ToString() + "|  ");
			if(c != null) c.Reset();
			Logger.Log(file.File);
			Logger.Log2(Netz.LOGPREFIX + Utils.FormatFileSize(fi.Length));
			fi = null;
			if(file.File.ToLower().EndsWith(".exe"))
			{
				ProcessExeFile(file.File);
			}
			else if(file.File.ToLower().EndsWith(".dll"))
			{
				ProcessDllFile(file);
			}
			else throw new Exception("E1010 Unsupported file type suffix");
		}

		internal static void LogZipSize(long zipLength)
		{
			if(lastLength <= 0L) return;
			int gain = (int)(100L - ((zipLength * 100L) / lastLength));
			Logger.Log(" -> "
				+ Utils.FormatFileSize(zipLength)
				+ " - "
				+ gain.ToString("00")
				+ "%");
		}
		
		private static void TestLoad(string file)
		{
			try
			{
				System.Reflection.Assembly a = System.Reflection.Assembly.Load(Utils.ReadFile(file));
				string temp = a.FullName;
				a = null;
			}
			catch(Exception ex)
			{
				PrintWarning("1006 Assembly load test failed! " + ex.GetType().ToString(), ex);
			}	
		}

		private static void ProcessExeFile(string file)
		{
			genData.ExeFileName = Path.GetFileName(file);
			try
			{
				genData.AssemblyInfo = starter.AssemblyInfoGen.MakeAssemblyInfo(file, genData);
			}
			catch(Exception ex)
			{
				Console.WriteLine(string.Empty);
				PrintWarning("1001 Cannot process assembly metadata!", ex);
			}
			try
			{
				genData.LicenseResourceFile = starter.AssemblyInfoGen.MakeAssemblyLicense(file);
			}
			catch(Exception ex)
			{
				PrintWarning("1005 Cannot process assembly license information!", ex);
			}
			TestLoad(file);

			// zip
	
			string zipFile = null;
			try
			{
				zipFile = Zipper.MakeZipFileName(file, true);
				Zipper.ZipFile(file, zipFile, genData);
				rm.AddResource("A6C24BF5-3690-4982-887E-11E1B159B249", Utils.ReadFile(zipFile));
			} 
			finally
			{
				if(zipFile != null)
					try
					{
						File.Delete(zipFile);
					}
					catch{}
			}

			ProcessExeIcon(file);
		}

		private static void ProcessExeIcon(string file)
		{
			if(genData.UserIconFile)
			{
				if(genData.BatchMode)
				{ 
					try
					{
						string ico = OutDirMan.MakeOutFileName(Path.GetFileName(genData.IconFile));
						if(!File.Exists(ico))
						{
							File.Copy(genData.IconFile, ico);
						}
					}
					catch{}
				}
			}
			else // extract icon
			{
				try
				{
					genData.IconFile = OutDirMan.MakeOutFileName("App.ico");
					starter.IconExtractor.SaveExeIcon(file, genData.IconFile);
				}
				catch(Exception ex)
				{
					genData.IconFile = null;
					PrintWarning("1002 Icon " + ex.Message, ex);
				}
			}
		}

		private static void RemoveIcon()
		{
			if(genData.UserIconFile) return;
			if(genData.BatchMode) return;
			if(File.Exists(genData.IconFile))
				File.Delete(genData.IconFile);
		}

		private static void ProcessDllFile(InputFile file)
		{
			TestLoad(file.File);
			string zipFile = Zipper.MakeZipFileName(file.File, true);
			Zipper.ZipFile(file.File, zipFile, genData);
			if(genData.SingleExe)
			{ 
				rm.AddResource(GetDLLName(file), Utils.ReadFile(zipFile));
				File.Delete(zipFile);
			}
		}

		private static string GetDLLName(InputFile file)
		{
			try
			{
				if(file.MangleName)
				{
					string temp = starter.AssemblyInfoGen.GetFullDLLName(file.File);
					return MangleDllName(temp);
				}
				else
				{
					if(file.ResourceName == null)
					{
						return Path.GetFileNameWithoutExtension(file.File).ToLower();
					}
					else if(file.ResourceName.Equals("@"))
					{
						return starter.AssemblyInfoGen.GetFullDLLName(file.File);
					}
					else if(file.ResourceName.Equals("#"))
					{
						string temp = starter.AssemblyInfoGen.GetFullDLLName(file.File);
						int k = temp.IndexOf(',');
						if(k <= 0) return temp;
						return temp.Substring(0, k);
					}
					else
					{
						return file.ResourceName;
					}
				}
			}
			catch(Exception ex)
			{
				PrintWarning("1001 Cannot process assembly metadata!", ex);
				return Path.GetFileNameWithoutExtension(file.File);
			}
		}

		private static string MangleDllName(string dll)
		{
			string temp = dll;
			temp = temp.Replace(" ", "!1");
			temp = temp.Replace(",", "!2");
			temp = temp.Replace(".Resources", "!3");
			temp = temp.Replace(".resources", "!3");
			temp = temp.Replace("Culture", "!4");
			return temp;
		}

		#endregion processfiles

		#region makestarter

		private static void MakeStarterApp()
		{
			try
			{
				starter.StarterGen sg = new starter.StarterGen(genData);
				sg.Make(rm.ResourceFilePath);
				if(!genData.BatchMode)
				{ 
					File.Delete(rm.ResourceFilePath);
				}
				HandleZipDll();
				RemoveIcon();
			}
			finally
			{
				HandleLicenseFile();
			}
		}

		private static void HandleZipDll()
		{
			if(genData.ZipDllName == null) return;
			string outZip = OutDirMan.MakeOutFileName(genData.ZipDllName);
			bool exists = File.Exists(outZip);
			if((!genData.PackZipDll) || genData.BatchMode)
			{
				if(!exists)
					File.Copy(genData.ZipDllName, outZip, false);
			}
			else
			{
				if(exists) 
					File.Delete(outZip);
			}
		}

		private static void HandleLicenseFile()
		{
			if(genData.LicenseResourceFile == null) return;
			string file = genData.LicenseResourceFile;
			if(File.Exists(file)) File.Delete(file);
		}

		#endregion makestarter

	}//EOC
}
