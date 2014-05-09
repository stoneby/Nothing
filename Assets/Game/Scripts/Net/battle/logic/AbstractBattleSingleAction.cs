namespace com.kx.sglm.gs.battle.share.logic
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;

	public abstract class AbstractBattleSingleAction : IBattleAction
	{
		public abstract bool DeadInTime {get;}
		public abstract void createDeadth();
		public abstract void onAction();

		private Battle battle;

		protected internal BattleFighter attacker;


		public AbstractBattleSingleAction(Battle battle, BattleFighter attacker)
		{
			this.battle = battle;
			this.attacker = attacker;
		}


		public virtual Battle Battle
		{
			get
			{
				return battle;
			}
		}


		public virtual BattleFighter Attacker
		{
			get
			{
				return attacker;
			}
		}


	}

}