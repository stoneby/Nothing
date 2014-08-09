namespace com.kx.sglm.gs.battle.share.input
{

	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;
	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;
	using BattleState = com.kx.sglm.gs.battle.share.enums.BattleState;
	using BeforeAttackEvent = com.kx.sglm.gs.battle.share.@event.impl.BeforeAttackEvent;
	using BattleLogicHelper = com.kx.sglm.gs.battle.share.helper.BattleLogicHelper;

	public class ProduceFighterIndexAction : IBattleInputEvent
	{

		private int[] heroIndex;
		private int sideIndex = BattleSideEnum.SIDE_LEFT.Index;
		private int targetIndex;


	//	@Override
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
			_team.beforeAttack(new BeforeAttackEvent());
			BattleLogicHelper.refreshState(battle.BattleArmy);
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