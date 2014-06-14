using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.utils
{


	public class ArrayUtils
	{

		public static void switchArray<T>(T[] array, int indexA, int indexB)
		{
			T _aT = array[indexA];
			array[indexA] = array[indexB];
			array[indexB] = _aT;
		}

		public static bool isRightArrayIndex(int index, object[] array)
		{
			return isRightArrayIndex(index, array.Length);
		}

		public static bool isRightArrayIndex(int index, int arraySize)
		{
			return index >= 0 && index < arraySize;
		}

		public static bool isEmpty<T>(List<T> list)
		{
			return list == null || list.Count == 0;
		}

	}

}