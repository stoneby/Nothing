namespace com.kx.sglm.gs.battle.share.skill.creater
{

	using AllTargetGetter = com.kx.sglm.gs.battle.share.skill.target.AllTargetGetter;
	using ColorTargetGetter = com.kx.sglm.gs.battle.share.skill.target.ColorTargetGetter;
	using DefaultTargetGetter = com.kx.sglm.gs.battle.share.skill.target.DefaultTargetGetter;
	using IndexTargetGetter = com.kx.sglm.gs.battle.share.skill.target.IndexTargetGetter;
	using RandomTargetGetter = com.kx.sglm.gs.battle.share.skill.target.RandomTargetGetter;
	using SelfTargetGetter = com.kx.sglm.gs.battle.share.skill.target.SelfTargetGetter;

	public abstract class SkillTargetEnum : BaseSkillFactoryEnum
	{

		public static readonly SkillTargetEnum NIL = new SkillTargetEnumAnonymousInnerClassHelper();

		private class SkillTargetEnumAnonymousInnerClassHelper : SkillTargetEnum
		{
			public SkillTargetEnumAnonymousInnerClassHelper() : base(0)
			{
			}


			internal override ISkillPartInfo createInstance()
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


			internal override ISkillPartInfo createInstance()
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


			internal override ISkillPartInfo createInstance()
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


			internal override ISkillPartInfo createInstance()
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


			internal override ISkillPartInfo createInstance()
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


			internal override ISkillPartInfo createInstance()
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


			internal override ISkillPartInfo createInstance()
			{
				return new ColorTargetGetter();
			}
		}

		public SkillTargetEnum(int index) : base(index)
		{
		}

		private static SkillTargetEnum[] VALUES = new SkillTargetEnum[] {NIL, DEFAULT_TARGET, INDEX_TARGET, SELF_TARGET, ALL_TARGET, RANDOM_TARGET,COLOR_TARGET};

		public static SkillTargetEnum[] values()
		{
			return VALUES;
		}


	}

}