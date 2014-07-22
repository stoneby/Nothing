using System;

namespace com.kx.sglm.gs.battle.share.skill.target
{

	using PointDirection = com.kx.sglm.gs.battle.share.enums.PointDirection;

	/// <summary>
	/// ¥Ú ˙≈≈
	/// @author liyuan2
	/// 
	/// </summary>
	public class UprightTargetGetter : AbstractFormationTargetGetter
	{

		internal override int initStartPoint(params string[] param)
		{
			return Convert.ToInt32(param[0]);
		}

		internal override PointDirection[] initPointArray(params string[] param)
		{
			PointDirection[] _pd = new PointDirection[] {PointDirection.UP, PointDirection.DOWN};
			return _pd;
		}

	}

}