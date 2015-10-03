using System;
using System.Runtime.InteropServices;

namespace netz
{

	public class ColorConsole
	{

#region colors

		public static readonly int FOREGROUND_BLUE =       0x0001;
		public static readonly int FOREGROUND_GREEN =      0x0002;
		public static readonly int FOREGROUND_RED  =       0x0004;
		public static readonly int FOREGROUND_INTENSITY =  0x0008;
		public static readonly int BACKGROUND_BLUE  =      0x0010;
		public static readonly int BACKGROUND_GREEN =      0x0020;
		public static readonly int BACKGROUND_RED   =      0x0040;
		public static readonly int BACKGROUND_INTENSITY =  0x0080;

#endregion colors

#region win32

		private static readonly int STD_OUTPUT_HANDLE = -11;

		[DllImport("Kernel32")]
		private static extern IntPtr GetStdHandle(int nStdHandle);

		[DllImport("Kernel32")]
		private static extern bool SetConsoleTextAttribute(
			IntPtr hConsoleOutput,
			int wAttributes
			);

		[DllImport("Kernel32")]
		private static extern bool GetConsoleScreenBufferInfo(
			IntPtr hConsoleOutput,
			ref CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo
			);

		[StructLayoutAttribute(LayoutKind.Sequential)]
			private struct CONSOLE_SCREEN_BUFFER_INFO 
		{ 
			public int dwSize;
			public int dwCursorPosition;
			public int wAttributes; 
			public long srWindow;
			public long dwMaximumWindowSize;
		}

#endregion win32

		public ColorConsole()
		{
			hStdout = GetStdHandle(STD_OUTPUT_HANDLE);
			if(IsHandleValid())
			{
				CONSOLE_SCREEN_BUFFER_INFO csbiInfo = new CONSOLE_SCREEN_BUFFER_INFO();
				GetConsoleScreenBufferInfo(hStdout, ref csbiInfo);
				attributes = csbiInfo.wAttributes;
			}
		}

		private bool IsHandleValid()
		{
			return !(hStdout.Equals(new IntPtr(-1)));
		}

		public void SetColor(int color)
		{
			if(IsHandleValid())
			{
				SetConsoleTextAttribute(hStdout, color);
			}
		}

		public void Reset()
		{
			SetColor(attributes);
		}

#region state

		private IntPtr hStdout;
		private int attributes = 0;

#endregion state

	}//EOC

}