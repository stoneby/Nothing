namespace com.kx.sglm.gs.battle.share.skill.model
{

	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;

	/// <summary>
	/// 技能内需要传出的数据
	/// @author liyuan2
	/// 
	/// </summary>
	public class SkillDataHolder
	{

		private BattleFightRecord record;

		public SkillDataHolder(BattleFightRecord record)
		{
			this.record = record;
		}


		public virtual BattleFightRecord Record
		{
			get
			{
				return record;
			}
		}
	}

}