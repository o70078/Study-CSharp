using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Collections;
using System.Dynamic;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Runtime.Remoting;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Security.Principal;
using System.Security.Cryptography;
using System.Management;
using System.IO.IsolatedStorage;
using System.IO;
using System.Web.Script.Serialization;
using System.Messaging;

/// <summary>
/// 
/// </summary>
public static class ObjectExtend
{
	/// <summary>
	/// 弹出内容
	/// </summary>
	/// <param name="Obj"></param>
	public static void Msgbox(this object Obj)
	{
		MessageBox.Show(Obj.ToString());
	}

	/// <summary>
	/// 输出到控制台
	/// </summary>
	/// <param name="Obj"></param>
	public static void ToCMD(this object Obj)
	{
		Console.Write(Obj);
	}
}//End Class

class Program
{
	BitArray BA = new BitArray(5, true);//Bit数组
	BitVector32 BV = new BitVector32(122);

	#region 一个操作数据时会发出Event的集合
	static void TestObservableCollection()
	{
		ObservableCollection<int> OC = new ObservableCollection<int>();

		OC.CollectionChanged += (X, Y) =>
			{
				//MessageBox.Show(X.ToString());
				if (Y.Action == NotifyCollectionChangedAction.Add) MessageBox.Show(Y.NewItems[0].ToString());
				if (Y.Action == NotifyCollectionChangedAction.Remove) MessageBox.Show(Y.OldItems[0].ToString());
			};

		OC.Add(12);
		OC.Remove(12);
	}
	#endregion

	#region 两种数据唯一集合
	static void TestSet()
	{
		SortedSet<string> SSS = new SortedSet<string>();//有排序
		HashSet<string> HS = new HashSet<string>();//无排序
		HS.Add("A");
		HS.Add("B");
		HS.Add("B");

		SSS.Add("B");
		SSS.Add("C");

		var A = HS.Except(SSS).ToArray();//从HS里面去掉SSS有的值

#pragma warning disable CS1717 // 对同一变量进行了赋值
		A = A;
#pragma warning restore CS1717 // 对同一变量进行了赋值
	}
	#endregion

	#region 用Linq把多个数组的值.不同排列方式组合起来
	static void TestLinQ1()
	{
		int[][] NN = new int[][] { new int[] { 1, 2, 4, 8 }, new int[] { 3, 1, 122 }, new int[] { 55, 88, 99 } };

		var VVV = (from AA in NN[0]
							 from BB in NN[1]
							 from CC in NN[2]
							 select new int[] { AA, BB, CC }).ToList().ToArray();


#pragma warning disable CS1717 // 对同一变量进行了赋值
		VVV = VVV;
#pragma warning restore CS1717 // 对同一变量进行了赋值
	}
	#endregion

	#region 测试Lookup
	static void TestLookup()
	{
		//System.Linq.Lookup<int,int> L =new Lookup<int, int>();

	}
	#endregion

	#region 初始化数组
	static void InitialArray()
	{
		int[] IA = Enumerable.Range(0, 23).ToArray();
#pragma warning disable CS1717 // 对同一变量进行了赋值
		IA = IA;
#pragma warning restore CS1717 // 对同一变量进行了赋值
	}
	#endregion

	#region Int数组连接
	static void IntZIP()
	{
		int[] A = { 1, 2, 3, 4, 5 }, B = { 6, 7, 8, 9, 0 };
		int[] C = A.Zip(B, (X, Y) => X + Y).ToArray();
#pragma warning disable CS1717 // 对同一变量进行了赋值
		C = C;
#pragma warning restore CS1717 // 对同一变量进行了赋值
	}
	#endregion

	#region LinqToXML
	public static void LinqXML()
	{
		XElement XE = XElement.Load("TestXML.xml");

		string[] F = (from A in XE.Elements("Item") select A.Value).ToArray();
#pragma warning disable CS1717 // 对同一变量进行了赋值
		F = F;
#pragma warning restore CS1717 // 对同一变量进行了赋值
	}
	#endregion

	#region 动态类型
	public class MyDY : DynamicObject
	{
		Dictionary<string, object> D = new Dictionary<string, object>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="binder"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			if (!D.ContainsKey(binder.Name))
			{
				result = null;
				return false;
			}

