namespace com.kx.sglm.gs.battle.share.enums
{

	using IndexedEnum = com.kx.sglm.core.constant.IndexedEnum;

	public abstract class FighterState : IndexedEnum
	{



		private int index;
		/// <summary>
		/// 是否可出�?
		/// </summary>
		private bool fightAble;
		private bool skillAble;

		private FighterState(int index, bool fightAble, bool skillAble)
		{
			this.index = index;
			this.fightAble = fightAble;
			this.skillAble = skillAble;
		}

		public virtual int Index
		{
			get
			{
				return index;
			}
		}

		public virtual bool FightAble
		{
			get
			{
				return fightAble;
			}
		}


		public virtual bool SkillAble
		{
			get
			{
				return skillAble;
			}
		}
	}

}