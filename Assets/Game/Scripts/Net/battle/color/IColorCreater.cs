using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.color
{

	using HeroColor = com.kx.sglm.gs.battle.share.enums.HeroColor;

	public interface IColorCreater : IBattle
	{

		List<HeroColor> createColorList(int createCount);

		void build(params string[] @params);

	}

}