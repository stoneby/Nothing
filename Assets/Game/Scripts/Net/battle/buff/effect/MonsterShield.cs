namespace com.kx.sglm.gs.battle.share.buff.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using FighterStateEnum = com.kx.sglm.gs.battle.share.enums.FighterStateEnum;
	using InnerBattleEvent = com.kx.sglm.gs.battle.share.@event.InnerBattleEvent;

	public class MonsterShield : AbstractBuffEffect
	{



		public override void onActive(BattleFighter fighter)
		{
			// TODO Auto-generated method stub

		}

		public override void onRemove(BattleFighter fighter)
		{
			// TODO Auto-generated method stub

		}

		public override void onEffect(BattleFighter fighter)
		{
			// TODO Auto-generated method stub

		}

		public override void onEvent(InnerBattleEvent @event, BattleFighter fighter)
		{
			// TODO Auto-generated method stub

		}

		public override void build(params string[] param)
		{
			// TODO Auto-generated method stub

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