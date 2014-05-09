namespace com.kx.sglm.gs.battle.share.data.record
{


	public class BattleRoundCountRecord : BaseViewRecord<SingleActionRecord>
	{





		public override void show(IBattleView viewObj)
		{
			viewObj.showBattleRoundCountRecord(this);

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