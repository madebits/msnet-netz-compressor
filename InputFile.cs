using System;
using System.Collections;

namespace netz
{
	public class InputFile : IComparable
	{
		private string file = null;
		private bool mangleName = true;
		private string resourceName = null;

		public InputFile(){}
		
		public InputFile(string file)
		{
			this.file = file;
		}

		public InputFile(string file, bool mangleName)
		{
			this.file = file;
			this.mangleName = mangleName;
		}

		public string File
		{
			get { return file; }
			set { file = value; }
		}

		public bool MangleName
		{
			get { return mangleName; }
			set { mangleName = value; }
		}
		
		public string ResourceName
		{
			get { return resourceName; }
			set { resourceName = value; }
		}
		
		public int CompareTo(object obj)
		{
			if(obj == null) return -1;
			if(!(obj is InputFile)) return -1;
			InputFile other = (InputFile)obj;
			return this.File.CompareTo(other.File);
		}

	}//EOC
}
