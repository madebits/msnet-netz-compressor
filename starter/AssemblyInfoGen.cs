using System;
using System.Text;
using System.Reflection;

namespace netz.starter
{

	public class AssemblyInfoGen
	{
		private static Assembly lastEXE = null;
		
		public AssemblyInfoGen()
		{}

		public static string MakeAssemblyInfo(string file, netz.GenData genData)
		{
			return MakeAssemlbyInfo(Assembly.LoadFrom(file), genData);
		}

		private static void GenStringAttrib(ref StringBuilder sb, string name, string val)
		{
			val = val.Replace("\\", "\\\\");
			val = val.Replace("\t", "\\t");
			val = val.Replace("\r", "\\r");
			val = val.Replace("\n", "\\n");
			val = val.Replace("\"", "\\\"");
			GenAttrib(ref sb, name, "\"" + val + "\"");
		}

		private static void GenAttrib(ref StringBuilder sb, string name, string val)
		{
			string NL = Environment.NewLine;
			sb.Append("[assembly: " + name + "(");
			sb.Append(val);
			sb.Append(")]").Append(NL);
		}

		public static string MakeAssemlbyInfo(Assembly a, netz.GenData genData)
		{
			if(a == null) return null;
			lastEXE = a;
			bool versionSet = false;
			string NL = Environment.NewLine;
			StringBuilder sb = new StringBuilder();
			sb.Append("using System.Reflection;").Append(NL);
			sb.Append("using System.Runtime.CompilerServices;").Append(NL);

			foreach(object att in a.GetCustomAttributes(true))
			{
				if(att is System.Reflection.AssemblyTitleAttribute)
				{
					GenStringAttrib(ref sb, "AssemblyTitle",
						((System.Reflection.AssemblyTitleAttribute)att).Title);
				}
				else if(att is System.Reflection.AssemblyDescriptionAttribute)
				{
					GenStringAttrib(ref sb, "AssemblyDescription",
						((System.Reflection.AssemblyDescriptionAttribute)att).Description);
				}
				else if(att is System.Reflection.AssemblyConfigurationAttribute)
				{
					GenStringAttrib(ref sb, "AssemblyConfiguration",
						((System.Reflection.AssemblyConfigurationAttribute)att).Configuration);
				}
				else if(att is System.Reflection.AssemblyCompanyAttribute)
				{
					GenStringAttrib(ref sb, "AssemblyCompany",
						((System.Reflection.AssemblyCompanyAttribute)att).Company);
				}
				else if(att is System.Reflection.AssemblyProductAttribute)
				{
					GenStringAttrib(ref sb, "AssemblyProduct",
						((System.Reflection.AssemblyProductAttribute)att).Product);
				}
				else if(att is System.Reflection.AssemblyCopyrightAttribute)
				{
					GenStringAttrib(ref sb, "AssemblyCopyright",
						((System.Reflection.AssemblyCopyrightAttribute)att).Copyright);
				}
				else if(att is System.Reflection.AssemblyTrademarkAttribute)
				{
					GenStringAttrib(ref sb, "AssemblyTrademark",
						((System.Reflection.AssemblyTrademarkAttribute)att).Trademark);
				}
				else if(att is System.Reflection.AssemblyCultureAttribute)
				{
					GenStringAttrib(ref sb, "AssemblyCulture",
						((System.Reflection.AssemblyCultureAttribute)att).Culture);
				}
				else if(att is System.Reflection.AssemblyVersionAttribute)
				{
					versionSet = true;
					GenStringAttrib(ref sb, "AssemblyVersion",
						((System.Reflection.AssemblyVersionAttribute)att).Version);
				}
				else if(att is System.Reflection.AssemblyKeyFileAttribute)
				{
					if(genData.KeyGetFromAttributes)
					{
						GenStringAttrib(ref sb, "AssemblyKeyFile",
							((System.Reflection.AssemblyKeyFileAttribute)att).KeyFile);
					}
				}
				else if(att is System.Reflection.AssemblyKeyNameAttribute)
				{
					if(genData.KeyGetFromAttributes)
					{
						GenStringAttrib(ref sb, "AssemblyKeyName",
							((System.Reflection.AssemblyKeyNameAttribute)att).KeyName);
					}
				}
				else if(att is System.Reflection.AssemblyAlgorithmIdAttribute)
				{
					if(genData.KeyGetFromAttributes)
					{
						GenAttrib(ref sb, "AssemblyAlgorithmId",
							((AssemblyAlgorithmIdAttribute)att).AlgorithmId.ToString("D"));
					}
				}
				else if(att is System.Reflection.AssemblyDelaySignAttribute)
				{
					if(genData.KeyGetFromAttributes)
					{
						GenAttrib(ref sb, "AssemblyDelaySign",
							(((System.Reflection.AssemblyDelaySignAttribute)att).DelaySign ? "true" : "false"));
					}
				}

					#region removed
				/*
				else if(att is System.Reflection.AssemblyDefaultAliasAttribute)
				{
					GenStringAttrib(ref sb, "AssemblyDefaultAlias",
						((System.Reflection.AssemblyDefaultAliasAttribute)att).DefaultAlias);
				}
				else if(att is System.Reflection.AssemblyFileVersionAttribute)
				{
					GenStringAttrib(ref sb, "AssemblyFileVersion",
						((System.Reflection.AssemblyFileVersionAttribute)att).Version);
				}
				else if(att is System.Reflection.AssemblyInformationalVersionAttribute)
				{
					GenStringAttrib(ref sb, "AssemblyInformationalVersion",
						((System.Reflection.AssemblyInformationalVersionAttribute)att).InformationalVersion);
				}
				else if(att is System.Reflection.AssemblyFlagsAttribute)
				{ // int
					GenAttrib(ref sb, "AssemblyFlags",
						((AssemblyFlagsAttribute)att).Flags.ToString("D"));
				}
				else if(att is System.Diagnostics.DebuggableAttribute)
				{
					System.Diagnostics.DebuggableAttribute da = (System.Diagnostics.DebuggableAttribute)att;
					string dav = string.Empty + (da.IsJITTrackingEnabled ? "true" : "false") + "," + (da.IsJITOptimizerDisabled ? "true" : "false");
					GenAttrib(ref sb, "System.Diagnostics.DebuggableAttribute", dav);
				}
				else if(att is System.CLSCompliantAttribute)
				{
					GenAttrib(ref sb, "System.CLSCompliantAttribute", ((System.CLSCompliantAttribute)att).IsCompliant ? "true" : "false");
				}
				else if(att is System.Runtime.InteropServices.GuidAttribute)
				{
					GenStringAttrib(ref sb, "System.Runtime.InteropServices.GuidAttribute", ((System.Runtime.InteropServices.GuidAttribute)att).Value);
				}
				*/

					#endregion removed
				else
				{
					string t = att.ToString();
					try
					{
						string prefix = "System.Reflection.";
						if(t.StartsWith(prefix)) t = t.Substring(prefix.Length, t.Length - prefix.Length); 
					}
					catch{}
					if(!MatchUserDefined(t, genData.UserAssemblyAttributes))
					{
						if(genData.ReportEXEAttributes)
						{
							Netz.PrintWarning("1003 Unhandled main assembly attribute : " + t + " ?", null);
						}
					}
					else
					{
						Logger.Log("! Matched user defined attribute name       : " + t);
					}
				}
			}
			if(!versionSet)
			{
				string[] data = a.FullName.Split(',');
				for(int i = 0; i < data.Length; i++)
				{
					if(data[i] == null) continue;
					string ver = data[i].Trim(',' , ' ').ToLower();
					if(ver.StartsWith("version"))
					{
						int j = data[i].IndexOf('=');
						if(j > 0)
						{
							ver = ver.Substring(j, ver.Length - j);
							
							GenStringAttrib(ref sb, "AssemblyVersion",
								ver);
							versionSet = true;
						}
					}
				}
			}
			if(!versionSet)
			{
				sb.Append("[assembly: AssemblyVersion(\"");
				sb.Append("1.0.*");
				sb.Append("\")]").Append(NL);
			}
			if(!genData.KeyGetFromAttributes)
			{
				bool keySet = false;
				if(genData.KeyFile != null)
				{
					keySet = true;
					GenStringAttrib(ref sb, "AssemblyKeyFile",
						genData.KeyFile);
				}
				if(genData.KeyName != null)
				{
					keySet = true;
					GenStringAttrib(ref sb, "AssemblyKeyName",
						genData.KeyName);
				}
				if(keySet)
				{
					GenAttrib(ref sb, "AssemblyDelaySign",
						(genData.KeyDelay ? "true" : "false"));
				}
			}
			sb.Append(NL).Append("// Add any other attributes here").Append(NL);
			return sb.ToString();
		}

