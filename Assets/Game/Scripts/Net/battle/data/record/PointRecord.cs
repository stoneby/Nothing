namespace com.kx.sglm.gs.battle.share.data.record
{

	public class PointRecord
	{

		private int index;
		private int color;

		public PointRecord(int index, int color)
		{
			this.index = index;
			this.color = color;
		}


		public virtual int Index
		{
			get
			{
				return index;
			}
			set
			{
				this.index = value;
			}
		}
		public virtual int Color
		{
			get
			{
				return color;
			}
			set
			{
				this.color = value;
			}
		}



	}

}