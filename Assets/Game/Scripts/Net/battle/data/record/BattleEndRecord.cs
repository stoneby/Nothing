namespace com.kx.sglm.gs.battle.share.data.record
{

	/// <summary>
	/// ս������/�����л�
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleEndRecord : BaseViewRecord<SingleActionRecord>
	{

		private int endType;

		public virtual int EndType
		{
			set
			{
				this.endType = value;
			}
			get
			{
				return endType;
			}
		}


		public override bool Empty
		{
			get
			{
				return false;
			}
		}

		public override void show(IBattleView viewObj)
		{
			viewObj.showBattleEndRecord(this);
		}

		internal override SingleActionRecord createRecord()
		{
			return new SingleActionRecord();
		}

		public override string toReportStr()
		{
			// TODO Auto-generated method stub
			return null;
		}

	}

}