namespace com.kx.sglm.gs.battle.share.logic
{


	public interface IBattleAction : IBattle
	{

		/// <summary>
		/// ս���Ļ�������
		/// </summary>
		void onAction();

		void createDeadth();

		bool DeadInTime {get;}

	}

}