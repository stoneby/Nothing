using System.Text;

namespace com.kx.sglm.gs.battle.share.data.record
{

	public class BattleBuffRecord : BaseViewRecord<SingleFighterRecord>
	{

		public BattleBuffRecord(int curRound) : base(curRound)
		{
		}


		private int sideIndex;

		public virtual int SideIndex
		{
			set
			{
				this.sideIndex = value;
			}
			get
			{
				return sideIndex;
			}
		}


		public override void show(IBattleView viewObj)
		{
			viewObj.showBattleBuffRecord(this);
		}

		internal override SingleFighterRecord createRecord()
		{
			return new SingleFighterRecord();
		}


		public override string toReportStr()
		{
			StringBuilder _sb = new StringBuilder();
			_sb.Append("index:").Append(sideIndex).Append(",");
			foreach (SingleFighterRecord _record in RecordList)
			{
				_sb.Append(_record.toRecordString());
				_sb.Append("/t/r");
			}
			return _sb.ToString();
		}

	}

}