namespace com.kx.sglm.gs.battle.share.data.record
{

	/// <summary>
	/// �佫����״̬<br>
	/// ��<seealso cref="#leftRound"/>Ϊ0����ֱ���Ƴ����״̬
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class FighterStateRecord
	{

		private int buffId;
		private sbyte state;
		private int showId;
		private sbyte leftRound;

		public virtual int BuffId
		{
			get
			{
				return buffId;
			}
			set
			{
				this.buffId = value;
			}
		}


		public virtual int ShowId
		{
			get
			{
				return showId;
			}
			set
			{
				this.showId = value;
			}
		}


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