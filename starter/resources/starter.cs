// (c) 2004-present by Vasian Cepa - GPL - GNU GENERAL PUBLIC LICENSE
// This is a template. Do not remove //# strings
using System;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Collections.Specialized;

//#ZIPHEAD

// for log only in winexe mode
using System.Windows.Forms;

//#AI
//#UAI


//
namespace netz
{

//#NTSERVICE


public class NetzStarter//#VER
{
	#region constants

	private static readonly string Name = "Name";
	private static readonly string Culture = "Culture";
	private static readonly string NetzSuffix = "z.dll";

	#endregion constants

	//http://weblogs.asp.net/justin_rogers/archive/2004/06/07/149964.aspx
	private static HybridDictionary cache = null;
	private static ResourceManager rm = null;
	private static System.Collections.ArrayList xrRm = null;

	[STAThread]
	public static int Main(string[] args)
	{
		try
		{
			InitXR();
			AppDomain currentDomain = AppDomain.CurrentDomain;
			//#PATH

			currentDomain.AssemblyResolve += new ResolveEventHandler(NetzResolveEventHandler);

			// return StartApp(args);
			//#MAINENTRYPOINT
		}
		catch(Exception ex)
		{
			string NR = " .NET Runtime: ";
			Log("#Error: " + ex.GetType().ToString() + Environment.NewLine
				+ ex.Message + Environment.NewLine
				+ ex.StackTrace + Environment.NewLine
				+ ex.InnerException + Environment.NewLine
				+ "Using" + NR + Environment.Version.ToString() + Environment.NewLine
				+ "Created with" + NR + //#CTNETVER);
			return -1;
		}
	}

	private static void InitXR()
	{
		try
		{
			string FILEP = "file:\\";
			string EXT = "-netz.resources";
			string path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
			if(path.StartsWith(FILEP)) path = path.Substring(FILEP.Length, path.Length - FILEP.Length);
			string[] files = Directory.GetFiles(path, "*" + EXT);
			if((files != null) && (files.Length > 0))
			{
				xrRm = new System.Collections.ArrayList();
				for(int i = 0; i < files.Length; i++)
				{
					string name = Path.GetFileName(files[i]);
					name = name.Substring(0, name.Length - EXT.Length);
					ResourceManager temp = ResourceManager.CreateFileBasedResourceManager(name + "-netz", path, null);
					if(temp != null)
					{
						xrRm.Add(temp);
					}
				}
			}
		}catch
		{
			// fail silently here if something bad with regard to permissions happens
		}
	}

	public static int StartApp(string[] args)
	{
		byte[] data = GetResource("A6C24BF5-3690-4982-887E-11E1B159B249");
		if(data == null) throw new Exception("application data cannot be found");
		Assembly assembly = GetAssembly(data);
		int returnCode = InvokeApp(assembly, args);
        	data = null;
		return returnCode;
	}

	private static Assembly GetAssembly(byte[] data)
	{
		MemoryStream ms = null;
		Assembly assembly = null;
		try{
			ms = UnZip(data);
			ms.Seek(0, SeekOrigin.Begin);
			assembly = Assembly.Load(ms.ToArray());
		}
		finally
		{
			if(ms != null) ms.Close();
			ms = null;
		}
		return assembly;
	}

	//#ZIPSTART
	// supports -z option
	private static Assembly LoadZipDll()
	{
		Assembly assembly = null;
		MemoryStream ms = null;
		try{
			byte[] zip = GetResource("//#ZIPDLL.dll");
			if(zip == null) return null;
			ms = new MemoryStream(zip);
			assembly = Assembly.Load(ms.ToArray());
		}
		catch
		{
			assembly = null;
		}
		finally
		{
			if(ms != null) ms.Close();
			ms = null;
		}
		return assembly;
	}
	//#ZIPEND

	private static int InvokeApp(Assembly assembly, string[] args)
	{
		MethodInfo mi = assembly.EntryPoint;
		ParameterInfo[] pars = mi.GetParameters();
		object[] iargs = null;
		if((pars != null) && (pars.Length > 0))
		{
			iargs = new object[]{ args };
		}
		object returnValue = mi.Invoke(null, iargs);
		if(returnValue == null) return 0;
		if(returnValue is int) return (int) returnValue;
		return 0;
	}

	private static Assembly NetzResolveEventHandler(object sender, ResolveEventArgs args)
	{
		 if(inResourceResolveFlag) return null;
		 return GetAssemblyByName(args.Name);
	}

	private static bool inResourceResolveFlag = false;
	private static byte[] GetResource(string id)
	{
		byte[] data = null;
		if(rm == null)
		{
			rm = new ResourceManager("app", Assembly.GetExecutingAssembly());
		}
		try
		{
			inResourceResolveFlag = true;
			string temp = MangleDllName(id);
			if((data == null) && (xrRm != null))
			{
				for(int i = 0; i < xrRm.Count; i++)
				{
					try
					{
						ResourceManager xr = (ResourceManager)xrRm[i];
						if(xr != null) data = (byte[])xr.GetObject(temp);
					}
					catch{ /*nothing to do */ }
					if(data != null) break;
				}
			}
			if(data == null)
			{
				data = (byte[])rm.GetObject(temp);
			}
		}
		finally
		{
			inResourceResolveFlag = false;
		}
		return data;
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

	private static MemoryStream UnZip(byte[] data)
	{
//#ZIPBODY
	}

#region probe

	//ms-help://MS.VSCC/MS.MSDNVS/cpguide/html/cpconhowruntimelocatesassemblies.htm
	//ms-help://MS.VSCC/MS.MSDNVS/cpguide/html/cpconstep4locatingassemblythroughcodebasesorprobing.htm
	//The custom 'z' .NETZ suffix is not used in the dir paths
	private static byte[] ResolveDLL(StringDictionary assName)
	{
		string temp = null;
		byte[] data = null;
		string culture = assName[Culture];
		string dllName = assName[Name];

		bool neutral = (culture == null) || culture.ToLower().Equals("neutral");
		AppDomain cd = AppDomain.CurrentDomain;
		StringCollection probePaths = new StringCollection();

		temp = cd.SetupInformation.ApplicationBase;
		if(!neutral)
		{
			temp = Path.Combine(temp, culture);
		}
		probePaths.Add(temp);
		probePaths.Add(Path.Combine(temp, dllName));
		data = ProbeDirs(probePaths, dllName);
		if(data != null) return data;

		probePaths = new StringCollection();
		temp = cd.SetupInformation.PrivateBinPath;
		if((temp == null) || (temp.Trim().Length <= 0)) return null;
		string[] paths = temp.Split(Path.PathSeparator);
		for(int i = 0; i < paths.Length; i++)
		{
			temp = paths[i].Trim(' ', '\t', Path.PathSeparator);
			if(!Path.IsPathRooted(temp))
				temp = Path.Combine(cd.SetupInformation.ApplicationBase, temp);
			if(!neutral)
			{
				temp = Path.Combine(temp, culture);
			}
			probePaths.Add(temp);
			probePaths.Add(Path.Combine(temp, dllName));
		}
		return ProbeDirs(probePaths, dllName);
	}

	private static byte[] ProbeDirs(StringCollection probePaths, string assName)
	{
		for(int i = 0; i < probePaths.Count; i++)
		{
			string temp = Path.Combine(probePaths[i], assName + NetzSuffix);
			if(File.Exists(temp)) return ReadFile(temp);
		}
		return null;
	}

	private static byte[] ReadFile(string file)
	{
		FileStream fs = null;
		byte[] buffer = null;
		try
		{
			fs = File.OpenRead(file);
			buffer = new byte[fs.Length];
			fs.Read(buffer, 0, buffer.Length);
			fs.Flush();
		}
		finally
		{
			if(fs != null) fs.Close();
			fs = null;
		}
		return buffer;
	}

#endregion probe

	private static void Log(string s)
	{
		MessageBox.Show(null, s, "Error");
	}

	private static Assembly GetAssemblyByName(string name)
	{
		if(name == null) return null;
		if(cache == null) cache = new HybridDictionary();
		name = name.Trim();
		string tname = name.ToLower();
		if(cache[tname] != null)
		{
			return (Assembly)cache[tname];
		}
		else
		{
			 StringDictionary assName = ParseAssName(name);
			 string dllName = assName[Name];
			 if(dllName == null) return null;
			 byte[] data = null;
			 //#ZIPSTART
			 if(dllName.ToLower().Equals("//#ZIPDLL"))
			 {
				Assembly temp = LoadZipDll();
				cache[tname] = temp;
				return temp;
			 }
			 //#ZIPEND
			 data = GetResource(name);
			 if(data == null)
			 {
				 data = GetResource(name.ToLower());
			 }
			 if(data == null)
			 {
				 data = GetResource(dllName);
			 }
			 if(data == null)
			 {
				 data = GetResource(dllName.ToLower());
			 }
			 if(data == null)
			 {
				 data = GetResource(Path.GetFileNameWithoutExtension(dllName).ToLower());
			 }
			 //#OPSSTART
			 if(data == null)
			 {
				try
				{
					data = ResolveDLL(assName);
				}
				catch
				{
					data = null;
				}
			 }
			 //#OPSEND
			 if(data == null) return null;
			 Assembly temp1 = GetAssembly(data);
			 data = null;
			 cache[tname] = temp1;
			 return temp1;
		}
	}

	private static StringDictionary ParseAssName(string fullAssName)
	{
		StringDictionary assName = new StringDictionary();
		string[] parts = fullAssName.Split(',');
		for(int i = 0; i < parts.Length; i++)
		{
			string[] temp = parts[i].Trim(' ', ',').Split('=');
			if(temp.Length < 2) assName.Add(Name, temp[0]);
			else assName.Add(temp[0].Trim(' ', '='), temp[1].Trim(' ', '='));
		}
		return assName;
	}

}//EOC

}












