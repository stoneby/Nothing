namespace com.kx.sglm.gs.battle.share.skill.enums
{

	using BaseBattleFactoryEnum = com.kx.sglm.gs.battle.share.enums.BaseBattleFactoryEnum;
	using IBattlePartInfo = com.kx.sglm.gs.battle.share.enums.IBattlePartInfo;
	using BuffSkillEffect = com.kx.sglm.gs.battle.share.skill.effect.BuffSkillEffect;
	using ColorChangeSkillEffect = com.kx.sglm.gs.battle.share.skill.effect.ColorChangeSkillEffect;
	using DebuffSkillEffect = com.kx.sglm.gs.battle.share.skill.effect.DebuffSkillEffect;
	using MutiHitEffect = com.kx.sglm.gs.battle.share.skill.effect.MutiHitEffect;
	using NormalAttackEffect = com.kx.sglm.gs.battle.share.skill.effect.NormalAttackEffect;
	using NormalRecoverEffect = com.kx.sglm.gs.battle.share.skill.effect.NormalRecoverEffect;
	using RecoverPercentEffect = com.kx.sglm.gs.battle.share.skill.effect.RecoverPercentEffect;

	public abstract class SkillEffectEnum : BaseBattleFactoryEnum
	{

		public static readonly SkillEffectEnum NIL = new SkillEffectEnumAnonymousInnerClassHelper();

		private class SkillEffectEnumAnonymousInnerClassHelper : SkillEffectEnum
		{
			public SkillEffectEnumAnonymousInnerClassHelper() : base(0)
			{
			}


			public override IBattlePartInfo createInstance()
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


			public override IBattlePartInfo createInstance()
			{
				return new NormalAttackEffect();
			}
		}

		public static readonly SkillEffectEnum NORMAL_HERO_RECOVER = new SkillEffectEnumAnonymousInnerClassHelper3();

		private class SkillEffectEnumAnonymousInnerClassHelper3 : SkillEffectEnum
		{
			public SkillEffectEnumAnonymousInnerClassHelper3() : base(2)
			{
			}


			public override IBattlePartInfo createInstance()
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


			public override IBattlePartInfo createInstance()
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


			public override IBattlePartInfo createInstance()
			{
				return new NormalAttackEffect();
			}
		}

		public static readonly SkillEffectEnum COLOR_CHANGE = new SkillEffectEnumAnonymousInnerClassHelper6();

		private class SkillEffectEnumAnonymousInnerClassHelper6 : SkillEffectEnum
		{
			public SkillEffectEnumAnonymousInnerClassHelper6() : base(5)
			{
			}


			public override IBattlePartInfo createInstance()
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


			public override IBattlePartInfo createInstance()
			{
				return new RecoverPercentEffect();
			}
		}

		public static readonly SkillEffectEnum BUFF_ADD = new SkillEffectEnumAnonymousInnerClassHelper8();

		private class SkillEffectEnumAnonymousInnerClassHelper8 : SkillEffectEnum
		{
			public SkillEffectEnumAnonymousInnerClassHelper8() : base(7)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new BuffSkillEffect();
			}
		}

		public static readonly SkillEffectEnum DEBUFF_ADD = new SkillEffectEnumAnonymousInnerClassHelper9();

		private class SkillEffectEnumAnonymousInnerClassHelper9 : SkillEffectEnum
		{
			public SkillEffectEnumAnonymousInnerClassHelper9() : base(8)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new DebuffSkillEffect();
			}
		}

		public SkillEffectEnum(int index) : base(index)
		{
		}

		private static readonly SkillEffectEnum[] VALUES = new SkillEffectEnum[] {NIL, NORMAL_HERO_HIT, NORMAL_HERO_RECOVER, MUTI_HERO_HIT, NORMAL_MONSTER_HIT, COLOR_CHANGE, RECOVER_PERCENT, BUFF_ADD, DEBUFF_ADD};

		public static SkillEffectEnum[] values()
		{
			return VALUES;
		}

	}

}