namespace com.kx.sglm.gs.battle.share.logic
{

	using BattleState = com.kx.sglm.gs.battle.share.enums.BattleState;

	/// <summary>
	/// 战斗内需要循环被操作的逻辑
	/// @author liyuan2
	/// 
	/// </summary>
	public interface IBattleLoop : IBattleAction
	{


		/// <summary>
		/// 更新战斗状态 </summary>
		/// <param name="battelState"> </param>
		/// <param name="updateSub"> </param>
		void updateBattleState(BattleState battelState, bool updateSub);

		bool Finished {get;}

		/// <summary>
		/// 逻辑内的操作者是否全部死亡
		/// @return
		/// </summary>
		bool Dead {get;}

		void createNewSubAction();

	}

}