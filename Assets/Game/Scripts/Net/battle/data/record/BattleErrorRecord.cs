using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.data.record
{


	public class BattleErrorRecord : IBattleViewRecord
	{


		private List<string> errorStrList;

		public BattleErrorRecord()
		{
			errorStrList = new List<string>();
		}


		public virtual void addErrorString(string errorString)
		{
			errorStrList.Add(errorString);
		}

		public virtual bool hasError()
		{
			return errorStrList.Count > 0;
		}

		public virtual void clearError()
		{
			errorStrList.Clear();
		}


		public virtual string toReportStr()
		{
			// TODO Auto-generated method stub
			return null;
		}

		public virtual void show(IBattleView viewObj)
		{
			viewObj.showBattleErrorRecord(this);
		}

	}

}