namespace com.kx.sglm.gs.battle.data.record
{

	/// <summary>
	/// 武将单个状态<br>
	/// 若<seealso cref="#leftRound"/>为0，则直接移除这个状态
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