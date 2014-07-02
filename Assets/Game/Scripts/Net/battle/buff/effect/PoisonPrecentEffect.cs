namespace com.kx.sglm.gs.battle.share.buff.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;

	public class PoisonPrecentEffect : PoisonEffect
	{

		internal override int getCostValue(BattleFighter fighter)
		{
			float _totalHp = fighter.BattleTotalHp;
			float _percent = ReduceValue;
			float _costHp = _totalHp * (_percent / BattleConstants.BATTLE_RATIO_BASE);
			return (int) _costHp;
		}

	}

}