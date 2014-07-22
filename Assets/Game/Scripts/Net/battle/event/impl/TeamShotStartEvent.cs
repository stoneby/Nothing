namespace com.kx.sglm.gs.battle.share.@event.impl
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeamFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleTeamFightRecord;
	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;
	using BattleScene = com.kx.sglm.gs.battle.share.logic.loop.BattleScene;

	public class TeamShotStartEvent : AbstractBattleSceneEvent
	{

		private BattleTeamFightRecord teamFightRecord;
		private BattleSideEnum curSide;


		public TeamShotStartEvent(BattleScene battleScene) : base(BattleEventConstants.BATTLE_TEAM_SHOT_START, battleScene)
		{
		}

		public virtual BattleTeamFightRecord TeamFightRecord
		{
			set
			{
				this.teamFightRecord = value;
			}
			get
			{
				return teamFightRecord;
			}
		}


		public virtual BattleSideEnum CurSide
		{
			set
			{
				this.curSide = value;
			}
			get
			{
				return curSide;
			}
		}


		public virtual bool isSelfTeamShot(BattleFighter fighter)
		{
			return fighter.Side == CurSide.Index;
		}


	}

}