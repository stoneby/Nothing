namespace com.kx.sglm.gs.battle.share.skill
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using IBattlePartInfo = com.kx.sglm.gs.battle.share.enums.IBattlePartInfo;

	public abstract class ISkillCondition : IBattlePartInfo
	{


		public abstract bool canOptionSkill(BattleFighter attacker);


	}

}