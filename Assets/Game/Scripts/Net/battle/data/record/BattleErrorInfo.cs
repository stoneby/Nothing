namespace com.kx.sglm.gs.battle.share.data.record
{

	public class BattleErrorInfo
	{

		private int errorType;
		private string errorInfo;

		public BattleErrorInfo(int errorType, string errorInfo)
		{
			this.errorInfo = errorInfo;
			this.errorType = errorType;
		}

		public virtual int ErrorType
		{
			get
			{
				return errorType;
			}
			set
			{
				this.errorType = value;
			}
		}
		public virtual string ErrorInfo
		{
			get
			{
				return errorInfo;
			}
			set
			{
				this.errorInfo = value;
			}
		}



	}

}