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



		/// <summary>
		/// 上个buff，先算出个数是否已满，再查看buff增加的策略
		/// </summary>
		/// <param name="buffId"> </param>
		public virtual void addBuff(BuffInfo buffInfo)
		{
			// TODO: add battle report
			IBuffAction _buffAction = getBuffAction(buffInfo.BuffId);

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
			buffInfo.BuffAction = _buffAction;

			BuffPolicyEnum _policyType = calcPolicy(_buffAction);

			_policyType.addBuff(this, buffInfo);
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
			BuffTypeHolder _holder = new BuffTypeHolder(buffAction.TypeA);
			putHolderMap(buffAction, _holder);
		}

		public virtual BattleFighterBuff putToBuffHolder(BuffInfo buffInfo)
		{
			BuffTypeHolder _holder = getBuffTypeHolder(buffInfo.BuffAction);
			return _holder.putBuff(Owner, buffInfo);
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
			return allBuffs.ContainsKey(buffId) ? allBuffs[buffId] : null;
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

		public virtual HashSet<int> AllBuffIds
		{
			get
			{
				return getBuffIdsInHolder(buffHolderMap);
			}
		}

		public virtual HashSet<int> AllDebuffIds
		{
			get
			{
				return getBuffIdsInHolder(debuffHolderMap);
			}
		}

		protected internal virtual HashSet<int> getBuffIdsInHolder(Dictionary<int, BuffTypeHolder> holderMap)
		{
			HashSet<int> _buffIds = new HashSet<int>();
			foreach (BuffTypeHolder _holder in holderMap.Values)
			{
				foreach (int _buffId in _holder.BuffIds)
				{
					_buffIds.Add(_buffId);
				}
			}
			return _buffIds;
		}

		/// <summary>
		/// add buff effect to fighter, every team shot effect once
		/// </summary>
		public virtual void onTeamBeforeAttack()
		{
			onTeamBeforeAttack(buffHolderMap);
			onTeamBeforeAttack(debuffHolderMap);
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

			_holder.removeBuff(_buffAction);

			_buffAction.onRemove(Owner);
		}



		protected internal virtual void countDownRound(Dictionary<int, BuffTypeHolder> holderMap, BattleRoundCountRecord roundRecord)
		{
			foreach (BuffTypeHolder _holder in holderMap.Values)
			{
				_holder.countDownRound(roundRecord);
			}
		}

		protected internal virtual void onTeamBeforeAttack(Dictionary<int, BuffTypeHolder> holderMap)
		{
			foreach (BuffTypeHolder _holder in holderMap.Values)
			{
				_holder.onTeamBeforeAttack();
			}
		}

		protected internal virtual void actionOnEvent(InnerBattleEvent @event)
		{
			foreach (BattleFighterBuff _buff in allBuffs.Values)
			{
				_buff.actionBuff(@event);
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
				_policyType = BuffPolicyEnum.STACKING;
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
			HashSet<int> _buffIdSet = new HashSet<int>(allBuffs.Keys);
			foreach (int _buffId in _buffIdSet)
			{
				BattleFighterBuff _buff = allBuffs[_buffId];
				_buff.BuffAction.onAttack(Owner);
			}
		}

		public virtual void onDefence(BattleFighter attacker, BattleFightRecord fightRecord)
		{
			HashSet<int> _buffIdSet = new HashSet<int>(allBuffs.Keys);
			foreach (int _buffId in _buffIdSet)
			{
				BattleFighterBuff _buff = allBuffs[_buffId];
				_buff.BuffAction.onDefence(attacker, Owner);
			}
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
			activeAllBuff(BattleConstants.BUFF_ALL_FALG);
			recalcPropEffectBuffs();
			onTeamBeforeAttack();
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
			debuffHolderMap.Clear();
		}

		public virtual void onSceneStop()
		{
			printAllBuffs();
			removeOnSceneStop(buffHolderMap);
			removeOnSceneStop(debuffHolderMap);
		}

		protected internal virtual void removeOnSceneStop(Dictionary<int, BuffTypeHolder> toRemoveHolder)
		{
			HashSet<int> _allIds = getBuffIdsInHolder(toRemoveHolder);
			foreach (int _id in _allIds)
			{
				BattleFighterBuff _buff = get(_id);
				if (_buff.BuffAction.SceneClearBuff)
				{
					removeSingleBuff(_buff);
				}
			}

		}


		/// <summary>
		/// 单纯的重算所有对属性的影响，无视增加时间和位置，只对属性影响的buff生效，这里有待重构因为用了强转
		/// </summary>
		public virtual void recalcPropEffectBuffs()
		{
			Owner.FighterProp.resetBuffProp();
			foreach (BattleFighterBuff _buff in allBuffs.Values)
			{
				_buff.effectProp();
			}
		}




		protected internal virtual void printAllBuffs()
		{
			foreach (BattleFighterBuff _buff in allBuffs.Values)
			{
				_buff.printInfo();
			}
		}

	}

}