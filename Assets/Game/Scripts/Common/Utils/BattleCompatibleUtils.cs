using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.kx.sglm.gs.battle.share.data.store;
using com.kx.sglm.gs.battle.share.utils;

class BattleCompatibleUtils : IBattleCompatibleUtils
{
        public string[] splitString(string baseString, string splitElem)
        {
            return baseString.Split(new[] { BattleStoreConstants.BATTLE_STORE_KEY_SPLIT }, StringSplitOptions.RemoveEmptyEntries);

        }
}

