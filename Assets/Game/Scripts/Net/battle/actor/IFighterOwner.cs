namespace com.kx.sglm.gs.battle.share.actor
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;

	/// <summary>
	/// ÊôÓÚÄ³¸öFighter µÄ
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public interface IFighterOwner
	{

		BattleFighter Owner {get;}

	}

}