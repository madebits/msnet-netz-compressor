using System;
using System.Threading;

class Service
{
	public static int Main(string[] args)
	{
		Service s = new Service();
		s.DoWork(args);
		return 0;
	}

	private void DoWork(string[] args)
	{
		while(true)
		{
			System.Diagnostics.Debug.WriteLine("ServiceTest: " + DateTime.Now.Ticks.ToString());
			Thread.Sleep(5000);
		}
	}
}