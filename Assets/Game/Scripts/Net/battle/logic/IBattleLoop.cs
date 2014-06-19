namespace com.kx.sglm.gs.battle.share.logic
{

	using BattleState = com.kx.sglm.gs.battle.share.enums.BattleState;

	/// <summary>
	/// ս������Ҫѭ�����������߼�
	/// @author liyuan2
	/// 
	/// </summary>
	public interface IBattleLoop : IBattleAction
	{


		/// <summary>
		/// ����ս��״̬ </summary>
		/// <param name="battelState"> </param>
		/// <param name="updateSub"> </param>
		void updateBattleState(BattleState battelState, bool updateSub);

		bool Finished {get;}

		/// <summary>
		/// �߼��ڵĲ������Ƿ�ȫ������
		/// @return
		/// </summary>
		bool Dead {get;}

		void createNewSubAction();

	}

}