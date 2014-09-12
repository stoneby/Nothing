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