namespace com.kx.sglm.gs.battle.share.buff.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using FighterStateEnum = com.kx.sglm.gs.battle.share.enums.FighterStateEnum;

	public class PetrifiedDebuffEffect : StateBuffEffect
	{



		public override void onAttack(BattleFighter attacker)
		{

		}

		public override FighterStateEnum StateEnum
		{
			get
			{
				return FighterStateEnum.ATTACK_DIS_FLAG;
			}
		}

	}

}