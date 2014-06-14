using System.Collections.Generic;
using System.Security.Permissions;
using com.kx.sglm.gs.battle.share;
using com.kx.sglm.gs.battle.share.data;
using com.kx.sglm.gs.battle.share.data.record;
using com.kx.sglm.gs.battle.share.logic.loop;
using KXSGCodec;
using Template;
using UnityEngine;
using System.Collections;

namespace Assets.Game.Scripts.Common.Model
{
    class BattleModelLocator
    {
        private static volatile BattleModelLocator instance;
        private static readonly object SyncRoot = new Object();
        private BattleModelLocator() { }
        public static BattleModelLocator Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (instance == null)
                            instance = new BattleModelLocator();
                    }
                }
                return instance;
            }
        }

        //PVE战斗数据
        public sbyte BattleType;
        //public List<BattleMsgHero> FighterList;
        public List<int> MonsterGroup;
        //public List<BattleMsgMonster> MonsterList;
        public int RaidID;
        public long Uuid;

        public BattleSource Source;
        public IList<FighterInfo> HeroList;
        public IList<FighterInfo> MonsterList;
        public int MonsterIndex;
        public Battle MainBattle;
        public List<PointRecord> NextList;

        public PointRecord GetNextFromNextList(int f)
        {
            Logger.Log("Get Next ===== " + f);
            if (MainBattle == null || NextList.Count == 0) return null;
            var k = NextList[0];
            NextList.RemoveAt(0);
            
            return k;
        }

        public PointRecord GetNext()
        {
            var next = NextList[0];
            NextList.RemoveAt(0);
            return next;
        }

        public bool CanSelectHero = true;
        public SkillTemplate Skill;
    }
}
