using System;
using System.Reflection;

namespace netz
{
	public class CompressProvider
	{
		private Assembly provider = null;
		private netz.compress.ICompress icompress = null;
		
		public CompressProvider(string dllName)
		{
			Init(dllName);
		}

		private void Init(string dllName)
		{
			try
			{
				provider = Assembly.LoadFrom(dllName);
				Type[] types = provider.GetTypes();
				for(int i = 0; i < types.Length; i++)
				{
					Type[] interfaces = types[i].GetInterfaces();
					for(int j = 0; j < interfaces.Length; j++)
					{
						if(interfaces[j].FullName.Equals("netz.compress.ICompress"))
						{
							ConstructorInfo ci = types[i].GetConstructor(Type.EmptyTypes);
							icompress = (netz.compress.ICompress)ci.Invoke(null);
							break;
						}
					}
					if(icompress != null) break;
				}
			}
			catch(Exception ex)
			{
				throw new Exception("E1001 Cannot initialize compression provider " + ex.Message);
			}
		}

		public netz.compress.ICompress Provider
		{
			get{ return icompress; }
		}
	}//EOC
}
