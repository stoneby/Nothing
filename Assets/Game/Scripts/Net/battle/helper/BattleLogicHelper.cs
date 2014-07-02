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
			if (defencer.Hero)
			{
				costHeroHp(costHp, defencer, record);
			}
			else
			{
				costMonsterHp(costHp, defencer, record);
			}
		}

		public static void costHeroHp(int costHp, BattleFighter defencer, SingleActionRecord record)
		{
			BattleTeam _heroTeam = defencer.getOwnerTeam();
			_heroTeam.changeHp(-costHp, defencer);
			record.ResultHp = _heroTeam.CurHp;
		}


		public static void costMonsterHp(int costHp, BattleFighter defencer, SingleActionRecord record)
		{
			defencer.changeCurHp(-costHp);
			record.ResultHp = defencer.CurHp;
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
			fighter.clearState();
			List<BattleFighterState> _stateList = fighter.BuffManager.AllFighterState;
			foreach (BattleFighterState _state in _stateList)
			{
				fighter.addState(_state);
			}
		}

	}

}