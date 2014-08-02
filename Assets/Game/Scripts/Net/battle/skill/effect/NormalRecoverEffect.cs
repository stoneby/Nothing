namespace com.kx.sglm.gs.battle.share.skill.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;
	using BattleLogicHelper = com.kx.sglm.gs.battle.share.helper.BattleLogicHelper;
	using HeroArrLogicHelper = com.kx.sglm.gs.battle.share.helper.HeroArrLogicHelper;
	using SkillDataHolder = com.kx.sglm.gs.battle.share.skill.model.SkillDataHolder;

	public class NormalRecoverEffect : AbstractRecoverEffect
	{

		protected internal override float recoverOption(BattleFighter attacker, BattleFighter defencer)
		{
			float _recover = attacker.Recover;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _curIndex = attacker.getOwnerTeam().getCurFightIndex();
			int _curIndex = attacker.getOwnerTeam().CurFightIndex;
			float _ratio = HeroArrLogicHelper.getAttackRatio(_curIndex);
			_recover *= (_ratio / BattleConstants.BATTLE_RATIO_BASE);
			_recover = BattleLogicHelper.calcAttackerState(attacker, _recover);
			attacker.getOwnerTeam().changeHp((int) _recover, attacker);
			return _recover;
		}

		protected internal override void recoverRecord(BattleFighter attacker, BattleFighter defencer, SkillDataHolder resultData)
		{
			SingleActionRecord _singleRecord = resultData.Record.OrCreateAttack;
			_singleRecord.ResultHp = defencer.getOwnerTeam().CurHp;
		}

		public override void build(params string[] param)
		{
		}

	}

}