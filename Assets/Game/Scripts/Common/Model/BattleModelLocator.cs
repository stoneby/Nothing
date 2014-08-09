using com.kx.sglm.gs.battle.share.ai;
using Template.Auto.Monster;
using Template.Auto.Raid;
using Template.Auto.Skill;
using com.kx.sglm.gs.battle.share;
using com.kx.sglm.gs.battle.share.data;
using com.kx.sglm.gs.battle.share.data.record;
using System.Collections.Generic;
using System.Text;
using Template;
using UnityEngine;

namespace Assets.Game.Scripts.Common.Model
{
    class BattleModelLocator
    {
        #region Public Fields

        public sbyte BattleType;
        public List<int> EnemyGroup;
        public int RaidID;
        public long Uuid;

        public BattleSource Source;
        public List<FighterInfo> HeroList;
        public List<FighterInfo> EnemyList;
        public int MonsterIndex;
        public Battle MainBattle;
        public List<PointRecord> NextList;

        public HeroBattleSkillTemplate Skill;

        public bool CanSelectHero = true;

        private readonly BattleTemplateModelLocator templateModel;
        

        /// <summary>
        /// Minimun count of all heros.
        /// </summary>
        public const int MinHerosCount = 12;

        //TODO: 转到专门的常量文件？
        private const string SkillTemlatePath = "Templates/Skill";
        private const string MonsterTemlatePath = "Templates/Monster";

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

        #endregion

        #region Private Methods

        private BattleModelLocator()
        {
            templateModel = BattleTemplateModelLocator.Instance;
        }

        #endregion

        #region Public Methods

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

            List<MonsterTemplate> monsterList = templateModel.CreatemMonsterTemplates(_allMonsterIdList);

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
