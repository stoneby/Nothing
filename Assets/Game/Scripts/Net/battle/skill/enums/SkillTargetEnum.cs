namespace com.kx.sglm.gs.battle.share.skill.enums
{

	using BaseBattleFactoryEnum = com.kx.sglm.gs.battle.share.enums.BaseBattleFactoryEnum;
	using IBattlePartInfo = com.kx.sglm.gs.battle.share.enums.IBattlePartInfo;
	using AllTargetGetter = com.kx.sglm.gs.battle.share.skill.target.AllTargetGetter;
	using ColorTargetGetter = com.kx.sglm.gs.battle.share.skill.target.ColorTargetGetter;
	using CrossTargetGetter = com.kx.sglm.gs.battle.share.skill.target.CrossTargetGetter;
	using DefaultTargetGetter = com.kx.sglm.gs.battle.share.skill.target.DefaultTargetGetter;
	using IndexTargetGetter = com.kx.sglm.gs.battle.share.skill.target.IndexTargetGetter;
	using JobTargetGetter = com.kx.sglm.gs.battle.share.skill.target.JobTargetGetter;
	using RandomTargetGetter = com.kx.sglm.gs.battle.share.skill.target.RandomTargetGetter;
	using SelfTargetGetter = com.kx.sglm.gs.battle.share.skill.target.SelfTargetGetter;
	using SidewaysTargetGetter = com.kx.sglm.gs.battle.share.skill.target.SidewaysTargetGetter;
	using UprightTargetGetter = com.kx.sglm.gs.battle.share.skill.target.UprightTargetGetter;

	public abstract class SkillTargetEnum : BaseBattleFactoryEnum
	{

		public static readonly SkillTargetEnum NIL = new SkillTargetEnumAnonymousInnerClassHelper();

		private class SkillTargetEnumAnonymousInnerClassHelper : SkillTargetEnum
		{
			public SkillTargetEnumAnonymousInnerClassHelper() : base(0)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return null;
			}
		}

		public static readonly SkillTargetEnum DEFAULT_TARGET = new SkillTargetEnumAnonymousInnerClassHelper2();

		private class SkillTargetEnumAnonymousInnerClassHelper2 : SkillTargetEnum
		{
			public SkillTargetEnumAnonymousInnerClassHelper2() : base(1)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new DefaultTargetGetter();
			}
		}

		public static readonly SkillTargetEnum INDEX_TARGET = new SkillTargetEnumAnonymousInnerClassHelper3();

		private class SkillTargetEnumAnonymousInnerClassHelper3 : SkillTargetEnum
		{
			public SkillTargetEnumAnonymousInnerClassHelper3() : base(2)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new IndexTargetGetter();
			}
		}

		public static readonly SkillTargetEnum SELF_TARGET = new SkillTargetEnumAnonymousInnerClassHelper4();

		private class SkillTargetEnumAnonymousInnerClassHelper4 : SkillTargetEnum
		{
			public SkillTargetEnumAnonymousInnerClassHelper4() : base(3)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new SelfTargetGetter();
			}
		}

		public static readonly SkillTargetEnum ALL_TARGET = new SkillTargetEnumAnonymousInnerClassHelper5();

		private class SkillTargetEnumAnonymousInnerClassHelper5 : SkillTargetEnum
		{
			public SkillTargetEnumAnonymousInnerClassHelper5() : base(4)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new AllTargetGetter();
			}
		}

		public static readonly SkillTargetEnum RANDOM_TARGET = new SkillTargetEnumAnonymousInnerClassHelper6();

		private class SkillTargetEnumAnonymousInnerClassHelper6 : SkillTargetEnum
		{
			public SkillTargetEnumAnonymousInnerClassHelper6() : base(5)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new RandomTargetGetter();
			}
		}

		public static readonly SkillTargetEnum COLOR_TARGET = new SkillTargetEnumAnonymousInnerClassHelper7();

		private class SkillTargetEnumAnonymousInnerClassHelper7 : SkillTargetEnum
		{
			public SkillTargetEnumAnonymousInnerClassHelper7() : base(6)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new ColorTargetGetter();
			}
		}

		public static readonly SkillTargetEnum JOB_TARGET = new SkillTargetEnumAnonymousInnerClassHelper8();

		private class SkillTargetEnumAnonymousInnerClassHelper8 : SkillTargetEnum
		{
			public SkillTargetEnumAnonymousInnerClassHelper8() : base(7)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new JobTargetGetter();
			}
		}

		public static readonly SkillTargetEnum CROSS_TARGET = new SkillTargetEnumAnonymousInnerClassHelper9();

		private class SkillTargetEnumAnonymousInnerClassHelper9 : SkillTargetEnum
		{
			public SkillTargetEnumAnonymousInnerClassHelper9() : base(8)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new CrossTargetGetter();
			}
		}

		public static readonly SkillTargetEnum SIDEWAYS_TARGET = new SkillTargetEnumAnonymousInnerClassHelper10();

		private class SkillTargetEnumAnonymousInnerClassHelper10 : SkillTargetEnum
		{
			public SkillTargetEnumAnonymousInnerClassHelper10() : base(9)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new SidewaysTargetGetter();
			}
		}

		public static readonly SkillTargetEnum UPRIGHT_TARGET = new SkillTargetEnumAnonymousInnerClassHelper11();

		private class SkillTargetEnumAnonymousInnerClassHelper11 : SkillTargetEnum
		{
			public SkillTargetEnumAnonymousInnerClassHelper11() : base(10)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new UprightTargetGetter();
			}
		}

		public SkillTargetEnum(int index) : base(index)
		{
		}

		private static SkillTargetEnum[] VALUES = new SkillTargetEnum[] {NIL, DEFAULT_TARGET, INDEX_TARGET, SELF_TARGET, ALL_TARGET, RANDOM_TARGET, COLOR_TARGET, JOB_TARGET, CROSS_TARGET, SIDEWAYS_TARGET, UPRIGHT_TARGET};

		public static SkillTargetEnum[] values()
		{
			return VALUES;
		}

	}

}