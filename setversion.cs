using System;
using System.IO;

public class SetVersion
{
	public static void Main(string[] args)
	{


		string file = "InputParser.cs";
		string mark = "#DOTNETVERSION#";
		string version = Environment.Version.ToString();

		if((args != null) && (args.Length > 0))
		{
			if(args[0].ToLower().Equals("-unset"))
			{
				string temp = version;
				version = mark;
				mark = temp;
			}
		}

		Console.WriteLine("!!! SetVersion !!! " + version);

		using(StreamReader sr = new StreamReader(file))
		{
			string data = sr.ReadToEnd();
			sr.Close();
			data = data.Replace(mark, version);
			using(StreamWriter sw = new StreamWriter(file))
			{
				sw.Write(data);
			}
		}
	}
}