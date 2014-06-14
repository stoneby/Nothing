namespace com.kx.sglm.gs.battle.share.ai
{

	using IRolette = com.kx.sglm.core.model.IRolette;

	public class SkillRoulettePair : IRolette
	{

		private int skillId;
		private int rottleRate;

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


		public virtual int RottleRate
		{
			get
			{
				return rottleRate;
			}
			set
			{
				this.rottleRate = value;
			}
		}


		public override int Rate
		{
			get
			{
				return rottleRate;
			}
		}



	}

}