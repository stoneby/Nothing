using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill.target
{


	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;

	public class DefaultTargetGetter : AbstractHeroTeamGetter
	{

		public override List<BattleFighter> getTarget(BattleFighter attacker, BattleTeam targetTeam)
		{
			List<BattleFighter> _curFighterList = new List<BattleFighter>();
			int _index = attacker.getOwnerTeam().getIntProp(BattleKeyConstants.BATTLE_KEY_HERO_TEAM_TARGET);
			BattleFighter _fighter = targetTeam.getActor(_index);
			_curFighterList.Add(_fighter);
			return _curFighterList;
		}

		public override void build(params string[] param)
		{
			// TODO Auto-generated method stub

		}

	}

}