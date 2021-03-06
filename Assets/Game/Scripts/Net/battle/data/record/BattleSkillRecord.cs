namespace com.kx.sglm.gs.battle.share.data.record
{


	/// <summary>
	/// 手动释放技能的记录类
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleSkillRecord : BaseViewRecord<SingleActionRecord>
	{

		public BattleSkillRecord(int curRound) : base(curRound)
		{
		}

		/// <summary>
		/// 释放者Index
		/// </summary>
		private int index;
		/// <summary>
		/// 阵营ID
		/// </summary>
		private int teamSide;
		/// <summary>
		/// 释放技能ID
		/// </summary>
		private int skillId;

		private BattleFightRecord fightRecord;

		public virtual BattleFightRecord OrCreateFightRecord
		{
			get
			{
				if (fightRecord == null)
				{
					fightRecord = new BattleFightRecord();
				}
				return fightRecord;
			}
		}



		public virtual int Index
		{
			get
			{
				return index;
			}
			set
			{
				this.index = value;
			}
		}


		public virtual int TeamSide
		{
			get
			{
				return teamSide;
			}
			set
			{
				this.teamSide = value;
			}
		}


		public virtual int SkillId
		{
			get
			{
				return skillId;
			}
			set
			{
				this.skillId = value;
				setModified();
			}
		}


		public virtual BattleFightRecord FightRecord
		{
			get
			{
				return fightRecord;
			}
		}

		public virtual void finishFighterRecord()
		{
			if (fightRecord == null || fightRecord.Empty)
			{
				setEmpty();
			}
		}

		public override void show(IBattleView viewObj)
		{
			viewObj.showBattleSkillRecord(this);
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