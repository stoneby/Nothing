using com.kx.sglm.gs.battle.share;
using com.kx.sglm.gs.battle.share.data;
using com.kx.sglm.gs.battle.share.data.record;
using com.kx.sglm.gs.battle.share.enums;
using com.kx.sglm.gs.battle.share.factory.creater;
using KXSGCodec;
using System.Collections.Generic;
using System.Text;
using Template.Auto.Raid;
using Template.Auto.Skill;
using UnityEngine;

namespace Assets.Game.Scripts.Common.Model
{
    class BattleModelLocator
    {
        #region Public Fields

        public LevelManager LevelManager { get; set; }

        #region Level Manger Wrapper

        public List<FighterInfo> MonsterList
        {
            get { return LevelManager.MonsterList; }
        }

        public List<int> MonsterGroup
        {
            get { return LevelManager.MonsterGroupList; }
        }

        public int MonsterIndex
        {
            get { return LevelManager.CurrentLevelMonsterBaseIndex; }
        }

        #endregion

        public sbyte BattleType;
        public int RaidID;
        public long Uuid;

        public BattleSource Source;
        public List<FighterInfo> HeroList;
        public Battle MainBattle;
        public List<PointRecord> NextList;

        public HeroBattleSkillTemplate Skill;

        public bool CanSelectHero = true;

        /// <summary>
        /// Minimun count of all heros.
        /// </summary>
        public const int MinHerosCount = 12;

        public static BattleModelLocator Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new BattleModelLocator();
                        }
                    }
                }
                return instance;
            }
        }

        #endregion

        #region Private Fields

        private static volatile BattleModelLocator instance;
        private static readonly object SyncRoot = new Object();

        private readonly BattleTemplateModelLocator templateModel;

        #endregion

        #region Constructors

        private BattleModelLocator()
        {
            templateModel = BattleTemplateModelLocator.Instance;
        }

        #endregion

        #region Public Methods

        public void Init(SCBattlePveStartMsg battleStartMsg)
        {
            
            var service = BattleTemplateModelLocator.Instance;
            var battleSourceCreator = new BattleSourceTemplateCreater(service);
            var battleSource = battleSourceCreator.createPVESource(battleStartMsg);
            battleSource.Uuid = battleStartMsg.Uuid;

            // level manager setup, including levels and monsters controll.
            LevelManager = new LevelManager
            {
                MonsterList = battleSource.getSideFighters(BattleSideEnum.SIDEB_RIGHT),
                MonsterGroupList = battleSource.MonsterGroup
            };
            if (LevelManager.MonsterList == null)
            {
                Debug.Log("MonsterList == null");
            }
            if (LevelManager.MonsterGroupList == null)
            {
                Debug.Log("MonsterGroupList == null");
            }

            // Important, will do level manager input data validation.
            LevelManager.Validate();

            if (PersistenceHandler.Instance.Mode == PersistenceHandler.PersistenceMode.ReStartBattleWithPersistence)
            {
                LevelManager.RestorePersistent(PersistenceHandler.Instance.PersistentInfor);
            }
            LevelManager.InitLevel();

            BattleType = battleStartMsg.BattleType;
            RaidID = battleStartMsg.RaidID;
            Uuid = battleStartMsg.Uuid;
            Source = battleSource;

            var factory = battleSource.BattleType.Factory;
            MainBattle = factory.createBattle(Source, templateModel);
            MainBattle.start();

            // hero setup.
            HeroList = battleSource.getSideFighters(BattleSideEnum.SIDE_LEFT);
        }

        public BattleTemplateModelLocator TemplateModel
        {
            get { return templateModel; }
        }

        public void CreateRaidMonsterTemplateInfo(int stageId, BattleSource source)
        {
            RaidStageTemplate _stageTemp = MissionModelLocator.Instance.GetRaidStagrByTemplateId(stageId);
            if (_stageTemp == null)
            {
                //TODO: loggers.error
                return;
            }
            int _monsterGropId = _stageTemp.MonsterGroupId;
            RaidMonsterGroupTemplate _monsterGropTemp =
                MissionModelLocator.Instance.GetRaidMonsterGroupTemplateId(_monsterGropId);
            if (_monsterGropTemp == null)
            {
                //TODO: loggers.error
                return;
            }

            List<MonsterGroup> _allMonster = _monsterGropTemp.MonsterGroup;

            List<int> _allMonsterIdList = new List<int>();
            List<int> _monsterGroupList = new List<int>();

            foreach (MonsterGroup _monsterGroup in _allMonster)
            {
                List<int> _groupList = getSingleMonsterGroupIdList(_monsterGroup);
                if (_groupList.Count == 0)
                {
                    break;
                }
                _allMonsterIdList.AddRange(_groupList);
                _monsterGroupList.Add(_groupList.Count);
            }
        }

        public List<int> getSingleMonsterGroupIdList(MonsterGroup monsterGroup)
        {
            var _idList = new List<int>();
            addSingleMonsterId(monsterGroup.Arg_A, _idList);
            addSingleMonsterId(monsterGroup.Arg_B, _idList);
            addSingleMonsterId(monsterGroup.Arg_C, _idList);
            return _idList;
        }

        public static void addSingleMonsterId(int monsterId, List<int> monsterList)
        {
            if (monsterId > 0)
            {
                monsterList.Add(monsterId);
            }
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            foreach (var pointRecord in NextList)
            {
                str.Append(string.Format("(index={0},color={1})", pointRecord.Index, pointRecord.Color));
            }
            return str.ToString();
        }

        #endregion
    }
}
