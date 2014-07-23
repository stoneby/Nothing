namespace com.kx.sglm.gs.battle.share.buff.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using FighterStateEnum = com.kx.sglm.gs.battle.share.enums.FighterStateEnum;

	public class SpMaxBuffEffect : StateBuffEffect
	{


		public override void onAttack(BattleFighter attacker)
		{
			BattleFighterBuff _buffInst = attacker.BuffManager.getBattleBuff(this);
			if (_buffInst == null)
			{
				return;
			}
			attacker.BuffManager.removeSingleBuff(_buffInst);
		}

		public override FighterStateEnum StateEnum
		{
			get
			{
				return FighterStateEnum.SP_MAX_STATE;
			}
		}

	}

}