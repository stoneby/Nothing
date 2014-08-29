namespace com.kx.sglm.gs.battle.share.buff.policy
{


	/// <summary>
	/// policy for stacking buff, stacking buff into exist buff instance
	/// @author liyuan2
	/// 
	/// </summary>
	public class BuffStackingPolicy : AbstractBuffPolicy
	{

		internal override BattleFighterBuff optionBuffByType(BattleBuffManager manager, BuffInfo info)
		{
			BattleFighterBuff _buff = manager.getBattleBuff(info.BuffAction);
			_buff.stackingBuff(info);
			return _buff;
		}

	}

}