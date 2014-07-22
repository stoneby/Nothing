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

		public static readonly FighterStateEnum SP_MAX_STATE = new FighterStateEnumAnonymousInnerClassHelper2();

		private class FighterStateEnumAnonymousInnerClassHelper2 : FighterStateEnum
		{
			public FighterStateEnumAnonymousInnerClassHelper2() : base(1, BattleConstants.SP_MAX_FALG)
			{
			}

		}

		/// <summary>
		/// Âé±Ô </summary>
		public static readonly FighterStateEnum PARALYSIS = new FighterStateEnumAnonymousInnerClassHelper3();

		private class FighterStateEnumAnonymousInnerClassHelper3 : FighterStateEnum
		{
			public FighterStateEnumAnonymousInnerClassHelper3() : base(2, BattleConstants.ATTACK_DIS_FLAG)
			{
			}

		}
		/// <summary>
		/// Ê¯»¯ </summary>
		public static readonly FighterStateEnum PETRIFIED = new FighterStateEnumAnonymousInnerClassHelper4();

		private class FighterStateEnumAnonymousInnerClassHelper4 : FighterStateEnum
		{
			public FighterStateEnumAnonymousInnerClassHelper4() : base(3, BattleConstants.ATTACK_DIS_FLAG)
			{
			}

		}
		/// <summary>
		/// ·âÄ§ </summary>
		public static readonly FighterStateEnum SEAL_SKILL = new FighterStateEnumAnonymousInnerClassHelper5(BattleConstants.ATTACK_DIS_FLAG | BattleConstants.SKILL_DIS_FLAG);

		private class FighterStateEnumAnonymousInnerClassHelper5 : FighterStateEnum
		{
			public FighterStateEnumAnonymousInnerClassHelper5(int SKILL_DIS_FLAG) : base(4, SKILL_DIS_FLAG)
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

		private static readonly FighterStateEnum[] VALUES = new FighterStateEnum[] {NORMAL_STATE, SP_MAX_STATE};

		public static FighterStateEnum getValue(int index)
		{
			return VALUES[index];
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