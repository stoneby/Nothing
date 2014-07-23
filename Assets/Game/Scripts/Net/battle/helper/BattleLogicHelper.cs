using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.helper
{

	using BattleArmy = com.kx.sglm.gs.battle.share.actor.impl.BattleArmy;
	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleFighterState = com.kx.sglm.gs.battle.share.actor.impl.BattleFighterState;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;

	public class BattleLogicHelper
	{


		public static void costBaseHp(int costHp, BattleFighter defencer, SingleActionRecord record)
		{
			defencer.getOwnerTeam().costFighterHp(costHp, defencer);
			record.ResultHp = defencer.FighterCurHp;
		}

		public static void refreshState(BattleArmy battelArmy)
		{
			foreach (BattleTeam _team in battelArmy.ActorList)
			{
				refreshState(_team);
				BattleRecordHelper.recordTeamBuffState(_team);
			}
		}

		public static void refreshState(BattleTeam battleTeam)
		{
			foreach (BattleFighter _fighter in battleTeam.ActorList)
			{
				refreshState(_fighter);
			}
		}

		public static void refreshState(BattleFighter fighter)
		{
			fighter.refreshState();
			List<BattleFighterState> _stateList = fighter.BuffManager.AllFighterState;
			foreach (BattleFighterState _state in _stateList)
			{
				fighter.addState(_state);
			}
		}

		public static int calcBattleFighterTotalHpPercent(int percent, BattleFighter fighter)
		{
			float _totalHp = fighter.BattleTotalHp;
			float _percent = percent;
			float _costHp = _totalHp * (_percent / BattleConstants.BATTLE_RATIO_BASE);
			return (int) _costHp;
		}

		public static int calcBattleFighterCurHpPercent(int percent, BattleFighter fighter)
		{
			float _totalHp = fighter.FighterCurHp;
			float _percent = percent;
			float _costHp = _totalHp * (_percent / BattleConstants.BATTLE_RATIO_BASE);
			return (int) _costHp;
		}

	}

}