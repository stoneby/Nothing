using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.buff
{


	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleFighterState = com.kx.sglm.gs.battle.share.actor.impl.BattleFighterState;
	using BasePropEffectBuff = com.kx.sglm.gs.battle.share.buff.effect.BasePropEffectBuff;
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

		private bool permanent;

		/// <summary>
		/// buff叠加层数列表 </summary>
		private LinkedList<BuffStackInfo> stackingList;

		private Dictionary<int, int> paramMap;

		public BattleFighterBuff(BattleFighter owner, IBuffAction buffAction)
		{
			this.buffAction = buffAction;
			this.owner = owner;
			this.stackingList = new LinkedList<BuffStackInfo>();
			this.paramMap = new Dictionary<int, int>();
			this.permanent = buffAction.CDRound <= 0;
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

		public virtual void onTeamBeforeAttack()
		{
			foreach (BuffStackInfo _info in stackingList)
			{
				if (_info.Active)
				{
					buffAction.onTeamBeforeAttack(Owner);
				}
			}
		}

		public virtual BattleFighterState FighterState
		{
			get
			{
				//如果needShow为false，说明这个buff不用被显示，也说明这个buff没有特殊状态
				if (!needShow())
				{
					return null;
				}
				BattleFighterState _state = new BattleFighterState(buffAction.Id, buffAction.StateEnum, buffAction.BuffShowId, LeftRound);
				_state.ParamMap = paramMap;
				return _state;
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


		public virtual void effectProp()
		{
			if (!PropEffectBuff)
			{
				return;
			}
			BasePropEffectBuff _propEffectBuff = (BasePropEffectBuff)BuffAction;
			foreach (BuffStackInfo _info in stackingList)
			{
				if (_info.Active)
				{
					_propEffectBuff.effectProp(Owner);
				}
			}
		}

		protected internal virtual bool PropEffectBuff
		{
			get
			{
				return BuffAction is BasePropEffectBuff;
			}
		}

		protected internal virtual bool PermanentBuff
		{
			get
			{
				return permanent;
			}
		}

		public virtual bool Permanent
		{
			set
			{
				this.permanent = value;
			}
		}

		protected internal virtual bool needShow()
		{
			return buffAction.needShow(this);
		}

		public virtual int ExtraRound
		{
			set
			{
				stackingList.First.Value.LeftRound = value;
			}
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


		public virtual int getParam(int key, int defaultValue)
		{
			int _param = defaultValue;
			if (paramMap.ContainsKey(key))
			{
				_param = paramMap[key];
			}
			return _param;
		}

		public virtual void setParam(int key, int value)
		{
			paramMap[key] = value;
		}

		public virtual bool Active
		{
			get
			{
				bool _active = true;
				foreach (BuffStackInfo _info in stackingList)
				{
					if (!_info.Active)
					{
						_active = false;
					}
				}
				return _active;
			}
		}


		// TODO: 删除将死的BUFF

		public virtual bool Dying
		{
			get
			{
				return this.stackingList.Count == 0;
			}
		}

		public virtual void printInfo()
		{
			Logger.Log(string.Format("buffId = {0:D}, leftRound = {1:D}", buffAction.Id, stackingList.First.Value.LeftRound));
		}
	}

}