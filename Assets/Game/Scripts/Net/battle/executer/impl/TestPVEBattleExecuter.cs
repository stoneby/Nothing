namespace com.kx.sglm.gs.battle.share.executer.impl
{

	using RandomColorCreater = com.kx.sglm.gs.battle.share.color.RandomColorCreater;
	using BattleType = com.kx.sglm.gs.battle.share.enums.BattleType;

	/// <summary>
	/// ����ս������ִ����
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