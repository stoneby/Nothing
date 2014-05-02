namespace com.kx.sglm.gs.battle
{

	using BattleArmy = com.kx.sglm.gs.battle.actor.impl.BattleArmy;
	using BattleSource = com.kx.sglm.gs.battle.data.BattleSource;
	using BattleRecord = com.kx.sglm.gs.battle.data.record.BattleRecord;
	using BattleState = com.kx.sglm.gs.battle.enums.BattleState;
	using BattleType = com.kx.sglm.gs.battle.enums.BattleType;
	using IBattleExecuter = com.kx.sglm.gs.battle.executer.IBattleExecuter;
	using IBattleInputEvent = com.kx.sglm.gs.battle.input.IBattleInputEvent;
	using BattleField = com.kx.sglm.gs.battle.logic.loop.BattleField;

	public class Battle
	{

		private BattleSource battleSource;
		private BattleField battleField;
		private IBattleExecuter battleExcuter;
		private BattleType battleType;
		private BattleRecord record;

		public Battle(BattleType battleType, BattleSource source)
		{
			this.battleType = battleType;
			this.battleSource = source;
			this.record = new BattleRecord();
		}


		public virtual void start()
		{
			if (!canStartBattle())
			{
				//TODO: loggers.error
				return;
			}
			battleField = new BattleField(this);
	//		battleField.createNewSubAction();
			battleField.onAction();
		}

		public virtual void onAction()
		{
			battleField.onAction();
		}

		protected internal virtual bool canStartBattle()
		{
	//		if (isBattleArmyCurrect()) {
	//			return false;
	//		}
			return true;
		}

		public virtual void updateBattleState(BattleState state)
		{
			 battleField.updateBattleState(state, true);
		}



		public virtual void handleBattleEvent(IBattleInputEvent @event)
		{
			if (battleField.CurState == BattleState.STOP)
			{
				return; //TODO: 改成直接判断
			}
			@event.fireEvent(this);
		}


		public virtual BattleArmy BattleArmy
		{
			get
			{
				return battleField.BattleArmy;
			}
		}

		public virtual IBattleExecuter BattleExcuter
		{
			get
			{
				return battleExcuter;
			}
			set
			{
				this.battleExcuter = value;
			}
		}

		public virtual BattleField BattleField
		{
			get
			{
				return battleField;
			}
			set
			{
				this.battleField = value;
			}
		}

		public virtual BattleSource BattleSource
		{
			get
			{
				return battleSource;
			}
		}


		public virtual void finish()
		{
			// TODO Auto-generated method stub
			BattleExcuter.onBattleFinish();


		}

		public virtual int CurSceneIndex
		{
			get
			{
				return BattleField.LoopCount;
			}
		}

		public virtual BattleType BattleType
		{
			get
			{
				return battleType;
			}
		}



		public virtual BattleRecord Record
		{
			get
			{
				return record;
			}
		}


	}

}