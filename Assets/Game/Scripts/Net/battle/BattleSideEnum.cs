﻿namespace com.kx.sglm.gs.battle.enums
{

	using IndexedEnum = com.kx.sglm.core.constant.IndexedEnum;

	public abstract class BattleSideEnum : IndexedEnum
	{

		public static readonly BattleSideEnum SIDEA = new BattleSideEnumAnonymousInnerClassHelper();

		private class BattleSideEnumAnonymousInnerClassHelper : BattleSideEnum
		{
			public BattleSideEnumAnonymousInnerClassHelper() : base(0)
			{
			}

			public override BattleSideEnum DefaultOpponent
			{
				get
				{
					return SIDEB;
				}
			}
		}
		public static readonly BattleSideEnum SIDEB = new BattleSideEnumAnonymousInnerClassHelper2();

		private class BattleSideEnumAnonymousInnerClassHelper2 : BattleSideEnum
		{
			public BattleSideEnumAnonymousInnerClassHelper2() : base(1)
			{
			}

			public override BattleSideEnum DefaultOpponent
			{
				get
				{
					return SIDEA;
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

		private static readonly BattleSideEnum[] VALUES = new BattleSideEnum[] {SIDEA, SIDEB};

	}
}