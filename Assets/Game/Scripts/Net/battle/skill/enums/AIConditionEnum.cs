namespace com.kx.sglm.gs.battle.share.skill.enums
{

	using BaseBattleFactoryEnum = com.kx.sglm.gs.battle.share.enums.BaseBattleFactoryEnum;
	using IBattlePartInfo = com.kx.sglm.gs.battle.share.enums.IBattlePartInfo;
	using AIRoundCondition = com.kx.sglm.gs.battle.share.skill.aicondition.AIRoundCondition;


	public abstract class AIConditionEnum : BaseBattleFactoryEnum
	{

		public static readonly AIConditionEnum NIL = new AIConditionEnumAnonymousInnerClassHelper();

		private class AIConditionEnumAnonymousInnerClassHelper : AIConditionEnum
		{
			public AIConditionEnumAnonymousInnerClassHelper() : base(0)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return null;
			}
		}

		/// <summary>
		/// 回合数条件 </summary>
		public static readonly AIConditionEnum ROUND_COUNT = new AIConditionEnumAnonymousInnerClassHelper2();

		private class AIConditionEnumAnonymousInnerClassHelper2 : AIConditionEnum
		{
			public AIConditionEnumAnonymousInnerClassHelper2() : base(1)
			{
			}


			public override IBattlePartInfo createInstance()
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