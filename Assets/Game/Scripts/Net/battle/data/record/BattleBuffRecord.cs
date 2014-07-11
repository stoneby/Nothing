namespace com.kx.sglm.gs.battle.share.data.record
{

	public class BattleBuffRecord : BaseViewRecord<SingleFighterRecord>
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
			// TODO Auto-generated method stub
			return null;
		}

	}

}