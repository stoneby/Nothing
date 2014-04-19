namespace com.kx.sglm.gs.battle.data.record
{

	/// <summary>
	/// 战斗结束/场景切换
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleEndRecord : BaseViewRecord
	{

		private int endType;

		public virtual int EndType
		{
			set
			{
				this.endType = value;
			}
			get
			{
				return endType;
			}
		}


		public override void show(IBattleView viewObj)
		{
			viewObj.showBattleEndRecord(this);
		}

	}

}