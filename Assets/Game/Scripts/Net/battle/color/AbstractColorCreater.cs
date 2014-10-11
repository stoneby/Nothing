using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.color
{


	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using HeroColor = com.kx.sglm.gs.battle.share.enums.HeroColor;

	public abstract class AbstractColorCreater : IColorCreater
	{
		public abstract void build(params string[] @params);

		private Battle battle;

		public AbstractColorCreater(Battle battle)
		{
			this.battle = battle;
		}

		public virtual Battle Battle
		{
			get
			{
				return this.battle;
			}
		}

		public virtual List<HeroColor> createColorList(int createCount)
		{
			List<HeroColor> _list = new List<HeroColor>();
			for (int _i = 0; _i < createCount; _i++)
			{
				_list.Add(randomColor());
			}
			return _list;
		}

		internal abstract HeroColor randomColor();

		internal virtual HeroColor randomBaseColor()
		{
	//		int _index = MathUtils.random(HeroColor.RED.getIndex(), HeroColor.PINK.getIndex());
			int _index = MathUtils.random(HeroColor.RED.Index, HeroColor.RED.Index);
			return HeroColor.Values[_index];
		}

	}

}