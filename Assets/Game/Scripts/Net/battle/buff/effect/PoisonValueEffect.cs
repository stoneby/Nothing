namespace com.kx.sglm.gs.battle.share.buff.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using FighterStateEnum = com.kx.sglm.gs.battle.share.enums.FighterStateEnum;

	public class PoisonValueEffect : RoundCostHPEffect
	{

		internal override int getCostPercentValue(BattleFighter fighter)
		{
			return ReducePercent;
		}


		public override FighterStateEnum StateEnum
		{
			get
			{
				return FighterStateEnum.NORMAL_STATE;
			}
		}

	}

}