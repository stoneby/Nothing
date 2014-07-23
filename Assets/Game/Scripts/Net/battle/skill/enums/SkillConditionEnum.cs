namespace com.kx.sglm.gs.battle.share.skill.enums
{

	using BaseBattleFactoryEnum = com.kx.sglm.gs.battle.share.enums.BaseBattleFactoryEnum;
	using IBattlePartInfo = com.kx.sglm.gs.battle.share.enums.IBattlePartInfo;
	using FighterLeaderCondition = com.kx.sglm.gs.battle.share.skill.condition.FighterLeaderCondition;
	using HPLeftMoreCondition = com.kx.sglm.gs.battle.share.skill.condition.HPLeftMoreCondition;
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

		public SkillConditionEnum(int index) : base(index)
		{
		}


		private static readonly SkillConditionEnum[] VALUES = new SkillConditionEnum[] {NIL, RATE, HP_LEFT_CONDITION, LEADER_CONDITION};

		public static SkillConditionEnum[] values()
		{
			return VALUES;
		}

	}

}