using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.model
{


	/// <summary>
	/// 带有公共Key的List元组，战斗内用到的这种结构较多，统一处理
	/// 
	/// @author liyuan2
	/// </summary>
	/// @param <T> </param>
	public class KeyListHolder<T>
	{

		private int key;
		private List<T> holdList;

		public KeyListHolder(int key)
		{
			this.key = key;
			this.holdList = new List<T>();
		}

		public virtual int Key
		{
			get
			{
				return key;
			}
		}

		public virtual void addData(T data)
		{
			this.holdList.Add(data);
		}

		public virtual List<T> HoldList
		{
			get
			{
				return holdList;
			}
		}

	}

}