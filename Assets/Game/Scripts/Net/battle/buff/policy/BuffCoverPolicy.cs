namespace com.kx.sglm.gs.battle.share.buff.policy
{


	public class BuffCoverPolicy : AbstractBuffPolicy
	{

		internal override BattleFighterBuff optionBuffByType(BattleBuffManager manager, IBuffAction buffAction)
		{

			BuffTypeHolder _holder = manager.getBuffTypeHolder(buffAction);
			BattleFighterBuff _buff = _holder.getBuff(buffAction.TypeB);
			BattleFighterBuff _resultBuff = null;
			IBuffAction _hadBuffAction = _buff.BuffAction;
			if (buffAction.priority() > _hadBuffAction.priority())
			{
				_resultBuff = replaceBuff(manager, _buff, buffAction);
			}
			else
			{
				_resultBuff = _buff;
			}
			return _resultBuff;
		}

		/// <summary>
		/// 1.remove old buff; 2. add new buff </summary>
		/// <param name="manager"> </param>
		/// <param name="toReplaceBuff"> </param>
		/// <param name="addBuffAction">
		/// @return </param>
		protected internal virtual BattleFighterBuff replaceBuff(BattleBuffManager manager, BattleFighterBuff toReplaceBuff, IBuffAction addBuffAction)
		{
			manager.removeSingleBuff(toReplaceBuff);
			BattleFighterBuff _resultBuff = manager.putToBuffHolder(addBuffAction);
			return _resultBuff;
		}


	}

}