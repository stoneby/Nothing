using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.executer
{

	using BattleArmy = com.kx.sglm.gs.battle.share.actor.impl.BattleArmy;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;
	using HeroTeam = com.kx.sglm.gs.battle.share.actor.impl.HeroTeam;
	using BattleRoundCountRecord = com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord;
	using BattleStoreData = com.kx.sglm.gs.battle.share.data.store.BattleStoreData;
	using BattleType = com.kx.sglm.gs.battle.share.enums.BattleType;
	using HeroColor = com.kx.sglm.gs.battle.share.enums.HeroColor;
	using BattleRound = com.kx.sglm.gs.battle.share.logic.loop.BattleRound;
	using BattleScene = com.kx.sglm.gs.battle.share.logic.loop.BattleScene;
	using BattleTeamShot = com.kx.sglm.gs.battle.share.logic.loop.BattleTeamShot;

	/// <summary>
	/// ����ս�����Ͳ�ͬ����ͬ�Ľӿڣ���ս�����;��������� ���ܻ����һЩ�洢������
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

		void recoveryData(BattleStoreData storeData);

		void beforeBattleStart(BattleScene battleScene, BattleRoundCountRecord record);

		/// 
		/// <param name="battleTeamShot"> </param>
		void onBattleTeamShotStart(BattleTeamShot battleTeamShot);

		/// 
		/// <param name="battleTeamShot"> </param>
		// TODO: �ǵ�������Ҫ�ж���������佫�����Ͳ��ٽ�����һ����Ŷ~~
		void onBattleTeamShotFinish(BattleTeamShot battleTeamShot);

		/// <summary>
		/// ս���غϳ��ֽ��������Ͳ���
		/// </summary>
		/// <param name="battleRound"> </param>
		void onBattleRoundFinish(BattleRound battleRound);

		/// <summary>
		/// ��һ��ս����������������PVE�д���һ���֡�
		/// </summary>
		/// <param name="battleScene"> </param>
		void onBattleSceneFinish(BattleScene battleScene);

		/// <summary>
		/// �ڶ�����ֺ��Ƿ���Ҫ�������
		/// </summary>
		/// <param name="round"> </param>
		/// <param name="attackTeam">
		/// @return </param>
		bool needHungUp(BattleRound round, BattleTeam attackTeam);

		/// <summary>
		/// ��ս���غϿ�ʼ�Ĳ���
		/// </summary>
		/// <param name="battleRound"> </param>
		/// <param name="attackTeam">
		/// @return </param>
		void onBattleRoundStart(BattleRound battleRound, BattleTeam attackTeam);

		/// <summary>
		/// ս��ȫ������
		/// </summary>
		void onBattleFinish();

		/// <summary>
		/// �Ƿ�����һ�غϳ�����PVP��ֻ��һ�غϣ�PVE�д��ڶ��������
		/// 
		/// @return
		/// </summary>
		bool hasNextBattleScene();

		/// <summary>
		/// ������һ��ս������
		/// 
		/// @return
		/// </summary>
		BattleScene createNextBattleScene();

		BattleType BattleType {get;}

		List<HeroColor> createColorList(int createCount);

		bool BattleDead {get;}

	}

}