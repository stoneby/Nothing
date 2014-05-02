namespace com.kx.sglm.gs.battle.enums
{

	using IndexedEnum = com.kx.sglm.core.constant.IndexedEnum;
	using BattleFighter = com.kx.sglm.gs.battle.actor.impl.BattleFighter;
	using HeroSkillManager = com.kx.sglm.gs.battle.skill.HeroSkillManager;
	using IBattleSkillManager = com.kx.sglm.gs.battle.skill.IBattleSkillManager;
	using MonsterSkillManager = com.kx.sglm.gs.battle.skill.MonsterSkillManager;

	/// <summary>
	/// 武将类型，暂时只有英雄和怪物两种
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public abstract class FighterType : IndexedEnum
	{
		public static readonly FighterType HERO = new FighterTypeAnonymousInnerClassHelper();

		private class FighterTypeAnonymousInnerClassHelper : FighterType
		{
			public FighterTypeAnonymousInnerClassHelper() : base(0)
			{
			}


			public override IBattleSkillManager createSkillManager(BattleFighter fighter)
			{
				return new HeroSkillManager(fighter);
			}
		}
		public static readonly FighterType MONSTER = new FighterTypeAnonymousInnerClassHelper2();

		private class FighterTypeAnonymousInnerClassHelper2 : FighterType
		{
			public FighterTypeAnonymousInnerClassHelper2() : base(1)
			{
			}


			public override IBattleSkillManager createSkillManager(BattleFighter fighter)
			{
				return new MonsterSkillManager(fighter);
			}
		}



		private FighterType(int index)
		{
			this.index = index;
		}

		public virtual bool Hero
		{
			get
			{
				return this == HERO;
			}
		}

		public abstract IBattleSkillManager createSkillManager(BattleFighter fighter);

		protected internal int index;

		public virtual int Index
		{
			get
			{
				return index;
			}
		}

	}
}