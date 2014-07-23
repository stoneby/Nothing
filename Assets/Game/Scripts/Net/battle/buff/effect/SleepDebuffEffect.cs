namespace com.kx.sglm.gs.battle.share.buff.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using FighterStateEnum = com.kx.sglm.gs.battle.share.enums.FighterStateEnum;

	public class SleepDebuffEffect : AbstractBuffEffect
	{

		public override FighterStateEnum StateEnum
		{
			get
			{
				return FighterStateEnum.ATTACK_ZERO_FLAG;
			}
		}

		public override void onActive(BattleFighter fighter)
		{

		}

		public override void onRemove(BattleFighter fighter)
		{

		}

		public override void onEffect(BattleFighter fighter)
		{

		}

		public override void onAttack(BattleFighter attacker)
		{

		}

		public override void onDefence(BattleFighter attacker, BattleFighter owner)
		{
			BattleFighterBuff _buff = owner.BuffManager.getBattleBuff(this);
			if (_buff == null)
			{
				return;
			}
			owner.BuffManager.removeSingleBuff(_buff);
		}

		public override bool needShow(BattleFighterBuff buffInst)
		{
			return BuffShowId > 0;
		}

		public override void build(params string[] param)
		{

		}

	}

}