using System;
using System.IO;
using System.Collections;

namespace netz
{

	public class ReadOnly
	{
		protected bool readOnly = false;

		protected void CheckReadOnly()
		{
			if(readOnly) throw new Exception("E1002 Readonly");
		}

		public void MakeReadOnly()
		{
			readOnly = true;
		}
	}
	
	public class GenData : ReadOnly
	{
		public GenData()
		{
			compressProviderDLL = GetFullPath("defcomp.dll");
		}

		public static string GetFullPath(string file)
		{
			string temp = System.Windows.Forms.Application.ExecutablePath;
			temp = Path.GetDirectoryName(temp);
			return Path.Combine(temp, file);
		}

		private string exeFileName = "starter.exe";
		private string privatePath = null;
		private string assemblyInfo = null;
		private string iconFile = null;
		private bool userIconFile = false;
		private bool console = false;
		private bool auto = true;
		private bool batchMode = false;
		
		private bool setNetzVersion = false;
		private static bool singleExe = false;
		private string userAssemblyAttributes = string.Empty;
		private bool mtaAttribute = false;

		// ZIP
		private bool packZipDll = false;
		private string zipDllName = null; //"zip.dll";
		private string compressProviderDLL = null; //"defcomp.dll";
		private CompressProvider compressProvider = null;

		//LICENSING
		private string licenseResourceFile = null;

		private string keyFile = null;
		private string keyName = null;
		private bool keyDelay = false;
		private bool keyGetFromAttributes = false;

		private bool reportEXEAttributes = false;
		private string xPlatform = null;
		private bool optimize = false;

		private string otherCompOptions = null;

		private bool isService = false;
		public Hashtable serviceParams = new Hashtable();

		internal bool MtaAttribute
		{
			get{ return mtaAttribute; }
			set{ mtaAttribute = value; }
		}

		internal bool ReportEXEAttributes
		{
			get{ return reportEXEAttributes; }
			set{ CheckReadOnly(); reportEXEAttributes = value; }
		}

		internal bool IsService
		{
			get{ return isService; }
			set{ CheckReadOnly(); isService = value; }
		}

		internal bool KeyDelay
		{
			get{ return keyDelay; }
			set{ CheckReadOnly(); keyDelay = value; }
		}

		internal bool Optimize
		{
			get{ return optimize; }
			set{ CheckReadOnly(); optimize = value; }
		}

		internal string XPlatform
		{
			get{ return xPlatform; }
			set{ CheckReadOnly(); xPlatform = value; }
		}

		internal bool KeyGetFromAttributes
		{
			get{ return keyGetFromAttributes; }
			set{ CheckReadOnly(); keyGetFromAttributes = value; }
		}

		internal string KeyName
		{
			get{ return keyName; }
			set{ CheckReadOnly(); keyName = value; }
		}

		internal string KeyFile
		{
			get{ return keyFile; }
			set{ CheckReadOnly(); keyFile = value; }
		}

		internal string LicenseResourceFile
		{
			get{ return licenseResourceFile; }
			set{ CheckReadOnly(); licenseResourceFile = value; }
		}

		internal CompressProvider CompressProvider
		{
			get{ return compressProvider; }
			set{ CheckReadOnly(); compressProvider = value; }
		}

		internal string CompressProviderDLL
		{
			get{ return compressProviderDLL; }
			set{ CheckReadOnly(); compressProviderDLL = value; }
		}

		internal string ExeFileName
		{
			get{ return exeFileName; }
			set{ CheckReadOnly(); exeFileName = value; }
		}

		internal string PrivatePath
		{
			get{ return privatePath; }
			set{ CheckReadOnly(); privatePath = value; }
		}

		internal string AssemblyInfo
		{
			get{ return assemblyInfo; }
			set{ CheckReadOnly(); assemblyInfo = value; }
		}

		internal string IconFile
		{
			get{ return iconFile; }
			set{ CheckReadOnly(); iconFile = value; }
		}

		internal bool Console
		{
			get{ return console; }
			set{ CheckReadOnly(); console = value; }
		}

		internal bool Auto
		{
			get{ return auto; }
			set{ CheckReadOnly(); auto = value; }
		}

		internal bool BatchMode
		{
			get{ return batchMode; }
			set{ CheckReadOnly(); batchMode = value; }
		}

		internal bool PackZipDll
		{
			get{ return packZipDll; }
			set{ CheckReadOnly(); packZipDll = value; }
		}

		internal bool UserIconFile
		{
			get{ return userIconFile; }
			set{ CheckReadOnly(); userIconFile = value; }
		}

		internal bool SetNetzVersion
		{
			get{ return setNetzVersion; }
			set{ CheckReadOnly(); setNetzVersion = value; }
		}

		internal bool SingleExe
		{
			get{ return singleExe; }
			set{ CheckReadOnly(); singleExe = value; }
		}

		internal string ZipDllName
		{
			get{ return zipDllName; }
			set{ CheckReadOnly(); zipDllName = value; }
		}

		internal string UserAssemblyAttributes
		{
			get{ return userAssemblyAttributes; }
			set{ CheckReadOnly(); userAssemblyAttributes = value; }
		}

		internal string OtherCompOptions
		{
			get{ return otherCompOptions; }
			set{ CheckReadOnly(); otherCompOptions = value; }
		}
	
	}//EOC
}
