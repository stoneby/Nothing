namespace com.kx.sglm.gs.battle.data.record
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

	}

}