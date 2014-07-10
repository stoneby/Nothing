namespace com.kx.sglm.gs.battle.share.buff.policy
{


	/// <summary>
	/// policy for stacking buff, stacking buff into exist buff instance
	/// @author liyuan2
	/// 
	/// </summary>
	public class BuffStackingPolicy : AbstractBuffPolicy
	{

		internal override BattleFighterBuff optionBuffByType(BattleBuffManager manager, IBuffAction buffAction)
		{
			BattleFighterBuff _buff = manager.getBattleBuff(buffAction);
			_buff.stackingBuff();
			return _buff;
		}

	}

}