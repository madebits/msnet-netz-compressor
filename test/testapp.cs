using System;
using System.Windows.Forms;

public class TestApp
{
	public static void Main(string[] args)
	{
		TestLib tl = new TestLib();
		MessageBox.Show(null, tl.LibOk(), "Test App");
	}
	
	public void Test()
	{
		
	}
}