using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.executer
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using HeroTeam = com.kx.sglm.gs.battle.share.actor.impl.HeroTeam;
	using BattleStoreData = com.kx.sglm.gs.battle.share.data.store.BattleStoreData;

	public abstract class AbstractBattleExcuter : IBattleExecuter
	{
		public abstract bool BattleDead {get;}
		public abstract List<com.kx.sglm.gs.battle.share.enums.HeroColor> createColorList(int createCount);
		public abstract com.kx.sglm.gs.battle.share.enums.BattleType BattleType {get;}
		public abstract com.kx.sglm.gs.battle.share.logic.loop.BattleScene createNextBattleScene();
		public abstract bool hasNextBattleScene();
		public abstract void onBattleFinish();
		public abstract void onBattleRoundStart(com.kx.sglm.gs.battle.share.logic.loop.BattleRound battleRound, com.kx.sglm.gs.battle.share.actor.impl.BattleTeam attackTeam);
		public abstract bool needHungUp(com.kx.sglm.gs.battle.share.logic.loop.BattleRound round, com.kx.sglm.gs.battle.share.actor.impl.BattleTeam attackTeam);
		public abstract void onBattleSceneFinish(com.kx.sglm.gs.battle.share.logic.loop.BattleScene battleScene);
		public abstract void onBattleRoundFinish(com.kx.sglm.gs.battle.share.logic.loop.BattleRound battleRound);
		public abstract void onBattleTeamShotFinish(com.kx.sglm.gs.battle.share.logic.loop.BattleTeamShot battleTeamShot);
		public abstract void onBattleTeamShotStart(com.kx.sglm.gs.battle.share.logic.loop.BattleTeamShot battleTeamShot);
		public abstract void beforeBattleStart(com.kx.sglm.gs.battle.share.logic.loop.BattleScene battleScene, com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord record);
		public abstract void initDataOnCreate();
		public abstract void initDefencerTeam();
		public abstract com.kx.sglm.gs.battle.share.actor.impl.BattleArmy createSceneBattleArmy();

		private Battle battle;

		private HeroTeam attackerTeam_Renamed;

		public AbstractBattleExcuter(Battle battle)
		{
			this.battle = battle;
		}

		public virtual void recoveryData(BattleStoreData storeData)
		{
			if (storeData.Empty)
			{
				return;
			}
			recoverDataByType(storeData);
		}

		protected internal abstract void recoverDataByType(BattleStoreData storeData);

		protected internal virtual void loadHeroSPBuff(BattleStoreData storeData)
		{
			List<int> _spIndexList = storeData.loadSpIndexList();
			foreach (int _index in _spIndexList)
			{
				BattleFighter _fighter = attackerTeam_Renamed.getActor(_index);
				_fighter.addSpMaxBuff();
			}
		}

		protected internal virtual void resetHeroTeamHp(BattleStoreData storeData)
		{
			int _curHp = storeData.getIntValue(BattleKeyConstants.BATTLE_STORE_CUR_HERO_HP);
			attackerTeam_Renamed.HeroCurHp = _curHp;
			int _curMp = storeData.getIntValue(BattleKeyConstants.BATTLE_STORE_CUR_HERO_MP);
			attackerTeam_Renamed.HeroCurMp = _curMp;
		}

		protected internal virtual void resetBattleSceneIndex(BattleStoreData storeData)
		{
			int _curIndex = storeData.getIntValue(BattleKeyConstants.BATTLE_STORE_CUR_SCENE_INDEX);
			Battle.BattleField.LoopCountForce = _curIndex;
		}

		public virtual HeroTeam AttackerTeam
		{
			set
			{
				this.attackerTeam_Renamed = value;
			}
		}


		public virtual HeroTeam attackerTeam()
		{
			return attackerTeam_Renamed;
		}


		public virtual Battle Battle
		{
			get
			{
				return battle;
			}
		}

	}

}