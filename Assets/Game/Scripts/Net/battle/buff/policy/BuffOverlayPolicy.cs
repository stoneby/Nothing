namespace com.kx.sglm.gs.battle.share.buff.policy
{


	/// <summary>
	/// 存在TypeA但不存在TypeB
	/// @author liyuan2
	/// 
	/// </summary>
	public class BuffOverlayPolicy : AbstractBuffPolicy
	{

		internal override BattleFighterBuff optionBuffByType(BattleBuffManager manager, IBuffAction buffAction)
		{
			return manager.putToBuffHolder(buffAction);
		}

	}

}