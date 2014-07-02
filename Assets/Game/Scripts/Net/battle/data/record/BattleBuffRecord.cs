namespace com.kx.sglm.gs.battle.share.data.record
{

	public class BattleBuffRecord : BaseViewRecord<SingleActionRecord>
	{

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


		public override bool Empty
		{
			get
			{
				return RecordList.Count == 0;
			}
		}

		public override void show(IBattleView viewObj)
		{
			viewObj.showBattleBuffRecord(this);
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

	    public override string ToString()
	    {
	        return string.Format("{0}: side index: {1}", GetType().Name, sideIndex);
	    }
	}

}