namespace com.kx.sglm.gs.battle.share.buff
{

	using IBuffCondition = com.kx.sglm.gs.battle.share.buff.condition.IBuffCondition;

	public class BuffInfo
	{

		private int buffId;
		private IBuffCondition condition;
		private IBuffAction buffAction;

		public BuffInfo(int buffId, IBuffCondition condition)
		{
			this.buffId = buffId;
			this.condition = condition;
		}

		public virtual int BuffId
		{
			get
			{
				return buffId;
			}
			set
			{
				this.buffId = value;
			}
		}


		public virtual IBuffAction BuffAction
		{
			set
			{
				this.buffAction = value;
			}
			get
			{
				return buffAction;
			}
		}


		public virtual IBuffCondition Condition
		{
			get
			{
				return condition;
			}
			set
			{
				this.condition = value;
			}
		}




	}

}