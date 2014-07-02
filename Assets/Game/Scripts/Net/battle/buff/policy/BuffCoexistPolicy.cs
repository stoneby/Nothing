namespace com.kx.sglm.gs.battle.share.buff.policy
{


	/// <summary>
	/// TypeA��TypeB�������ڵļ���BUFF����
	/// @author liyuan2
	/// 
	/// </summary>
	public class BuffCoexistPolicy : AbstractBuffPolicy
	{

		internal override BattleFighterBuff optionBuffByType(BattleBuffManager manager, IBuffAction buffAction)
		{
			manager.createBuffHolder(buffAction);
			return manager.putToBuffHolder(buffAction);
		}



	}

}