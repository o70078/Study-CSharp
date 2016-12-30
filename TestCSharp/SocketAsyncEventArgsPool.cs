using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace TestCSharp
{
	public class SocketAsyncEventArgsMetadata : SocketAsyncEventArgs
	{
		/**记录索引**/
		private int index;
		private SocketAsyncEventArgs args;

		public static SocketAsyncEventArgsMetadata valueOf(int index)
		{
			SocketAsyncEventArgsMetadata result = new SocketAsyncEventArgsMetadata();
			result.index = index;
			return result;
		}

		internal int GetIndex()
		{
			return this.index;
		}
	}

	public class SocketAsyncEventArgsPool
	{
		//已使用记录
		private List<Int32> usedRecord;
		//未使用记录
		private List<Int32> unUsedRecord;
		//池子
		private List<SocketAsyncEventArgsMetadata> pool;
		//池子最大容量
		private int capacity;
		//是否动态扩展容量
		// private bool dynamic = false;

		/**池子初始化*/
		private void init()
		{
			this.pool = new List<SocketAsyncEventArgsMetadata>(this.capacity);
			this.usedRecord = new List<Int32>(this.capacity);
			this.unUsedRecord = new List<Int32>(this.capacity);
			for (int i = 0; i < this.capacity; i++)
			{
				this.unUsedRecord.Add(i);
				this.pool.Add(SocketAsyncEventArgsMetadata.valueOf(i));
			}
		}

		///////////////////公开方法////////////////////////
		/**获取可使用数量**/
		public int GetUsedCount()
		{
			return this.capacity - this.usedRecord.Count;
		}
		/**获取可使用 SocketAsyncEventArgs */
		public SocketAsyncEventArgsMetadata Pop()
		{
			int index = 0;
			lock (this)
			{
				if (GetUsedCount() <= 0)
				{
					extCapacity();
				}
				index = this.unUsedRecord[0];
				this.unUsedRecord.RemoveAt(0);
				this.usedRecord.Add(index);
				return this.pool[index];
			}
		}
		/**回收 SocketAsyncEventArgs */
		public void Push(SocketAsyncEventArgsMetadata args)
		{
			int index = 0;
			lock (this)
			{
				index = args.GetIndex();
				this.unUsedRecord.Add(index);
				this.usedRecord.Remove(index);
			}
		}

		/** 扩展容量   */
		private void extCapacity()
		{
			int minNewCapacity = 200;
			int newCapacity = Math.Min(this.capacity, minNewCapacity);

			//每次以minNewCapacity倍数扩展
			if (newCapacity > minNewCapacity)
			{
				newCapacity += minNewCapacity;
			}
			else
			{
				//以自身双倍扩展空间
				newCapacity = 64;
				while (newCapacity < minNewCapacity)
				{
					newCapacity <<= 1;
				}
			}


			for (int i = this.capacity; i < newCapacity; i++)
			{
				this.unUsedRecord.Add(i);
				this.pool.Add(SocketAsyncEventArgsMetadata.valueOf(i));
			}

			this.capacity = newCapacity;
		}


		//getter

		public int GetCapacity()
		{
			return this.capacity;
		}

		/**构建方法*/
		public static SocketAsyncEventArgsPool valueOf(int maxCapacity)
		{
			SocketAsyncEventArgsPool result = new SocketAsyncEventArgsPool();
			result.capacity = maxCapacity;
			result.init();
			return result;
		}
	}
}
