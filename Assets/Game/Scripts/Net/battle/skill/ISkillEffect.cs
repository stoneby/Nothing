using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using IBattlePartInfo = com.kx.sglm.gs.battle.share.enums.IBattlePartInfo;
	using SkillDataHolder = com.kx.sglm.gs.battle.share.skill.model.SkillDataHolder;

	public abstract class ISkillEffect : IBattlePartInfo
	{

		public abstract void onAction(BattleFighter attacker, List<BattleFighter> defencerList, SkillDataHolder resultData);

		public abstract bool EnemyEffect {get;}

	}

}