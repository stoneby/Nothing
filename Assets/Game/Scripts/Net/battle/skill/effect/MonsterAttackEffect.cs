namespace com.kx.sglm.gs.battle.share.skill.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleRecordConstants = com.kx.sglm.gs.battle.share.data.record.BattleRecordConstants;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;
	using BattleLogicHelper = com.kx.sglm.gs.battle.share.helper.BattleLogicHelper;

	/// <summary>
	/// ¹ÖÎï¹¥»÷
	/// @author liyuan2
	/// 
	/// </summary>
	public class MonsterAttackEffect : AbstractAttackEffect
	{

	//	@Override
	//	public float calcDamage(float attack, BattleFighter attacker, BattleFighter defencer, SingleActionRecord record) {
	//		float _damageFree = defencer.getDamageFree();
	//		return calcDamagefree(_damageFree, attack);
	//	}

		public override int getIndexAttackRatio(BattleFighter attacker)
		{
			return BattleConstants.BATTLE_HERO_INDEX_RATIO[0];
		}

		public override void costHp(int costHp, BattleFighter defencer, SingleActionRecord record)
		{
			BattleLogicHelper.costHeroHp(costHp, defencer, record);
		}

		public override void build(params string[] param)
		{
			// TODO Auto-generated method stub

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