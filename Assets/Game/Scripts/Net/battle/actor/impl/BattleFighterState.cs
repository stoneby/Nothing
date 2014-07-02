namespace com.kx.sglm.gs.battle.share.actor.impl
{

	using FighterStateEnum = com.kx.sglm.gs.battle.share.enums.FighterStateEnum;

	public class BattleFighterState
	{

		private int buffId;
		private FighterStateEnum state;
		private int showId;
		private int round;

		public BattleFighterState(int buffId, FighterStateEnum state, int showId, int round)
		{
			this.state = state;
			this.buffId = buffId;
			this.round = round;
		}

		public virtual int BuffId
		{
			get
			{
				return buffId;
			}
		}

		public virtual int Round
		{
			get
			{
				return round;
			}
		}

		public virtual FighterStateEnum State
		{
			get
			{
				return state;
			}
		}

		public virtual int ShowId
		{
			get
			{
				return showId;
			}
		}

		public virtual sbyte Index
		{
			get
			{
				return (sbyte)State.Index;
			}
		}

	}

}