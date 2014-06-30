namespace com.kx.sglm.gs.battle.share.data.record
{

	/// <summary>
	/// ��ʾ�ӿڣ����ڸ���Record����ʾ����
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

		void showBattleErrorRecord(BattleErrorRecord battleErrorRecord);

        void showBattleDebugRecord(BattleDebugRecord battleDebugRecord);
	}

}