namespace com.kx.sglm.gs.battle.share.skill.creater
{

	using AIRoundCondition = com.kx.sglm.gs.battle.share.skill.aicondition.AIRoundCondition;


	public abstract class AIConditionEnum : BaseSkillFactoryEnum
	{

		public static readonly AIConditionEnum NIL = new AIConditionEnumAnonymousInnerClassHelper();

		private class AIConditionEnumAnonymousInnerClassHelper : AIConditionEnum
		{
			public AIConditionEnumAnonymousInnerClassHelper() : base(0)
			{
			}


			internal override ISkillPartInfo createInstance()
			{
				return null;
			}
		}

		/// <summary>
		/// 回合数条�? </summary>
		public static readonly AIConditionEnum ROUND_COUNT = new AIConditionEnumAnonymousInnerClassHelper2();

		private class AIConditionEnumAnonymousInnerClassHelper2 : AIConditionEnum
		{
			public AIConditionEnumAnonymousInnerClassHelper2() : base(1)
			{
			}


			internal override ISkillPartInfo createInstance()
			{
				return new AIRoundCondition();
			}
		}



		public AIConditionEnum(int index) : base(index)
		{
		}

		private static readonly AIConditionEnum[] VALUES = new AIConditionEnum[] {NIL, ROUND_COUNT};

		public static AIConditionEnum[] values()
		{
			return VALUES;
		}



	}

}