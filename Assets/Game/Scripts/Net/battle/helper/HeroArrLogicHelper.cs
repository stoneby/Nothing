namespace com.kx.sglm.gs.battle.share.helper
{

	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using HeroPoint = com.kx.sglm.gs.battle.share.actor.impl.HeroPoint;
	using HeroTeam = com.kx.sglm.gs.battle.share.actor.impl.HeroTeam;
	using HeroColor = com.kx.sglm.gs.battle.share.enums.HeroColor;
	using PointDirection = com.kx.sglm.gs.battle.share.enums.PointDirection;

	/// <summary>
	/// �������<seealso cref="HeroTeam"/>�ڵ�ս������Array��һЩ�������ɲ���
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class HeroArrLogicHelper
	{

		public static int getAttackRatio(int curIndex)
		{
			int _ratio = 0;
			if (curIndex < BattleConstants.BATTLE_HERO_INDEX_RATIO.Length)
			{
				_ratio = BattleConstants.BATTLE_HERO_INDEX_RATIO[curIndex];
			}
			return _ratio;
		}

		/// <summary>
		/// ��ʼ���÷�������ʼ�����е�Index�Ŀ����ӵ��int_flag��<br>
		/// length Ϊ3�������index˳��Ϊ<br>
		/// 0,1,2<br>
		/// 3,4,5<br>
		/// 6,7,8<br>
		/// ����2�Ŀ����ӵ�1,4,5,������Ϊ011010,ת��intΪ26<br>
		/// ������ʼ����<seealso cref="BattleConstants#POINT_CONNECT_ARR"/>�����У�������ս����ֱ��ʹ��
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
		/// �Ƿ�����ȷ��λ�����У�����1.�Ƿ�Ϸ���2.�Ƿ�����ӣ�3.�Ƿ�ͬһ��ɫ<br>
		/// ����Ϊ�˱�֤�����Ļ��֣�û��ʹ��ȫ���Ĵ�ѭ���������ܻ��˷�һ�����ܡ�<br>
		/// ս����Ŀǰ�������Ϊ9����������������ϲ�����������̫�����ġ�
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
		/// �Ƿ��ǺϷ��ĵ����С�<br>
		/// ���1.�Ƿ����ظ��� 2.���ֵ�Ƿ����ս���������趨
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
		/// �Ƿ��ǿ����ӵĵ�����
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
		/// �������Ƿ�ȫ����ͬһ��ɫ�����ҷǿ�
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
				if (_curColor == null) // ���ȡ������ɫ�ǿգ�ֱ�ӷ��ش���
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
		/// ����������Ƿ��ǿ����ӵ�����
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
		/// һ����������Ƿ�Ϸ�
		/// </summary>
		/// <param name="index">
		/// @return </param>
		public static bool isRightPointIndex(int index)
		{
			return isRightPointIndex(BattleConstants.HERO_BATTLE_ARR_LENGTH, index);
		}

		/// <summary>
		/// ����������ж�һ����������Ƿ�Ϸ�
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