namespace com.kx.sglm.gs.battle.share.input
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;
	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;

	/// <summary>
	/// 使用主动技能
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class UseActiveSkillAction : IBattleInputEvent
	{

		private int fighterIndex;

		public virtual void fireEvent(Battle battle)
		{
			BattleTeam _heroTeam = battle.BattleArmy.getActor(BattleSideEnum.SIDEA.Index);
			if (_heroTeam == null)
			{
				//TODO: loggers.error
				return;
			}
			BattleFighter _fighter = _heroTeam.getActor(fighterIndex);
			_fighter.useActiveSkill();
		}

		public virtual int FighterIndex
		{
			set
			{
				this.fighterIndex = value;
			}
		}

	}

}