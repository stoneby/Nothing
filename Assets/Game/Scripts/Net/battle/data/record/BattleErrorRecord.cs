using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.data.record
{



	public class BattleErrorRecord : IBattleViewRecord
	{


		private List<BattleErrorInfo> errorList;

		public BattleErrorRecord()
		{
			errorList = new List<BattleErrorInfo>();
		}


		public virtual void addErrorString(int key, string errorString)
		{
			errorList.Add(new BattleErrorInfo(key, errorString));
		}

		public virtual bool hasError()
		{
			return !Empty;
		}

		public virtual void clearError()
		{
			errorList.Clear();
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

		public virtual List<BattleErrorInfo> ErrorList
		{
			get
			{
				return errorList;
			}
		}

	//	@Override
		public virtual bool Empty
		{
			get
			{
				return errorList.Count == 0;
			}
		}

	}

}