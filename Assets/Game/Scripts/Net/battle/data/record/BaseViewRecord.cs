using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.data.record
{


	public abstract class BaseViewRecord<T> : AbstractBaseRecord, IBattleViewRecord where T : AbstractBaseRecord
	{
		public override abstract string toReportStr();
		public abstract void show(IBattleView viewObj);

		private List<T> recordList;

		/// <summary>
		/// LogicUseOnly </summary>
		private T curRecord;

		private bool empty = true;

		/// <summary>
		/// LogicUseOnly
		/// 
		/// @return
		/// </summary>
		internal abstract T createRecord();


		public virtual T OrCreateRecord
		{
			get
			{
				if (curRecord == null)
				{
					curRecord = createRecord();
					addRecord(curRecord);
				}
				setModified();
				return curRecord;
			}
		}

		public virtual void finishCurRecord()
		{
			curRecord = null;
		}

		public BaseViewRecord()
		{
			recordList = new List<T>();
		}

		public virtual void addRecord(T record)
		{
			recordList.Add(record);
			setModified();
		}

		public virtual List<T> RecordList
		{
			get
			{
				return recordList;
			}
		}

		public virtual void setModified()
		{
			this.empty = false;
		}

		public virtual void setEmpty()
		{
			this.empty = true;
		}

		public bool Empty
		{
			get
			{
				return empty;
			}
		}


	}

}