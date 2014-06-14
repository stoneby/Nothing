namespace com.kx.sglm.gs.battle.share.skill.creater
{

	using HPLeftCondition = com.kx.sglm.gs.battle.share.skill.condition.HPLeftCondition;
	using SkillRateCondition = com.kx.sglm.gs.battle.share.skill.condition.SkillRateCondition;

	public abstract class SkillConditionEnum : BaseSkillFactoryEnum
	{

		public static readonly SkillConditionEnum NIL = new SkillConditionEnumAnonymousInnerClassHelper();

		private class SkillConditionEnumAnonymousInnerClassHelper : SkillConditionEnum
		{
			public SkillConditionEnumAnonymousInnerClassHelper() : base(0)
			{
			}


			internal override ISkillPartInfo createInstance()
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


			internal override ISkillPartInfo createInstance()
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


			internal override ISkillPartInfo createInstance()
			{
				return new HPLeftCondition();
			}
		}

		public SkillConditionEnum(int index) : base(index)
		{
		}


		private static readonly SkillConditionEnum[] VALUES = new SkillConditionEnum[] {NIL, RATE, HP_LEFT_CONDITION};

		public static SkillConditionEnum[] values()
		{
			return VALUES;
		}

	}

}