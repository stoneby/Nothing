namespace com.kx.sglm.gs.battle.share.buff
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleFighterState = com.kx.sglm.gs.battle.share.actor.impl.BattleFighterState;
	using BuffStateEnum = com.kx.sglm.gs.battle.share.buff.enums.BuffStateEnum;
	using BattleRoundCountRecord = com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord;
	using InnerBattleEvent = com.kx.sglm.gs.battle.share.@event.InnerBattleEvent;

	/// <summary>
	/// Õ½¶·BUFFÊµÀý
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleFighterBuff
	{

		private IBuffAction buffAction;

		private int leftRound;

		private BuffStateEnum state;

		private BattleFighter owner;

		public BattleFighterBuff(BattleFighter owner, IBuffAction buffAction)
		{
			this.state = BuffStateEnum.NIL;
			this.buffAction = buffAction;
			this.owner = owner;
			init();
		}

		private void init()
		{
			state = BuffStateEnum.NEW_BORN;
			leftRound = buffAction.CDRound;
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


		public virtual void activeBuff()
		{
			if (needActive())
			{
				state = state.nexState();
				buffAction.onActive(Owner);
			}
		}

		public virtual void effectBuff()
		{
			if (Active)
			{
				buffAction.onEffect(Owner);
			}
		}

		public virtual BattleFighterState FighterState
		{
			get
			{
				if (!needShow())
				{
					return null;
				}
				BattleFighterState _state = new BattleFighterState(buffAction.Id, buffAction.StateEnum, buffAction.BuffShowId, LeftRound);
				return _state;
			}
		}

		public virtual void onBuffEvent(InnerBattleEvent @event)
		{
			if (Active)
			{
				buffAction.onEvent(@event, Owner);
			}
		}

		public virtual IBuffAction BuffAction
		{
			get
			{
				return buffAction;
			}
		}

		public virtual int LeftRound
		{
			get
			{
				return leftRound;
			}
		}

		public virtual BuffStateEnum State
		{
			get
			{
				return state;
			}
		}

		public virtual void changeToNextState()
		{
			state = state.nexState();
		}

		public virtual void countDown(BattleRoundCountRecord record)
		{
			if (PermanentBuff)
			{
				return;
			}
			leftRound--;
			if (leftRound == 0)
			{
				changeToNextState();
			}
		}

		protected internal virtual bool PermanentBuff
		{
			get
			{
				return buffAction.CDRound <= 0;
			}
		}

		protected internal virtual bool needShow()
		{
			return buffAction.BuffShowId > 0;
		}

		public virtual BattleFighter Owner
		{
			get
			{
				return owner;
			}
		}

		public virtual int BuffId
		{
			get
			{
				return BuffAction.Id;
			}
		}

		public virtual bool Buff
		{
			get
			{
				return BuffAction.Buff;
			}
		}

		// TODO: result record
		public virtual void resetRound()
		{
			this.leftRound = BuffAction.CDRound;
		}

	}

}