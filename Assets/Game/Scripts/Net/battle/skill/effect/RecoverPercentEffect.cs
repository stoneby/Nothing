namespace com.kx.sglm.gs.battle.share.skill.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;
	using SkillDataHolder = com.kx.sglm.gs.battle.share.skill.model.SkillDataHolder;

	public class RecoverPercentEffect : AbstractRecoverEffect
	{

		private float percent;

		protected internal override float recoverOption(BattleFighter attacker, BattleFighter defencer)
		{
			float _totalHp = attacker.getOwnerTeam().TotalHp;
			float _ratio = percent / BattleConstants.BATTLE_RATIO_BASE;
			float _recover = _totalHp * _ratio;
			attacker.getOwnerTeam().changeHp((int)_recover, attacker);
			return _totalHp * _ratio;
		}

		protected internal override void recoverRecord(BattleFighter attacker, BattleFighter defencer, SkillDataHolder resultData)
		{
			SingleActionRecord _singleRecord = resultData.Record.OrCreateAttack;
			_singleRecord.ResultHp = defencer.getOwnerTeam().CurHp;
		}

		public override void build(params int[] param)
		{
			this.percent = param[0];
		}

	}

}