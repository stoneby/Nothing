namespace com.kx.sglm.gs.battle.helper
{

	using BattleFighter = com.kx.sglm.gs.battle.actor.impl.BattleFighter;
	using HeroPoint = com.kx.sglm.gs.battle.actor.impl.HeroPoint;
	using HeroTeam = com.kx.sglm.gs.battle.actor.impl.HeroTeam;
	using BattleIndexRecord = com.kx.sglm.gs.battle.data.record.BattleIndexRecord;
	using SingleActionRecord = com.kx.sglm.gs.battle.data.record.SingleActionRecord;

	public class BattleRecordHelper
	{

		public static void initFighterSingleRecord(BattleFighter fighter, SingleActionRecord record)
		{
			record.Index = fighter.Index;
			record.SideIndex = fighter.Side;
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
			record.TargetSide = heroTeam.actorSize();
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