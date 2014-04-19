namespace com.kx.sglm.gs.battle.data.record
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

		void showBattleEndRecord(BattleEndRecord battleEndRecord);

	}

}