using System;
using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.color
{


	using HeroColor = com.kx.sglm.gs.battle.share.enums.HeroColor;

	public class GreenhandColorCreater : AbstractColorCreater
	{

		private int colorIndex;
		private List<HeroColor> colorArray;


		public GreenhandColorCreater(Battle battle) : base(battle)
		{
			this.colorIndex = 0;
			colorArray = new List<HeroColor>();
		}

		internal override HeroColor randomColor()
		{
			return colorIndex < colorArray.Count ? colorArray[colorIndex++] : randomBaseColor();
		}


		protected internal virtual void initTempColor(string colors)
		{
			string[] _colorArr = colors.Split(",", true);
			foreach (string _colorStr in _colorArr)
			{
				addColorArr(_colorStr);
			}
		}



		internal virtual void addColorArr(string colorStr)
		{
			int _colorIndex = Convert.ToInt32(colorStr);
			HeroColor _color = HeroColor.getValue(_colorIndex);
			if (_color != null)
			{
				colorArray.Add(_color);
			}
		}

		public override void build(params string[] @params)
		{
			initTempColor(@params[0]);
		}

	}

}