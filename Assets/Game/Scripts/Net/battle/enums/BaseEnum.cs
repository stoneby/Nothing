namespace com.kx.sglm.gs.battle.share.enums
{

	using IndexedEnum = com.kx.sglm.core.constant.IndexedEnum;

	public abstract class BaseEnum : IndexedEnum
	{

		protected internal int index;

		public BaseEnum(int index)
		{
			this.index = index;
		}

		public virtual int Index
		{
			get
			{
				return index;
			}
		}

	}

}