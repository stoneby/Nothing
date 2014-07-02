namespace com.kx.sglm.gs.battle.share.helper
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;

	public class BattleSkillLogicHelper
	{


		public static void costBaseHp(int costHp, BattleFighter defencer, SingleActionRecord record)
		{
			if (defencer.Hero)
			{
				costHeroHp(costHp, defencer, record);
			}
			else
			{
				costMonsterHp(costHp, defencer, record);
			}
		}

		public static void costHeroHp(int costHp, BattleFighter defencer, SingleActionRecord record)
		{
			BattleTeam _heroTeam = defencer.getOwnerTeam();
			_heroTeam.changeHp(-costHp, defencer);
			record.ResultHp = _heroTeam.CurHp;
		}


		public static void costMonsterHp(int costHp, BattleFighter defencer, SingleActionRecord record)
		{
			defencer.changeCurHp(-costHp);
			record.ResultHp = defencer.CurHp;
		}

	}

}