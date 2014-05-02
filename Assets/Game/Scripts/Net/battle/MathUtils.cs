using System;
using System.Collections.Generic;
using System.Text;

namespace com.kx.sglm.core.util
{


	public class MathUtils
	{

		public const float EPSILON = 0.00004f; // 再把误差调得大一点,现在这样,在150级时百万次检查大概会出现8次超出误差值

		/// <summary>
		/// 返回>=low, <=hi的整数随机数，均匀分布
		/// </summary>
		/// <param name="low"> </param>
		/// <param name="hi">
		/// @return </param>
		public static int random(int low, int hi)
		{
			return (int)(low + (hi - low + 0.9) * new Random(1).NextDouble());
		}

		/// <summary>
		/// 返回>=low, <hi的浮点随机数，均匀分布
		/// </summary>
		/// <param name="low"> </param>
		/// <param name="hi">
		/// @return </param>
		public static float random(float low, float hi)
		{
			float result = (float)(low + (hi - low) * new Random(1).NextDouble());
			if (result >= hi || result < low)
			{
				result = low;
			}
			return result;
		}

		/// <summary>
		/// 返回是否满足概率值。
		/// </summary>
		/// <param name="shakeNum">
		///            float 概率值 0.0---1.0 </param>
		/// <returns> 比如某操作有２０％的概率，shakeNum=0.2 如果返回true表明概率满足。 </returns>
		public static bool shake(float shakeNum)
		{
			if (shakeNum >= 1)
			{
				return true;
			}
			if (shakeNum <= 0)
			{
				return false;
			}

			double a = new Random(1).NextDouble();
			return a < shakeNum;
		}

		/// <summary>
		/// 从概率数组中挑选一个概率
		/// </summary>
		/// <param name="rateAry"> </param>
		/// <returns> 返回被选中的概率数组索引,-1,没有符合概率的;0~rateAry.length-1,符合概率的数组索引 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static int luckyDraw(final float[] rateAry)
		public static int luckyDraw(float[] rateAry)
		{
			if (rateAry == null)
			{
				return -1;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] balls = new int[rateAry.length * 2];
			int[] balls = new int[rateAry.Length * 2];
			int pt = 0;
			int _ballsLength = 0;
			for (int i = 0, j = 0; i < rateAry.Length; i += 1, j += 2)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int mulRate = (int)(rateAry[i] * 100) - 1;
				int mulRate = (int)(rateAry[i] * 100) - 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _s = j;
				int _s = j;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _e = j + 1;
				int _e = j + 1;
				if (mulRate < 0)
				{
					balls[_s] = -1;
					balls[_e] = -1;
				}
				else
				{
					balls[_s] = pt;
					int _end = pt + mulRate;
					if (_end > 99)
					{
						_end = 99;
					}
					balls[_e] = _end;
					pt = _end + 1;
				}
				_ballsLength++;
				if (pt > 99)
				{
					break;
				}
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _rnd = random(0, 99);
			int _rnd = random(0, 99);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _ballsLengthReal = _ballsLength * 2;
			int _ballsLengthReal = _ballsLength * 2;
			for (int i = 0; i < _ballsLengthReal; i += 2)
			{
				if (balls[i] <= _rnd && balls[i + 1] >= _rnd)
				{
					return i / 2;
				}
			}
			return -1;
		}

		/// <summary>
		/// 轮盘赌 建议仅在不确定选择库的时候使用此函数。如果已经知道要从什么里面选，建议事先加好轮盘赌概率
		/// </summary>
		/// <param name="rateAry">
		///            概率数组 </param>
		/// <returns> 选中的下标 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static int rolette(final int[] rateAry)
		public static int rolette(int[] rateAry)
		{
			if (rateAry == null)
			{
				return -1;
			}

			if (rateAry.Length == 0)
			{
				return -1;
			}

			int[] itemListPer = new int[rateAry.Length];
			itemListPer[0] = rateAry[0];
			for (int i = 1; i < rateAry.Length; i++)
			{
				itemListPer[i] = itemListPer[i - 1] + rateAry[i];
			}
			int maxper = itemListPer[itemListPer.Length - 1];
			int per = MathUtils.random(0, maxper - 1);
			if ((per >= 0) && (per < itemListPer[0]))
			{
				return 0;
			}
			else
			{
				for (int j = 1; j < itemListPer.Length; j++)
				{
					if ((per >= itemListPer[j - 1]) && (per < itemListPer[j]))
					{
						return j;
					}
				}
				return -1;
			}
		}

