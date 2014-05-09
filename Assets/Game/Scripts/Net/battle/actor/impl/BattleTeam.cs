namespace com.kx.sglm.gs.battle.share.actor.impl
{

	using com.kx.sglm.gs.battle.share.actor;
	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;
	using FighterType = com.kx.sglm.gs.battle.share.enums.FighterType;
	using IBattleExecuter = com.kx.sglm.gs.battle.share.executer.IBattleExecuter;
	using BattleTeamShot = com.kx.sglm.gs.battle.share.logic.loop.BattleTeamShot;
	using PropertyRawSet = com.kx.sglm.gs.battle.share.utils.PropertyRawSet;

	/// <summary>
	/// 抽象的战斗队�?
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public abstract class BattleTeam : AbstractBattleActorList<BattleFighter>, IBattleSideActor
	{
		public abstract void tryDead();

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
		/// 队伍额外属�?
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


		public abstract bool hasFightFighter();

		public abstract bool handleBattleFightInfo(int targetIndex, int[] battleIndexes);

		public abstract BattleFighter CurFighter {get;}

		public abstract void onTeamShotFinish(BattleTeamShot teamShot);

		public abstract FighterType FighterType {get;}

		public abstract bool hasFighterIndex(int fighterIndex);

		public abstract void changeHp(int costHp, BattleFighter defencer);

		public abstract int CurHp {get;}

		public abstract BattleFighter getFighterByIndex(int fighterIndex);


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



		public override bool Alive
		{
			get
			{
				return !deadth;
			}
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