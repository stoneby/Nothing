using System;
using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.executer.impl
{


	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using BattleArmy = com.kx.sglm.gs.battle.actor.impl.BattleArmy;
	using BattleFighter = com.kx.sglm.gs.battle.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.actor.impl.BattleTeam;
	using MonsterTeam = com.kx.sglm.gs.battle.actor.impl.MonsterTeam;
	using BattleIndexRecord = com.kx.sglm.gs.battle.data.record.BattleIndexRecord;
	using BattleRecord = com.kx.sglm.gs.battle.data.record.BattleRecord;
	using BattleRecordConstants = com.kx.sglm.gs.battle.data.record.BattleRecordConstants;
	using BattleRoundCountRecord = com.kx.sglm.gs.battle.data.record.BattleRoundCountRecord;
	using BattleType = com.kx.sglm.gs.battle.enums.BattleType;
	using FighterType = com.kx.sglm.gs.battle.enums.FighterType;
	using HeroColor = com.kx.sglm.gs.battle.enums.HeroColor;
	using BattleFighterCreater = com.kx.sglm.gs.battle.factory.creater.BattleFighterCreater;
	using BattleRecordHelper = com.kx.sglm.gs.battle.helper.BattleRecordHelper;
	using BattleRound = com.kx.sglm.gs.battle.logic.loop.BattleRound;
	using BattleScene = com.kx.sglm.gs.battle.logic.loop.BattleScene;
	using BattleTeamShot = com.kx.sglm.gs.battle.logic.loop.BattleTeamShot;

	public class TestPVEBattleExecuter : AbstractBattleExcuter
	{

		private IList<MonsterTeam> monsterList;

		public TestPVEBattleExecuter(Battle battle) : base(battle)
		{
			monsterList = new List<MonsterTeam>();
		}

		public virtual MonsterTeam MonsterTeam
		{
			get
			{
				return monsterList[Battle.CurSceneIndex];
			}
		}


		public override void onBattleTeamShotStart(BattleTeamShot battleTeamShot)
		{
			if (battleTeamShot.CurAttacker.FighterType.Hero)
			{
				attackerTeam().createNextWatingHero();
				recordWaitingList();
			}
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
			attackerTeam().emptyFightHeroArr();
			attackerTeam().fillHeroArrayInside();
			joinWaitingHero();
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
			_indexRecord.addProp(BattleRecordConstants.BATTLE_HERO_TOTAL_SP, attackerTeam().TotalSp);
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

		public override void onBattleSceneFinish(BattleScene battleScene)
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

		public override BattleType BattleType
		{
			get
			{
				return BattleType.TESTPVE;
			}
		}

		public override IList<HeroColor> createColorList(int createCount)
		{
			IList<HeroColor> _list = new List<HeroColor>();
			for (int _i = 0; _i < createCount; _i++)
			{
				_list.Add(randomColor());
			}
			return _list;
		}

		protected internal virtual HeroColor randomColor()
		{
			// TODO: 增加正式逻辑
		    int _index = MathUtils.random(HeroColor.RED.Index, HeroColor.PINK.Index);
			//int _index = MathUtils.random(HeroColor.RED.Index, HeroColor.RED.Index);
			return HeroColor.Values[_index];
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
				return !attackerTeam().Alive;
			}
		}

		public override void beforeBattleStart(BattleScene battleScene, BattleRoundCountRecord record)
		{
			BattleArmy _army = battleScene.CurAttacker;
	//		initSkill(attackerTeam(), record);
			foreach (BattleTeam _team in _army.ActorList)
			{
				initSkill(_team, record);
			}

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