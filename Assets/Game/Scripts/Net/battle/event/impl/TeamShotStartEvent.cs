namespace com.kx.sglm.gs.battle.share.@event.impl
{

	using BattleTeamFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleTeamFightRecord;
	using BattleScene = com.kx.sglm.gs.battle.share.logic.loop.BattleScene;

	public class TeamShotStartEvent : AbstractBattleSceneEvent
	{

		private BattleTeamFightRecord teamFightRecord;


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



	}

}