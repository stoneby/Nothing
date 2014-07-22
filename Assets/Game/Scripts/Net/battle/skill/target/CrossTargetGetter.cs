namespace com.kx.sglm.gs.battle.share.skill.target
{

	using PointDirection = com.kx.sglm.gs.battle.share.enums.PointDirection;

	public class CrossTargetGetter : AbstractFormationTargetGetter
	{

		internal override int initStartPoint(params string[] param)
		{
			return BattleConstants.MIDDLE_POINT_INDEX;
		}

		internal override PointDirection[] initPointArray(params string[] param)
		{
			PointDirection[] _pd = new PointDirection[] {PointDirection.UP, PointDirection.DOWN, PointDirection.LEFT, PointDirection.RIGHT};
			return _pd;
		}

	}

}