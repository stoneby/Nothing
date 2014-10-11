namespace com.kx.sglm.gs.battle.share.executer.impl
{

	using IColorCreater = com.kx.sglm.gs.battle.share.color.IColorCreater;
	using BattleType = com.kx.sglm.gs.battle.share.enums.BattleType;

	public class RaidPVEBattleExecuter : AbstractPVEBattleExecuter
	{

		public RaidPVEBattleExecuter(Battle battle, IColorCreater attackerColorCreater) : base(battle, attackerColorCreater)
		{
		}

		public override BattleType BattleType
		{
			get
			{
				return BattleType.RAIDPVE;
			}
		}

	}

}