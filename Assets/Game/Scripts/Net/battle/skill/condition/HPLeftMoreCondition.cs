using System;

namespace com.kx.sglm.gs.battle.share.skill.condition
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;

	public class HPLeftMoreCondition : AbstractSkillCondition
	{

		private int hpRate;

		public override bool canOptionSkill(BattleFighter attacker)
		{
			return hpRate <= attacker.FighterCurHpPercent;
		}

		public override void build(params string[] param)
		{
			this.hpRate = Convert.ToInt32(param[0]);
		}

	}

}