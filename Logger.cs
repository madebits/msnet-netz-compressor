using System;

namespace netz
{
	public class Logger
	{
		private Logger()
		{}

		public static void Log2(string s)
		{
			Console.Write(s);
		}

		public static void Log(string s)
		{
			Console.WriteLine(s);
		}

		public static void LogErr(string s)
		{
			ColorConsole c = null;
			try
			{
				c = new ColorConsole();
				c.SetColor(ColorConsole.FOREGROUND_RED
					| ColorConsole.FOREGROUND_INTENSITY);
			}
			catch{}
			Log(Environment.NewLine + "# Error: " + s);
			if(c != null) c.Reset();
		}

		public static void LogWarn(string s)
		{
			ColorConsole c = null;
			try
			{
				c = new ColorConsole();
				c.SetColor(ColorConsole.FOREGROUND_RED | ColorConsole.FOREGROUND_BLUE
					| ColorConsole.FOREGROUND_INTENSITY);
			}
			catch{}
			Log(s);
			if(c != null) c.Reset();
		}
	}//EOC
}
