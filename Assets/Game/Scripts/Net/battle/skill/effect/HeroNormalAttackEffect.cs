namespace com.kx.sglm.gs.battle.share.skill.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleRecordConstants = com.kx.sglm.gs.battle.share.data.record.BattleRecordConstants;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;
	using BattleLogicHelper = com.kx.sglm.gs.battle.share.helper.BattleLogicHelper;
	using HeroArrLogicHelper = com.kx.sglm.gs.battle.share.helper.HeroArrLogicHelper;

	public class HeroNormalAttackEffect : AbstractAttackEffect
	{


	//	@Override
		public virtual float calcDamage(float attack, BattleFighter attacker, BattleFighter defencer, SingleActionRecord record)
		{
	//		float _damageFree = defencer.getDamageFree();

	//		float _attackValMuti = this.attackValMuti;


			float _damage = 0;
			recordDamageInfo((int)_damage, record);
			return _damage;
		}





		public override int getIndexAttackRatio(BattleFighter attacker)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _curIndex = attacker.getOwnerTeam().getCurFightIndex();
			int _curIndex = attacker.getOwnerTeam().CurFightIndex;
			return HeroArrLogicHelper.getAttackRatio(_curIndex);
		}

		public override void costHp(int costHp, BattleFighter defencer, SingleActionRecord record)
		{
			BattleLogicHelper.costMonsterHp(costHp, defencer, record);
		}

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