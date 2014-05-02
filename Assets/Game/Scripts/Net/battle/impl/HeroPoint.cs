namespace com.kx.sglm.gs.battle.actor.impl
{

	using HeroColor = com.kx.sglm.gs.battle.enums.HeroColor;

	/// <summary>
	/// 英雄站点
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class HeroPoint
	{

		/// <summary>
		/// 通用的空点，无颜色无武将
		/// </summary>
		public static readonly HeroPoint emptyPoint = new HeroPoint(true);

		protected internal BattleFighter fighter;
		protected internal HeroColor color;
		protected internal int teamIndex;
		protected internal bool inBattle;
		protected internal bool empty;

		private HeroPoint(bool empty)
		{
			this.empty = empty;
		}

		public HeroPoint(BattleFighter fighter, HeroColor color) : this(false)
		{
			this.fighter = fighter;
			this.color = color;
			this.inBattle = false;
			this.teamIndex = BattleConstants.BATTLE_FIGHTER_NON_INDEX;
		}

		public virtual bool Empty
		{
			get
			{
				return empty;
			}
		}

		public virtual int TeamIndex
		{
			set
			{
				this.teamIndex = value;
			}
			get
			{
				return teamIndex;
			}
		}

		public virtual void exitTeam()
		{
			this.color = HeroColor.NIL;
			this.inBattle = false;
			this.teamIndex = BattleConstants.BATTLE_FIGHTER_NON_INDEX;
		}


		public virtual BattleFighter Fighter
		{
			get
			{
				return fighter;
			}
		}

		public virtual HeroColor Color
		{
			get
			{
				return color;
			}
		}

		public virtual bool InBattle
		{
			get
			{
				return inBattle;
			}
			set
			{
				this.inBattle = value;
			}
		}

	}

}