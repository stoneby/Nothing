namespace com.kx.sglm.gs.battle.share.enums
{

	using IndexedEnum = com.kx.sglm.core.constant.IndexedEnum;

	public abstract class BattleState : IndexedEnum
	{

		public static readonly BattleState HUNGUP = new BattleStateAnonymousInnerClassHelper();

		private class BattleStateAnonymousInnerClassHelper : BattleState
		{
			public BattleStateAnonymousInnerClassHelper() : base(0, true, false)
			{
			}

		}
		public static readonly BattleState RUNTIME = new BattleStateAnonymousInnerClassHelper2();

		private class BattleStateAnonymousInnerClassHelper2 : BattleState
		{
			public BattleStateAnonymousInnerClassHelper2() : base(1, false, false)
			{
			}

		}
		public static readonly BattleState STOP = new BattleStateAnonymousInnerClassHelper3();

		private class BattleStateAnonymousInnerClassHelper3 : BattleState
		{
			public BattleStateAnonymousInnerClassHelper3() : base(2, true, true)
			{
			}

		}
		public static readonly BattleState PREPAIR = new BattleStateAnonymousInnerClassHelper4();

		private class BattleStateAnonymousInnerClassHelper4 : BattleState
		{
			public BattleStateAnonymousInnerClassHelper4() : base(3, false, true)
			{
			}

		}

		private int index;
		private bool hangUp;
		private bool stoped;

		private BattleState(int index, bool hangUp, bool stoped)
		{
			this.index = index;
			this.hangUp = hangUp;
			this.stoped = stoped;
		}

		public virtual int Index
		{
			get
			{
				return index;
			}
		}

		public virtual bool HangUp
		{
			get
			{
				return hangUp;
			}
		}

		public virtual bool Stoped
		{
			get
			{
				return stoped;
			}
		}
	}
}