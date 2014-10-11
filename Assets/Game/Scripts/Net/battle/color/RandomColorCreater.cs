namespace com.kx.sglm.gs.battle.share.color
{

	using HeroColor = com.kx.sglm.gs.battle.share.enums.HeroColor;

	public class RandomColorCreater : AbstractColorCreater
	{

		public RandomColorCreater(Battle battle) : base(battle)
		{
		}

		internal override HeroColor randomColor()
		{
			return randomBaseColor();
		}

		public override void build(params string[] @params)
		{
			//do nothing
		}

	}

}