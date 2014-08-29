namespace com.kx.sglm.gs.battle.share.buff.enums
{

	using IndexedEnum = com.kx.sglm.core.constant.IndexedEnum;
	using BuffCoexistPolicy = com.kx.sglm.gs.battle.share.buff.policy.BuffCoexistPolicy;
	using BuffCoverPolicy = com.kx.sglm.gs.battle.share.buff.policy.BuffCoverPolicy;
	using BuffOverlayPolicy = com.kx.sglm.gs.battle.share.buff.policy.BuffOverlayPolicy;
	using BuffStackingPolicy = com.kx.sglm.gs.battle.share.buff.policy.BuffStackingPolicy;
	using IBuffAddPolicy = com.kx.sglm.gs.battle.share.buff.policy.IBuffAddPolicy;

	/// <summary>
	/// BUFF添加时不同的逻辑关系
	/// @author liyuan2
	/// 
	/// </summary>
	public abstract class BuffPolicyEnum : IndexedEnum
	{

		private static readonly BuffCoexistPolicy BUFF_COEXIT_POLICY = new BuffCoexistPolicy();

		private static readonly BuffOverlayPolicy BUFF_OVERLAY_POLICY = new BuffOverlayPolicy();

		private static readonly BuffCoverPolicy BUFF_COVER_POLICY = new BuffCoverPolicy();

		private static readonly BuffStackingPolicy BUFF_STACKING_POLICY = new BuffStackingPolicy();


		/// <summary>
		///共存： !contain(typeA) && !contain(typeB) </summary>
		public static readonly BuffPolicyEnum COEXIST = new BuffPolicyEnumAnonymousInnerClassHelper(BUFF_COEXIT_POLICY);

		private class BuffPolicyEnumAnonymousInnerClassHelper : BuffPolicyEnum
		{
			public BuffPolicyEnumAnonymousInnerClassHelper(BuffCoexistPolicy BUFF_COEXIT_POLICY) : base(1, BUFF_COEXIT_POLICY)
			{
			}

		}

		/// <summary>
		///叠加： !contain(typeA) && contain(typeB) </summary>
		public static readonly BuffPolicyEnum OVERLAY = new BuffPolicyEnumAnonymousInnerClassHelper2(BUFF_OVERLAY_POLICY);

		private class BuffPolicyEnumAnonymousInnerClassHelper2 : BuffPolicyEnum
		{
			public BuffPolicyEnumAnonymousInnerClassHelper2(BuffOverlayPolicy BUFF_OVERLAY_POLICY) : base(2, BUFF_OVERLAY_POLICY)
			{
			}

		}

		/// <summary>
		///顶替： contain(typeA) && contain(typeB) && !contain(buffId) </summary>
		public static readonly BuffPolicyEnum COVER = new BuffPolicyEnumAnonymousInnerClassHelper3(BUFF_COVER_POLICY);

		private class BuffPolicyEnumAnonymousInnerClassHelper3 : BuffPolicyEnum
		{
			public BuffPolicyEnumAnonymousInnerClassHelper3(BuffCoverPolicy BUFF_COVER_POLICY) : base(2, BUFF_COVER_POLICY)
			{
			}

		}

		/// <summary>
		///加层数： contain(buffId) </summary>
		public static readonly BuffPolicyEnum STACKING = new BuffPolicyEnumAnonymousInnerClassHelper4(BUFF_STACKING_POLICY);

		private class BuffPolicyEnumAnonymousInnerClassHelper4 : BuffPolicyEnum
		{
			public BuffPolicyEnumAnonymousInnerClassHelper4(BuffStackingPolicy BUFF_STACKING_POLICY) : base(3, BUFF_STACKING_POLICY)
			{
			}

		}


		public BuffPolicyEnum(int index, IBuffAddPolicy policy)
		{
			this.index = index;
			this.policy = policy;
		}

		private int index;

		private IBuffAddPolicy policy;

		public virtual int Index
		{
			get
			{
				return index;
			}
		}


		public virtual void addBuff(BattleBuffManager manager, BuffInfo info)
		{
			IBuffAddPolicy _policy = Policy;
			_policy.optionBuff(manager, info);
		}

		public virtual IBuffAddPolicy Policy
		{
			get
			{
				return policy;
			}
		}


	}

}