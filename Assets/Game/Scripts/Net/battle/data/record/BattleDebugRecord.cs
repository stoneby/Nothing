using System.Collections.Generic;
using System.Text;

namespace com.kx.sglm.gs.battle.share.data.record
{
    public sealed class BattleDebugRecord : IBattleViewRecord
    {
        public List<PointRecord> PointList = new List<PointRecord>();

        public BattleDebugRecord(bool empty)
        {
            Empty = empty;
        }

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

        public bool Empty
        {
            get { return false; }
            set { }
        }

        public override string ToString()
        {
            return toReportStr();
        }
    }
}
