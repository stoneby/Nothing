using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;
	using IBattlePartInfo = com.kx.sglm.gs.battle.share.enums.IBattlePartInfo;

	public abstract class ISkillTargetGetter : IBattlePartInfo
	{

		public abstract List<BattleFighter> getTarget(BattleFighter attacker, BattleTeam targetTeam);

	}

}