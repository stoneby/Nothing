namespace com.kx.sglm.gs.battle.share.buff.policy
{


	public class BuffCoverPolicy : AbstractBuffPolicy
	{

		internal override BattleFighterBuff optionBuffByType(BattleBuffManager manager, BuffInfo info)
		{
			IBuffAction _action = info.BuffAction;
			BuffTypeHolder _holder = manager.getBuffTypeHolder(_action);
			BattleFighterBuff _buff = _holder.getBuff(_action.TypeB);
			BattleFighterBuff _resultBuff = null;
			IBuffAction _hadBuffAction = _buff.BuffAction;
			if (_action.priority() > _hadBuffAction.priority())
			{
				_resultBuff = replaceBuff(manager, _buff, info);
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
		protected internal virtual BattleFighterBuff replaceBuff(BattleBuffManager manager, BattleFighterBuff toReplaceBuff, BuffInfo buffInfo)
		{
			manager.removeSingleBuff(toReplaceBuff);
			BattleFighterBuff _resultBuff = manager.putToBuffHolder(buffInfo);
			return _resultBuff;
		}


	}

}