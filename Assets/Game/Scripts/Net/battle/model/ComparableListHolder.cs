using System;

namespace com.kx.sglm.gs.battle.share.model
{

	/// <summary>
	/// �������<seealso cref="KeyListHolder"/>�������������<seealso cref="#getKey()"/>
	/// @author liyuan2
	/// </summary>
	/// @param <T> </param>
	public class ComparableListHolder<T> : KeyListHolder<T>, IComparable<ComparableListHolder<T>>
	{

		public ComparableListHolder(int key) : base(key)
		{
		}

		private bool asc;


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