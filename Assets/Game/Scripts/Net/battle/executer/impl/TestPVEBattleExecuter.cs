namespace com.kx.sglm.gs.battle.share.executer.impl
{

	using RandomColorCreater = com.kx.sglm.gs.battle.share.color.RandomColorCreater;
	using BattleType = com.kx.sglm.gs.battle.share.enums.BattleType;

	/// <summary>
	/// 测试战斗流程执行器
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class TestPVEBattleExecuter : AbstractPVEBattleExecuter
	{

		public TestPVEBattleExecuter(Battle battle) : base(battle, new RandomColorCreater(battle))
		{
		}

		public override BattleType BattleType
		{
			get
			{
				return BattleType.TESTPVE;
			}
		}

	}

}