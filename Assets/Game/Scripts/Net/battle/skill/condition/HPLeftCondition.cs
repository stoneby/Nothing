namespace com.kx.sglm.gs.battle.share.skill.condition
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;

	public class HPLeftCondition : AbstractSkillCondition
	{

		private float hpRate;

		public override bool canOptionSkill(BattleFighter attacker)
		{
			// TODO add logic
			return false;
		}

		public override void build(params int[] param)
		{
			this.hpRate = param[0];
		}

	}

}