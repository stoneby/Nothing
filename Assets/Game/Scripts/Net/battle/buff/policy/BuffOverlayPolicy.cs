namespace com.kx.sglm.gs.battle.share.buff.policy
{


	/// <summary>
	/// ����TypeA��������TypeB
	/// @author liyuan2
	/// 
	/// </summary>
	public class BuffOverlayPolicy : AbstractBuffPolicy
	{

		internal override BattleFighterBuff optionBuffByType(BattleBuffManager manager, BuffInfo info)
		{
			return manager.putToBuffHolder(info);
		}

	}

}