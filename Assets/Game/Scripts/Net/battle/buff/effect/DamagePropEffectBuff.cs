using System;

namespace com.kx.sglm.gs.battle.share.buff.effect
{




	public class DamagePropEffectBuff : AbstractDamagePropEffectBuff
	{


		protected internal override int getValue(string param)
		{
			return Convert.ToInt32(param);
		}
	}

}