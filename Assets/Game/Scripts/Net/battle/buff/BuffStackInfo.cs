namespace com.kx.sglm.gs.battle.share.buff
{

	using BuffStateEnum = com.kx.sglm.gs.battle.share.buff.enums.BuffStateEnum;

	public class BuffStackInfo
	{

		private BuffStateEnum state;
		private int leftRound;

		public BuffStackInfo()
		{
			state = BuffStateEnum.NIL;
		}

		protected internal virtual void init(int defaultRound)
		{
			LeftRound = defaultRound;
			resetState();
		}


		protected internal virtual void countDown()
		{
			reduceLeftRound();
			if (leftRound == 0)
			{
				changeToNextState();
			}
		}

		protected internal virtual void reduceLeftRound()
		{
			if (leftRound > 0)
			{
				leftRound--;
			}
		}

		public virtual int LeftRound
		{
			set
			{
				this.leftRound = value;
			}
			get
			{
				return leftRound;
			}
		}


		public virtual void changeToNextState()
		{
			this.state = state.nexState();
		}

		public virtual void resetState()
		{
			this.state = BuffStateEnum.NEW_BORN;
		}

		public virtual bool needActive()
		{
			return state.NeedActive;
		}

		public virtual bool Active
		{
			get
			{
				return state.Active;
			}
		}

		public virtual bool Dying
		{
			get
			{
				return state.Dying;
			}
		}

	}

}