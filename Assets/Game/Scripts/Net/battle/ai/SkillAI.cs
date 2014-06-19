namespace com.kx.sglm.gs.battle.share.ai
{

	using IRolette = com.kx.sglm.core.model.IRolette;
	using IAICondition = com.kx.sglm.gs.battle.share.skill.IAICondition;

	public class SkillAI : IRolette
	{

		/// <summary>
		/// ���ܻ�����Ϣ </summary>
		private SkillRoulettePair skillInfo;
		/// <summary>
		/// ���ȼ� </summary>
		private int priority;
		/// <summary>
		///���� </summary>
		private IAICondition condition;

		public virtual SkillRoulettePair getSkillInfo()
		{
			return skillInfo;
		}

		public virtual void setSkillInfo(SkillRoulettePair skillInfo)
		{
			this.skillInfo = skillInfo;
		}

		public virtual IAICondition Condition
		{
			set
			{
				this.condition = value;
			}
			get
			{
				return condition;
			}
		}


		public virtual int Priority
		{
			get
			{
				return priority;
			}
			set
			{
				this.priority = value;
			}
		}


		public override int Rate
		{
			get
			{
				return skillInfo.Rate;
			}
		}

	}

}