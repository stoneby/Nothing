namespace com.kx.sglm.gs.battle.share.data.record
{

	public class BattleTeamInfoRecord : BaseViewRecord<SingleFighterRecord>
	{

		private int side;

		public virtual int Side
		{
			set
			{
				this.side = value;
			}
			get
			{
				return side;
			}
		}


		public override void show(IBattleView viewObj)
		{
			viewObj.showBattleTeamInfoRecord(this);
		}

		internal override SingleFighterRecord createRecord()
		{
			return new SingleFighterRecord();
		}

		public override string toReportStr()
		{
			return null;
		}

	}

}