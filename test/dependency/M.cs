using System;
using System.Windows.Forms;

public class M
{
	public static void Main(string[] args)
	{
		Y y = new Y();
		System.Console.WriteLine("TEST " + y.LibOk());
		Z z = new Z();
		System.Console.WriteLine("TEST " + z.LibOk());
		X x = new X();
		System.Console.WriteLine("TEST " + x.LibOk() + " X");

		Y y1 = new Y();
		System.Console.WriteLine("TEST " + y1.LibOk());
		Z z1 = new Z();
		System.Console.WriteLine("TEST " + z1.LibOk());
		X x1 = new X();
		System.Console.WriteLine("TEST " + x1.LibOk() + " X");

	}
	
}