namespace com.kx.sglm.gs.battle.share.enums
{

	using IndexedEnum = com.kx.sglm.core.constant.IndexedEnum;

	public abstract class FighterStateEnum : IndexedEnum
	{

		public static readonly FighterStateEnum NORMAL_STATE = new FighterStateEnumAnonymousInnerClassHelper();

		private class FighterStateEnumAnonymousInnerClassHelper : FighterStateEnum
		{
			public FighterStateEnumAnonymousInnerClassHelper() : base(0, 0)
			{
			}

		}


		private int index;
		private int stateFlag;

		private FighterStateEnum(int index, int stateFlag)
		{
			this.index = index;
			this.stateFlag = stateFlag;
		}

		public virtual int Index
		{
			get
			{
				return index;
			}
		}

		public virtual int StateFlag
		{
			get
			{
				return stateFlag;
			}
		}


	}

}