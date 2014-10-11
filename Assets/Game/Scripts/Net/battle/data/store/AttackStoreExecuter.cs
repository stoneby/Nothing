namespace com.kx.sglm.gs.battle.share.data.store
{


	public class AttackStoreExecuter : AbstractBattleStoreExecuter
	{

		private int curCount;
		private int curAttack;
		private int totalCount;


		public AttackStoreExecuter(BattleStoreData data) : base(data)
		{
		}

		public virtual void handleStartAttack(int totalCount)
		{
			cleanCount();
			this.totalCount = totalCount;
		}

		public virtual void handleAttackInfo(int attackValue)
		{
			curAttack += attackValue;
			curCount++;
			checkMaxCount();
		}

		protected internal virtual void cleanCount()
		{
			curCount = 0;
			curAttack = 0;
			totalCount = 0;
		}

		protected internal virtual void checkMaxCount()
		{
			if (curCount >= totalCount)
			{
				saveToData();
				cleanCount();
			}
		}



		public override void saveToData()
		{
			int _curMax = data.CurMaxFight;
			if (_curMax < curAttack)
			{
				data.CurMax = curAttack;
			}
		}

	}

}