namespace com.kx.sglm.core.util
{


	public class IntFloatPair
	{

		private int key;
		private float value;

		public IntFloatPair(int key, float value)
		{
			this.key = key;
			this.value = value;
		}



		public virtual int Key
		{
			get
			{
				return key;
			}
		}



		public virtual float Value
		{
			get
			{
				return value;
			}
		}



		public static IntFloatPair[] newKeyValuePairArray(int size)
		{
			return new IntFloatPair[size];
		}

	}

}