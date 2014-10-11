namespace com.kx.sglm.gs.battle.share.enums
{

	using IndexedEnum = com.kx.sglm.core.constant.IndexedEnum;
	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using GreenhandPVEBattleFactory = com.kx.sglm.gs.battle.share.factory.GreenhandPVEBattleFactory;
	using IBattleFactory = com.kx.sglm.gs.battle.share.factory.IBattleFactory;
	using RaidPVEBattleFactory = com.kx.sglm.gs.battle.share.factory.RaidPVEBattleFactory;
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

		public static readonly IBattleFactory GREENHAND_FACTORY = new GreenhandPVEBattleFactory();

		public static readonly IBattleFactory RAID_FACTORY = new RaidPVEBattleFactory();

		/// <summary>
		/// �Զ�ս��flagλ������λ��ֵΪ1�������Զ�ս�� </summary>
		private const int AUTO_BATTLE_INDEX = 0x01;
		/// <summary>
		/// PVEս��flagλ�� </summary>
		private const int PVE_BATTLE_INDEX = 0X02;
		/// <summary>
		/// �Ƿ��е���flagλ�� </summary>
		private const int HAS_DROP = 0X04;

		// /////////////////////////�������ж������key///////////////////////////////////
		/// <summary>
		/// ս���Ƿ��ѡ�񹥻�Index�����巽ʽΪPVEս������Ϊ���Զ�ս�� </summary>
		private static readonly int TARGET_SELECTABLE = PVE_BATTLE_INDEX | (AUTO_BATTLE_INDEX & 0);

		public static readonly BattleType TESTPVE = new BattleTypeAnonymousInnerClassHelper(PVE_BATTLE_INDEX | HAS_DROP, TEST_FACTORY);

		private class BattleTypeAnonymousInnerClassHelper : BattleType
		{
			public BattleTypeAnonymousInnerClassHelper(int PVE_BATTLE_INDEX, IBattleFactory TEST_FACTORY) : base(0, PVE_BATTLE_INDEX | HAS_DROP, TEST_FACTORY)
			{
			}


		}

		public static readonly BattleType RAIDPVE = new BattleTypeAnonymousInnerClassHelper2(PVE_BATTLE_INDEX | HAS_DROP, RAID_FACTORY);

		private class BattleTypeAnonymousInnerClassHelper2 : BattleType
		{
			public BattleTypeAnonymousInnerClassHelper2(int PVE_BATTLE_INDEX, IBattleFactory RAID_FACTORY) : base(1, PVE_BATTLE_INDEX | HAS_DROP, RAID_FACTORY)
			{
			}


		}

		public static readonly BattleType GREENHANDPVE = new BattleTypeAnonymousInnerClassHelper3(GREENHAND_FACTORY);

		private class BattleTypeAnonymousInnerClassHelper3 : BattleType
		{
			public BattleTypeAnonymousInnerClassHelper3(IBattleFactory GREENHAND_FACTORY) : base(2, PVE_BATTLE_INDEX, GREENHAND_FACTORY)
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

		public virtual bool hasDrop()
		{
			return hasFlag(HAS_DROP);
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

		private static BattleType[] VALUES = new BattleType[] {TESTPVE, RAIDPVE, GREENHANDPVE};

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