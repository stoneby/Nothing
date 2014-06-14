using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using ISkillPartInfo = com.kx.sglm.gs.battle.share.skill.creater.ISkillPartInfo;
	using SkillDataHolder = com.kx.sglm.gs.battle.share.skill.model.SkillDataHolder;

	public abstract class ISkillEffect : ISkillPartInfo
	{

		public abstract void onAction(BattleFighter attacker, List<BattleFighter> defencerList, SkillDataHolder resultData);

		public abstract bool EnemyEffect {get;}

	}

}