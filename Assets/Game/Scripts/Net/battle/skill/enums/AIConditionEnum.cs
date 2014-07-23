namespace com.kx.sglm.gs.battle.share.skill.enums
{

	using BaseBattleFactoryEnum = com.kx.sglm.gs.battle.share.enums.BaseBattleFactoryEnum;
	using IBattlePartInfo = com.kx.sglm.gs.battle.share.enums.IBattlePartInfo;
	using AIDeadMonsterCondition = com.kx.sglm.gs.battle.share.skill.aicondition.AIDeadMonsterCondition;
	using AIHpLeftLessCondition = com.kx.sglm.gs.battle.share.skill.aicondition.AIHpLeftLessCondition;
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

		/// <summary>
		/// 剩余血量 </summary>
		public static readonly AIConditionEnum LEFT_HP = new AIConditionEnumAnonymousInnerClassHelper3();

		private class AIConditionEnumAnonymousInnerClassHelper3 : AIConditionEnum
		{
			public AIConditionEnumAnonymousInnerClassHelper3() : base(2)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new AIHpLeftLessCondition();
			}
		}

		/// <summary>
		/// 死亡怪物数 </summary>
		public static readonly AIConditionEnum DEAD_MONSTER_COUNT = new AIConditionEnumAnonymousInnerClassHelper4();

		private class AIConditionEnumAnonymousInnerClassHelper4 : AIConditionEnum
		{
			public AIConditionEnumAnonymousInnerClassHelper4() : base(3)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new AIDeadMonsterCondition();
			}
		}

		public AIConditionEnum(int index) : base(index)
		{
		}

		private static readonly AIConditionEnum[] VALUES = new AIConditionEnum[] {NIL, ROUND_COUNT, LEFT_HP, DEAD_MONSTER_COUNT};

		public static AIConditionEnum[] values()
		{
			return VALUES;
		}

	}

}