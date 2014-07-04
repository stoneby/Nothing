using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.buff
{


	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleFighterState = com.kx.sglm.gs.battle.share.actor.impl.BattleFighterState;
	using BuffPolicyEnum = com.kx.sglm.gs.battle.share.buff.enums.BuffPolicyEnum;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using BattleRoundCountRecord = com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord;
	using InnerBattleEvent = com.kx.sglm.gs.battle.share.@event.InnerBattleEvent;
	using TeamShotStartEvent = com.kx.sglm.gs.battle.share.@event.impl.TeamShotStartEvent;

	/// <summary>
	/// buff manager in battle, all fighters(hero and monster) use this manager
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleBuffManager : IBattleBuffManager
	{

		/// <summary>
		/// all buff and debuff collection, key is buffId </summary>
		private Dictionary<int, BattleFighterBuff> allBuffs;

		/// <summary>
		/// all buff(not debuff) holder, key is buffTypeA </summary>
		private Dictionary<int, BuffTypeHolder> buffHolderMap;

		/// <summary>
		/// all debuff holder, key is buffTypeA </summary>
		private Dictionary<int, BuffTypeHolder> debuffHolderMap;

		/// <summary>
		/// owner of buff manager, it's a final owner </summary>
		private BattleFighter owner;

		public BattleBuffManager(BattleFighter owner)
		{
			this.owner = owner;
			allBuffs = new Dictionary<int, BattleFighterBuff>();
			buffHolderMap = new Dictionary<int, BuffTypeHolder>();
			debuffHolderMap = new Dictionary<int, BuffTypeHolder>();
		}

		public virtual void recalcBuffEffect()
		{
			owner.FighterProp.resetBuffProp();
			effectAllBuff();
		}


		/// <summary>
		/// �ϸ�buff������������Ƿ��������ٲ鿴buff���ӵĲ���
		/// </summary>
		/// <param name="buffId"> </param>
		public virtual void addBuff(int buffId)
		{
			// TODO: add battle report
			IBuffAction _buffAction = getBuffAction(buffId);
			if (_buffAction == null)
			{
				// TODO: add error log
				return;
			}
			if (hasMaxCountBuff(_buffAction))
			{
				// TODO: log max buff
				return;
			}
			BuffPolicyEnum _policyType = calcPolicy(_buffAction);

			_policyType.addBuff(this, _buffAction);
		}

		/// <summary>
		/// is the buff count max, use when need real add buff, not cover or refresh </summary>
		/// <param name="buffAction">
		/// @return </param>
		public virtual bool hasMaxCountBuff(IBuffAction buffAction)
		{
			if (size() >= BattleConstants.BUFF_MAX_SIZE)
			{
				return false;
			}

			int _maxSize = buffAction.Buff ? BattleConstants.BUFF_MAX_SIZE : BattleConstants.DEBUFF_MAX_SIZE;

			return hasMaxBuff(buffAction, _maxSize);
		}

		protected internal virtual bool hasMaxBuff(IBuffAction buffAction, int maxSize)
		{
			return getBuffCount(buffAction) >= maxSize;
		}

		protected internal virtual int getBuffCount(IBuffAction buffAction)
		{
			Dictionary<int, BuffTypeHolder> _holderMap = getHolderMap(buffAction);
			int _buffCount = 0;
			foreach (BuffTypeHolder _holder in _holderMap.Values)
			{
				_buffCount += _holder.size();
			}
			return _buffCount;
		}

		public virtual void putBuff(BattleFighterBuff buff)
		{
			allBuffs[buff.BuffId] = buff;
		}

		public virtual void createBuffHolder(IBuffAction buffAction)
		{
			BuffTypeHolder _holder = new BuffTypeHolder(buffAction.TypeB);
			putHolderMap(buffAction, _holder);
		}

		public virtual BattleFighterBuff putToBuffHolder(IBuffAction buffAction)
		{
			BuffTypeHolder _holder = getBuffTypeHolder(buffAction);
			return _holder.putBuff(Owner, buffAction);
		}

		public virtual void putHolderMap(IBuffAction buffAction, BuffTypeHolder typeHolder)
		{
			getHolderMap(buffAction)[buffAction.TypeA] = typeHolder;
		}

		protected internal virtual Dictionary<int, BuffTypeHolder> getHolderMap(IBuffAction buffAction)
		{
			return buffAction.Buff ? buffHolderMap : debuffHolderMap;
		}

		public virtual BuffTypeHolder getBuffTypeHolder(IBuffAction buffAction)
		{
			BuffTypeHolder _holder = null;
			//if not contain type, there will be a null point exception in c# code
			if (containTypeAInMap(buffAction))
			{
				_holder = getHolderMap(buffAction)[buffAction.TypeA];
			}
			return _holder;
		}

		protected internal virtual bool containTypeAInMap(IBuffAction buffAction)
		{
			return getHolderMap(buffAction).ContainsKey(buffAction.TypeA);
		}

		protected internal virtual bool containTypeA(IBuffAction buffAction)
		{
			bool _containTypeA = false;
			bool _hasTypeAHolder = containTypeAInMap(buffAction);
			if (_hasTypeAHolder)
			{
				_containTypeA = !getBuffTypeHolder(buffAction).Empty;
			}
			return _containTypeA;
		}

		protected internal virtual bool hasSameBuff(IBuffAction buffAction)
		{
			bool _contain = true;
			bool _typeA = containTypeA(buffAction);
			_contain = _typeA;
			if (_contain)
			{
				_contain = getBuffTypeHolder(buffAction).hasSameBuff(buffAction);
			}
			return _contain;
		}

		protected internal virtual void removeFromAllBuff(int buffId)
		{
			this.allBuffs.Remove(buffId);
		}

		public virtual bool containBuffId(IBuffAction buffAction)
		{
			return containBuffId(buffAction.Id);
		}

		public virtual bool containBuffId(int buffId)
		{
			return allBuffs.ContainsKey(buffId);
		}

		public virtual BattleFighterBuff getBattleBuff(IBuffAction buffAction)
		{
			return get(buffAction.Id);
		}

		protected internal virtual BattleFighterBuff get(int buffId)
		{
			return allBuffs[buffId];
		}



		public virtual void beforeBattleStart(BattleRoundCountRecord roundRecord)
		{


		}


		/// <summary>
		/// change newborn buff state to active
		/// </summary>
		public virtual void activeAllBuff(int buffFlag)
		{
			if (hasFlag(buffFlag, BattleConstants.BUFF_FLAG))
			{
				activeBuffHolderMap(buffHolderMap);
			}
			if (hasFlag(buffFlag, BattleConstants.DEBUFF_FALG))
			{
				activeBuffHolderMap(debuffHolderMap);
			}
		}

		public virtual void activeBuffHolderMap(Dictionary<int, BuffTypeHolder> holderMap)
		{
			foreach (BuffTypeHolder _holder in holderMap.Values)
			{
				_holder.activeBuff();
			}
		}

		/// <summary>
		/// add buff effect to fighter, every team shot effect once
		/// </summary>
		public virtual void effectAllBuff()
		{
			effectBuff(buffHolderMap);
			effectBuff(debuffHolderMap);
		}


		public virtual void countDownRound(BattleRoundCountRecord roundRecord)
		{
			countDownRound(buffHolderMap, roundRecord);
			countDownRound(debuffHolderMap, roundRecord);
			clearDyingBuff();
		}


		protected internal virtual void clearDyingBuff()
		{
			IEnumerator<KeyValuePair<int, BattleFighterBuff>> _iterator = allBuffs.GetEnumerator();
			List<BattleFighterBuff> _dyingBuffs = new List<BattleFighterBuff>();
			while (_iterator.MoveNext())
			{
				BattleFighterBuff _buff = _iterator.Current.Value;
				if (_buff.Dying)
				{
					_dyingBuffs.Add(_buff);
				}
			}
			foreach (BattleFighterBuff _buff in _dyingBuffs)
			{
				removeSingleBuff(_buff);
			}
		}


		/// <summary>
		/// remove single buff from all collection and reset buff effect
		/// </summary>
		/// <param name="buffId"> </param>
		public virtual void removeSingleBuff(BattleFighterBuff buff)
		{

			int buffId = buff.BuffId;

			removeFromAllBuff(buffId);

			IBuffAction _buffAction = buff.BuffAction;

			BuffTypeHolder _holder = getBuffTypeHolder(_buffAction);

			_holder.removeBuff(buffId);

			_buffAction.onRemove(Owner);
		}



		protected internal virtual void countDownRound(Dictionary<int, BuffTypeHolder> holderMap, BattleRoundCountRecord roundRecord)
		{
			foreach (BuffTypeHolder _holder in holderMap.Values)
			{
				_holder.countDownRound(roundRecord);
			}
		}

		protected internal virtual void effectBuff(Dictionary<int, BuffTypeHolder> holderMap)
		{
			foreach (BuffTypeHolder _holder in holderMap.Values)
			{
				_holder.effectBuff();
			}
		}

		protected internal virtual void actionOnEvent(InnerBattleEvent @event)
		{
			foreach (BattleFighterBuff _buff in allBuffs.Values)
			{
				if (_buff.Active)
				{
					_buff.BuffAction.onEvent(@event, Owner);
				}
			}
		}

		public virtual Battle Battle
		{
			get
			{
				return Owner.Battle;
			}
		}

		public virtual BattleFighter Owner
		{
			get
			{
				return owner;
			}
		}

		protected internal virtual IBuffAction getBuffAction(int buffId)
		{
			return BattleActionService.Service.getBuffAction(buffId);
		}

		protected internal virtual BuffPolicyEnum calcPolicy(IBuffAction buffAction)
		{
			BuffPolicyEnum _policyType = null;
			if (containBuffId(buffAction))
			{
				_policyType = BuffPolicyEnum.UNCHANGED;
			}
			else
			{
				if (containTypeA(buffAction))
				{
					_policyType = getBuffTypeHolder(buffAction).calcPolicy(buffAction);
				}
				else
				{
					_policyType = BuffPolicyEnum.COEXIST;
				}
			}
			return _policyType;
		}

		public virtual void onAttack(BattleFightRecord fightRecord)
		{
			//TODO :add logic

		}

		public virtual int size()
		{
			return allBuffs.Count;
		}


		public virtual void onRoundFinish(BattleRoundCountRecord roundRecord)
		{
			countDownRound(roundRecord);
		}

		public virtual void onTeamShotStart(TeamShotStartEvent @event)
		{
			recalcBuffEffect();
		}

		public virtual List<BattleFighterState> AllFighterState
		{
			get
			{
				List<BattleFighterState> _stateList = new List<BattleFighterState>();
				foreach (BattleFighterBuff _buff in allBuffs.Values)
				{
					BattleFighterState _state = _buff.FighterState;
					if (_state != null)
					{
						_stateList.Add(_state);
					}
				}
				return _stateList;
			}
		}


		protected internal virtual bool hasFlag(int buffFlag, int baseFlag)
		{
			return (buffFlag & baseFlag) > 0;
		}

		public virtual void clearAllBuff()
		{
			allBuffs.Clear();
			buffHolderMap.Clear();
		}
	}

}