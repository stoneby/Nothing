namespace com.kx.sglm.gs.battle.share.skill.condition
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;

	public class FighterLeaderCondition : AbstractSkillCondition
	{

		public override bool canOptionSkill(BattleFighter attacker)
		{
			return attacker.Leader;
		}

		public override void build(params string[] param)
		{
			// TODO Auto-generated method stub

		}

	}

}