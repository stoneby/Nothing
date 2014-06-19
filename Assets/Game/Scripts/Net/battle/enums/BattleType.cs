namespace com.kx.sglm.gs.battle.share.enums
{

	using IndexedEnum = com.kx.sglm.core.constant.IndexedEnum;
	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using IBattleFactory = com.kx.sglm.gs.battle.share.factory.IBattleFactory;
	using TestBattleFactory = com.kx.sglm.gs.battle.share.factory.TestBattleFactory;

	/// <summary>
	/// ս������ö�١� ����ʹ�ó���������ΪҪת��C#���룬Javaö���޷�ת����C#����
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public abstract class BattleType : IndexedEnum
	{
		// TODO ���Factory���������ﴴ��������λ��
		public static readonly IBattleFactory TEST_FACTORY = new TestBattleFactory();

		/// <summary>
		/// �Զ�ս��flagλ������λ��ֵΪ1�������Զ�ս�� </summary>
		private const int AUTO_BATTLE_INDEX = 0x01;
		/// <summary>
		/// PVEս��flagλ�� </summary>
		private const int PVE_BATTLE_INDEX = 0X02;

		// /////////////////////////�������ж������key///////////////////////////////////
		/// <summary>
		/// ս���Ƿ��ѡ�񹥻�Index�����巽ʽΪPVEս������Ϊ���Զ�ս�� </summary>
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