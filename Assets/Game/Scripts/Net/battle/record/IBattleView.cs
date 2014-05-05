namespace com.kx.sglm.gs.battle.data.record
{

	/// <summary>
	/// 显示接口，用于各个Record的显示接入
	/// @author liyuan2
	/// 
	/// </summary>
	public interface IBattleView
	{

		void showBattleSkillRecord(BattleSkillRecord battleSkillRecord);//技能

		void showBattleTeamFightRecord(BattleTeamFightRecord battleTeamFightRecord);//一边队伍出手的所有信息

		void showBattleRoundCountRecord(BattleRoundCountRecord roundCountRecord);//回合变化。怪物（cd）+武将（buff）

		void showBattleIndexRecord(BattleIndexRecord battleIndexRecord);//武将颜色和位置变化

		void showBattleEndRecord(BattleEndRecord battleEndRecord);//推屏和战斗结束

	}

}