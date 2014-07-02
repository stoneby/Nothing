namespace com.kx.sglm.gs.battle.share.data.record
{


	/// <summary>
	/// �ֶ��ͷż��ܵļ�¼��
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleSkillRecord : BaseViewRecord<SingleActionRecord>
	{

		/// <summary>
		/// �ͷ���Index
		/// </summary>
		private int index;
		/// <summary>
		/// ��ӪID
		/// </summary>
		private int teamSide;
		/// <summary>
		/// �ͷż���ID
		/// </summary>
		private int skillId;

		private BattleFightRecord fightRecord;

		public BattleSkillRecord()
		{
		}

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

		public override bool Empty
		{
			get
			{
				//because there are some other msg not in #recordList
				return false;
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
			}
		}


		public virtual BattleFightRecord FightRecord
		{
			get
			{
				return fightRecord;
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