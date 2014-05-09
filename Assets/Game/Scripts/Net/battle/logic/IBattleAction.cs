namespace com.kx.sglm.gs.battle.share.logic
{


	public interface IBattleAction : IBattle
	{

		/// <summary>
		/// 战斗的基本动�?
		/// </summary>
		void onAction();

		void createDeadth();

		bool DeadInTime {get;}

	}

}