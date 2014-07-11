namespace com.kx.sglm.gs.battle.share.@event.impl
{


	public class BeforeAttackEvent : InnerBattleEvent
	{


		public virtual int EventType
		{
			get
			{
				return BattleEventConstants.BATTLE_BEFORE_FIGHTER_ATTACK;
			}
		}

	}

}