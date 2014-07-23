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
		/// <summary>
		/// SP技能100%（蓝光） </summary>
		public static readonly FighterStateEnum SP_MAX_STATE = new FighterStateEnumAnonymousInnerClassHelper2();

		private class FighterStateEnumAnonymousInnerClassHelper2 : FighterStateEnum
		{
			public FighterStateEnumAnonymousInnerClassHelper2() : base(1, BattleConstants.SP_MAX_FALG)
			{
			}

		}

		/// <summary>
		/// 麻痹 </summary>
		public static readonly FighterStateEnum ATTACK_ZERO_FLAG = new FighterStateEnumAnonymousInnerClassHelper3();

		private class FighterStateEnumAnonymousInnerClassHelper3 : FighterStateEnum
		{
			public FighterStateEnumAnonymousInnerClassHelper3() : base(2, BattleConstants.ATTACK_ZERO_FLAG)
			{
			}

		}
		/// <summary>
		/// 石化 </summary>
		public static readonly FighterStateEnum ATTACK_DIS_FLAG = new FighterStateEnumAnonymousInnerClassHelper4();

		private class FighterStateEnumAnonymousInnerClassHelper4 : FighterStateEnum
		{
			public FighterStateEnumAnonymousInnerClassHelper4() : base(3, BattleConstants.ATTACK_DIS_FLAG)
			{
			}

		}
		/// <summary>
		/// 封魔 </summary>
		public static readonly FighterStateEnum SKILL_DIS_FLAG = new FighterStateEnumAnonymousInnerClassHelper5();

		private class FighterStateEnumAnonymousInnerClassHelper5 : FighterStateEnum
		{
			public FighterStateEnumAnonymousInnerClassHelper5() : base(4, BattleConstants.SKILL_DIS_FLAG)
			{
			}

		}
		/// <summary>
		/// 护盾 </summary>
		public static readonly FighterStateEnum MONSTER_SHIELD = new FighterStateEnumAnonymousInnerClassHelper6();

		private class FighterStateEnumAnonymousInnerClassHelper6 : FighterStateEnum
		{
			public FighterStateEnumAnonymousInnerClassHelper6() : base(5, BattleConstants.MONSTER_SHIELD_FLAG)
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