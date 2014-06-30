using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.kx.sglm.gs.battle.share.data.record
{
    public class BattleDebugRecord : IBattleViewRecord
    {
        public List<PointRecord> PointList = new List<PointRecord>();
 
        public string toReportStr()
        {
            var result = new StringBuilder('{');
            foreach (var pointRecord in PointList)
            {
                result.Append(pointRecord).Append('\n');
            }
            result.Append('}');
            return result.ToString();
        }

        public void show(IBattleView viewObj)
        {
            viewObj.showBattleDebugRecord(this);
        }

        public override string ToString()
        {
            return toReportStr();
        }
    }
}