		public static int parseInt(object input, int defaultValue)
		{
			if (input == null)
			{
				return defaultValue;
			}
			try
			{
				return Convert.ToInt32(input.ToString());
			}
			catch (Exception)
			{
			}
			return defaultValue;
		}

		public static int compareFloat(float f1, float f2)
		{
			float delta = f1 - f2;
			if (Math.Abs(delta) > EPSILON)
			{
				if (delta > 0)
				{
					return 1; // f1> f2
				}
				else if (delta < 0)
				{
					return -1; // f1<f2
				}
			}
			return 0; // f1==f2
		}

		public static int compareToByDay(DateTime dayone, DateTime daytwo)
		{
			if (dayone.Year > daytwo.Year)
			{
				return 1;
			}
			else if (dayone.Year < daytwo.Year)
			{
				return -1;
			}
			else
			{
				if (dayone.Month > daytwo.Month)
				{
					return 1;
				}
				else if (dayone.Month < daytwo.Month)
				{
					return -1;
				}
				else
				{
					if (dayone.Day > daytwo.Day)
					{
						return 1;
					}
					else if (dayone.Day < daytwo.Day)
					{
						return -1;
					}
					else
					{
						return 0;
					}
				}
			}
		}

		/// <summary>
		/// 计算两个日期间相差的天数(按24小时算)
		/// </summary>
		/// <param name="enddate"> </param>
		/// <param name="begindate">
		/// @return </param>
		public static int getIntervalDays(DateTime enddate, DateTime begindate)
		{
			long millisecond = enddate.Ticks - begindate.Ticks;
			int day = (int)(millisecond / 24l / 60l / 60l / 1000l);
			return day;
		}

		/// <summary>
		/// 计算两个日期间相差的天数(按24小时算)
		/// </summary>
		/// <param name="enddate"> </param>
		/// <param name="begindate">
		/// @return </param>
		public static int getIntervalDays(long enddate, long begindate)
		{
			long millisecond = enddate - begindate;
			int day = (int)(millisecond / 24l / 60l / 60l / 1000l);
			return day;
		}

		/// <summary>
		/// 计算两个日期间相差的分钟数
		/// </summary>
		/// <param name="enddate"> </param>
		/// <param name="begindate">
		/// @return </param>
		public static int getIntervalMinutes(DateTime enddate, DateTime begindate)
		{
			long millisecond = enddate.Ticks - begindate.Ticks;
			int minute = (int)(millisecond / 60l / 1000l);
			return minute;
		}

		/// <summary>
		/// 限置为 >=min <=max的值
		/// </summary>
		/// <param name="original"> </param>
		/// <param name="min"> </param>
		/// <param name="max">
		/// @return </param>
		public static int setBetween(int original, int min, int max)
		{
			if (original > max)
			{
				original = max;
			}
			if (original < min)
			{
				original = min;
			}
			return original;
		}

		/// <param name="ary1"> </param>
		/// <param name="ary2"> </param>
		/// <returns> ary1 >= ary2 true else false </returns>
		public static bool compareArrays(int[] ary1, int[] ary2)
		{
			if (ary1 != null && ary2 != null)
			{
				if (ary1.Length == ary2.Length)
				{
					for (int i = 0; i < ary1.Length; i++)
					{
						if (ary1[i] < ary2[i])
						{
							return false;
						}
					}
				}
			}

			return true;
		}

		public static int float2Int(float f)
		{
			return (int)(f + 0.5f);
		}

