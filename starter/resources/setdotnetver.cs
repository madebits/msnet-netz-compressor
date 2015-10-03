using System;
using System.IO;

public class SetDNETVer
{
	static string data = "<?xml version =\"1.0\"?>\r\n"
	+ "<configuration>\r\n"
  +   "\t<startup>\r\n"
  +       "\t\t<requiredRuntime version=\"v1.0.0\"  />\r\n"
  +   "\t</startup>\r\n"
	+ "</configuration>\r\n";
	
	public static int Main(string[] args)
	{
		try
		{
			string file = "makeres.exe.config";
			string ver = Environment.Version.Major + "."  + Environment.Version.Minor + "." + Environment.Version.Build;
			
			if(args != null)
			{
				for(int i = 0; i < args.Length; ++i)
				{
					string arg = args[i].ToLower();
					switch(arg)
					{
						case "-v": ver = args[++i];
						break;
						case "-f": file = args[++i];
						break;	
					}	
				}
			}
			
			using(StreamWriter sw = new StreamWriter(file))
			{
				Console.WriteLine("!!! Required .NET run-time version set to: " + ver + " for file: " + file);
				sw.Write(data.Replace("1.0.0", ver));	
			}
		}catch(Exception ex)
		{
			Console.Write("#SetDotNetVer Error: " + ex.Message + "\r\n" +  ex.StackTrace);
			return 1;	
		}
		return 0;	
	}	
}