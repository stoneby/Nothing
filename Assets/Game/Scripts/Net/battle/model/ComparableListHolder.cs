using System;

namespace com.kx.sglm.gs.battle.share.model
{

	/// <summary>
	/// 可排序的<seealso cref="KeyListHolder"/>，用于排序的是<seealso cref="#getKey()"/>
	/// @author liyuan2
	/// </summary>
	/// @param <T> </param>
	public abstract class ComparableListHolder<T> : KeyListHolder<T>, IComparable<ComparableListHolder<T>>
	{

		public ComparableListHolder(int key) : base(key)
		{
		}

		protected internal bool asc;


		public virtual bool Asc
		{
			set
			{
				this.asc = value;
			}
		}

		public virtual int CompareTo(ComparableListHolder<T> o)
		{
			return asc ? Key - o.Key : o.Key - Key;
		}

	}

}