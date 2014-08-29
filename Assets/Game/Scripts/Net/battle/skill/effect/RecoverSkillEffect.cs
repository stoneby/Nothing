using System;

namespace com.kx.sglm.gs.battle.share.skill.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;
	using BattleLogicHelper = com.kx.sglm.gs.battle.share.helper.BattleLogicHelper;
	using SkillDataHolder = com.kx.sglm.gs.battle.share.skill.model.SkillDataHolder;

	public class RecoverSkillEffect : AbstractRecoverEffect
	{

		private float percent;
		private float baseValue;

		protected internal override float recoverOption(BattleFighter attacker, BattleFighter defencer)
		{
			float _totalHp = attacker.getOwnerTeam().TotalHp;
			float _ratio = percent / BattleConstants.BATTLE_RATIO_BASE;
			float _recover = _totalHp * _ratio + baseValue;
			_recover = BattleLogicHelper.calcAttackerState(attacker, _recover);
			attacker.getOwnerTeam().changeHp((int)_recover, attacker);
			return _totalHp * _ratio;
		}

		protected internal override void recoverRecord(BattleFighter attacker, BattleFighter defencer, SkillDataHolder resultData)
		{
			SingleActionRecord _singleRecord = resultData.Record.OrCreateAttack;
			_singleRecord.ResultHp = defencer.getOwnerTeam().CurHp;
		}

		public override void build(params string[] param)
		{
			this.percent = Convert.ToInt32(param[0]);
			if (param.Length > 1)
			{
				this.baseValue = Convert.ToInt32(param[1]);
			}
		}

	}

}