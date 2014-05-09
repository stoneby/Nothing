namespace com.kx.sglm.gs.battle.share.actor.impl
{

	using HeroColor = com.kx.sglm.gs.battle.share.enums.HeroColor;

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
		}

		public virtual bool Empty
		{
			get
			{
				return empty;
			}
		}


		public virtual void exitTeam()
		{
			this.color = HeroColor.NIL;
			this.inBattle = false;
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