using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.buff
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleFighterState = com.kx.sglm.gs.battle.share.actor.impl.BattleFighterState;
	using BattleRoundCountRecord = com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord;
	using InnerBattleEvent = com.kx.sglm.gs.battle.share.@event.InnerBattleEvent;

	/// <summary>
	/// 战斗BUFF实例
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleFighterBuff
	{

		private IBuffAction buffAction;

		private int showLeftRound;

		private BattleFighter owner;

		private LinkedList<BuffStackInfo> stackingList;

		public BattleFighterBuff(BattleFighter owner, IBuffAction buffAction)
		{
			this.buffAction = buffAction;
			this.owner = owner;
			this.stackingList = new LinkedList<BuffStackInfo>();
		}

		private void addStackingRound()
		{
			BuffStackInfo _info = new BuffStackInfo();
			_info.init(buffAction.CDRound);
			stackingList.AddLast(_info);
		}

		private void refreshShowLeftRound()
		{
			BuffStackInfo _lastInfo = LastBuffStack;
			if (_lastInfo == null)
			{
				// TODO: loggers.error
				return;
			}
			this.showLeftRound = _lastInfo.LeftRound;

		}

		private BuffStackInfo LastBuffStack
		{
			get
			{
				return stackingList.Count == 0 ? null : stackingList.Last.Value;
			}
		}

		public virtual bool canStacking()
		{
			return buffAction.MaxStackingCount > stackingList.Count;
		}

		public virtual void activeBuff()
		{
			foreach (BuffStackInfo _info in stackingList)
			{
				if (_info.needActive())
				{
					_info.changeToNextState();
					buffAction.onActive(Owner);
				}
			}
		}

		public virtual void effectBuff()
		{
			foreach (BuffStackInfo _info in stackingList)
			{
				if (_info.Active)
				{
					buffAction.onEffect(Owner);
				}
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
			foreach (BuffStackInfo _info in stackingList)
			{
				if (_info.Active)
				{
					buffAction.onEvent(@event, Owner);
				}
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
				return showLeftRound;
			}
		}

		public virtual void countDown(BattleRoundCountRecord record)
		{
			if (PermanentBuff)
			{
				return;
			}
			foreach (BuffStackInfo _info in stackingList)
			{
				_info.countDown();
			}
			while (stackingList.Count > 0 && stackingList.First.Value.Dying)
			{
				stackingList.RemoveFirst();
			}
			refreshShowLeftRound();
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

		public virtual void actionBuff(InnerBattleEvent @event)
		{

		}

		// TODO: result record
		public virtual void stackingBuff()
		{
			if (!canStacking())
			{
				removeFirstStack();
			}
			stackingNewBuff();
		}

		protected internal virtual void stackingNewBuff()
		{
			addStackingRound();
			refreshShowLeftRound();
		}

		protected internal virtual void removeFirstStack()
		{
			this.stackingList.RemoveFirst();
		}

		public virtual void resetBuff()
		{
			this.stackingList.Clear();
			stackingBuff();
		}

		// TODO: 删除将死的BUFF

		public virtual bool Dying
		{
			get
			{
				return this.stackingList.Count == 0;
			}
		}
	}

}