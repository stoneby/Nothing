using System;
using System.Collections.Generic;
using System.Text;

namespace com.kx.sglm.gs.battle.share.data.record
{


	public class BattleRecord : IBattleRecord
	{

		private List<IBattleViewRecord> recordList;

		private List<IBattleViewRecord> toReportRecordList;

		private BattleTeamFightRecord curFightRecord;

		private BattleRoundCountRecord curRoundCountRecord;

		private BattleIndexRecord curIndexRecord;

		private BattleSkillRecord curSkillRecord;

		private BattleBuffRecord curBuffRecord;

		private BattleEndRecord curEndRecord;

		private BattleErrorRecord errorRecord;


		public BattleRecord()
		{
			recordList = new List<IBattleViewRecord>();
			toReportRecordList = new List<IBattleViewRecord>();
			errorRecord = new BattleErrorRecord();
		}

		public virtual BattleEndRecord OrCreateEndRecord
		{
			get
			{
				if (curEndRecord == null)
				{
					curEndRecord = new BattleEndRecord();
					addList(curEndRecord);
				}
				return curEndRecord;
			}
		}


		public virtual void finishCurEndRecord()
		{
			this.curEndRecord = null;
		}

		public virtual BattleSkillRecord OrCreateSkillRecord
		{
			get
			{
				if (curSkillRecord == null)
				{
					curSkillRecord = new BattleSkillRecord();
					addList(curSkillRecord);
				}
				return curSkillRecord;
			}
		}

		public virtual void finishCurSkillRecord()
		{
			this.curSkillRecord = null;
		}

		public virtual BattleTeamFightRecord OrCreateTeamFighterRecord
		{
			get
			{
				if (curFightRecord == null)
				{
					curFightRecord = new BattleTeamFightRecord();
					addList(curFightRecord);
				}
				return curFightRecord;
			}
		}

		public virtual BattleBuffRecord OrCreateBuffRecord
		{
			get
			{
				if (curBuffRecord == null)
				{
					curBuffRecord = new BattleBuffRecord();
					addList(curBuffRecord);
				}
				return curBuffRecord;
			}
		}

		public virtual void finishCurTeamRecord()
		{
			this.curFightRecord = null;
		}

		public virtual void finishCurBuffRecord()
		{
			this.curBuffRecord = null;
		}

		public virtual BattleRoundCountRecord OrCreateRoundCountRecord
		{
			get
			{
				if (curRoundCountRecord == null)
				{
					curRoundCountRecord = new BattleRoundCountRecord();
					addList(curRoundCountRecord);
				}
				return curRoundCountRecord;
			}
		}

		public virtual void finishCurRoundCountRecord()
		{
			curRoundCountRecord = null;
		}

		public virtual BattleIndexRecord OrCreateIndexRecord
		{
			get
			{
				if (curIndexRecord == null)
				{
					curIndexRecord = new BattleIndexRecord();
					addList(curIndexRecord);
				}
				return curIndexRecord;
			}
		}

		public virtual void finishIndexRecord()
		{
			StringBuilder _sb = new StringBuilder();
			foreach (PointRecord _point in curIndexRecord.AllPointList)
			{
				_sb.Append(string.Format("index:{0:D}; color:{1:D}", _point.Index, _point.Color));
			}
			Console.WriteLine(_sb.ToString());
			curIndexRecord = null;
		}

		protected internal virtual void addList(IBattleViewRecord record)
		{
			this.recordList.Add(record);
			this.toReportRecordList.Add(record);
		}

		public virtual List<IBattleViewRecord> RecordList
		{
			get
			{
				return recordList;
			}
		}

		public virtual List<IBattleViewRecord> reportRecordListAndClear()
		{
			List<IBattleViewRecord> _recordList = new List<IBattleViewRecord>();
			foreach (IBattleViewRecord _record in toReportRecordList)
			{
				if (_record.Empty)
				{
					continue;
				}
				_recordList.Add(_record);
			}
			toReportRecordList.Clear();
			return _recordList;
		}

		public virtual void addErrorInfo(string error)
		{
			this.errorRecord.addErrorString(error);
		}

		public virtual void clearError()
		{
			this.errorRecord.clearError();
		}

	//	@Override
		public virtual string toReportStr()
		{
			// TODO Auto-generated method stub
			return null;
		}

	}

}