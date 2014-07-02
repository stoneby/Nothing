namespace com.kx.sglm.gs.battle.share.buff.policy
{


	/// <summary>
	/// TypeA和TypeB都不存在的加入BUFF策略
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