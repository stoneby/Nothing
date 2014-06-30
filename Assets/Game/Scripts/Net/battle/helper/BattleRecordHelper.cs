using com.kx.sglm.gs.battle.share.data.record;

namespace com.kx.sglm.gs.battle.share.helper
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using HeroPoint = com.kx.sglm.gs.battle.share.actor.impl.HeroPoint;
	using HeroTeam = com.kx.sglm.gs.battle.share.actor.impl.HeroTeam;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using BattleIndexRecord = com.kx.sglm.gs.battle.share.data.record.BattleIndexRecord;
	using BattleSkillRecord = com.kx.sglm.gs.battle.share.data.record.BattleSkillRecord;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;

	public class BattleRecordHelper
	{

		public static void initBattelSkill(BattleSkillRecord record, BattleFighter fighter)
		{
			record.Index = fighter.Index;
			record.SkillId = fighter.BaseProp.ActiveSkillId;
			record.TeamSide = fighter.Side;
		}


		/// <summary>
		/// ��ʼ����Ҫ�����ݣ������߶������ͣ������߳�ʼ����
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
			record.Index = fighter.Index;
			record.SideIndex = fighter.Side;
		}

		public static void recordSingleRecordState(BattleFighter fighter, SingleActionRecord singleRecord, sbyte key, int value)
		{
			initSingleRecord(fighter, singleRecord);
			singleRecord.addState(key, value);
		}

        public static void FillDebugRecord(BattleDebugRecord debugRecord, HeroTeam heroTeam)
        {
            debugRecord.PointList.Clear();
            foreach (var hero in heroTeam.battlingHeroArr)
            {
                debugRecord.PointList.Add(new PointRecord(hero.fighter.index, hero.Color.Index));
            }

            foreach (var wait in heroTeam.waitingHeroList)
            {
                debugRecord.PointList.Add(new PointRecord(wait.fighter.index, wait.color.Index));
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

	}

}