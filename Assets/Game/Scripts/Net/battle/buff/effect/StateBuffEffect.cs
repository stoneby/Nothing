namespace com.kx.sglm.gs.battle.share.buff.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;

	public abstract class StateBuffEffect : AbstractBuffEffect
	{


		public override void onActive(BattleFighter fighter)
		{

		}

		public override void onRemove(BattleFighter fighter)
		{

		}

		public override void onTeamBeforeAttack(BattleFighter fighter)
		{

		}


		public override void onDefence(BattleFighter attacker, BattleFighter owner)
		{

		}


		public override void build(params string[] param)
		{

		}


		public override bool needShow(BattleFighterBuff buffInst)
		{
			return BuffShowId > 0;
		}

	}

}