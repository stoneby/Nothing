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
			this.inBattle = false;
			Color = color;
		}

		public virtual bool Empty
		{
			get
			{
				return empty;
			}
		}

		//TODO: maybe add battle record
		public virtual void updateColor(HeroColor color)
		{
			Color = color;
		}


		public virtual void exitTeam()
		{
			Color = HeroColor.NIL;
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
			set
			{
				this.color = value;
			}
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


		public virtual string toLogString()
		{
			return string.Format("{0:D}", Fighter.Index);
		}
	}

}