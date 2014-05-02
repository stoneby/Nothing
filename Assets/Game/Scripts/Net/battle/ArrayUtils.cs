namespace com.kx.sglm.gs.battle.utils
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
			return index >= 0 && index < array.Length;
		}

	}

}