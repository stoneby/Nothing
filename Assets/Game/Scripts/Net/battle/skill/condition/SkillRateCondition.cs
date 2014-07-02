using System;

namespace com.kx.sglm.gs.battle.share.skill.condition
{

	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;

	public class SkillRateCondition : AbstractSkillCondition
	{

		private int rate;

		public override bool canOptionSkill(BattleFighter attacker)
		{
			return MathUtils.randomRate(rate, BattleConstants.BATTLE_RATIO_BASE);
		}

		public override void build(params string[] param)
		{
			this.rate = Convert.ToInt32(param[0]);

		}

	}

}