namespace com.kx.sglm.gs.battle.share.enums
{

	using IndexedEnum = com.kx.sglm.core.constant.IndexedEnum;
	using ArrayUtils = com.kx.sglm.gs.battle.share.utils.ArrayUtils;

	public abstract class HeroColor : IndexedEnum
	{
		public static readonly HeroColor NIL = new HeroColorAnonymousInnerClassHelper();

		private class HeroColorAnonymousInnerClassHelper : HeroColor
		{
			public HeroColorAnonymousInnerClassHelper() : base(0, false)
			{
			}

		}
		public static readonly HeroColor RED = new HeroColorAnonymousInnerClassHelper2();

		private class HeroColorAnonymousInnerClassHelper2 : HeroColor
		{
			public HeroColorAnonymousInnerClassHelper2() : base(1, false)
			{
			}

		}
		public static readonly HeroColor YELLOW = new HeroColorAnonymousInnerClassHelper3();

		private class HeroColorAnonymousInnerClassHelper3 : HeroColor
		{
			public HeroColorAnonymousInnerClassHelper3() : base(2, false)
			{
			}

		}
		public static readonly HeroColor GREEN = new HeroColorAnonymousInnerClassHelper4();

		private class HeroColorAnonymousInnerClassHelper4 : HeroColor
		{
			public HeroColorAnonymousInnerClassHelper4() : base(3, false)
			{
			}

		}
		public static readonly HeroColor BLUE = new HeroColorAnonymousInnerClassHelper5();

		private class HeroColorAnonymousInnerClassHelper5 : HeroColor
		{
			public HeroColorAnonymousInnerClassHelper5() : base(4, false)
			{
			}

		}
		public static readonly HeroColor PINK = new HeroColorAnonymousInnerClassHelper6();

		private class HeroColorAnonymousInnerClassHelper6 : HeroColor
		{
			public HeroColorAnonymousInnerClassHelper6() : base(5, true)
			{
			}

		}

		private int index;
		private bool recover;

		private HeroColor(int index, bool recover)
		{
			this.index = index;
			this.recover = recover;
		}

		public virtual int Index
		{
			get
			{
				return index;
			}
		}

		public virtual bool Recover
		{
			get
			{
				return recover;
			}
		}

		public static HeroColor[] Values
		{
			get
			{
				return VALUES;
			}
		}

		public static HeroColor getValue(int index)
		{
			if (!ArrayUtils.isRightArrayIndex(index, Values))
			{
				//TODO: loggers.error
				return NIL;
			}
			return Values[index];
		}

		private static readonly HeroColor[] VALUES = new HeroColor[] {NIL, RED, YELLOW, GREEN, BLUE, PINK};

	}
}