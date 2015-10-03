using System;
using System.IO;

namespace netz
{

	public class Utils
	{
		private Utils()
		{}

		public static string NetzVersion
		{
			get 
			{ 
				//return "0.2.9";
				try
				{
					string name = System.Reflection.Assembly.GetExecutingAssembly().FullName;
					if(name == null) throw new Exception();
					string[] parts = name.Split(',');
					for(int i = 0; i < parts.Length; ++i)
					{
						string part = parts[i].Trim().ToUpper();
						if(part.StartsWith("VERSION"))
						{
							string[] ver = part.Split('=');
							int j = ver[1].LastIndexOf('.');
							if(j <= 0) return ver[1];
							return ver[1].Substring(0, j);
						}
					}
				}
				catch
				{}
				return "?.?.?";
			}
		}

		public static byte[] ReadFile(string file)
		{
			FileStream fs = null;
			byte[] buffer = null;
			try
			{
				fs = File.OpenRead(file);
				buffer = new byte[fs.Length];
				fs.Read(buffer, 0, buffer.Length);
			}
			finally
			{
				if(fs != null) fs.Close();
			}
			return buffer;
		}		

		public static void WriteTextFile(string file, string data)
		{
			using(StreamWriter sw = new StreamWriter(OutDirMan.MakeOutFileName(file)))
			{
				sw.WriteLine(data);
			}
		}

		public static string FormatFileSize(long length)
		{
			return "[" + length + " byte(s) ~ " + (length / 1024) + "KB]";
		}

	}//EOC
}
