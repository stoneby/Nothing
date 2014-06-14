namespace com.kx.sglm.gs.battle.share.skill
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using ISkillPartInfo = com.kx.sglm.gs.battle.share.skill.creater.ISkillPartInfo;

	public abstract class IAICondition : ISkillPartInfo
	{

		//TODO: addparam
		public abstract bool canOption(BattleFighter attacker);

	}

}