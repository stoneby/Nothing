namespace com.kx.sglm.gs.battle.share.buff.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;

	public class PoisonValueEffect : PoisonEffect
	{

		internal override int getCostValue(BattleFighter fighter)
		{
			return ReduceValue;
		}



	}

}