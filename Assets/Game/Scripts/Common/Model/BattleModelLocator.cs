using System.ComponentModel;
using System.Text;
using com.kx.sglm.gs.battle.share;
using com.kx.sglm.gs.battle.share.data;
using com.kx.sglm.gs.battle.share.data.record;
using System.Collections.Generic;
using Template;
using UnityEngine;

namespace Assets.Game.Scripts.Common.Model
{
    class BattleModelLocator
    {
        #region Public Fields

        public sbyte BattleType;
        public List<int> MonsterGroup;
        public int RaidID;
        public long Uuid;

        public BattleSource Source;
        public IList<FighterInfo> HeroList;
        public IList<FighterInfo> MonsterList;
        public int MonsterIndex;
        public Battle MainBattle;
        public List<PointRecord> NextList;

        public bool CanSelectHero = true;
        public SkillTemplate Skill;

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
        }

        #endregion

        #region Public Methods

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
