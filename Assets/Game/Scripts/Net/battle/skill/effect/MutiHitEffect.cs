namespace com.kx.sglm.gs.battle.share.skill.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleRecordConstants = com.kx.sglm.gs.battle.share.data.record.BattleRecordConstants;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;

	public class MutiHitEffect : HeroNormalAttackEffect
	{




	//	@Override
		public override float calcDamage(float attack, BattleFighter attacker, BattleFighter defencer, SingleActionRecord record)
		{
	//		float _baseDamage = super.calcDamage(attack, attacker, defencer, record);
	//		return _baseDamage * attackValMuti * hitCount;
			return 0;
		}


		public override void build(params int[] param)
		{
			this.attackValMuti = param[0];
			this.hitCount = param[1];
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