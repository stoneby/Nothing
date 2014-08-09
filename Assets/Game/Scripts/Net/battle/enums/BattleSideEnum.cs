namespace com.kx.sglm.gs.battle.share.enums
{

	using IndexedEnum = com.kx.sglm.core.constant.IndexedEnum;

	public abstract class BattleSideEnum : IndexedEnum
	{

		//TODO: modify to side left and side right 
		public static readonly BattleSideEnum SIDE_LEFT = new BattleSideEnumAnonymousInnerClassHelper();

		private class BattleSideEnumAnonymousInnerClassHelper : BattleSideEnum
		{
			public BattleSideEnumAnonymousInnerClassHelper() : base(1)
			{
			}

			public override BattleSideEnum DefaultOpponent
			{
				get
				{
					return SIDEB_RIGHT;
				}
			}
		}
		public static readonly BattleSideEnum SIDEB_RIGHT = new BattleSideEnumAnonymousInnerClassHelper2();

		private class BattleSideEnumAnonymousInnerClassHelper2 : BattleSideEnum
		{
			public BattleSideEnumAnonymousInnerClassHelper2() : base(0)
			{
			}

			public override BattleSideEnum DefaultOpponent
			{
				get
				{
					return SIDE_LEFT;
				}
			}
		}

		private BattleSideEnum(int index)
		{
			this.index = index;
		}

		protected internal int index;

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

		public abstract BattleSideEnum DefaultOpponent {get;}

		public static BattleSideEnum getBattleSide(int index)
		{
			return VALUES[index];
		}

		public static int size()
		{
			return VALUES.Length;
		}

		public static BattleSideEnum[] values()
		{
			return VALUES;
		}

		private static readonly BattleSideEnum[] VALUES = new BattleSideEnum[] {SIDEB_RIGHT, SIDE_LEFT};

	}
}