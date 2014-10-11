using System;

namespace com.kx.sglm.gs.battle.share.skill.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleRecordConstants = com.kx.sglm.gs.battle.share.data.record.BattleRecordConstants;
	using SkillDataHolder = com.kx.sglm.gs.battle.share.skill.model.SkillDataHolder;

	public class MutiHitEffect : AbstractAttackEffect
	{



		public override void build(params string[] param)
		{
			this.attackValMuti = Convert.ToInt32(param[0]);
			this.hitCount = Convert.ToInt32(param[1]);
		}

		protected internal override int AttackType
		{
			get
			{
				return BattleRecordConstants.SINGLE_ACTION_TYPE_SP_ATTACK;
			}
		}
	}

}