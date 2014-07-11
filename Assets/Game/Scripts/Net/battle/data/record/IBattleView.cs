namespace com.kx.sglm.gs.battle.share.data.record
{

	/// <summary>
	/// 显示接口，用于各个Record的显示接入
	/// @author liyuan2
	/// 
	/// </summary>
	public interface IBattleView
	{

		void showBattleSkillRecord(BattleSkillRecord battleSkillRecord);

		void showBattleTeamFightRecord(BattleTeamFightRecord battleTeamFightRecord);

		void showBattleRoundCountRecord(BattleRoundCountRecord roundCountRecord);

		void showBattleIndexRecord(BattleIndexRecord battleIndexRecord);

		void showBattleBuffRecord(BattleBuffRecord battleErrorRecord);

		void showBattleEndRecord(BattleEndRecord battleEndRecord);

		void showBattleErrorRecord(BattleErrorRecord battleErrorRecord);

		void showBattleTeamInfoRecord(BattleTeamInfoRecord battletTeamInfoRecord);

	    void showBattleDebugRecord(BattleDebugRecord battleDebugRecord);
	}

}