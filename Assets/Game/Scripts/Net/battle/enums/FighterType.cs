namespace com.kx.sglm.gs.battle.share.enums
{

	using IndexedEnum = com.kx.sglm.core.constant.IndexedEnum;

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


		}
		public static readonly FighterType MONSTER = new FighterTypeAnonymousInnerClassHelper2();

		private class FighterTypeAnonymousInnerClassHelper2 : FighterType
		{
			public FighterTypeAnonymousInnerClassHelper2() : base(1)
			{
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

	//	abstract public IBattleSkillManager createSkillManager(BattleFighter fighter);

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