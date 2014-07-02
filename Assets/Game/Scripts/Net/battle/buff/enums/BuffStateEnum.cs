namespace com.kx.sglm.gs.battle.share.buff.enums
{

	using IndexedEnum = com.kx.sglm.core.constant.IndexedEnum;

	public abstract class BuffStateEnum : IndexedEnum
	{

		public static readonly BuffStateEnum NIL = new BuffStateEnumAnonymousInnerClassHelper();

		private class BuffStateEnumAnonymousInnerClassHelper : BuffStateEnum
		{
			public BuffStateEnumAnonymousInnerClassHelper() : base(0, false, false)
			{
			}


			public override BuffStateEnum nexState()
			{
				return NIL;
			}
		}

		public static readonly BuffStateEnum NEW_BORN = new BuffStateEnumAnonymousInnerClassHelper2();

		private class BuffStateEnumAnonymousInnerClassHelper2 : BuffStateEnum
		{
			public BuffStateEnumAnonymousInnerClassHelper2() : base(1, false, true)
			{
			}


			public override BuffStateEnum nexState()
			{
				return ACTIVE;
			}
		}

		public static readonly BuffStateEnum ACTIVE = new BuffStateEnumAnonymousInnerClassHelper3();

		private class BuffStateEnumAnonymousInnerClassHelper3 : BuffStateEnum
		{
			public BuffStateEnumAnonymousInnerClassHelper3() : base(2, true, false)
			{
			}


			public override BuffStateEnum nexState()
			{
				return DYING;
			}
		}

		public static readonly BuffStateEnum DYING = new BuffStateEnumAnonymousInnerClassHelper4();

		private class BuffStateEnumAnonymousInnerClassHelper4 : BuffStateEnum
		{
			public BuffStateEnumAnonymousInnerClassHelper4() : base(3, false, false)
			{
			}


			public override BuffStateEnum nexState()
			{
				return NIL;
			}
		}



		internal BuffStateEnum(int index, bool active, bool needActive)
		{
			this.index = index;
			this.active = active;
			this.needActive = needActive;
		}


		private int index;

		private bool active;

		private bool needActive;

		public abstract BuffStateEnum nexState();

		public virtual int Index
		{
			get
			{
				return index;
			}
		}

		public virtual bool Active
		{
			get
			{
				return active;
			}
		}

		public virtual bool NeedActive
		{
			get
			{
				return needActive;
			}
		}

		public virtual bool Dying
		{
			get
			{
				return !active && !needActive;
			}
		}
	}

}