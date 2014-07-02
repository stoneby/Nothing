namespace com.kx.sglm.gs.battle.share.buff.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using FighterStateEnum = com.kx.sglm.gs.battle.share.enums.FighterStateEnum;
	using InnerBattleEvent = com.kx.sglm.gs.battle.share.@event.InnerBattleEvent;
	using AmendTriple = com.kx.sglm.gs.role.properties.amend.model.AmendTriple;

	public abstract class AbstractPropEffectBuff : AbstractBuffEffect
	{

		private AmendTriple amendTriple;

		public override void onActive(BattleFighter fighter)
		{

		}

		public override void onRemove(BattleFighter fighter)
		{

		}

		public override void onEffect(BattleFighter fighter)
		{
			fighter.FighterProp.addBuffProp(amendTriple);
		}

		public override void onEvent(InnerBattleEvent @event, BattleFighter fighter)
		{
			//no event
		}

		public override void build(params string[] param)
		{
			int _value = getValue(param[1]);
			amendTriple = BattleActionService.Service.AmendManager.createAmendTriple(param[0], _value);
		}


		public override FighterStateEnum StateEnum
		{
			get
			{
				return FighterStateEnum.NORMAL_STATE;
			}
		}

		protected internal abstract int getValue(string param);




	}

}