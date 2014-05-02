namespace com.kx.sglm.gs.battle.data.record
{


	/// <summary>
	/// 手动释放技能的记录类
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleSkillRecord : BaseViewRecord<SingleActionRecord>
	{

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

		public BattleSkillRecord()
		{
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


		public override void show(IBattleView viewObj)
		{
			viewObj.showBattleSkillRecord(this);
		}

		internal override SingleActionRecord createRecord()
		{
			return new SingleActionRecord();
		}

	}

}