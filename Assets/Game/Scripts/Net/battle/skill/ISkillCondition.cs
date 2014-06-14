namespace com.kx.sglm.gs.battle.share.skill
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using ISkillPartInfo = com.kx.sglm.gs.battle.share.skill.creater.ISkillPartInfo;

	public abstract class ISkillCondition : ISkillPartInfo
	{


		public abstract bool canOptionSkill(BattleFighter attacker);


	}

}