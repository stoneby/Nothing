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