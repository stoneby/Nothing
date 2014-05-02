namespace com.kx.sglm.gs.battle.enums
{

	using IndexedEnum = com.kx.sglm.core.constant.IndexedEnum;
	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using IBattleFactory = com.kx.sglm.gs.battle.factory.IBattleFactory;
	using TestBattleFactory = com.kx.sglm.gs.battle.factory.TestBattleFactory;

	/// <summary>
	/// 战斗类型枚举。 这里使用抽象类是因为要转成C#代码，Java枚举无法转换成C#代码
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public abstract class BattleType : IndexedEnum
	{
		// TODO 这个Factory不该在这里创建，另找位置
		public static readonly IBattleFactory TEST_FACTORY = new TestBattleFactory();

		/// <summary>
		/// 自动战斗flag位，若该位数值为1，则是自动战斗 </summary>
		private const int AUTO_BATTLE_INDEX = 0x01;
		/// <summary>
		/// PVE战斗flag位， </summary>
		private const int PVE_BATTLE_INDEX = 0X02;

		// /////////////////////////以下是判断用组合key///////////////////////////////////
		/// <summary>
		/// 战斗是否可选择攻击Index，具体方式为PVE战斗并且为非自动战斗 </summary>
		private static readonly int TARGET_SELECTABLE = PVE_BATTLE_INDEX | (AUTO_BATTLE_INDEX & 0);

		public static readonly BattleType TESTPVE = new BattleTypeAnonymousInnerClassHelper(TEST_FACTORY);

		private class BattleTypeAnonymousInnerClassHelper : BattleType
		{
			public BattleTypeAnonymousInnerClassHelper(IBattleFactory TEST_FACTORY) : base(0, PVE_BATTLE_INDEX, TEST_FACTORY)
			{
			}


		}

		public static readonly BattleType RAIDPVE = new BattleTypeAnonymousInnerClassHelper2();

		private class BattleTypeAnonymousInnerClassHelper2 : BattleType
		{
			public BattleTypeAnonymousInnerClassHelper2() : base(1, PVE_BATTLE_INDEX, null)
			{
			}


		}
		protected internal int index;

		protected internal IBattleFactory factory;

		protected internal int battleFlags;

		private BattleType(int index, int battleFlags, IBattleFactory factory)
		{
			this.index = index;
			this.battleFlags = battleFlags;
			this.factory = factory;
		}

		public virtual bool canSelectTarget()
		{
			return hasFlag(TARGET_SELECTABLE);
		}

		public virtual bool AutoBattle
		{
			get
			{
				return hasFlag(AUTO_BATTLE_INDEX);
			}
		}

		public virtual bool PveBattle
		{
			get
			{
				return hasFlag(PVE_BATTLE_INDEX);
			}
		}

		public virtual IBattleFactory Factory
		{
			get
			{
				return factory;
			}
		}

		public virtual int Index
		{
			get
			{
				return index;
			}
		}

		public virtual sbyte IndexByte
		{
			get
			{
				return (sbyte) index;
			}
		}

		private static BattleType[] VALUES = new BattleType[] {TESTPVE, RAIDPVE};

		public static BattleType[] values()
		{
			return VALUES;
		}

		public virtual bool hasFlag(int flagValue)
		{
			return MathUtils.andFlag(battleFlags, flagValue);
		}

		public static BattleType getValue(int index)
		{
			return values()[index];
		}
	}
}