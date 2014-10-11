using System;
using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.executer.impl
{

	using IColorCreater = com.kx.sglm.gs.battle.share.color.IColorCreater;
	using BattleType = com.kx.sglm.gs.battle.share.enums.BattleType;

	public class GreenhandPVEBattleExecuter : AbstractPVEBattleExecuter
	{

		private int spIndex;

		public GreenhandPVEBattleExecuter(Battle battle, IColorCreater attackerColorCreater) : base(battle, attackerColorCreater)
		{
		}

		public override void initDataOnCreate()
		{
			base.initDataOnCreate();
			attackerTeam().getActor(spIndex).addSpMaxBuff();
		}

		public virtual void initTemplInfo(Template.Auto.Greenhand.GreenhandTemplate tmpl)
		{
			if (tmpl != null)
			{
				AttackerColorCreater.build(getTempParam(tmpl, BattleConstants.GREENHAND_TEMP_COLOR_INDEX));
				initTempSpIndex(getTempParam(tmpl, BattleConstants.GREENHAND_TEMP_SP_INDEX));
			}
		}

		protected internal virtual string getTempParam(Template.Auto.Greenhand.GreenhandTemplate tmpl, int index)
		{
			List<string> _params = tmpl.OptionParams;
			return _params[index];
		}

		internal virtual void initTempSpIndex(string spIndex)
		{
			this.spIndex = Convert.ToInt32(spIndex);
		}

		public override BattleType BattleType
		{
			get
			{
				return BattleType.GREENHANDPVE;
			}
		}

	}

}