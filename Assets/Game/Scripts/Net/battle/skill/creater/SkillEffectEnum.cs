namespace com.kx.sglm.gs.battle.share.skill.creater
{

	using ColorChangeSkillEffect = com.kx.sglm.gs.battle.share.skill.effect.ColorChangeSkillEffect;
	using HeroNormalAttackEffect = com.kx.sglm.gs.battle.share.skill.effect.HeroNormalAttackEffect;
	using MonsterAttackEffect = com.kx.sglm.gs.battle.share.skill.effect.MonsterAttackEffect;
	using MutiHitEffect = com.kx.sglm.gs.battle.share.skill.effect.MutiHitEffect;
	using NormalRecoverEffect = com.kx.sglm.gs.battle.share.skill.effect.NormalRecoverEffect;
	using RecoverPercentEffect = com.kx.sglm.gs.battle.share.skill.effect.RecoverPercentEffect;

	public abstract class SkillEffectEnum : BaseSkillFactoryEnum
	{

		public static readonly SkillEffectEnum NIL = new SkillEffectEnumAnonymousInnerClassHelper();

		private class SkillEffectEnumAnonymousInnerClassHelper : SkillEffectEnum
		{
			public SkillEffectEnumAnonymousInnerClassHelper() : base(0)
			{
			}


			internal override ISkillPartInfo createInstance()
			{
				return null;
			}
		}

		public static readonly SkillEffectEnum NORMAL_HERO_HIT = new SkillEffectEnumAnonymousInnerClassHelper2();

		private class SkillEffectEnumAnonymousInnerClassHelper2 : SkillEffectEnum
		{
			public SkillEffectEnumAnonymousInnerClassHelper2() : base(1)
			{
			}


			internal override ISkillPartInfo createInstance()
			{
				return new HeroNormalAttackEffect();
			}
		}

		public static readonly SkillEffectEnum NORMAL_HERO_RECOVER = new SkillEffectEnumAnonymousInnerClassHelper3();

		private class SkillEffectEnumAnonymousInnerClassHelper3 : SkillEffectEnum
		{
			public SkillEffectEnumAnonymousInnerClassHelper3() : base(2)
			{
			}


			internal override ISkillPartInfo createInstance()
			{
				return new NormalRecoverEffect();
			}
		}

		public static readonly SkillEffectEnum MUTI_HERO_HIT = new SkillEffectEnumAnonymousInnerClassHelper4();

		private class SkillEffectEnumAnonymousInnerClassHelper4 : SkillEffectEnum
		{
			public SkillEffectEnumAnonymousInnerClassHelper4() : base(3)
			{
			}


			internal override ISkillPartInfo createInstance()
			{
				return new MutiHitEffect();
			}
		}

		public static readonly SkillEffectEnum NORMAL_MONSTER_HIT = new SkillEffectEnumAnonymousInnerClassHelper5();

		private class SkillEffectEnumAnonymousInnerClassHelper5 : SkillEffectEnum
		{
			public SkillEffectEnumAnonymousInnerClassHelper5() : base(4)
			{
			}


			internal override ISkillPartInfo createInstance()
			{
				return new MonsterAttackEffect();
			}
		}

		public static readonly SkillEffectEnum COLOR_CHANGE = new SkillEffectEnumAnonymousInnerClassHelper6();

		private class SkillEffectEnumAnonymousInnerClassHelper6 : SkillEffectEnum
		{
			public SkillEffectEnumAnonymousInnerClassHelper6() : base(5)
			{
			}


			internal override ISkillPartInfo createInstance()
			{
				return new ColorChangeSkillEffect();
			}
		}

		public static readonly SkillEffectEnum RECOVER_PERCENT = new SkillEffectEnumAnonymousInnerClassHelper7();

		private class SkillEffectEnumAnonymousInnerClassHelper7 : SkillEffectEnum
		{
			public SkillEffectEnumAnonymousInnerClassHelper7() : base(6)
			{
			}


			internal override ISkillPartInfo createInstance()
			{
				return new RecoverPercentEffect();
			}
		}


		public SkillEffectEnum(int index) : base(index)
		{
		}

		private static readonly SkillEffectEnum[] VALUES = new SkillEffectEnum[] {NIL, NORMAL_HERO_HIT, NORMAL_HERO_RECOVER, MUTI_HERO_HIT, NORMAL_MONSTER_HIT, COLOR_CHANGE, RECOVER_PERCENT};

		public static SkillEffectEnum[] values()
		{
			return VALUES;
		}



	}

}