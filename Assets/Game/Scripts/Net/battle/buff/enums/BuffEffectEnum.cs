namespace com.kx.sglm.gs.battle.share.buff.enums
{

	using DamagePropEffectBuff = com.kx.sglm.gs.battle.share.buff.effect.DamagePropEffectBuff;
	using DamagePropEffectDebuff = com.kx.sglm.gs.battle.share.buff.effect.DamagePropEffectDebuff;
	using FiringPercentEffect = com.kx.sglm.gs.battle.share.buff.effect.FiringPercentEffect;
	using FrozenDebuffEffect = com.kx.sglm.gs.battle.share.buff.effect.FrozenDebuffEffect;
	using MonsterShield = com.kx.sglm.gs.battle.share.buff.effect.MonsterShield;
	using ParalysisDebuffEffect = com.kx.sglm.gs.battle.share.buff.effect.ParalysisDebuffEffect;
	using PetrifiedDebuffEffect = com.kx.sglm.gs.battle.share.buff.effect.PetrifiedDebuffEffect;
	using PoisonDebuffEffect = com.kx.sglm.gs.battle.share.buff.effect.PoisonDebuffEffect;
	using PropEffectBuff = com.kx.sglm.gs.battle.share.buff.effect.PropEffectBuff;
	using PropEffectDebuff = com.kx.sglm.gs.battle.share.buff.effect.PropEffectDebuff;
	using SealSkillDebuffEffect = com.kx.sglm.gs.battle.share.buff.effect.SealSkillDebuffEffect;
	using SleepDebuffEffect = com.kx.sglm.gs.battle.share.buff.effect.SleepDebuffEffect;
	using SpMaxBuffEffect = com.kx.sglm.gs.battle.share.buff.effect.SpMaxBuffEffect;
	using BaseBattleFactoryEnum = com.kx.sglm.gs.battle.share.enums.BaseBattleFactoryEnum;
	using IBattlePartInfo = com.kx.sglm.gs.battle.share.enums.IBattlePartInfo;

	/// <summary>
	/// All buff can use
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public abstract class BuffEffectEnum : BaseBattleFactoryEnum
	{

		/// <summary>
		/// add or reduce fighter prop </summary>
		public static readonly BuffEffectEnum NIL_EFFECT = new BuffEffectEnumAnonymousInnerClassHelper();

		private class BuffEffectEnumAnonymousInnerClassHelper : BuffEffectEnum
		{
			public BuffEffectEnumAnonymousInnerClassHelper() : base(0, false)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return null;
			}
		}

		/// <summary>
		/// add or reduce fighter prop </summary>
		public static readonly BuffEffectEnum PROP_ADD_EFFECT = new BuffEffectEnumAnonymousInnerClassHelper2();

		private class BuffEffectEnumAnonymousInnerClassHelper2 : BuffEffectEnum
		{
			public BuffEffectEnumAnonymousInnerClassHelper2() : base(1, true)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new PropEffectBuff();
			}
		}

		/// <summary>
		/// shield for monster </summary>
		public static readonly BuffEffectEnum MONSTER_SHIELD = new BuffEffectEnumAnonymousInnerClassHelper3();

		private class BuffEffectEnumAnonymousInnerClassHelper3 : BuffEffectEnum
		{
			public BuffEffectEnumAnonymousInnerClassHelper3() : base(2, true)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new MonsterShield();
			}
		}

		/// <summary>
		/// ÖÐ¶¾ it is a debuff </summary>
		public static readonly BuffEffectEnum POISON_VALUE = new BuffEffectEnumAnonymousInnerClassHelper4();

		private class BuffEffectEnumAnonymousInnerClassHelper4 : BuffEffectEnum
		{
			public BuffEffectEnumAnonymousInnerClassHelper4() : base(3, false)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new PoisonDebuffEffect();
			}
		}

		/// <summary>
		/// ×ÆÉÕ fire effect debuff, like poison, but hurt fighter from cur HP percent </summary>
		public static readonly BuffEffectEnum FIRING_DEBUFF = new BuffEffectEnumAnonymousInnerClassHelper5();

		private class BuffEffectEnumAnonymousInnerClassHelper5 : BuffEffectEnum
		{
			public BuffEffectEnumAnonymousInnerClassHelper5() : base(4, false)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new FiringPercentEffect();
			}
		}

		/// <summary>
		/// Âé±Ô fighter can connected but has no attack </summary>
		public static readonly BuffEffectEnum PARALYSIS_DEBUFF = new BuffEffectEnumAnonymousInnerClassHelper6();

		private class BuffEffectEnumAnonymousInnerClassHelper6 : BuffEffectEnum
		{
			public BuffEffectEnumAnonymousInnerClassHelper6() : base(5, false)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new ParalysisDebuffEffect();
			}
		}

		/// <summary>
		/// fighter can connected but has no attack </summary>
		public static readonly BuffEffectEnum SLEEP_DEBUFF = new BuffEffectEnumAnonymousInnerClassHelper7();

		private class BuffEffectEnumAnonymousInnerClassHelper7 : BuffEffectEnum
		{
			public BuffEffectEnumAnonymousInnerClassHelper7() : base(6, false)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new SleepDebuffEffect();
			}
		}

		/// <summary>
		///fighter cannot connected </summary>
		public static readonly BuffEffectEnum FROZEN_DEBUFF = new BuffEffectEnumAnonymousInnerClassHelper8();

		private class BuffEffectEnumAnonymousInnerClassHelper8 : BuffEffectEnum
		{
			public BuffEffectEnumAnonymousInnerClassHelper8() : base(7, false)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new FrozenDebuffEffect();
			}
		}

		/// <summary>
		/// Ê¯»¯ </summary>
		public static readonly BuffEffectEnum PETRIFIED_DEBUFF = new BuffEffectEnumAnonymousInnerClassHelper9();

		private class BuffEffectEnumAnonymousInnerClassHelper9 : BuffEffectEnum
		{
			public BuffEffectEnumAnonymousInnerClassHelper9() : base(8, false)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new PetrifiedDebuffEffect();
			}
		}

		/// <summary>
		/// ·âÄ§ fighter cannot use active skill </summary>
		public static readonly BuffEffectEnum SEAL_SKILL_DEBUFF = new BuffEffectEnumAnonymousInnerClassHelper10();

		private class BuffEffectEnumAnonymousInnerClassHelper10 : BuffEffectEnum
		{
			public BuffEffectEnumAnonymousInnerClassHelper10() : base(9, false)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new SealSkillDebuffEffect();
			}
		}

		public static readonly BuffEffectEnum SP_MAX_BUFF = new BuffEffectEnumAnonymousInnerClassHelper11();

		private class BuffEffectEnumAnonymousInnerClassHelper11 : BuffEffectEnum
		{
			public BuffEffectEnumAnonymousInnerClassHelper11() : base(10, true)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new SpMaxBuffEffect();
			}
		}

		/// <summary>
		/// add or reduce fighter prop </summary>
		public static readonly BuffEffectEnum PROP_REDUCE_EFFECT = new BuffEffectEnumAnonymousInnerClassHelper12();

		private class BuffEffectEnumAnonymousInnerClassHelper12 : BuffEffectEnum
		{
			public BuffEffectEnumAnonymousInnerClassHelper12() : base(11, false)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new PropEffectDebuff();
			}
		}

		/// <summary>
		/// prop effect for damage, after attack calculate </summary>
		public static readonly BuffEffectEnum DAMAGE_ADD_BUFF_EFFECT = new BuffEffectEnumAnonymousInnerClassHelper13();

		private class BuffEffectEnumAnonymousInnerClassHelper13 : BuffEffectEnum
		{
			public BuffEffectEnumAnonymousInnerClassHelper13() : base(12, true)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new DamagePropEffectBuff();
			}
		}

		/// <summary>
		/// prop effect for damage, reduce damage </summary>
		public static readonly BuffEffectEnum DAMAGE_REDUCE_DEBUFF_EFFECT = new BuffEffectEnumAnonymousInnerClassHelper14();

		private class BuffEffectEnumAnonymousInnerClassHelper14 : BuffEffectEnum
		{
			public BuffEffectEnumAnonymousInnerClassHelper14() : base(13, false)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new DamagePropEffectDebuff();
			}
		}

		public BuffEffectEnum(int index, bool buff) : base(index)
		{
			this.buff = buff;
		}

		private bool buff;

		public virtual bool Buff
		{
			get
			{
				return buff;
			}
		}

		public static readonly BuffEffectEnum[] VALUES = new BuffEffectEnum[] {NIL_EFFECT, PROP_ADD_EFFECT, MONSTER_SHIELD, POISON_VALUE, FIRING_DEBUFF, PARALYSIS_DEBUFF, SLEEP_DEBUFF, FROZEN_DEBUFF, PETRIFIED_DEBUFF, SEAL_SKILL_DEBUFF, SP_MAX_BUFF, PROP_REDUCE_EFFECT, DAMAGE_ADD_BUFF_EFFECT, DAMAGE_REDUCE_DEBUFF_EFFECT};

		public static BuffEffectEnum[] values()
		{
			return VALUES;
		}

	}

}