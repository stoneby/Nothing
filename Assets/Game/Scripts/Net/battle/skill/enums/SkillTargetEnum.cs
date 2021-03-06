namespace com.kx.sglm.gs.battle.share.skill.enums
{

	using BaseBattleFactoryEnum = com.kx.sglm.gs.battle.share.enums.BaseBattleFactoryEnum;
	using IBattlePartInfo = com.kx.sglm.gs.battle.share.enums.IBattlePartInfo;
	using AllBattlingTargetGetter = com.kx.sglm.gs.battle.share.skill.target.AllBattlingTargetGetter;
	using AllTargetGetter = com.kx.sglm.gs.battle.share.skill.target.AllTargetGetter;
	using ColorAllTargetGetter = com.kx.sglm.gs.battle.share.skill.target.ColorAllTargetGetter;
	using ColorBattlingTargetGetter = com.kx.sglm.gs.battle.share.skill.target.ColorBattlingTargetGetter;
	using CrossTargetGetter = com.kx.sglm.gs.battle.share.skill.target.CrossTargetGetter;
	using DefaultTargetGetter = com.kx.sglm.gs.battle.share.skill.target.DefaultTargetGetter;
	using IndexTargetGetter = com.kx.sglm.gs.battle.share.skill.target.IndexTargetGetter;
	using JobAllTargetGetter = com.kx.sglm.gs.battle.share.skill.target.JobAllTargetGetter;
	using JobBattlingTargetGetter = com.kx.sglm.gs.battle.share.skill.target.JobBattlingTargetGetter;
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
				return new ColorAllTargetGetter();
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
				return new JobAllTargetGetter();
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

		public static readonly SkillTargetEnum ALL_BATTLING_TARGET = new SkillTargetEnumAnonymousInnerClassHelper12();

		private class SkillTargetEnumAnonymousInnerClassHelper12 : SkillTargetEnum
		{
			public SkillTargetEnumAnonymousInnerClassHelper12() : base(11)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new AllBattlingTargetGetter();
			}
		}

		public static readonly SkillTargetEnum COLOR_BATTLING_TARGET = new SkillTargetEnumAnonymousInnerClassHelper13();

		private class SkillTargetEnumAnonymousInnerClassHelper13 : SkillTargetEnum
		{
			public SkillTargetEnumAnonymousInnerClassHelper13() : base(12)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new ColorBattlingTargetGetter();
			}
		}

		public static readonly SkillTargetEnum JOB_BATTLING_TARGET = new SkillTargetEnumAnonymousInnerClassHelper14();

		private class SkillTargetEnumAnonymousInnerClassHelper14 : SkillTargetEnum
		{
			public SkillTargetEnumAnonymousInnerClassHelper14() : base(13)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new JobBattlingTargetGetter();
			}
		}

		public SkillTargetEnum(int index) : base(index)
		{
		}

		private static SkillTargetEnum[] VALUES = new SkillTargetEnum[] {NIL, DEFAULT_TARGET, INDEX_TARGET, SELF_TARGET, ALL_TARGET, RANDOM_TARGET, COLOR_TARGET, JOB_TARGET, CROSS_TARGET, SIDEWAYS_TARGET, UPRIGHT_TARGET, ALL_BATTLING_TARGET, COLOR_BATTLING_TARGET, JOB_BATTLING_TARGET};

		public static SkillTargetEnum[] values()
		{
			return VALUES;
		}

	}

}