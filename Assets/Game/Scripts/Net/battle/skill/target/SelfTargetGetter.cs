using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill.target
{


	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;

	public class SelfTargetGetter : AbstractHeroTeamGetter
	{

		public override List<BattleFighter> getTarget(BattleFighter attacker, BattleTeam targetTeam)
		{
			List<BattleFighter> _curFighterList = new List<BattleFighter>();
			_curFighterList.Add(attacker);
			return _curFighterList;

		}

		public override void build(params string[] param)
		{

		}

	}

}