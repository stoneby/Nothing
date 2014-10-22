namespace com.kx.sglm.gs.battle.share
{

	using BattleArmy = com.kx.sglm.gs.battle.share.actor.impl.BattleArmy;
	using BattleSource = com.kx.sglm.gs.battle.share.data.BattleSource;
	using BattleRecord = com.kx.sglm.gs.battle.share.data.record.BattleRecord;
	using BattleStoreData = com.kx.sglm.gs.battle.share.data.store.BattleStoreData;
	using BattleStoreHandler = com.kx.sglm.gs.battle.share.data.store.BattleStoreHandler;
	using BattleState = com.kx.sglm.gs.battle.share.enums.BattleState;
	using BattleType = com.kx.sglm.gs.battle.share.enums.BattleType;
	using InnerBattleEvent = com.kx.sglm.gs.battle.share.@event.InnerBattleEvent;
	using IBattleExecuter = com.kx.sglm.gs.battle.share.executer.IBattleExecuter;
	using IBattleInputEvent = com.kx.sglm.gs.battle.share.input.IBattleInputEvent;
	using BattleField = com.kx.sglm.gs.battle.share.logic.loop.BattleField;
	using BattleScene = com.kx.sglm.gs.battle.share.logic.loop.BattleScene;

	/// <summary>
	/// 战斗入口
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class Battle
	{

		/// <summary>
		/// 战斗数据源 </summary>
		private BattleSource battleSource;
		/// <summary>
		/// 战斗场地 </summary>
		private BattleField battleField;
		/// <summary>
		/// 战斗执行器，根据类型不同具体逻辑处理不同 </summary>
		private IBattleExecuter battleExcuter;
		/// <summary>
		/// 战斗类型 </summary>
		private BattleType battleType;
		/// <summary>
		/// 战报 </summary>
		private BattleRecord record;
		/// <summary>
		/// 战斗数据存储 </summary>
		private BattleStoreData storeData;
		/// <summary>
		/// 战斗数据存储入口 </summary>
		private BattleStoreHandler storeHandler;

		public Battle(BattleType battleType, BattleSource source)
		{
			this.battleType = battleType;
			this.battleSource = source;
			this.record = new BattleRecord();
			this.storeData = new BattleStoreData(source.BattleCompatibleUtils);
			this.storeHandler = new BattleStoreHandler(storeData);
		}

		/// <summary>
		/// 战斗开始触发
		/// </summary>
		public virtual void start()
		{
			if (!canStartBattle())
			{
				// TODO: loggers.error
				return;
			}
			this.battleField = new BattleField(this);
			battleExcuter.recoveryData(storeData);
			battleField.onAction();
		}


		public virtual void startFromDataStore(string storeStr)
		{
			storeData.fromStoreStr(storeStr);
			start();
		}


		/// <summary>
		/// 战斗动作
		/// </summary>
		public virtual void onAction()
		{
			battleField.onAction();
		}

		/// <summary>
		/// 目前逻辑不多，之后会增加
		/// 
		/// @return
		/// </summary>
		protected internal virtual bool canStartBattle()
		{
			return true;
		}

		/// <summary>
		/// 循环更新战斗状态
		/// </summary>
		/// <param name="state"> </param>
		public virtual void updateBattleState(BattleState state)
		{
			battleField.updateBattleState(state, true);
		}

		/// <summary>
		/// 接收战斗输入数据
		/// </summary>
		/// <param name="event"> </param>
		public virtual void handleBattleEvent(IBattleInputEvent @event)
		{
			if (battleField.CurState.Stoped)
			{
				//TODO loggers.error
				return;
			}
			@event.fireEvent(this);
		}

		public virtual int CurSceneRound
		{
			get
			{
				return battleField.CurSubAction.LoopCount;
			}
		}

		/// <summary>
		/// 转发战斗内部事件 </summary>
		/// <param name="innerEvent"> </param>
		public virtual void sendBattleSceneEvent(InnerBattleEvent innerEvent)
		{
			if (battleField.CurState.Stoped)
			{
				return;
			}
			battleField.CurSubAction.handleBattleInnerEvent(innerEvent);
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

		public virtual BattleScene CurScene
		{
			get
			{
				return battleField.CurSubAction;
			}
		}

		public virtual BattleSource BattleSource
		{
			get
			{
				return battleSource;
			}
		}

		/// <summary>
		/// 战斗结束
		/// </summary>
		public virtual void finish()
		{
			// TODO 增加其他逻辑
			BattleExcuter.onBattleFinish();

		}

		/// <summary>
		/// 获取当前推图index
		/// 
		/// @return
		/// </summary>
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

		public virtual BattleStoreHandler StoreHandler
		{
			get
			{
				return storeHandler;
			}
		}

		public virtual BattleStoreData StoreData
		{
			get
			{
				return storeData;
			}
		}

	}

}