namespace com.kx.sglm.gs.battle.data.record
{

	/// <summary>
	/// 单个动作，是一个很多父动作公用的类
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class SingleActionRecord : AbstractBaseRecord
	{

		/// <summary>
		/// 动作类型
		/// </summary>
		private int actType;
		/// <summary>
		/// 目标阵营
		/// </summary>
		private int targetSide;
		/// <summary>
		/// 目标位置
		/// </summary>
		private int targetIndex;
		/// <summary>
		/// 目标结算HP
		/// </summary>
		private int resultHp;
		/// <summary>
		/// 显示顺序ID
		/// </summary>
		private int showIndex;

		/// <summary>
		/// 目标结束状态
		/// </summary>
		private int targetState;

		public virtual int ActType
		{
			get
			{
				return actType;
			}
			set
			{
				this.actType = value;
			}
		}


		public virtual int TargetSide
		{
			get
			{
				return targetSide;
			}
			set
			{
				this.targetSide = value;
			}
		}


		public virtual int TargetIndex
		{
			get
			{
				return targetIndex;
			}
			set
			{
				this.targetIndex = value;
			}
		}


		public virtual int ResultHp
		{
			get
			{
				return resultHp;
			}
			set
			{
				this.resultHp = value;
			}
		}


		public virtual int ShowIndex
		{
			get
			{
				return showIndex;
			}
			set
			{
				this.showIndex = value;
			}
		}


		public virtual int TargetState
		{
			get
			{
				return targetState;
			}
			set
			{
				this.targetState = value;
			}
		}


	}

}