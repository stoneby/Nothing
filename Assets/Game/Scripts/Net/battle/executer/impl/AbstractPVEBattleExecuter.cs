using System;
using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.executer.impl
{


	using BattleArmy = com.kx.sglm.gs.battle.share.actor.impl.BattleArmy;
	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;
	using MonsterTeam = com.kx.sglm.gs.battle.share.actor.impl.MonsterTeam;
	using IColorCreater = com.kx.sglm.gs.battle.share.color.IColorCreater;
	using BattleIndexRecord = com.kx.sglm.gs.battle.share.data.record.BattleIndexRecord;
	using BattleRecord = com.kx.sglm.gs.battle.share.data.record.BattleRecord;
	using BattleRecordConstants = com.kx.sglm.gs.battle.share.data.record.BattleRecordConstants;
	using BattleRoundCountRecord = com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord;
	using BattleTeamFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleTeamFightRecord;
	using BattleStoreData = com.kx.sglm.gs.battle.share.data.store.BattleStoreData;
	using FighterType = com.kx.sglm.gs.battle.share.enums.FighterType;
	using HeroColor = com.kx.sglm.gs.battle.share.enums.HeroColor;
	using BattleFighterCreater = com.kx.sglm.gs.battle.share.factory.creater.BattleFighterCreater;
	using BattleRecordHelper = com.kx.sglm.gs.battle.share.helper.BattleRecordHelper;
	using BattleRound = com.kx.sglm.gs.battle.share.logic.loop.BattleRound;
	using BattleScene = com.kx.sglm.gs.battle.share.logic.loop.BattleScene;
	using BattleTeamShot = com.kx.sglm.gs.battle.share.logic.loop.BattleTeamShot;

	public abstract class AbstractPVEBattleExecuter : AbstractBattleExcuter
	{

		public AbstractPVEBattleExecuter(Battle battle, IColorCreater attackerColorCreater) : base(battle, attackerColorCreater)
		{
			this.monsterList = new List<MonsterTeam>();
		}

		protected internal List<MonsterTeam> monsterList;


		/// <summary>
		/// @return
		/// </summary>
		public virtual MonsterTeam MonsterTeam
		{
			get
			{
				return monsterList[Battle.CurSceneIndex];
			}
		}



		protected internal override void recoverDataByType(BattleStoreData storeData)
		{
			loadHeroSPBuff(storeData);
			resetHeroTeamHp(storeData);
			resetBattleSceneIndex(storeData);

		}


		public override void onBattleTeamShotStart(BattleTeamShot battleTeamShot)
		{
	//		if (battleTeamShot.getCurAttacker().getFighterType().isHero()) {
	//			attackerTeam().createNextWatingHero();
	//			recordWaitingList();
	//		}
		}

		public override void onBattleTeamShotFinish(BattleTeamShot battleTeamShot)
		{
			if (battleTeamShot.CurAttacker.FighterType.Hero)
			{
				optionHeroTeam();
			}
		}

		protected internal virtual void optionHeroTeam()
		{
			attackerTeam().createNextWatingHero();
			recordWaitingList();
			attackerTeam().emptyFightHeroArr();
			attackerTeam().fillHeroArrayInside();
			joinWaitingHero();
			recordHeroMp();
		}

		protected internal virtual void recordHeroMp()
		{
			BattleRecord _record = Battle.Record;
			BattleTeamFightRecord _fightRecord = _record.OrCreateTeamFighterRecord;
			_fightRecord.addProp(BattleRecordConstants.BATTLE_HERO_PROP_MP, attackerTeam().CurMp);
		}


		protected internal virtual void recordAllIndexedRecord()
		{
			BattleRecord _record = Battle.Record;
			BattleIndexRecord _indexRecord = _record.OrCreateIndexRecord;
			BattleRecordHelper.recordAllIndex(attackerTeam(), _indexRecord);
			_record.finishIndexRecord();
		}

		public override void initDataOnCreate()
		{
			//初始化英雄站位
			attackerTeam().recalcTeamProp();
			attackerTeam().initHero();
			recordInitDate();
			//记录等待列表
			recordWaitingList();
			joinWaitingHero();

		}

		protected internal virtual void recordInitDate()
		{
			BattleRecord _record = Battle.Record;
			BattleIndexRecord _indexRecord = _record.OrCreateIndexRecord;
			_indexRecord.addProp(BattleRecordConstants.BATTLE_HERO_TOTAL_HP, attackerTeam().TotalHp);
			_indexRecord.addProp(BattleRecordConstants.BATTLE_HERO_TOTAL_MP, attackerTeam().TotalMp);
		}

		protected internal virtual void recordWaitingList()
		{
			BattleRecord _record = Battle.Record;
			BattleIndexRecord _indexRecord = _record.OrCreateIndexRecord;
			BattleRecordHelper.recordFillIndex(attackerTeam(), _indexRecord);
		}

		protected internal virtual void joinWaitingHero()
		{
			attackerTeam().fillArrayFromWaitingHero();
			recordAllIndexedRecord();
		}

		public override void onBattleRoundFinish(BattleRound battleRound)
		{

		}


		public override bool needHungUp(BattleRound round, BattleTeam attackTeam)
		{
			return attackTeam.FighterType == FighterType.HERO;
		}

		public override void onBattleRoundStart(BattleRound battleRound, BattleTeam attackTeam)
		{

		}

		public override void onBattleFinish()
		{
		}

		public override bool hasNextBattleScene()
		{
			return Battle.CurSceneIndex < monsterList.Count;
		}

		public override BattleScene createNextBattleScene()
		{
			Console.WriteLine("#TestPVEBattleExecuter.createNextBattleScene");
			return new BattleScene(Battle, createSceneBattleArmy());
		}


		public override List<HeroColor> createColorList(int createCount)
		{
			return AttackerColorCreater.createColorList(createCount);
		}

		public override BattleArmy createSceneBattleArmy()
		{
			BattleArmy _army = new BattleArmy(Battle);
			_army.addActor(MonsterTeam);
			_army.addActor(attackerTeam());
			return _army;
		}

		public override void initDefencerTeam()
		{
			monsterList = BattleFighterCreater.createMonsterTeamList(Battle);
		}

		public override bool BattleDead
		{
			get
			{
				return !attackerTeam().hasHp();
			}
		}

		public override void beforeBattleStart(BattleScene battleScene, BattleRoundCountRecord record)
		{
			BattleArmy _army = battleScene.CurAttacker;
			foreach (BattleTeam _team in _army.ActorList)
			{
				_team.addProp(BattleKeyConstants.BATTLE_KEY_HERO_TEAM_TARGET, 0);
				initSkill(_team, record);
			}

		}


		public override void onBattleSceneFinish(BattleScene battleScene)
		{
			Battle _battle = Battle;
			_battle.StoreHandler.handleSceneEnd(_battle.CurSceneIndex, attackerTeam());
		}

		protected internal virtual void initSkill(BattleTeam team, BattleRoundCountRecord record)
		{
			foreach (BattleFighter _fighter in team.ActorList)
			{
				_fighter.SkillManager.beforeBattleStart(record);
			}
		}

	}

}