namespace com.kx.sglm.gs.battle.share.skill.enums
{

	using BaseBattleFactoryEnum = com.kx.sglm.gs.battle.share.enums.BaseBattleFactoryEnum;
	using IBattlePartInfo = com.kx.sglm.gs.battle.share.enums.IBattlePartInfo;
	using FighterLeaderCondition = com.kx.sglm.gs.battle.share.skill.condition.FighterLeaderCondition;
	using HPLeftMoreCondition = com.kx.sglm.gs.battle.share.skill.condition.HPLeftMoreCondition;
	using NIndexConnectCondition = com.kx.sglm.gs.battle.share.skill.condition.NIndexConnectCondition;
	using NPointConnectCondition = com.kx.sglm.gs.battle.share.skill.condition.NPointConnectCondition;
	using SkillRateCondition = com.kx.sglm.gs.battle.share.skill.condition.SkillRateCondition;

	public abstract class SkillConditionEnum : BaseBattleFactoryEnum
	{

		public static readonly SkillConditionEnum NIL = new SkillConditionEnumAnonymousInnerClassHelper();

		private class SkillConditionEnumAnonymousInnerClassHelper : SkillConditionEnum
		{
			public SkillConditionEnumAnonymousInnerClassHelper() : base(0)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return null;
			}
		}

		public static readonly SkillConditionEnum RATE = new SkillConditionEnumAnonymousInnerClassHelper2();

		private class SkillConditionEnumAnonymousInnerClassHelper2 : SkillConditionEnum
		{
			public SkillConditionEnumAnonymousInnerClassHelper2() : base(1)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new SkillRateCondition();
			}
		}

		public static readonly SkillConditionEnum HP_LEFT_CONDITION = new SkillConditionEnumAnonymousInnerClassHelper3();

		private class SkillConditionEnumAnonymousInnerClassHelper3 : SkillConditionEnum
		{
			public SkillConditionEnumAnonymousInnerClassHelper3() : base(2)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new HPLeftMoreCondition();
			}
		}

		public static readonly SkillConditionEnum LEADER_CONDITION = new SkillConditionEnumAnonymousInnerClassHelper4();

		private class SkillConditionEnumAnonymousInnerClassHelper4 : SkillConditionEnum
		{
			public SkillConditionEnumAnonymousInnerClassHelper4() : base(3)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new FighterLeaderCondition();
			}
		}

		public static readonly SkillConditionEnum NINDEX_CONNECT = new SkillConditionEnumAnonymousInnerClassHelper5();

		private class SkillConditionEnumAnonymousInnerClassHelper5 : SkillConditionEnum
		{
			public SkillConditionEnumAnonymousInnerClassHelper5() : base(4)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new NIndexConnectCondition();
			}
		}

		public static readonly SkillConditionEnum NPOINT_CONNECT = new SkillConditionEnumAnonymousInnerClassHelper6();

		private class SkillConditionEnumAnonymousInnerClassHelper6 : SkillConditionEnum
		{
			public SkillConditionEnumAnonymousInnerClassHelper6() : base(5)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new NPointConnectCondition();
			}
		}

		public SkillConditionEnum(int index) : base(index)
		{
		}

		private static readonly SkillConditionEnum[] VALUES = new SkillConditionEnum[] {NIL, RATE, HP_LEFT_CONDITION, LEADER_CONDITION, NINDEX_CONNECT, NPOINT_CONNECT};

		public static SkillConditionEnum[] values()
		{
			return VALUES;
		}

	}

}