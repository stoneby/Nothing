namespace com.kx.sglm.gs.battle.share.buff.policy
{


	/// <summary>
	/// TypeA��TypeB�������ڵļ���BUFF����
	/// @author liyuan2
	/// 
	/// </summary>
	public class BuffCoexistPolicy : AbstractBuffPolicy
	{

		internal override BattleFighterBuff optionBuffByType(BattleBuffManager manager, BuffInfo info)
		{
			manager.createBuffHolder(info.BuffAction);
			return manager.putToBuffHolder(info);
		}



	}

}