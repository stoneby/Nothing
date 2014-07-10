using System;
using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.actor
{


	using BattleRoundCountRecord = com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord;
	using InnerBattleEvent = com.kx.sglm.gs.battle.share.@event.InnerBattleEvent;
	using SceneStartEvent = com.kx.sglm.gs.battle.share.@event.impl.SceneStartEvent;

	/// <summary>
	/// 带有{@code List}结构的战斗参与者，一般用于{@code AbstractBattleLoopedNest}
	/// @author liyuan2
	/// </summary>
	/// @param <T> </param>
	public abstract class AbstractBattleActorList<T> : IBattleActor where T : IBattleActor
	{
		public abstract void onDead();
		public abstract bool hasHp();

		protected internal List<T> actorList;

		protected internal int curIndex_Renamed;

		protected internal Battle battle;

		public AbstractBattleActorList(Battle battle)
		{
			actorList = new List<T>();
			curIndex_Renamed = 0;
			this.battle = battle;
		}


		public virtual void activeSceneStartSkill(SceneStartEvent @event)
		{
			foreach (T _actor in actorList)
			{
				_actor.activeSceneStartSkill(@event);
			}
		}


		public virtual void activeSceneStartBuff(SceneStartEvent @event)
		{
			foreach (T _actor in actorList)
			{
				_actor.activeSceneStartBuff(@event);
			}
		}


		public virtual void onSceneStop()
		{
			foreach (T _actor in actorList)
			{
				_actor.onSceneStop();
			}
		}

		public virtual T CurActor
		{
			get
			{
				return getActor(curIndex_Renamed);
			}
		}


		public virtual void addActor(T actor)
		{
			this.actorList.Add(actor);
		}

		public virtual bool hasCurActor()
		{
			return curIndex() < battlingActorSize();
		}


		public virtual void handleEvent(InnerBattleEvent @event)
		{
			fireBattleEvent(@event);
			foreach (T _actor in actorList)
			{
				_actor.handleEvent(@event);
			}
		}

		public abstract void fireBattleEvent(InnerBattleEvent @event);


		public virtual void onRoundFinish(BattleRoundCountRecord roundRecord)
		{
			foreach (T _actor in ActorList)
			{
				_actor.onRoundFinish(roundRecord);
			}
		}

		/// <summary>
		/// 只是获取值，并不执行加操作
		/// @return
		/// </summary>
		public virtual int nextActorIndex()
		{
			return curIndex_Renamed + 1;
		}

		public virtual int CurIndex
		{
			get
			{
				return curIndex_Renamed;
			}
		}


		/// <summary>
		/// 增加当前的Index
		/// </summary>
		public virtual void addCurIndex()
		{
			curIndex_Renamed++;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Console.WriteLine("add Actor Index " + curIndex_Renamed + " " + this.GetType().FullName);
		}

		public virtual T getActor(int index)
		{
			return actorList[index];
		}

		public virtual void resetOnNewAction()
		{
			this.curIndex_Renamed = 0;
			doResetReset();
		}



		public virtual int curIndex()
		{
			return curIndex_Renamed;
		}

		public virtual List<T> ActorList
		{
			get
			{
				return actorList;
			}
		}

		public virtual int battlingActorSize()
		{
			return allActorSize();
		}

		public virtual int allActorSize()
		{
			return actorList.Count;
		}

		public abstract void doResetReset();

		public abstract void onActorDead();


		public virtual Battle Battle
		{
			get
			{
				return battle;
			}
		}

	}

}