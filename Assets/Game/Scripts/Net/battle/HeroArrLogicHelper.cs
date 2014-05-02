namespace com.kx.sglm.gs.battle.helper
{

	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using HeroPoint = com.kx.sglm.gs.battle.actor.impl.HeroPoint;
	using HeroTeam = com.kx.sglm.gs.battle.actor.impl.HeroTeam;
	using HeroColor = com.kx.sglm.gs.battle.enums.HeroColor;
	using PointDirection = com.kx.sglm.gs.battle.enums.PointDirection;

	/// <summary>
	/// 包含针对<seealso cref="HeroTeam"/>内的战斗序列Array的一些检查和生成操作
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class HeroArrLogicHelper
	{

		/// <summary>
		/// 初始化用方法，初始化所有的Index的可连接点的int_flag。<br>
		/// length 为3的情况，index顺序为<br>
		/// 0,1,2<br>
		/// 3,4,5<br>
		/// 6,7,8<br>
		/// 比如2的可连接点1,4,5,二进制为011010,转成int为26<br>
		/// 方法初始化在<seealso cref="BattleConstants#POINT_CONNECT_ARR"/>数组中，可以在战斗中直接使用
		/// </summary>
		/// <param name="length">
		/// @return </param>
		public static int[] creatHeroPointConnecter(int length)
		{
			int[] _connecterArr = new int[length];
			for (int _i = 0; _i < length; _i++)
			{
				foreach (PointDirection _dir in PointDirection.values())
				{
					int _changedIndex = _dir.getChangedIndex(_i);
					if (isRightPointIndex(length, _changedIndex))
					{
						_connecterArr[_i] = MathUtils.optionOrFlag(_connecterArr[_i], _changedIndex);
					}
				}
			}
			return _connecterArr;
		}

		/// <summary>
		/// 是否是正确的位置序列，包括1.是否合法，2.是否可连接，3.是否同一颜色<br>
		/// 这里为了保证方法的划分，没有使用全部的大循环做，可能会浪费一点性能。<br>
		/// 战斗内目前最长的链接为9，这个数量级理论上不会对性能造成太大消耗。
		/// </summary>
		/// <param name="actionArray"> </param>
		/// <param name="battlingHeroArr">
		/// @return </param>
		public static bool isRightActionArr(int[] actionArray, HeroPoint[] battlingHeroArr)
		{
			bool _rightAction = true;
			if (actionArray.Length < BattleConstants.HERO_LEAST_POINT)
			{
				return false;
			}
			_rightAction = isRightPointArr(actionArray);
			if (_rightAction)
			{
				_rightAction = isConnectAbleArray(actionArray);
			}
			if (_rightAction)
			{
				_rightAction = isSameColors(actionArray, battlingHeroArr);
			}
			return _rightAction;
		}

		/// <summary>
		/// 是否是合法的点序列。<br>
		/// 检查1.是否有重复； 2.点的值是否符合战斗的数组设定
		/// </summary>
		/// <param name="indexArr">
		/// @return </param>
		public static bool isRightPointArr(int[] indexArr)
		{
			int _apperedPoint = 0;
			bool _rightPointArr = true;
			foreach (int _index in indexArr)
			{
				if (!isRightPointIndex(BattleConstants.HERO_BATTLE_ARR_LENGTH, _index))
				{
					_rightPointArr = false;
					break;
				}
				if (MathUtils.hasFlagIndex(_apperedPoint, _index))
				{
					_rightPointArr = false;
					break;
				}
				else
				{
					_apperedPoint = MathUtils.optionOrFlag(_apperedPoint, _index);
				}
			}
			return _rightPointArr;
		}

		/// <summary>
		/// 是否是可连接的点序列
		/// </summary>
		/// <param name="indexArr">
		/// @returnz </param>
		public static bool isConnectAbleArray(int[] indexArr)
		{
			int _length = indexArr.Length;
			bool _connectAble = true;
			for (int _i = 1; _i < _length; _i++)
			{
				if (!isConnectablePoint(_i - 1, _i))
				{
					_connectAble = false;
					break;
				}
			}
			return _connectAble;
		}

		/// <summary>
		/// 点序列是否全部是同一颜色，并且非空
		/// </summary>
		/// <param name="indexArray"> </param>
		/// <param name="battlePoint">
		/// @return </param>
		public static bool isSameColors(int[] indexArray, HeroPoint[] battlePoint)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _length = indexArray.length;
			int _length = indexArray.Length;
			bool _sameColor = true;
			HeroColor _color = null;
			for (int _index = 0; _index < _length; _index++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _realIndex = indexArray[_index];
				int _realIndex = indexArray[_index];
				HeroColor _curColor = battlePoint[_realIndex].Color;
				if (_curColor == null) // 如果取出的颜色是空，直接返回错误
				{
					_sameColor = false;
				}
				else
				{
					if (_color == null)
					{
						_color = _curColor;
					}
					_sameColor = _curColor == _color;
				}
				if (!_sameColor)
				{
					break;
				}
			}
			return _sameColor;
		}

		/// <summary>
		/// 检查两个点是否是可连接的两点
		/// </summary>
		/// <param name="index"> </param>
		/// <param name="nextIndex">
		/// @return </param>
		public static bool isConnectablePoint(int index, int nextIndex)
		{
			if (!isRightPointIndex(index))
			{
				return false;
			}
			return MathUtils.hasFlagIndex(BattleConstants.POINT_CONNECT_ARR[index], nextIndex);
		}

		/// <summary>
		/// 一个点的数字是否合法
		/// </summary>
		/// <param name="index">
		/// @return </param>
		public static bool isRightPointIndex(int index)
		{
			return isRightPointIndex(BattleConstants.HERO_BATTLE_ARR_LENGTH, index);
		}

		/// <summary>
		/// 根据最长限制判断一个点的数字是否合法
		/// </summary>
		/// <param name="length"> </param>
		/// <param name="index">
		/// @return </param>
		public static bool isRightPointIndex(int length, int index)
		{
			return index >= 0 && index < length;
		}

	}

}