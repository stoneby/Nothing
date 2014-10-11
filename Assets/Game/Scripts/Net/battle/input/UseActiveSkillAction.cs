namespace com.kx.sglm.gs.battle.share.input
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;
	using BattleRecord = com.kx.sglm.gs.battle.share.data.record.BattleRecord;
	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;
	using BattleLogicHelper = com.kx.sglm.gs.battle.share.helper.BattleLogicHelper;
	using BattleRecordHelper = com.kx.sglm.gs.battle.share.helper.BattleRecordHelper;

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
			BattleTeam _heroTeam = battle.BattleArmy.getActor(BattleSideEnum.SIDE_LEFT.Index);
			if (_heroTeam == null)
			{
				//TODO: loggers.error
				return;
			}
			BattleFighter _fighter = _heroTeam.getActor(fighterIndex);
			_fighter.useActiveSkill();

			//暂时只支持Hero队和对方对，不支持第三支队伍
			_heroTeam.activeAllBuff(BattleConstants.BUFF_FLAG);
			_heroTeam.OppositeTeam.activeAllBuff(BattleConstants.DEBUFF_FALG);
			_heroTeam.recalcBuffAndTeamProp();
			_heroTeam.OppositeTeam.recalcBuffAndTeamProp();

			BattleRecord _record = battle.Record;
			BattleRecordHelper.recordBattleTeamRecord(_heroTeam, _record);
			BattleRecordHelper.recordBattleTeamRecord(_heroTeam.OppositeTeam, _record);
			BattleLogicHelper.refreshState(battle.BattleArmy);
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