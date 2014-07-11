using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.helper
{


	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleFighterState = com.kx.sglm.gs.battle.share.actor.impl.BattleFighterState;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;
	using HeroPoint = com.kx.sglm.gs.battle.share.actor.impl.HeroPoint;
	using HeroTeam = com.kx.sglm.gs.battle.share.actor.impl.HeroTeam;
	using BattleBuffRecord = com.kx.sglm.gs.battle.share.data.record.BattleBuffRecord;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using BattleIndexRecord = com.kx.sglm.gs.battle.share.data.record.BattleIndexRecord;
	using BattleRecord = com.kx.sglm.gs.battle.share.data.record.BattleRecord;
	using BattleRecordConstants = com.kx.sglm.gs.battle.share.data.record.BattleRecordConstants;
	using BattleSkillRecord = com.kx.sglm.gs.battle.share.data.record.BattleSkillRecord;
	using BattleTeamInfoRecord = com.kx.sglm.gs.battle.share.data.record.BattleTeamInfoRecord;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;
	using SingleFighterRecord = com.kx.sglm.gs.battle.share.data.record.SingleFighterRecord;

	public class BattleRecordHelper
	{

		public static void initBattelActiveSkill(BattleSkillRecord record, BattleFighter fighter)
		{
			record.Index = fighter.Index;
			record.SkillId = fighter.BaseProp.ActiveSkillId;
			record.TeamSide = fighter.Side;
		}

		/// <summary>
		/// 初始化必要的数据：攻击者动作类型，攻击者初始数据
		/// </summary>
		/// <param name="actType"> </param>
		/// <param name="attacker"> </param>
		/// <param name="fightRecord"> </param>
		public static void initBattleFight(int actType, BattleFighter attacker, BattleFightRecord fightRecord)
		{
			fightRecord.SourceIndex = attacker.Index;
			SingleActionRecord _attackAction = fightRecord.OrCreateAttack;
			_attackAction.ActType = actType;
			initSingleRecord(attacker, _attackAction);
		}

		public static void initSingleRecord(BattleFighter fighter, SingleActionRecord record)
		{
			initFighterRecord(fighter, record.FighterInfo);
		}

		public static void initFighterRecord(BattleFighter fighter, SingleFighterRecord record)
		{
			record.Index = fighter.Index;
			record.Side = fighter.Side;
		}

		public static void initDefencerRecord(BattleFighter fighter, SingleActionRecord record)
		{
			initSingleRecord(fighter, record);
			record.ActType = BattleRecordConstants.SINGLE_ACTION_TYPE_DEFENCE;
		}

		public static void recordTeamBuffState(BattleTeam team)
		{
			BattleRecord _record = team.Battle.Record;
			BattleBuffRecord _buffRecord = _record.OrCreateBuffRecord;
			_buffRecord.SideIndex = team.BattleSide.Index;
			recordBuffState(_buffRecord, team.ActorList);
			_record.finishCurBuffRecord();
		}

		public static void recordBuffState(BattleBuffRecord _buffRecord, List<BattleFighter> fighterList)
		{
			foreach (BattleFighter _fighter in fighterList)
			{
				if (_fighter.Dead)
				{
					continue;
				}
				SingleFighterRecord _singleRecord = _buffRecord.OrCreateRecord;
				BattleRecordHelper.initFighterRecord(_fighter, _singleRecord);
				_singleRecord.StateFlag = _fighter.FighterState;
				_fighter.updateStateRecord(_singleRecord);
				_buffRecord.finishCurRecord();
			}
		}


		public static void recordFillIndex(HeroTeam heroTeam, BattleIndexRecord record)
		{
			foreach (HeroPoint _point in heroTeam.WaitingHeroList)
			{
				addSinglePointToFillList(record, _point);
			}
		}

		public static void recordAllIndex(HeroTeam heroTeam, BattleIndexRecord record)
		{
			record.TargetSide = heroTeam.battlingActorSize();
			foreach (HeroPoint _point in heroTeam.BattlingHeroArr)
			{
				addSinglePointToCheckList(record, _point);
			}
			foreach (HeroPoint _point in heroTeam.WaitingHeroList)
			{
				addSinglePointToCheckList(record, _point);
			}
		}

		protected internal static void addSinglePointToFillList(BattleIndexRecord record, HeroPoint point)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _index = point.getFighter().getIndex();
			int _index = point.Fighter.Index;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _color = point.getColor().getIndex();
			int _color = point.Color.Index;
			record.addFillPointList(_index, _color);
		}

		protected internal static void addSinglePointToCheckList(BattleIndexRecord record, HeroPoint point)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _index = point.getFighter().getIndex();
			int _index = point.Fighter.Index;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _color = point.getColor().getIndex();
			int _color = point.Color.Index;
			record.addPointList(_index, _color);
		}

		public static void updateStateRecord(SingleFighterRecord record, Dictionary<int, BattleFighterState> stateMap, bool removeState)
		{
			foreach (BattleFighterState _state in stateMap.Values)
			{
				int _round = removeState ? 0 : _state.Round;
				record.addState(_state.BuffId, _state.ShowId, _state.Index, _round);
			}
		}



		public static void recordBattleTeamRecord(BattleTeam battleTeam, BattleRecord record)
		{
			BattleTeamInfoRecord _teamInfoRecord = record.OrCreateTeamRecord;
			_teamInfoRecord.Side = battleTeam.BattleSide.Index;
			foreach (BattleFighter _fighter in battleTeam.AllAliveFighter)
			{
				updateBattleFighterRecord(_fighter, _teamInfoRecord);
			}
			record.finishCurTeamInfoRecord();
		}

		public static void updateBattleFighterRecord(BattleFighter fighter, BattleTeamInfoRecord teamInfoRecord)
		{
			SingleFighterRecord _singleFightRecord = teamInfoRecord.OrCreateRecord;
			Dictionary<int, int> _propMap = fighter.BattleProp;
			initFighterRecord(fighter, _singleFightRecord);
			foreach (KeyValuePair<int, int> _propEntry in _propMap)
			{
				_singleFightRecord.addProp(_propEntry.Key, _propEntry.Value);
			}
			fighter.updateStateRecord(_singleFightRecord);
			teamInfoRecord.finishCurRecord();
		}

	}



























}