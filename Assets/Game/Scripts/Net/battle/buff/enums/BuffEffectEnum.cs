namespace com.kx.sglm.gs.battle.share.buff.enums
{

	using MonsterShield = com.kx.sglm.gs.battle.share.buff.effect.MonsterShield;
	using PoisonValueEffect = com.kx.sglm.gs.battle.share.buff.effect.PoisonValueEffect;
	using PropEffectBuff = com.kx.sglm.gs.battle.share.buff.effect.PropEffectBuff;
	using PropEffectDebuff = com.kx.sglm.gs.battle.share.buff.effect.PropEffectDebuff;
	using BaseBattleFactoryEnum = com.kx.sglm.gs.battle.share.enums.BaseBattleFactoryEnum;
	using IBattlePartInfo = com.kx.sglm.gs.battle.share.enums.IBattlePartInfo;

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
		/// add or reduce fighter prop </summary>
		public static readonly BuffEffectEnum PROP_REDUCE_EFFECT = new BuffEffectEnumAnonymousInnerClassHelper3();

		private class BuffEffectEnumAnonymousInnerClassHelper3 : BuffEffectEnum
		{
			public BuffEffectEnumAnonymousInnerClassHelper3() : base(2, false)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new PropEffectDebuff();
			}
		}



		/// <summary>
		/// shield for monster </summary>
		public static readonly BuffEffectEnum MONSTER_SHIELD = new BuffEffectEnumAnonymousInnerClassHelper4();

		private class BuffEffectEnumAnonymousInnerClassHelper4 : BuffEffectEnum
		{
			public BuffEffectEnumAnonymousInnerClassHelper4() : base(3, true)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new MonsterShield();
			}
		}

		/// <summary>
		/// it is a debuff </summary>
		public static readonly BuffEffectEnum POISON = new BuffEffectEnumAnonymousInnerClassHelper5();

		private class BuffEffectEnumAnonymousInnerClassHelper5 : BuffEffectEnum
		{
			public BuffEffectEnumAnonymousInnerClassHelper5() : base(4, false)
			{
			}


			public override IBattlePartInfo createInstance()
			{
				return new PoisonValueEffect();
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

		public static readonly BuffEffectEnum[] VALUES = new BuffEffectEnum[] {NIL_EFFECT, PROP_ADD_EFFECT, PROP_REDUCE_EFFECT, MONSTER_SHIELD, POISON};

		public static BuffEffectEnum[] values()
		{
			return VALUES;
		}



	}

}