using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill.target
{


	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;

	public abstract class AbstractTeamTargetGetter : AbstractHeroTeamGetter
	{

		protected internal bool inBattle;

		public override List<BattleFighter> getTarget(BattleFighter attacker, BattleTeam targetTeam)
		{
			List<BattleFighter> _fighterList = new List<BattleFighter>();
			List<BattleFighter> _baseFighters = getBaseFighter(targetTeam);
			foreach (BattleFighter _fighter in _baseFighters)
			{
				if (isFitFlag(_fighter))
				{
					_fighterList.Add(_fighter);
				}
			}
			return _fighterList;
		}


		public abstract bool isFitFlag(BattleFighter curFighter);

		internal virtual List<BattleFighter> getBaseFighter(BattleTeam targetTeam)
		{
			return inBattle ? targetTeam.AllBattingFighter : targetTeam.AllAliveFighter;
		}

		public override void build(params string[] param)
		{
			setInBattle();
			buildByType(param);
		}

		public abstract void buildByType(params string[] param);
		public abstract void setInBattle();

	}

}