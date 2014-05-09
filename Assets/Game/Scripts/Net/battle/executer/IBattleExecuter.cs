using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.executer
{

	using BattleArmy = com.kx.sglm.gs.battle.share.actor.impl.BattleArmy;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;
	using HeroTeam = com.kx.sglm.gs.battle.share.actor.impl.HeroTeam;
	using BattleRoundCountRecord = com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord;
	using BattleType = com.kx.sglm.gs.battle.share.enums.BattleType;
	using HeroColor = com.kx.sglm.gs.battle.share.enums.HeroColor;
	using BattleRound = com.kx.sglm.gs.battle.share.logic.loop.BattleRound;
	using BattleScene = com.kx.sglm.gs.battle.share.logic.loop.BattleScene;
	using BattleTeamShot = com.kx.sglm.gs.battle.share.logic.loop.BattleTeamShot;

	/// <summary>
	/// 根据战斗类型不同处理不同的接口，由战斗类型决定创建�?可能会带有一些存储的数据
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public interface IBattleExecuter : IBattle
	{

		BattleArmy createSceneBattleArmy();

		HeroTeam AttackerTeam {set;}

		void initDefencerTeam();

		void initDataOnCreate();

		void beforeBattleStart(BattleScene battleScene, BattleRoundCountRecord record);

		/// 
		/// <param name="battleTeamShot"> </param>
		void onBattleTeamShotStart(BattleTeamShot battleTeamShot);

		/// 
		/// <param name="battleTeamShot"> </param>
		// TODO: 记得子类里要判断如果萌萌武将死亡就不再进入下一轮了哦~~
		void onBattleTeamShotFinish(BattleTeamShot battleTeamShot);

		/// <summary>
		/// 战斗回合出手结束的类型操�?
		/// </summary>
		/// <param name="battleRound"> </param>
		void onBattleRoundFinish(BattleRound battleRound);

		/// <summary>
		/// 当一个战斗场景结束【即在PVE中打死一波怪�?
		/// </summary>
		/// <param name="battleScene"> </param>
		void onBattleSceneFinish(BattleScene battleScene);

		/// <summary>
		/// 在队伍出手后是否需要挂起操�?
		/// </summary>
		/// <param name="round"> </param>
		/// <param name="attackTeam">
		/// @return </param>
		bool needHungUp(BattleRound round, BattleTeam attackTeam);

		/// <summary>
		/// 当战斗回合开始的操作
		/// </summary>
		/// <param name="battleRound"> </param>
		/// <param name="attackTeam">
		/// @return </param>
		void onBattleRoundStart(BattleRound battleRound, BattleTeam attackTeam);

		/// <summary>
		/// 战斗全部结束
		/// </summary>
		void onBattleFinish();

		/// <summary>
		/// 是否还有下一回合场景【PVP中只有一回合，PVE中存在多个场景�?
		/// 
		/// @return
		/// </summary>
		bool hasNextBattleScene();

		/// <summary>
		/// 产生下一个战斗场�?
		/// 
		/// @return
		/// </summary>
		BattleScene createNextBattleScene();

		BattleType BattleType {get;}

		IList<HeroColor> createColorList(int createCount);

		bool BattleDead {get;}

	}

}