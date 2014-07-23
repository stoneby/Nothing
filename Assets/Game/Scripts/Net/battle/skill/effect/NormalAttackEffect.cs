namespace com.kx.sglm.gs.battle.share.skill.effect
{

	using BattleRecordConstants = com.kx.sglm.gs.battle.share.data.record.BattleRecordConstants;

	public class NormalAttackEffect : AbstractAttackEffect
	{



		public override void build(params string[] param)
		{

		}

		protected internal override int AttackType
		{
			get
			{
				return BattleRecordConstants.SINGLE_ACTION_TYPE_ATTACK;
			}
		}

	}

}