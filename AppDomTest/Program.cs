using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;


namespace AppDomTest
{
	[Serializable]
	public class MyDomain
	{
		public MyDomain(int Pra)
		{
			Console.WriteLine("类MyDomain被构建了,参数:{0}!", Pra);
		}

		public void Doam(int In)
		{
			Console.WriteLine("In:{0}", In);
			List<string> BL = new List<string>();
			while (true)
			{
				BL.Add("0");
			}
		}
	}//End Class

	//沙盒
	[SecuritySafeCritical]
	class Program
	{
		static void Main(string[] args)
		{
			MyDomain MD = new MyDomain(0);
			MD.Doam(123);

		}
	}
}
