using System;

namespace com.kx.sglm.gs.battle.share.skill.condition
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;

	public class NPointConnectCondition : AbstractSkillCondition
	{

		private int pointCount;

		public override bool canOptionSkill(BattleFighter attacker)
		{
			if (!attacker.Hero)
			{
				return false;
			}
			BattleTeam _team = attacker.getOwnerTeam();
			int _curFighterCount = _team.CurTeamShotFighterCount;
			return _curFighterCount > pointCount;
		}

		public override void build(params string[] param)
		{
			pointCount = Convert.ToInt32(param[0]);
		}

	}

}