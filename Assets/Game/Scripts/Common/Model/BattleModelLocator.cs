using System.Collections.Generic;
using KXSGCodec;
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
        public List<BattleMsgHero> FighterList;
        public List<int> MonsterGroup;
        public List<BattleMsgMonster> MonsterList;
        public int RaidID;
        public long Uuid;
    }
}
