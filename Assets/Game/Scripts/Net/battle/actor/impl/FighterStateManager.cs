using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.actor.impl
{


	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using BattleRecordConstants = com.kx.sglm.gs.battle.share.data.record.BattleRecordConstants;
	using SingleFighterRecord = com.kx.sglm.gs.battle.share.data.record.SingleFighterRecord;
	using BattleRecordHelper = com.kx.sglm.gs.battle.share.helper.BattleRecordHelper;

	public class FighterStateManager : IFighterOwner
	{

		private BattleFighter owner;

		/// <summary>
		/// 可攻击/可使用技能等 </summary>
		protected internal int fighterStateFlag;

		protected internal Dictionary<int, BattleFighterState> stateMap;

		protected internal Dictionary<int, BattleFighterState> lastStateMap;

		public FighterStateManager(BattleFighter owner)
		{
			this.stateMap = new Dictionary<int, BattleFighterState>();
			this.lastStateMap = new Dictionary<int, BattleFighterState>();
		}


		public virtual bool canStateAttack()
		{
			return MathUtils.optionAndFlag(fighterStateFlag, BattleConstants.ATTACK_DIS_FLAG) == 0;
		}


		public virtual void updateStateRecord(SingleFighterRecord record)
		{
			record.clearState();
			record.addProp(BattleRecordConstants.BATTLE_FIGHTER_STATE_FLAG, FighterStateFlag);
			BattleRecordHelper.updateStateRecord(record, stateMap, false);
			BattleRecordHelper.updateStateRecord(record, lastStateMap, true);
		}



		public virtual void backupState()
		{
			this.lastStateMap = new Dictionary<int, BattleFighterState>(stateMap);
		}

		public virtual void clearState()
		{
			this.fighterStateFlag = 0;
			this.stateMap.Clear();
		}

		public virtual void addState(BattleFighterState state)
		{
			int _buffId = state.BuffId;
			this.stateMap[_buffId] = state;
			this.fighterStateFlag |= state.State.StateFlag;
			if (this.lastStateMap.ContainsKey(_buffId))
			{
				this.lastStateMap.Remove(_buffId);
			}
		}

		public virtual int FighterStateFlag
		{
			get
			{
				return fighterStateFlag;
			}
		}


		public virtual BattleFighter Owner
		{
			get
			{
				return owner;
			}
		}

	}

}