using System;

namespace com.kx.sglm.gs.battle.share.buff.effect
{


	public class PropEffectDebuff : AbstractPropEffectBuff
	{


		protected internal override int getValue(string param)
		{
			return -Convert.ToInt32(param);
		}

	}

}