		/// <summary>
		/// 比较两个float是否相等，用<seealso cref="Float#equals()"/>实现
		/// </summary>
		/// <param name="floatA"> </param>
		/// <param name="floatB">
		/// @return </param>
		public static bool floatEquals(float floatA, float floatB)
		{
			return ((float?) floatA).Equals(floatB);
		}

		/// <summary>
		/// 获取两数相除的结果,精确到小数
		/// </summary>
		/// <param name="num"> </param>
		/// <param name="deno">
		/// @return </param>
		public static float doDiv(int numerator, int denominator)
		{
			if (denominator != 0)
			{
				return numerator / (denominator + 0.0f);
			}
			return 0f;
		}

		public static float doDiv(float numerator, float denominator)
		{
			if (denominator != 0)
			{
				return numerator / (denominator);
			}
			return 0f;
		}

		/// <summary>
		/// 两个正整数相加
		/// </summary>
		/// <param name="n1">
		///            第一个参数 </param>
		/// <param name="n2">
		///            第二个参数 </param>
		/// <returns> 相加后的结果 </returns>
		/// <exception cref="IllegalArgumentException">
		///                ,如果n1或者n2有一个负数,则会抛出此异常;如果n1与n2相加后的结果是负数,即溢出了,也会抛出此异常 </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static int addPlusNumber(final int n1, final int n2)
		public static int addPlusNumber(int n1, int n2)
		{
			if (n1 < 0 || n2 < 0)
			{
				throw new System.ArgumentException("Both n1 and n2 must be plus,but n1=" + n1 + " and n2 =" + n2);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _sum = n1 + n2;
			int _sum = n1 + n2;
			if (_sum < 0)
			{
				throw new System.ArgumentException("Add n1 and n2 must be plus,but n1+n2=" + _sum);
			}
			return _sum;
		}

		/// <summary>
		/// 获得不重复的随机数，取值范围(>=min, <=max)，个数size
		/// </summary>
		/// <param name="min"> </param>
		/// <param name="max"> </param>
		/// <param name="size">
		/// @return </param>
		public static IList<int?> getRandomIntWithoutRepeat(int min, int max, int size)
		{
			if (min > max)
			{
				throw new System.ArgumentException(string.Format("min({0}) >= max({1})", min, max));
			}
			int _arraySize = max - min + 1;
			if (_arraySize < size)
			{
				throw new System.ArgumentException(string.Format("max({0}) - min({1}) >= size({2})", min, max, size));
			}
			IList<int?> _result = new List<int?>(size);
			int[] _intArray = new int[_arraySize];
			for (int i = 0; i < _arraySize; i++)
			{
				_intArray[i] = i + min;
			}
			for (int i = 0; i < size; i++)
			{
				int _index = random(min, max - i) - min;
				int _temp = _intArray[_index];
				_intArray[_index] = _intArray[_arraySize - 1 - i];
				_intArray[_arraySize - 1 - i] = _temp;
				_result.Add(_intArray[_arraySize - 1 - i]);
			}
			return _result;
		}

		/// <summary>
		/// 取多个整数当中的最小值
		/// </summary>
		/// <param name="args">
		/// @return </param>
		public static int min(params int[] args)
		{
			if (args.Length <= 0)
			{
				throw new System.ArgumentException("argument size should be positive");
			}
			int _min = int.MaxValue;
			foreach (int _arg in args)
			{
				if (_arg < _min)
				{
					_min = _arg;
				}
			}
			return _min;
		}

		/// <summary>
		/// 取多个整数当中的最大值
		/// </summary>
		/// <param name="args">
		/// @return </param>
		public static int max(params int[] args)
		{
			if (args.Length <= 0)
			{
				throw new System.ArgumentException("argument size should be positive");
			}
			int _max = int.MinValue;
			foreach (int _arg in args)
			{
				if (_arg > _max)
				{
					_max = _arg;
				}
			}
			return _max;
		}

		/// <summary>
		/// 转化成16进制并补全零
		/// </summary>
		/// <param name="value">
		/// @return </param>
		public static string toHexString(long value)
		{
			string _tmp = value.ToString("x");
			int _size0 = 16 - _tmp.Length;
			StringBuilder _sb = new StringBuilder();
			for (int i = 0; i < _size0; i++)
			{
				_sb.Append('0');
			}
			return _sb.ToString() + _tmp;
		}

		/// <summary>
		/// 取符号
		/// </summary>
		/// <param name="value"> </param>
		public static int sign(int value)
		{
			return value > 0 ? 1 : value < 0 ? - 1 : 0;
		}

		/// <summary>
		/// 取符号
		/// </summary>
		/// <param name="value"> </param>
		public static int sign(long value)
		{
			return value > 0 ? 1 : value < 0 ? - 1 : 0;
		}

		/// <summary>
		/// 计算以 base 为底的 value 值的对数
		/// </summary>
		/// <param name="value"> </param>
		/// <param name="base">
		/// @return </param>
		public static double log(double value, double @base)
		{
			return Math.Log(value) / Math.Log(@base);
		}

		/// <summary>
		/// 求不大于num的最大2的次幂
		/// </summary>
		/// <param name="num">
		/// @return </param>
		public static int get2pow(int num)
		{
			int _result = num & num - 1;
			return _result == 0 ? num : get2pow(_result);
		}

		/// <summary>
		/// 取得一个正整数的位数
		/// </summary>
		/// <param name="num">
		/// @return </param>
		public static int getBit(int num)
		{
			if (num < 0)
			{
				return 0;
			}
			int count = 0;
			while (num != 0)
			{
				num = num >> 1;
				count++;
			}
			return count;
		}

		/// <summary>
		/// 取得一个正整数为1的位数
		/// </summary>
		/// <param name="num">
		/// @return </param>
		public static int getMaskBit(int num)
		{
			if (num < 0)
			{
				return 0;
			}
			int count = 0;
			while (num != 0)
			{
				if ((num & 1) == 1)
				{
					count++;
				}
				num = num >> 1;
			}
			return count;
		}

		/// <summary>
		/// 功能等同于Math.ceil
		/// </summary>
		/// <param name="src"> </param>
		/// <param name="dep">
		/// @return </param>
		public static int ceil(int src, int dep)
		{
			if (src < 0)
			{
				return -1;
			}
			if (dep <= 0)
			{
				return -1;
			}
			int i = src / dep;
			int j = src % dep;
			if (j == 0)
			{
				return i;
			}
			return i + 1;
		}

		/// <summary>
		/// 对一个Int的某一位进行与(and)操作
		/// </summary>
		/// <param name="baseValue"> </param>
		/// <param name="andIndex">
		/// @return </param>
		public static int optionAndFlag(int baseValue, int andIndex)
		{
			return optionAndFlag(baseValue, andIndex, true);
		}

		/// <summary>
		/// 对一个Int的某一位进行或(or)操作
		/// </summary>
		/// <param name="baseValue"> </param>
		/// <param name="orIndex">
		/// @return </param>
		public static int optionOrFlag(int baseValue, int orIndex)
		{
			return baseValue | (1 << orIndex);
		}

		public static int optionAndFlag(int baseValue, int andIndex, bool flag)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _flagVal = flag ? 1 : 0;
			int _flagVal = flag ? 1 : 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _checkVal = _flagVal << andIndex;
			int _checkVal = _flagVal << andIndex;
			return baseValue & _checkVal;
		}

		/// 
		/// <summary>
		/// 检测某一位Index上是否为1 </summary>
		/// <param name="baseValue"> </param>
		/// <param name="checkFlagIndex">
		/// @return </param>
		public static bool hasFlagIndex(int baseValue, int checkFlagIndex)
		{
			return optionAndFlag(baseValue, checkFlagIndex) > 0;
		}

		/// <summary>
		/// 直接判断两个值的与值 </summary>
		/// <param name="baseValue"> </param>
		/// <param name="checkValue">
		/// @return </param>
		public static bool andFlag(int baseValue, int checkValue)
		{
			return (checkValue & baseValue) > 0;
		}

	}

}