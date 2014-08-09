using System.Collections.Generic;
using com.kx.sglm.gs.battle.share.data;
using com.kx.sglm.gs.battle.share.factory.creater;
using Template.Auto.Buff;
using Template.Auto.Monster;
using Template.Auto.Raid;
using Template.Auto.Skill;
using Thrift.Protocol;
using Object = UnityEngine.Object;

namespace Assets.Game.Scripts.Common.Model
{
    class BattleTemplateModelLocator : IBattleTemplateService
    {
        #region Private Fields

        private static BattleTemplateModelLocator instance;

        private static readonly object SyncRoot = new Object();

        //技能模板
        private Skill skillTemplate;
        //怪物模板
        private Monster monsterTemplate;
        //持续回合状态模板
        private Buff buffTemplate;




        private const string BaseTemplatePath = "Templates/";



        #endregion

        #region Public Fields

       // public const string SkillTemlatePath = BaseTemplatePath + "Skill";
        public const string SkillTemolatePath = "Templates/Skill";
        public const string MonsterTemlatePath = "Templates/Monster";
        public const string BuffTemlatePath = "Templates/Buff";

        #endregion

        #region Public Methods

        public static BattleTemplateModelLocator Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new BattleTemplateModelLocator();
                        }
                    }
                }
                return instance;
            }
        }

        public List<MonsterTemplate> CreatemMonsterTemplates(List<int> monsterIdList)
        {

            var resultList = new List<MonsterTemplate>();
            foreach (int id in monsterIdList)
            {
                MonsterTemplate monsterTemp = GetMonsterTemplate(id);
                if (monsterTemp != null)
                {
                    resultList.Add(monsterTemp);
                }
            }
            return resultList;
        }




        public HeroBattleSkillTemplate GetHeroBattleSkillTemplate(int skillId)
        {
            return SkillTemplate == null ? null : GetInst(skillId, skillTemplate.HeroBattleSkillTmpls);
        }

        public MonsterBattleSkillTemplate GetMonsterBattleSkillTemplate(int skillId)
        {
            return SkillTemplate == null ? null : GetInst(skillId, skillTemplate.MonsterBattleSkillTmpls);
        }

        public MonsterTemplate GetMonsterTemplate(int monsterId)
        {
            return MonsterTemplate == null ? null : GetInst(monsterId, monsterTemplate.MonsterTmpls);
        }

        public MonsterBattleAITemplate GetMonsterBattleAiTemplate(int aiId)
        {
            return MonsterTemplate == null ? null : GetInst(aiId, monsterTemplate.MonsterAITmpls);
        }


        public BattleBuffTemplate GetBattleBuffTemplate(int buffId)
        {
            return BuffTemplate == null ? null : GetInst(buffId, buffTemplate.BattleBuffTmpls);
        }




        public Skill SkillTemplate
        {
            get { return skillTemplate ?? (skillTemplate = Utils.Decode<Skill>(SkillTemolatePath)); }
        }

        public Monster MonsterTemplate
        {
            // get { return raidTemplates ?? (raidTemplates = Utils.Decode<Raid>(RaidTemlatePath)); }
            get { return monsterTemplate??(monsterTemplate = Utils.Decode<Monster>(MonsterTemlatePath)); }
        }

        public Buff BuffTemplate
        {
            get { return buffTemplate ?? (buffTemplate = Utils.Decode<Buff>(BuffTemlatePath)); }
        }


        #endregion

        #region Private Methods


        #endregion

        #region Static Methods

        public static FighterInfo CreateMonsterFighterInfo(MonsterTemplate template)
        {
            var info = new FighterInfo();

            return info;
        }

        

        public static T GetTemplate<T>(T template, string path) where T : TBase, new()
        {

            if (template == null)
            {
                template = Utils.Decode<T>(path);
            }
            return template;

        }

        public static T GetInst<T>(int key, Dictionary<int, T> dictionary)
        {
            T t = default(T);
            if (dictionary != null && dictionary.ContainsKey(key))
            {
                t = dictionary[key];
            }
            return t;
        }

        #endregion

        public RaidStageTemplate getRaidStageTemp(int stageId)
        {
            return MissionModelLocator.Instance.GetRaidStagrByTemplateId(stageId);
        }

        public RaidMonsterGroupTemplate getRaidMonsterGroupTemp(int monsterGroupId)
        {
            return MissionModelLocator.Instance.GetRaidMonsterGroupTemplateId(monsterGroupId);
        }

        public Dictionary<int, MonsterTemplate> AllMonsterMap { get { return MonsterTemplate.MonsterTmpls; } }
        public Dictionary<int, MonsterBattleAITemplate> AllMonsterAIMap { get { return MonsterTemplate.MonsterAITmpls; }}
        public Dictionary<int, HeroBattleSkillTemplate> AllHeroSkillMap { get
        {
            return SkillTemplate.HeroBattleSkillTmpls;
        } }
        public Dictionary<int, MonsterBattleSkillTemplate> AllMonsterSkillMap { get
        {
            return SkillTemplate.MonsterBattleSkillTmpls;
        }}
        public Dictionary<int, BattleBuffTemplate> AllBuffMap { get {return BuffTemplate.BattleBuffTmpls; } }
    }
}
