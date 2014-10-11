using System;

namespace com.kx.sglm.gs.battle.share.color
{

	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using HeroColor = com.kx.sglm.gs.battle.share.enums.HeroColor;

	public class RaidColorCreater : AbstractColorCreater
	{

		private int[] colorRateArray;

		public RaidColorCreater(Battle battle) : base(battle)
		{
		}

		public override void build(params string[] @params)
		{
			if (!isRightParam(@params))
			{
				Console.WriteLine();
				initBaseRaidArray();
			}
			else
			{
				initRaidRolette(@params[0]);
			}
		}

		internal virtual void initBaseRaidArray()
		{
			int _colorLength = HeroColor.Values.Length;
			colorRateArray = new int[_colorLength];
			colorRateArray[HeroColor.NIL.Index] = 0;
			for (int _i = 1; _i < _colorLength; _i++)
			{
				colorRateArray[_i] = BattleConstants.ROLETTE_BASE_RATE;
			}
		}

		internal virtual void initRaidRolette(string param)
		{
			string[] _rateStrArr = param.Split(",", true);
			HeroColor[] _colors = HeroColor.Values;
			int _length = _colors.Length;
			colorRateArray = new int[_length];
			colorRateArray[HeroColor.NIL.Index] = 0;
			for (int _i = 1; _i < _length; _i++)
			{
				colorRateArray[_i] = Convert.ToInt32(_rateStrArr[_i - 1]);
			}
		}

		internal virtual bool isRightParam(params string[] param)
		{
			if (param.Length < 1)
			{
				return false;
			}
			if (param[0] == null || param[0].Length == 0)
			{
				return false;
			}
			string[] _rateStr = param[0].Split(",", true);
			if (_rateStr.Length != HeroColor.Values.Length - 1)
			{
				return false;
			}
			return true;
		}

		internal override HeroColor randomColor()
		{
			int _index = MathUtils.rolette(colorRateArray);
			HeroColor _color = HeroColor.getValue(_index);
			return _color;
		}

	}

}