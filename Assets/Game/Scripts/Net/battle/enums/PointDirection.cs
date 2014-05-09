namespace com.kx.sglm.gs.battle.share.enums
{

	using IndexedEnum = com.kx.sglm.core.constant.IndexedEnum;

	/// <summary>
	/// 点的方向集合，没有使用枚举是因为无法转成c#的枚�?
	/// @author liyuan2
	/// 
	/// </summary>
	public abstract class PointDirection : IndexedEnum
	{
		public static readonly PointDirection UP = new PointDirectionAnonymousInnerClassHelper();

		private class PointDirectionAnonymousInnerClassHelper : PointDirection
		{
			public PointDirectionAnonymousInnerClassHelper() : base(0, -1)
			{
			}

		}
		public static readonly PointDirection DOWN = new PointDirectionAnonymousInnerClassHelper2();

		private class PointDirectionAnonymousInnerClassHelper2 : PointDirection
		{
			public PointDirectionAnonymousInnerClassHelper2() : base(1, 1)
			{
			}

		}
		public static readonly PointDirection LEFT = new PointDirectionAnonymousInnerClassHelper3();

		private class PointDirectionAnonymousInnerClassHelper3 : PointDirection
		{
			public PointDirectionAnonymousInnerClassHelper3() : base(2, 3)
			{
			}

		}
		public static readonly PointDirection RIGHT = new PointDirectionAnonymousInnerClassHelper4();

		private class PointDirectionAnonymousInnerClassHelper4 : PointDirection
		{
			public PointDirectionAnonymousInnerClassHelper4() : base(3, -3)
			{
			}

		}
		public static readonly PointDirection LEFTUP = new PointDirectionAnonymousInnerClassHelper5();

		private class PointDirectionAnonymousInnerClassHelper5 : PointDirection
		{
			public PointDirectionAnonymousInnerClassHelper5() : base(4, 2)
			{
			}

		}
		public static readonly PointDirection LEFTDOWN = new PointDirectionAnonymousInnerClassHelper6();

		private class PointDirectionAnonymousInnerClassHelper6 : PointDirection
		{
			public PointDirectionAnonymousInnerClassHelper6() : base(5, 4)
			{
			}

		}
		public static readonly PointDirection RIGHTUP = new PointDirectionAnonymousInnerClassHelper7();

		private class PointDirectionAnonymousInnerClassHelper7 : PointDirection
		{
			public PointDirectionAnonymousInnerClassHelper7() : base(6, -4)
			{
			}

		}
		public static readonly PointDirection RIGHTDOWN = new PointDirectionAnonymousInnerClassHelper8();

		private class PointDirectionAnonymousInnerClassHelper8 : PointDirection
		{
			public PointDirectionAnonymousInnerClassHelper8() : base(7, -2)
			{
			}

		}
		private int index;
		private int indexChange;

		private PointDirection(int index, int indexChange)
		{
			this.index = index;
			this.indexChange = indexChange;
		}

		public virtual int IndexChange
		{
			get
			{
				return indexChange;
			}
		}

		public virtual int getChangedIndex(int curIndex)
		{
			return curIndex + indexChange;
		}

		public virtual int Index
		{
			get
			{
				return index;
			}
		}

		private static readonly PointDirection[] VALUES = new PointDirection[] {UP, DOWN, LEFT, RIGHT, LEFTUP, LEFTDOWN, RIGHTUP, RIGHTDOWN};

		public static PointDirection[] values()
		{
			return VALUES;
		}
	}
}