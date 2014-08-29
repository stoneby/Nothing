namespace com.kx.sglm.gs.battle.share.buff.enums
{

	using ColorCondition = com.kx.sglm.gs.battle.share.buff.condition.ColorCondition;
	using HPLessCondition = com.kx.sglm.gs.battle.share.buff.condition.HPLessCondition;
	using HPMoreCondition = com.kx.sglm.gs.battle.share.buff.condition.HPMoreCondition;
	using BaseBattleFactoryEnum = com.kx.sglm.gs.battle.share.enums.BaseBattleFactoryEnum;
	using IBattlePartInfo = com.kx.sglm.gs.battle.share.enums.IBattlePartInfo;

	public abstract class BuffConditionEnum : BaseBattleFactoryEnum
	{

		public static readonly BuffConditionEnum NIL_CONDITION = new BuffConditionEnumAnonymousInnerClassHelper();

		private class BuffConditionEnumAnonymousInnerClassHelper : BuffConditionEnum
		{
			public BuffConditionEnumAnonymousInnerClassHelper() : base(0)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return null;
			}
		}

		public static readonly BuffConditionEnum FIGHTER_COLOR = new BuffConditionEnumAnonymousInnerClassHelper2();

		private class BuffConditionEnumAnonymousInnerClassHelper2 : BuffConditionEnum
		{
			public BuffConditionEnumAnonymousInnerClassHelper2() : base(1)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new ColorCondition();
			}
		}

		public static readonly BuffConditionEnum FIGHTER_HP_MORE = new BuffConditionEnumAnonymousInnerClassHelper3();

		private class BuffConditionEnumAnonymousInnerClassHelper3 : BuffConditionEnum
		{
			public BuffConditionEnumAnonymousInnerClassHelper3() : base(2)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new HPMoreCondition();
			}
		}

		public static readonly BuffConditionEnum FIGHTER_HP_LESS = new BuffConditionEnumAnonymousInnerClassHelper4();

		private class BuffConditionEnumAnonymousInnerClassHelper4 : BuffConditionEnum
		{
			public BuffConditionEnumAnonymousInnerClassHelper4() : base(3)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new HPLessCondition();
			}
		}

		public BuffConditionEnum(int index) : base(index)
		{
		}

		public override int Index
		{
			get
			{
				return index;
			}
		}

		private static readonly BuffConditionEnum[] VALUES = new BuffConditionEnum[] {NIL_CONDITION, FIGHTER_COLOR, FIGHTER_HP_MORE, FIGHTER_HP_LESS};

		public static BuffConditionEnum[] Values
		{
			get
			{
				return VALUES;
			}
		}


	}

}