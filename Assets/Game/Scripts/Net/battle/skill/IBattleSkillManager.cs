namespace com.kx.sglm.gs.battle.share.skill
{

	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using BattleTeamFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleTeamFightRecord;
	using SceneStartEvent = com.kx.sglm.gs.battle.share.@event.impl.SceneStartEvent;
	using TeamShotStartEvent = com.kx.sglm.gs.battle.share.@event.impl.TeamShotStartEvent;

	public interface IBattleSkillManager : IRoundCounter
	{


		ISingletonSkillAction AttackAction {get;}

		bool canAttack();

		void beforeAttack(BattleFightRecord record);

		void afterAttack(BattleFightRecord record);

		void onSceneStart(SceneStartEvent @event);

		void onTeamShotStart(TeamShotStartEvent @event);

		void onActiveOption();

		void onAttack(BattleFightRecord fightRecord);

		void onHandleEvent(BattleTeamFightRecord record);

	}

}