using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.kx.sglm.gs.battle.share.executer.impl
{


	using HeroColor = com.kx.sglm.gs.battle.share.enums.HeroColor;


	public class GreenhandPVEBattleExecuter : TestPVEBattleExecuter
	{

		private int colorIndex;
		private List<HeroColor> colorArray;
		private int spIndex;

		public GreenhandPVEBattleExecuter(Battle battle) : base(battle)
		{
			this.colorIndex = 0;
			colorArray = new List<HeroColor>();
			initTestColorArr();
		}

		public override void initDataOnCreate()
		{
			base.initDataOnCreate();
			attackerTeam().getActor(spIndex).addSpMaxBuff();
		}

		public virtual void initTemplInfo(Template.Auto.Greenhand.GreenhandTemplate tmpl)
		{
			if (tmpl == null)
			{
				initTestColorArr();
				return;
			}
			initTempColor(getTempParam(tmpl, BattleConstants.GREENHAND_TEMP_COLOR_INDEX));
			initTempSpIndex(getTempParam(tmpl, BattleConstants.GREENHAND_TEMP_SP_INDEX));
		}

		protected internal virtual string getTempParam(Template.Auto.Greenhand.GreenhandTemplate tmpl, int index)
		{
			List<string> _params = tmpl.OptionParams;
			return _params[index];
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

		internal virtual void initTempSpIndex(string spIndex)
		{
			this.spIndex = Convert.ToInt32(spIndex);
		}

		protected internal virtual void initTestColorArr()
		{
			colorArray.Add(HeroColor.RED);
			colorArray.Add(HeroColor.RED);
			colorArray.Add(HeroColor.RED);
			colorArray.Add(HeroColor.BLUE);
			colorArray.Add(HeroColor.GREEN);
			colorArray.Add(HeroColor.BLUE);
			colorArray.Add(HeroColor.YELLOW);
			colorArray.Add(HeroColor.RED);
			colorArray.Add(HeroColor.YELLOW);
			colorArray.Add(HeroColor.PINK);
			colorArray.Add(HeroColor.YELLOW);
			colorArray.Add(HeroColor.BLUE);
			colorArray.Add(HeroColor.BLUE);
			colorArray.Add(HeroColor.BLUE);
			colorArray.Add(HeroColor.BLUE);
			colorArray.Add(HeroColor.YELLOW);
			colorArray.Add(HeroColor.BLUE);
			colorArray.Add(HeroColor.PINK);
			colorArray.Add(HeroColor.GREEN);
			colorArray.Add(HeroColor.YELLOW);
			colorArray.Add(HeroColor.YELLOW);
			colorArray.Add(HeroColor.RED);
			spIndex = 10;
		}


		protected internal override HeroColor randomColor()
		{
		    //return HeroColor.BLUE;
			return colorIndex < colorArray.Count ? colorArray[colorIndex++] : base.randomColor();
		}



	}

}