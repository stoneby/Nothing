﻿namespace com.kx.sglm.gs.battle.enums
{

	using IndexedEnum = com.kx.sglm.core.constant.IndexedEnum;

	public abstract class BattleState : IndexedEnum
	{

		public static readonly BattleState HUNGUP = new BattleStateAnonymousInnerClassHelper();

		private class BattleStateAnonymousInnerClassHelper : BattleState
		{
			public BattleStateAnonymousInnerClassHelper() : base(0, true)
			{
			}

		}
		public static readonly BattleState RUNTIME = new BattleStateAnonymousInnerClassHelper2();

		private class BattleStateAnonymousInnerClassHelper2 : BattleState
		{
			public BattleStateAnonymousInnerClassHelper2() : base(1, false)
			{
			}

		}
		public static readonly BattleState STOP = new BattleStateAnonymousInnerClassHelper3();

		private class BattleStateAnonymousInnerClassHelper3 : BattleState
		{
			public BattleStateAnonymousInnerClassHelper3() : base(2, true)
			{
			}

		}

		private int index;
		private bool hangUp;

		private BattleState(int index, bool hangUp)
		{
			this.index = index;
			this.hangUp = hangUp;
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
	}
}