using System;
using System.IO;
using System.IO.Compression;

using System.Resources;
using System.Text;

public class Net20Comp : netz.compress.ICompress
{

		private string head = null;
		private string body = null;

		// a default, no parameter constructor is required
		public Net20Comp()
		{
			System.Reflection.Assembly a = this.GetType().Assembly;
			ResourceManager rm = new ResourceManager("net20comp", a);
			byte[] data = (byte[])rm.GetObject("head");
			head = Encoding.ASCII.GetString(data);
			data = (byte[])rm.GetObject("body");
			body = Encoding.ASCII.GetString(data);
			rm.ReleaseAllResources();
		}

		// netz.compress.ICompress implementation

		public long Compress(string file, string zipFile)
		{
			long length = -1;
			FileStream ifs = null;
			FileStream ofs = null;
			try
			{
				ifs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
				ofs = File.Open(zipFile, FileMode.Create, FileAccess.Write, FileShare.None);
				DeflateStream dos = new DeflateStream(ofs, CompressionMode.Compress, true);
				byte[] buff = new byte[ifs.Length];
				while(true)
				{
					int r = ifs.Read(buff, 0, buff.Length);
					if(r <= 0) break;
					dos.Write(buff, 0, r);
				}
				dos.Flush();
				dos.Close();
				length = ofs.Length;
			}
			finally
			{
				if(ifs != null) ifs.Close();
				if(ofs != null) ofs.Close();
			}
			return length;
		}

		// return null if none required
		public string GetRedistributableDLLPath()
		{
			return null;
		}

		public string GetHeadTemplate()
		{
			return head;
		}

		public string GetBodyTemplate()
		{
			return body;
		}

}//EOC