			result = D[binder.Name];
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="binder"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			if (!D.ContainsKey(binder.Name))
			{
				D.Add(binder.Name, value);
			}
			else
			{
				D[binder.Name] = value;
			}
			return true;
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			dynamic DY = D[binder.Name];
			return base.TryInvokeMember(binder, args, out result);
		}
	}

	static void Testdynamic()
	{
		//dynamic MD = new MyDY();
		dynamic MD = new ExpandoObject();
		MD.TTTT = 12;
		MessageBox.Show(MD.TTTT.ToString());
	}

	#endregion

	#region 应用程序域
	private static void BenQAppDomain()
	{
		AppDomain AD = AppDomain.CurrentDomain;
		Console.WriteLine(AD.FriendlyName);
		AppDomain AD2 = AppDomain.CreateDomain("NewAppDomain");
		//AD2.ExecuteAssembly("AppDomTest.exe");
		ObjectHandle OH = AD2.CreateInstance("AppDomTest", "AppDomTest.MyDomain", true, System.Reflection.BindingFlags.CreateInstance, null, new object[] { 999 }, null, null);
		AppDomTest.MyDomain OtherAppDomTestClass = OH.Unwrap() as AppDomTest.MyDomain;
		try
		{
			OtherAppDomTestClass.Doam(12344);
		}
		catch (Exception EEE)
		{
			Console.WriteLine(EEE.Message);
		}

		AppDomain.Unload(AD2);
	}
	#endregion

	#region 不变量
	static int BBLA = 0;

	[ContractInvariantMethod]
	public static void BBLTest()
	{
		BBLA = 50;
		Contract.Invariant(BBLA < 5);
	}
	#endregion

	#region 多线程循环
	static void TestForeach()
	{
		int[] Ia = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };
		Parallel.ForEach<int>(Ia, X =>
		{
			Console.WriteLine(X);
		});

		Console.WriteLine("X");

		Parallel.For(0, 10, X =>
		 {
			 Console.WriteLine(X);
		 });

	}
	#endregion

	#region 线程池
	static Value locktest = new Value();

	public class Value
	{
		public int A = 0;
	}

	private static void AThread(object Obj)
	{
		lock (locktest)
		{
			if (locktest.A == 0) locktest.A++;
			if (locktest.A != 1) Console.WriteLine("ThreadID:{0} A:{1}", Thread.CurrentThread.ManagedThreadId, locktest.A);
			locktest.A = 0;
		}
	}
	private static void MThread()
	{
		for (int i = 0; i < 5000; i++)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(AThread));
		}
	}

	#endregion

	#region 获取Windows当前登录用户名
	private static void WindowsUser()
	{
		//WindowsIndentity
		//AppDomain.CurrentDomain.SetPrincipalPolicy();
		System.Security.Principal.WindowsIdentity.GetCurrent().Name.Msgbox();
	}
	#endregion

	#region 获取硬盘ID
	/// <summary>
	/// 获取硬盘ID代码  
	/// </summary>
	/// <returns></returns>
	public static string GetHardDiskID()
	{
		try
		{
			string hdInfo = "";//硬盘序列号  
			ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"c:\"");
			hdInfo = disk.Properties["VolumeSerialNumber"].Value.ToString();
			disk = null;
			return hdInfo.Trim();
		}
		catch (Exception e)
		{
			return "uHnIk";
		}
	}
	#endregion

	#region 使用Net程序文件空间
	private static void TestIsolate()
	{
		IsolatedStorageFile ISF = IsolatedStorageFile.GetMachineStoreForDomain();
		IsolatedStorageFileStream ISFS = new IsolatedStorageFileStream("HI.TXT", FileMode.OpenOrCreate, FileAccess.ReadWrite);
		string[] F = ISF.GetDirectoryNames();
		F = F;
		ISFS.Write(new byte[] { 0x1, 0x4, 0x2 }, 0, 3);
		byte[] AA = new byte[ISFS.Length];
		//ISFS.Flush();//如果使用异步写,则可以用这个等待写完
		ISFS.Seek(0, SeekOrigin.Begin);
		ISFS.Read(AA, 0, (int)ISFS.Length);
		AA = AA;
	}
	#endregion

	#region 测试json序列化
	private struct UserLogin
	{
		/// <summary>
		/// 指令
		/// </summary>
		public string Command;
		/// <summary>
		/// 数据
		/// </summary>
		public string Data;
	}

	private static void JsonSerializer()
	{
		JavaScriptSerializer JSS = new JavaScriptSerializer();
		string[] Texts = new string[] { "1", "2", "3" };
		string sJson = JSS.Serialize(Texts);
		Console.WriteLine(sJson);
		Texts = JSS.Deserialize<string[]>(sJson);

		UserLogin UL = new UserLogin();
		UL.Command = "o70078";
		UL.Data = sJson;
		sJson = JSS.Serialize(UL);

		sJson = sJson;
	}
	#endregion

	#region 消息队列
	static bool MQLocal=false;
	static MessageQueue MQ = new MessageQueue(MQLocal? @".\Private$\abc":@"FormatName:Direct=TCP:121.42.8.67\Private$\AsmXQueue",false,true,QueueAccessMode.SendAndReceive);
	private static void TestMessageQueue()
	{
		
		{
			System.Messaging.Message message = new System.Messaging.Message();
			message.Priority = MessagePriority.High;//优先级
			message.Body = "Hello Message";
			message.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
			MQ.Send(message,MessageQueueTransactionType.Single);
			message.Dispose();
		}
		// */

		{
			System.Messaging.Message message = MQ.Receive(MessageQueueTransactionType.Single);
			message.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
			Console.WriteLine(message.Body.ToString());
		}
		// */
	}
	#endregion
	/*
	#region 
	private static void X()
	{
	}
	#endregion
	// */

	static void Main(string[] args)
	{

		TestMessageQueue();

		Console.WriteLine("End Test");
		Console.ReadKey(true);
	}

}//End Class
