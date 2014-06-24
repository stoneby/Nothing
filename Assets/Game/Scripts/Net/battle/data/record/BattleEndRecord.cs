namespace com.kx.sglm.gs.battle.share.data.record
{

	/// <summary>
	/// Õ½¶·½áÊø/³¡¾°ÇÐ»»
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

	    public override string ToString()
	    {
	        return string.Format("EndType: " + endType);
	    }
	}

}