namespace com.kx.sglm.gs.battle.share.data.store
{

	public class BattleStoreConstants
	{


		private static int BATTLE_STORE_DATA_INDEX = 0;
		public static readonly int BATTLE_PROCESS_STORE_DATA = BATTLE_STORE_DATA_INDEX++;
		public static readonly int BATTLE_RESULT_STORE_DATA = BATTLE_STORE_DATA_INDEX++;
		public static readonly int BATTLE_STORE_DATA_SIZE = BATTLE_STORE_DATA_INDEX;

		private static int BATTLE_STORE_KEY_BASE = 0;

		public static int BATTLE_STORE_MAX_FIGHT = BATTLE_STORE_KEY_BASE++;
		public static int BATTLE_STORE_CUR_SCENE_INDEX = BATTLE_STORE_KEY_BASE++;
		public static int BATTLE_STORE_CUR_HERO_HP = BATTLE_STORE_KEY_BASE++;
		public static int BATTLE_STORE_CUR_SP_INDEX_LIST = BATTLE_STORE_KEY_BASE++;
		public static int BATTLE_STORE_CUR_HERO_MP = BATTLE_STORE_KEY_BASE++;

		public static string BATTLE_STORE_TYPE_SPLIT = "|";
		public static string BATTLE_STORE_DATA_SPLIT = ";";
		public static string BATTLE_STORE_KEY_SPLIT = ",";

	}

}