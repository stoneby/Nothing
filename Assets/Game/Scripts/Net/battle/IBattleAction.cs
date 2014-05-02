namespace com.kx.sglm.gs.battle.logic
{


	public interface IBattleAction : IBattle
	{

		/// <summary>
		/// 战斗的基本动作
		/// </summary>
		void onAction();

		void createDeadth();

		bool DeadInTime {get;}

	}

}