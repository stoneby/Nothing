namespace com.kx.sglm.gs.battle.share.buff.policy
{


	/// <summary>
	/// policy for unchanged buff, refresh buff round only
	/// @author liyuan2
	/// 
	/// </summary>
	public class BuffUnchangedPolicy : AbstractBuffPolicy
	{

		internal override BattleFighterBuff optionBuffByType(BattleBuffManager manager, IBuffAction buffAction)
		{
			BattleFighterBuff _buff = manager.getBattleBuff(buffAction);
			_buff.resetRound();
			return _buff;
		}

	}

}