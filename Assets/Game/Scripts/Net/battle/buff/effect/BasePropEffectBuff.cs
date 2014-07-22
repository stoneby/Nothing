namespace com.kx.sglm.gs.battle.share.buff.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using InnerBattleEvent = com.kx.sglm.gs.battle.share.@event.InnerBattleEvent;
	using AmendTriple = com.kx.sglm.gs.role.properties.amend.model.AmendTriple;

	public abstract class BasePropEffectBuff : AbstractBuffEffect
	{

		protected internal AmendTriple amendTriple;



		public override void onRemove(BattleFighter fighter)
		{

		}

		public override void onEvent(InnerBattleEvent @event, BattleFighter fighter)
		{
			// do nothing
		}

		public override void onEffect(BattleFighter fighter)
		{

		}

		protected internal abstract int getValue(string param);

		public override void build(params string[] param)
		{
			int _value = getValue(param[1]);
			amendTriple = BattleActionService.Service.AmendManager.createAmendTriple(param[0], _value);
		}

	}

}