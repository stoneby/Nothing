using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.actor.impl
{


	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using BattleRecordConstants = com.kx.sglm.gs.battle.share.data.record.BattleRecordConstants;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;

	public class FighterStateManager : IFighterOwner
	{

		private BattleFighter owner;

		/// <summary>
		/// 可攻击/可使用技能等 </summary>
		protected internal int fighterStateFlag;

		protected internal Dictionary<int, BattleFighterState> stateMap;

		public FighterStateManager(BattleFighter owner)
		{
			this.stateMap = new Dictionary<int, BattleFighterState>();
		}


		public virtual bool canStateAttack()
		{
			return MathUtils.optionAndFlag(fighterStateFlag, BattleConstants.ATTACK_DIS_FLAG) == 0;
		}


		public virtual void updateStateRecord(SingleActionRecord record)
		{
			record.clearState();
			record.addProp(BattleRecordConstants.BATTLE_FIGHTER_STATE_FLAG, FighterStateFlag);
			foreach (BattleFighterState _state in stateMap.Values)
			{
				record.addState(_state.BuffId, _state.ShowId, _state.Index, _state.Round);
			}
		}

		public virtual void clearState()
		{
			this.fighterStateFlag = 0;
			this.stateMap.Clear();
		}

		public virtual void addState(BattleFighterState state)
		{
			this.stateMap[state.BuffId] = state;
			this.fighterStateFlag |= state.State.StateFlag;
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