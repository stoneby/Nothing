namespace com.kx.sglm.gs.role.properties.amend.model
{

	using FloatNumberPropertyObject = com.kx.sglm.core.model.FloatNumberPropertyObject;
	using IntNumberPropertyObject = com.kx.sglm.core.model.IntNumberPropertyObject;

	/// <summary>
	/// 属性修正规则三元组
	/// 
	/// @author fangyong
	/// @version 2014�?�?�?
	/// 
	/// </summary>
	public class AmendTriple
	{
		/// <summary>
		/// 用于标识被影响的属性类型及属性索�? </summary>
		public readonly Amend amend_Renamed;
		/// <summary>
		/// 计算规则 </summary>
		public readonly AbstractAmendCalcRule rule;
		/// <summary>
		/// 影响�? </summary>
		public readonly float value;

		public AmendTriple(Amend amend, AbstractAmendCalcRule rule, float value)
		{
			this.amend_Renamed = amend;
			this.rule = rule;
			this.value = value;
		}

		/// <summary>
		/// 计算对基准属性baseProperty的属性修正规�?并将计算结果累加到interimProperty�?
		/// </summary>
		/// <param name="baseProperty"> </param>
		/// <param name="interimProperty"> </param>
		public virtual void amend(FloatNumberPropertyObject baseProperty, FloatNumberPropertyObject interimProperty)
		{
			float _amendValue = this.rule.calculate(this.amend_Renamed, baseProperty, this.value);
			interimProperty.add(this.amend_Renamed.propertyIndex, _amendValue);
		}

		/// <summary>
		/// 计算对基准属性baseProperty的属性修正规�?并将计算结果累加到interimProperty�?
		/// </summary>
		/// <param name="baseProperty"> </param>
		/// <param name="interimProperty"> </param>
		public virtual void amend(IntNumberPropertyObject baseProperty, IntNumberPropertyObject interimProperty)
		{
			int _amendValue = this.rule.calculate(this.amend_Renamed, baseProperty, (int) this.value);
			interimProperty.add(this.amend_Renamed.propertyIndex, _amendValue);
		}

		/// <summary>
		/// 获得修正类别
		/// 
		/// @return
		/// </summary>
		public virtual int AmendType
		{
			get
			{
				return this.amend_Renamed.propertyType;
			}
		}

		/// <summary>
		/// 获得修正类别索引
		/// 
		/// @return
		/// </summary>
		public virtual int AmendIndex
		{
			get
			{
				return this.amend_Renamed.propertyIndex;
			}
		}

		/// <summary>
		/// 获得修正�?
		/// 
		/// @return
		/// </summary>
		public virtual float AmendValue
		{
			get
			{
				return this.value;
			}
		}

		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result + ((amend_Renamed == null) ? 0 : amend_Renamed.GetHashCode());
			result = prime * result + ((rule == null) ? 0 : rule.GetHashCode());
			result = prime * result + System.BitConverter.ToInt32(System.BitConverter.GetBytes(value),0);
			return result;
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
//ORIGINAL LINE: final AmendTriple other = (AmendTriple) obj;
			AmendTriple other = (AmendTriple) obj;
			if (amend_Renamed == null)
			{
				if (other.amend_Renamed != null)
				{
					return false;
				}
			}
			else if (!amend_Renamed.Equals(other.amend_Renamed))
			{
				return false;
			}
			if (rule == null)
			{
				if (other.rule != null)
				{
					return false;
				}
			}
			else if (rule.GetType() != other.rule.GetType())
			{
				return false;
			}
			if (System.BitConverter.ToInt32(System.BitConverter.GetBytes(value),0) != System.BitConverter.ToInt32(System.BitConverter.GetBytes(other.value),0))
			{
				return false;
			}
			return true;
		}
	}

}