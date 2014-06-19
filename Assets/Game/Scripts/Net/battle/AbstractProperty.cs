namespace com.kx.sglm.core.model
{


	public class AbstractProperty
	{
		/// <summary>
		/// 数值是否修改的标识 </summary>
		protected internal readonly SimpleBitSet bitSet;

		public AbstractProperty(int bitSize)
		{
			this.bitSet = new SimpleBitSet(bitSize);
		}

		/// <summary>
		/// 将当前的修改标识填充到toBitSet�?
		/// </summary>
		/// <param name="toBitSet"> </param>
		/// <returns> false,如果当前的属性没有修�?true,当前的属性有修改,并且已经将对应的值设置到toBitSet�? </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean fillChangedBit(final SimpleBitSet toBitSet)
		public virtual bool fillChangedBit(SimpleBitSet toBitSet)
		{
			if (this.bitSet.Empty)
			{
				return false;
			}
			else
			{
				toBitSet.or(this.bitSet);
				return true;
			}
		}

		/// <summary>
		/// 将当前的修改标识填充到toProp的BitSet�?
		/// </summary>
		/// <param name="toProp"> </param>
		/// <returns> false,如果当前的属性没有修�?true,当前的属性有修改,并且已经将对应的值设置到toBitSet�? </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean fillChangedBit(final AbstractProperty toProp)
		public virtual bool fillChangedBit(AbstractProperty toProp)
		{
			return fillChangedBit(toProp.bitSet);
		}

		/// <summary>
		/// 指定的索引是否有修改
		/// </summary>
		/// <param name="index"> </param>
		/// <returns> true,有修�?false,无修�? </returns>
		public virtual bool isChanged(int index)
		{
			return this.bitSet.get(index);
		}

		/// <summary>
		/// 强制设置索引位置为修�?
		/// <para>
		/// <b>一般情�?不建�?使用</b>
		/// </para>
		/// </summary>
		/// <param name="index"> </param>
		public virtual int Changed
		{
			set
			{
				this.bitSet.set(value);
			}
		}


		/// <summary>
		/// 总值计算产生变�?
		/// </summary>
		/// <param name="index"> </param>
		public virtual void totalValueChange(int index)
		{
			bitSet.set(index);
		}

	}
}