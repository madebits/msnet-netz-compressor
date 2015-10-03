using System;
using System.Resources;

namespace netz
{
	public class ResMan
	{
		private ResourceWriter rw = null;
		private string resourceFilePath = "app.resources";

		public string ResourceFilePath
		{
			get
			{
				return resourceFilePath;
			}
			set
			{
				if(value != null) resourceFilePath = value;
			}
		}

		public ResMan()
		{}

		public void AddResource(string id, byte[] data)
		{
			if(InputParser.printStackStrace)
			{
				Console.WriteLine(Netz.LOGPREFIX + "RID: " + id);
			}
			if(id == null || data == null)
				throw new Exception("E1011 Null resource");
			InitRes();
			rw.AddResource(id, data);
		}

		private void InitRes()
		{
			if(rw == null)
			{
				resourceFilePath = OutDirMan.MakeOutFileName(resourceFilePath);
				rw = new ResourceWriter(resourceFilePath);
			}
		}

		public void Save()
		{
			if(rw != null)
			{
				rw.Close();
				rw = null;
			}
		}

	}//EOC
}
