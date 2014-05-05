using com.kx.sglm.gs.battle.enums;

namespace com.kx.sglm.gs.battle.input
{

	using BattleTeam = com.kx.sglm.gs.battle.actor.impl.BattleTeam;
	using BattleState = com.kx.sglm.gs.battle.enums.BattleState;

	public class ProduceFighterIndexAction : IBattleInputEvent
	{

		private int[] heroIndex;
		private int sideIndex = BattleSideEnum.SIDEA.index;
		private int targetIndex;


		public virtual void fireEvent(Battle battle)
		{
			BattleTeam _team = battle.BattleArmy.getActor(sideIndex);
			if (_team == null)
			{
				//TODO: loggers.error
				return;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean _rightInfo = _team.handleBattleFightInfo(targetIndex, heroIndex);
			bool _rightInfo = _team.handleBattleFightInfo(targetIndex, heroIndex);
			if (!_rightInfo)
			{
				return;
			}

			battle.updateBattleState(BattleState.RUNTIME);
			battle.onAction();
		}

		public virtual int[] HeroIndex
		{
			set
			{
				this.heroIndex = value;
			}
		}

		public virtual int TargetIndex
		{
			set
			{
				this.targetIndex = value;
			}
		}

	}

}