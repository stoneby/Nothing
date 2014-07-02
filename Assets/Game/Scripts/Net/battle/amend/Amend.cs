namespace com.kx.sglm.gs.role.properties.amend.model
{

	/// <summary>
	/// 属性修正功能定�?
	/// 
	/// @author hongfu1
	/// @author fangyong
	/// 
	/// </summary>
	public sealed class Amend
	{
		/// <summary>
		/// 属性的类型 </summary>
		public readonly int propertyType;
		/// <summary>
		/// 属性的索引 </summary>
		public readonly int propertyIndex;
		/// <summary>
		/// 预计算的hash code�? </summary>
		private readonly int hashCode_Renamed;
		/// <summary>
		/// 属性修正的符号,如A0 </summary>
		private readonly string symbol;

		public Amend(int propertyType, int propertyIndex, string symbol)
		{
			this.propertyType = propertyType;
			this.propertyIndex = propertyIndex;
			this.hashCode_Renamed = this.initHashCode();
			this.symbol = symbol;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (this.GetType() != obj.GetType())
			{
				return false;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Amend other = (Amend) obj;
			Amend other = (Amend) obj;
			if (propertyIndex != other.propertyIndex)
			{
				return false;
			}
			if (propertyType != other.propertyType)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return this.hashCode_Renamed;
		}

	//	public String toString() {
	//		return StringUtils.obj2String(this, null);
	//	}

		private int initHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result + propertyIndex;
			result = prime * result + propertyType;
			return result;
		}

		public string Symbol
		{
			get
			{
				return symbol;
			}
		}
	}

}