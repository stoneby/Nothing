namespace com.kx.sglm.gs.battle.data.record
{

	public abstract class BaseViewRecord : AbstractBaseRecord, IBattleViewRecord
	{
		public abstract void show(IBattleView viewObj);

	}

}