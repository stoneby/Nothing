namespace com.kx.sglm.gs.battle.share.data.record
{

	/// <summary>
	/// 武将单个状�?br>
	/// 若{@link #leftRound}�?，则直接移除这个状�?
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class FighterStateRecord
	{

		private sbyte state;
		private sbyte leftRound;

		public virtual sbyte State
		{
			get
			{
				return state;
			}
			set
			{
				this.state = value;
			}
		}


		public virtual sbyte LeftRound
		{
			get
			{
				return leftRound;
			}
			set
			{
				this.leftRound = value;
			}
		}


	}

}