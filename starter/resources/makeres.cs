using System;
using System.IO;
using System.Resources;
using System.Security.Cryptography;


public class MakeRes
{
	public static int Main(string[] args)
	{
		try{
		Console.WriteLine("! MakeRes is using .NET version: " + Environment.Version.ToString());
		ResourceWriter rw = new ResourceWriter(args[0] + ".resources");
		for(int i = 1; i < args.Length; i = i + 2)
		{
			using(FileStream fs = File.OpenRead(args[i + 1]))
			{
				byte[] buffer = new byte[fs.Length];
				fs.Read(buffer, 0, buffer.Length);
				Console.WriteLine("ID = " + args[i]);
				rw.AddResource(args[i], buffer);
				fs.Close();
				SHA1 sha = new SHA1CryptoServiceProvider();
				byte[] result = sha.ComputeHash(buffer);
				WriteHash(args[0] + "."+ args[i], result);
			}
		}
		rw.Close();
	}catch(Exception ex)
	{
			Console.WriteLine("# MareRes Error: " + ex.Message + "\r\n" +  ex.StackTrace);
			return 1;
		}
		return 0;
	}

	private static void WriteHash(string title, byte[] b)
	{
		Console.Write(title + " ");
		for(int i = 0; i < b.Length; i++)
		{
			Console.Write(b[i].ToString("X"));
			if(i < b.Length -1) Console.Write(" ");
		}
		Console.WriteLine("");
	}

}