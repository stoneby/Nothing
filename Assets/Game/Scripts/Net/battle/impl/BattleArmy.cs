namespace com.kx.sglm.gs.battle.actor.impl
{

	using com.kx.sglm.gs.battle.actor;
	using BattleSideEnum = com.kx.sglm.gs.battle.enums.BattleSideEnum;

	/// <summary>
	/// 战斗
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleArmy : AbstractBattleActorList<BattleTeam>
	{

		protected internal BattleSideEnum deadBattleSide;

		public BattleArmy(Battle battle) : base(battle)
		{
			deadBattleSide = null;
		}

		public virtual BattleTeam SideA
		{
			get
			{
				return getActor(BattleSideEnum.SIDEA.Index);
			}
		}

		public virtual BattleTeam SideB
		{
			get
			{
				return getActor(BattleSideEnum.SIDEB.Index);
			}
		}

		public virtual BattleSideEnum CurrentSide
		{
			get
			{
				return BattleSideEnum.getBattleSide(curIndex_Renamed);
			}
		}

		public virtual BattleTeam getOppositeTeam(BattleTeam team)
		{
			BattleSideEnum _opponent = team.BattleSide.DefaultOpponent;
			return getActor(_opponent.Index);
		}

		public virtual BattleTeam getOppositeTeam(BattleFighter fighter)
		{
			BattleSideEnum _opponent = fighter.BattleSide.DefaultOpponent;
			return getActor(_opponent.Index);
		}

		public virtual BattleSideEnum DeadBattleSide
		{
			set
			{
				this.deadBattleSide = value;
			}
			get
			{
				return deadBattleSide;
			}
		}



		public override void doResetReset()
		{
			deadBattleSide = null;
		}

		public override bool Alive
		{
			get
			{
				return deadBattleSide == null;
			}
		}

		public override void onDead()
		{
			Battle.finish();

		}

		public override void onActorDead()
		{
			// TODO Auto-generated method stub

		}

	}

}