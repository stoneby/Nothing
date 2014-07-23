using System;

namespace com.kx.sglm.gs.battle.share.skill.condition
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;

	public class NIndexConnectCondition : AbstractSkillCondition
	{

		private int nIndex;

		public override bool canOptionSkill(BattleFighter attacker)
		{
			if (!attacker.Hero)
			{
				return false;
			}
			BattleTeam _fighterTeam = attacker.getOwnerTeam();
			int _realIndex = _fighterTeam.getCurTeamShotFighterIndex(attacker);

			return nIndex == _realIndex;
		}

		public override void build(params string[] param)
		{
			this.nIndex = Convert.ToInt32(param[0]);
		}

	}

}