using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.data.record
{

	/// <summary>
	/// 一方队伍的出手
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleTeamFightRecord : BaseViewRecord
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
		private List<int?> skillFighter;
		/// <summary>
		/// 攻击的所有武将动作
		/// </summary>
		private List<BattleFightRecord> fighterRecords;

		public BattleTeamFightRecord() : base()
		{
            fighterRecords = new List<BattleFightRecord>();
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






		public virtual List<int?> SkillFighter
		{
			get
			{
				return skillFighter;
			}
			set
			{
				this.skillFighter = value;
			}
		}






		public virtual List<BattleFightRecord> FighterRecords
		{
			get
			{
				return fighterRecords;
			}
		}



		public virtual void addFighterRecord(BattleFightRecord fighterRecord)
		{
			this.fighterRecords.Add(fighterRecord);
		}



		public override void show(IBattleView viewObj)
		{
			viewObj.showBattleTeamFightRecord(this);
		}

	}

}