		private static bool MatchUserDefined(string att, string userDefined)
		{
			string t = att;
			int i = t.LastIndexOf('.');
			if((i >= 0) && (i < (t.Length - 1)))
			{
				t = t.Substring(i + 1, t.Length - i - 1);
			}
			return (userDefined.IndexOf(t + "(") >= 0);
		}

		public static string GetFullDLLName(string file)
		{
			Assembly dll = Assembly.LoadFrom(file);
			return dll.FullName.Trim();
		}

		public static string MakeAssemblyLicense(string file)
		{
			Assembly a = AssemblyInfoGen.lastEXE; // reuse
			if(a == null) a = Assembly.LoadFrom(file);
			string[] resources = a.GetManifestResourceNames();
			string licenseFile = System.IO.Path.GetFileName(file)+ ".licenses";
			licenseFile = licenseFile.ToUpper();
			if((resources == null) || (resources.Length <= 0)) return null;
			for(int i = 0; i < resources.Length; ++i)
			{
				if(resources[i].ToUpper().Equals(licenseFile))
				{
					System.IO.Stream data = null;
					System.IO.FileStream lstr = null;
					try
					{
						data = a.GetManifestResourceStream(resources[i]);
						if(data == null) return null;
						licenseFile = OutDirMan.MakeOutFileName(licenseFile.ToLower());
						lstr = new System.IO.FileStream(licenseFile, System.IO.FileMode.Create, 
							System.IO.FileAccess.Write, System.IO.FileShare.None, 4096);
						byte[] buff = new byte[4096];
						while(true)
						{
							int c = data.Read(buff, 0, buff.Length);
							if(c <= 0) break;
							lstr.Write(buff, 0, c);
						}
					}
					finally
					{
						if(data != null) data.Close();
						if(lstr != null) lstr.Close();
					}
					return licenseFile;
				}
			}
			return null;
		}
	}//EOC
}
