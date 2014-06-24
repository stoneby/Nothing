using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.data.record
{

	/// <summary>
	/// 一方队伍的出手。 <seealso cref="#getRecordList()"/>表示每个武将出手的动作
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleTeamFightRecord : BaseViewRecord<BattleFightRecord>
	{

		/// <summary>
		/// 队伍类型
		/// </summary>
		private int teamType;
		/// <summary>
		/// 队伍阵营
		/// </summary>
		private int teamSide;
		/// <summary>
		/// 出手前播放技能fighter列表
		/// </summary>
		private List<int> skillFighter;

		public BattleTeamFightRecord() : base()
		{
			skillFighter = new List<int>();
		}

		public virtual int TeamType
		{
			get
			{
				return teamType;
			}
			set
			{
				this.teamType = value;
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


		public virtual List<int> SkillFighter
		{
			get
			{
				return skillFighter;
			}
		}

		public virtual void addSkillFighter(int singleFighter)
		{
			this.skillFighter.Add(singleFighter);
		}

		public override void show(IBattleView viewObj)
		{
			viewObj.showBattleTeamFightRecord(this);
		}

		internal override BattleFightRecord createRecord()
		{
			return new BattleFightRecord();
		}

		public override string toReportStr()
		{
			// TODO Auto-generated method stub
			return null;
		}


	    public override string ToString()
	    {
	        return string.Format("teamType: " + teamType + ", teamside: " + teamSide);
	    }
	}

}