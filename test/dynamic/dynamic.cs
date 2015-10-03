using System;
using System.Reflection;

public class Dynamic
{
	public static void Main()
	{
		try{
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(OnAssemblyResolve);


			Assembly ass = Assembly.Load("testlib");
			Type[] types = ass.GetTypes();
			for(int i = 0; i < types.Length; i++)
			{
				Console.WriteLine(types[i].Name);
			}
		}catch(Exception ex)
		{
			Console.WriteLine(ex.Message + " " + ex.StackTrace);
		}
	}

	private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
	{
		Console.WriteLine("OnAssemblyResolve: " + args.Name);
		return null;
	}
}