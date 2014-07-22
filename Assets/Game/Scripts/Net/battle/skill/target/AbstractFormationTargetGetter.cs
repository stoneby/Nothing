using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill.target
{


	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;
	using PointDirection = com.kx.sglm.gs.battle.share.enums.PointDirection;
	using ArrayUtils = com.kx.sglm.gs.battle.share.utils.ArrayUtils;

	public abstract class AbstractFormationTargetGetter : AbstractHeroTeamGetter
	{

		protected internal int startPoint;
		protected internal PointDirection[] pointArray;



		public override List<BattleFighter> getTarget(BattleFighter attacker, BattleTeam targetTeam)
		{
			int _startPoint = startPoint;
			List<int> _pointList = new List<int>();
			_pointList.Add(_startPoint);
			foreach (PointDirection _pd in pointArray)
			{
				int _index = _pd.getChangedIndex(_startPoint);
				if (ArrayUtils.isRightArrayIndex(_index, BattleConstants.HERO_BATTLE_ARR_LENGTH))
				{
					_pointList.Add(_index);
				}
			}
			List<BattleFighter> _fighterList = new List<BattleFighter>();
			foreach (int _index in _pointList)
			{
				_fighterList.Add(targetTeam.getFighterByIndex(_index));
			}
			return _fighterList;
		}

		public override void build(params string[] param)
		{
			this.startPoint = initStartPoint(param);
			this.pointArray = initPointArray(param);
		}



		internal abstract int initStartPoint(params string[] param);

		internal abstract PointDirection[] initPointArray(params string[] param);

	}

}