using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.data.record
{


	/// <summary>
	/// ˢ������fighter���Ե�ս��
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleTeamInfoRecord : BaseViewRecord<SingleFighterRecord>
	{

		public BattleTeamInfoRecord(int curRound) : base(curRound)
		{
			buffAction = new List<SingleActionRecord>();
		}

		private int side;

		private SingleActionRecord curBuffAction;

		private List<SingleActionRecord> buffAction;

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


		public virtual SingleActionRecord OrCreateCurBuffAction
		{
			get
			{
				if (curBuffAction == null)
				{
					curBuffAction = new SingleActionRecord();
					buffAction.Add(curBuffAction);
				}
				setModified();
				return curBuffAction;
			}
		}

		public virtual List<SingleActionRecord> BuffAction
		{
			get
			{
				return buffAction;
			}
		}

		public virtual void finishCurBuffAction()
		{
			curBuffAction = null;
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