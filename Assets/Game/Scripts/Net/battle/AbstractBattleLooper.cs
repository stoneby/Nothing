﻿namespace com.kx.sglm.gs.battle.logic
{

	using IBattleActor = com.kx.sglm.gs.battle.actor.IBattleActor;
	using BattleRecord = com.kx.sglm.gs.battle.data.record.BattleRecord;
	using BattleState = com.kx.sglm.gs.battle.enums.BattleState;

	/// <summary>
	/// 带有子逻辑（循环体本身）的容器 <br>
	/// 但其本身也有可能是被循环的对象
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public abstract class AbstractBattleLooper<T, A> : IBattleLoop where T : IBattleLoop where A : com.kx.sglm.gs.battle.actor.IBattleActor
	{
		public abstract void createDeadth();

		private Battle battle;

		private T curSubAction;

		private A curAttacker;

		private A curDefencer;

		private BattleState curState;

		private bool finished;

		private bool firstEnter;

		/// <summary>
		/// 循环次数的计数
		/// </summary>
		private int loopCount;

		public AbstractBattleLooper(Battle battle)
		{
			this.battle = battle;
			this.curState = BattleState.RUNTIME;
			this.firstEnter = true;
		}

		public AbstractBattleLooper(Battle battle, A curAttacker) : this(battle)
		{
			this.curAttacker = curAttacker;
		}

		public AbstractBattleLooper(Battle battle, A curAttacker, A curDefencer) : this(battle)
		{
			this.curAttacker = curAttacker;
			this.curDefencer = curDefencer;
		}

		public virtual void onAction()
		{
			// 这里关注的其实是子操作的循环逻辑
			// 这里在循环第第一步直接Action是因为会挂起，不能循环开始都create
			bool _dead = false;

			onFirstEnter();

			while (true)
			{
				if (curSubAction == null)
				{
					// TODO: do you need log?
					break;
				}
				if (HangUp)
				{
					break;
				}

				// 执行动作
				optionSubAction();
				if (HangUp)
				{
					break;
				}
				// 如果自动做死亡则尝试死亡
				if (curSubAction.Dead)
				{
					createDeadth();
					_dead = Dead;
				}
				if (_dead)
				{
					break;
				}
				addActorIndex();
				if (!hasNextSubAction())
				{
					break;
				}
				createNewSubAction();
			}
			// 如果全部死亡或全部动作做完，则结束
			// 因为跳出循环还有一种可能就是被挂起了
			if (!HangUp)
			{
				setFinish();
				onFinish();
				loopCount++; // 每生成一个新动作就会增加一回合
			}
		}

		/// <summary>
		/// 首次进入动作的初始化，但必须是非挂起状态<br>
		/// 因为如果是挂起状态，很多信息是没有的
		/// </summary>
		protected internal virtual void onFirstEnter()
		{
			if (firstEnter && !HangUp)
			{
				onStart();
				createNewSubAction();
				firstEnter = false;
			}
		}

		protected internal virtual void optionSubAction()
		{
			curSubAction.onAction();
			// 存在一些动作不能做完之后马上死亡
			if (curSubAction.DeadInTime)
			{
				curSubAction.createDeadth();
			}
		}

		public virtual bool HangUp
		{
			get
			{
				return curState.HangUp;
			}
		}

		public virtual void createNewSubAction()
		{
			if (HangUp)
			{
				return;
			}
			curSubAction = createSubActionByType();
			initOnCreateSubAction();
	//		if (curSubAction == null) {
	//			// TODO: do you need log?
	//			return;
	//		}
	//		getCurSubAction().createNewSubAction();
		}

		public virtual void updateBattleState(BattleState battelState, bool updateSub)
		{
			CurState = battelState;
			if (updateSub)
			{
				if (CurSubAction == null)
				{
					return;
				}
				CurSubAction.updateBattleState(battelState, updateSub);
			}
		}

		public virtual BattleState CurState
		{
			set
			{
				this.curState = value;
			}
			get
			{
				return curState;
			}
		}

		public virtual void setFinish()
		{
			this.finished = true;
			this.curState = BattleState.STOP;
		}

		public virtual A CurDefencer
		{
			set
			{
				this.curDefencer = value;
			}
			get
			{
				return curDefencer;
			}
		}

		public virtual A CurAttacker
		{
			get
			{
				return curAttacker;
			}
		}


		public virtual T CurSubAction
		{
			get
			{
				return curSubAction;
			}
		}


		public virtual Battle Battle
		{
			get
			{
				return battle;
			}
		}

		public virtual bool Finished
		{
			get
			{
				return finished;
			}
		}

		public virtual bool Dead
		{
			get
			{
				return isActorDead(curAttacker) || isActorDead(curDefencer);
			}
		}

		protected internal virtual bool isActorDead(A actor)
		{
			return actor == null ? false :!actor.Alive;
		}

		public virtual bool FirstEnter
		{
			get
			{
				return firstEnter;
			}
		}

		public virtual int LoopCount
		{
			get
			{
				return loopCount;
			}
		}

		public virtual BattleRecord Record
		{
			get
			{
				return battle.Record;
			}
		}

		/// <summary>
		/// 在动作后更新自身信息
		/// </summary>
		public abstract void addActorIndex();

		/// <summary>
		/// 是否出手已完成
		/// 
		/// @return
		/// </summary>
		public abstract bool hasNextSubAction();

		/// <summary>
		/// 获取下一个动作
		/// 
		/// @return
		/// </summary>
		public abstract T createSubActionByType();

		/// <summary>
		/// 初始化刚刚产生的动作
		/// </summary>
		public abstract void initOnCreateSubAction();

		/// <summary>
		/// 是否全部动作以及结束
		/// 
		/// @return
		/// </summary>
		public abstract bool AllActionFinish {get;}

		public abstract void onStart();

		public abstract void onFinish();

		public virtual bool DeadInTime
		{
			get
			{
				return true;
			}
		}

	}

}