using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.actor.impl
{


	using com.kx.sglm.gs.battle.share.actor;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;
	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;
	using FighterType = com.kx.sglm.gs.battle.share.enums.FighterType;
	using HeroColor = com.kx.sglm.gs.battle.share.enums.HeroColor;
	using InnerBattleEvent = com.kx.sglm.gs.battle.share.@event.InnerBattleEvent;
	using BeforeAttackEvent = com.kx.sglm.gs.battle.share.@event.impl.BeforeAttackEvent;
	using TeamShotStartEvent = com.kx.sglm.gs.battle.share.@event.impl.TeamShotStartEvent;
	using IBattleExecuter = com.kx.sglm.gs.battle.share.executer.IBattleExecuter;
	using BattleTeamShot = com.kx.sglm.gs.battle.share.logic.loop.BattleTeamShot;
	using PropertyRawSet = com.kx.sglm.gs.battle.share.utils.PropertyRawSet;

	/// <summary>
	/// 抽象的战斗队伍
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public abstract class BattleTeam : AbstractBattleActorList<BattleFighter>, IBattleSideActor
	{

		/// <summary>
		/// 所属的阵营
		/// </summary>
		protected internal BattleSideEnum battleSide;
		/// <summary>
		/// 当前出手Index
		/// </summary>
		protected internal int curFightIndex;
		/// <summary>
		/// 是否已经死亡
		/// </summary>
		protected internal bool deadth;

		/// <summary>
		/// 队伍额外属性
		/// </summary>
		protected internal PropertyRawSet teamProps;

		public BattleTeam(Battle battle, BattleSideEnum side) : base(battle)
		{
			this.battleSide = side;
			teamProps = new PropertyRawSet();
		}

		public virtual BattleSideEnum BattleSide
		{
			set
			{
				this.battleSide = value;
			}
			get
			{
				return battleSide;
			}
		}

		//TODO: addTeamShortStartAction
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void onTeamShotStart(final com.kx.sglm.gs.battle.share.event.impl.TeamShotStartEvent event)
		public virtual void onTeamShotStart(TeamShotStartEvent @event)
		{
			foreach (BattleFighter _fighter in ActorList)
			{
				_fighter.onTeamShotStart(@event);
			}
			new IteratorActorOptionAnonymousInnerClassHelper(this, @event)
			.itratorAction();

			recalcBuffAndTeamProp();
		}

		private class IteratorActorOptionAnonymousInnerClassHelper : IteratorActorOption
		{
			private readonly BattleTeam outerInstance;

			private TeamShotStartEvent @event;

			public IteratorActorOptionAnonymousInnerClassHelper(BattleTeam outerInstance, TeamShotStartEvent @event) : base(outerInstance)
			{
				this.outerInstance = outerInstance;
				this.@event = @event;
			}

			public override void option(BattleFighter actor)
			{
				actor.BuffManager.onTeamShotStart(@event);
			}
		}

		public virtual void recalcBuffAndTeamProp()
		{
			recalcPropEffectBuffs();
			recalcTeamProp();
		}

		public virtual void recalcTeamProp()
		{
			new IteratorActorOptionAnonymousInnerClassHelper2(this)
			.itratorAction();
		}

		private class IteratorActorOptionAnonymousInnerClassHelper2 : IteratorActorOption
		{
			private readonly BattleTeam outerInstance;

			public IteratorActorOptionAnonymousInnerClassHelper2(BattleTeam outerInstance) : base(outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public override void option(BattleFighter actor)
			{
				actor.recalcProp();
			}
		}



//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void beforeAttack(final com.kx.sglm.gs.battle.share.event.impl.BeforeAttackEvent event)
		public virtual void beforeAttack(BeforeAttackEvent @event)
		{
			new IteratorActorOptionAnonymousInnerClassHelper3(this, @event)
			.itratorAction();

			activeAllBuff(BattleConstants.BUFF_FLAG);
			recalcBuffAndTeamProp();

		}

		private class IteratorActorOptionAnonymousInnerClassHelper3 : IteratorActorOption
		{
			private readonly BattleTeam outerInstance;

			private BeforeAttackEvent @event;

			public IteratorActorOptionAnonymousInnerClassHelper3(BattleTeam outerInstance, BeforeAttackEvent @event) : base(outerInstance)
			{
				this.outerInstance = outerInstance;
				this.@event = @event;
			}


			public override void option(BattleFighter actor)
			{
				actor.beforeAttack(@event);
			}
		}

		public override void onDead()
		{

		}

		public virtual int CurFightIndex
		{
			get
			{
				return curFightIndex;
			}
			set
			{
				this.curFightIndex = value;
			}
		}

		//TODO: add record
		public virtual void activeAllBuff(int buffFlag)
		{
			foreach (BattleFighter _actor in ActorList)
			{
				if (_actor.Dead)
				{
					continue;
				}
				_actor.activeBuff(buffFlag);
			}
		}

		public virtual void recalcPropEffectBuffs()
		{
			new IteratorActorOptionAnonymousInnerClassHelper4(this)
			.itratorAction();
		}

		private class IteratorActorOptionAnonymousInnerClassHelper4 : IteratorActorOption
		{
			private readonly BattleTeam outerInstance;

			public IteratorActorOptionAnonymousInnerClassHelper4(BattleTeam outerInstance) : base(outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public override void option(BattleFighter actor)
			{
				actor.effectAllBuff();
			}
		}


		public abstract bool hasFightFighter();

		public abstract bool handleBattleFightInfo(int targetIndex, int[] battleIndexes);

		public abstract BattleFighter CurFighter {get;}

		public abstract void onTeamShotFinish(BattleTeamShot teamShot);

		public abstract FighterType FighterType {get;}

		public abstract bool hasFighterIndex(int fighterIndex);

		public abstract void changeHp(int costHp, BattleFighter defencer);

		public abstract int CurHp {get;}

		public abstract int TotalHp {get;}

		public abstract int CurMp {get;}

		public abstract BattleFighter getFighterByIndex(int fighterIndex);

		public abstract void changeFightColor(int fighterIndex, HeroColor color, SingleActionRecord actionRecord);

		public abstract int getFighterColor(int fighterIndex);

		public abstract List<BattleFighter> CurTeamShotFighters {get;}

		public abstract int getFighterCurHp(BattleFighter fighter);

		public abstract int getFighterTotalHp(BattleFighter fighter);

		public abstract void costFighterHp(int costHp, BattleFighter fighter);

		public abstract int getAttackRatioIndex(BattleFighter fighter);

		public abstract bool AllFighterDead {get;}

		/// <summary>
		/// 当前战场上的武将
		/// 
		/// @return
		/// </summary>
		public abstract List<BattleFighter> AllBattingFighter {get;}

		public abstract HeroColor CurFightColor {get;}


		public virtual void tryDead()
		{
			bool _isDead = AllFighterDead;
			if (!_isDead)
			{
				_isDead = canTeamAttack();
			}
			Deadth = _isDead;
		}

		public virtual bool canTeamAttack()
		{
			bool _needDead = true;
			foreach (BattleFighter _fighter in AllBattingFighter)
			{
				if (!_fighter.hasState(BattleConstants.ATTACK_DIS_FLAG))
				{
					_needDead = false;
				}
			}
			return _needDead;
		}

		public virtual int CurTeamShotFighterCount
		{
			get
			{
				return CurTeamShotFighters.Count;
			}
		}

		public virtual int getCurTeamShotFighterIndex(BattleFighter attacker)
		{
			int _index = BattleConstants.BATTLE_FIGHTER_NON_INDEX;
			List<BattleFighter> _teamShotFighter = CurTeamShotFighters;
			for (int _i = 0; _i < _teamShotFighter.Count; _i++)
			{
				BattleFighter _fighter = _teamShotFighter[_i];
				if (_fighter.isSameFighter(attacker))
				{
					_index = _i;
					break;
				}
			}
			return _index;
		}

		/// <summary>
		/// 当前所有活着的武将
		/// 
		/// @return
		/// </summary>
		public virtual List<BattleFighter> AllAliveFighter
		{
			get
			{
				List<BattleFighter> _allAliveFighter = new List<BattleFighter>();
				foreach (BattleFighter _fighter in ActorList)
				{
					if (!_fighter.Dead)
					{
						_allAliveFighter.Add(_fighter);
					}
				}
				return _allAliveFighter;
			}
		}


		public abstract bool isActiveFighter(BattleFighter fighter);

		public override void fireBattleEvent(InnerBattleEvent @event)
		{

		}

		public virtual void calcRoundCounter()
		{
			// TODO: addLogic
		}

		public virtual BattleTeam OppositeTeam
		{
			get
			{
				return Battle.BattleArmy.getOppositeTeam(this);
			}
		}



		public override bool hasHp()
		{
			return !deadth;
		}

		public virtual void addCurFightIndex()
		{
			curFightIndex++;
		}

		protected internal virtual bool Deadth
		{
			set
			{
				this.deadth = value;
			}
		}

		public virtual IBattleExecuter BattleExcuter
		{
			get
			{
				return Battle.BattleExcuter;
			}
		}


		public virtual PropertyRawSet TeamProps
		{
			get
			{
				return teamProps;
			}
		}

		public virtual void addProp(int key, object value)
		{
			teamProps.set(key, value);
		}

		public virtual int getIntProp(int key)
		{
			return teamProps.getInt(key, 0);
		}


	}

}