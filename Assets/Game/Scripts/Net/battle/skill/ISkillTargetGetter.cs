using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;
	using ISkillPartInfo = com.kx.sglm.gs.battle.share.skill.creater.ISkillPartInfo;

	public abstract class ISkillTargetGetter : ISkillPartInfo
	{

		public abstract List<BattleFighter> getTarget(BattleFighter attacker, BattleTeam targetTeam);

	}

}