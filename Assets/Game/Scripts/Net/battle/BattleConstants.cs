namespace com.kx.sglm.gs.battle.share
{

	using PointDirection = com.kx.sglm.gs.battle.share.enums.PointDirection;
	using HeroArrLogicHelper = com.kx.sglm.gs.battle.share.helper.HeroArrLogicHelper;
	using RoleAProperty = com.kx.sglm.gs.hero.properties.RoleAProperty;

	/// <summary>
	/// ս���߼���س���������һЩ��ֵ�ϵĳ�����û��Map�е�Key<br>
	/// �������key�����<seealso cref="BattleKeyConstants"/>
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleConstants
	{

		/// <summary>
		/// Ӣ�۶��������ο��
		/// </summary>
		public const int HERO_FIGHT_SIZE = 3;
		/// <summary>
		/// Ӣ�۶��������ܳ���
		/// </summary>
		public static readonly int HERO_BATTLE_ARR_LENGTH = HERO_FIGHT_SIZE * HERO_FIGHT_SIZE;

		/// <summary>
		/// ս�������и��ʶ�����ֱ�
		/// </summary>
		public const float BATTLE_RATIO_BASE = 10000.0f;

		/// <summary>
		/// ս��������������Ӫ
		/// </summary>
		public const int BATTLE_START_LEAST_SIDE = 2;

		public const int MIDDLE_POINT_INDEX = 4;

		/// <summary>
		/// -1ҲҪд�����������Ǹ�ǿ��֢����
		/// </summary>
		public const int BATTLE_FIGHTER_NON_INDEX = -1;

		public static readonly float[] BATTLE_HERO_INDEX_RATIO_FLOTA = new float[] {1.0f, 1.2f, 1.5f, 1.9f, 2.4f, 3.0f, 3.7f, 4.5f, 9.0f};

		public static readonly PointDirection[] MID_POINT_DIRECTION = new PointDirection[] {PointDirection.UP, PointDirection.DOWN, PointDirection.LEFT, PointDirection.RIGHT};
		/// <summary>
		/// ���������ӳ�����
		/// </summary>
		public static int[] BATTLE_HERO_INDEX_RATIO = HeroArrLogicHelper.initHeroIndexIntRatio(BATTLE_HERO_INDEX_RATIO_FLOTA, BATTLE_RATIO_BASE);

		/// <summary>
		/// ������������ </summary>
		public static readonly int[] BATTLE_MUTI_PROP_ARR = new int[] {RoleAProperty.DECRDAMAGE, RoleAProperty.INCRDAMAGE};

		/// <summary>
		/// ���ɵĿ����ӵ�����
		/// </summary>
		public static readonly int[] POINT_CONNECT_ARR = HeroArrLogicHelper.creatHeroPointConnecter(HERO_BATTLE_ARR_LENGTH);

		/// <summary>
		/// ���ӵ����ٵ� </summary>
		public const int HERO_LEAST_POINT = 1;

		/// <summary>
		/// ��Ҷ�������ٲ����佫 </summary>
		public const int HERO_TEAM_LEAST_FIGHTER = 12;

		/// <summary>
		/// ������ְҵ�������� </summary>
		public const int MONSTER_WEAK_RATIO = 2;

		/// <summary>
		/// Ĭ�Ϲ���Ŀ��index </summary>
		public const int MONSTER_DEFAULT_TARGET_INDEX = 0;

		/// <summary>
		/// ��������佫 </summary>
		public const int FIGHTER_TYPE_HERO = 1;
		/// <summary>
		/// ��������� </summary>
		public const int FIGHTER_TYPE_GUEST = 2;
		/// <summary>
		/// �����佫 </summary>
		public const int FIGHTER_TYPE_FREIND = 3;
		/// <summary>
		/// NPC�佫 </summary>
		public const int FIGHTER_TYPE_NPC = 4;

		public const int TEST_TOTAL_SP = 50;
		/// <summary>
		/// ���ԷŶӳ����ܵ�Index����0,1,2,9λ�õ��佫���Էż��� </summary>
		public const int FIGHTER_ACTIVE_SKILL_FLAG = 0x207;

		public const int FIGHTER_LEADER_SKILL_FLAG = 0x201;

		public const int FIGHTER_FIGHT_DEFAULT_COUNT = 1;

		public const float FIGHTER_FIGHT_DEFAULT_RATIO = 10000;

		public const int TARGET_GETTER_FOR_ENEMY_INDEX = 0;
		public const int TARGET_GETTER_FOR_FRIEND_INDEX = 1;
		/// <summary>
		/// buff��������� </summary>
		public const int BUFF_MAX_SIZE = 8;
		/// <summary>
		/// debuff��������� </summary>
		public const int DEBUFF_MAX_SIZE = 8;
		/// <summary>
		/// ����BUFF�������ֵ���� </summary>
		public static readonly int BUFF_ALL_MAX_SIZE = BUFF_MAX_SIZE + DEBUFF_MAX_SIZE;
		/// <summary>
		/// ֻ��DEBUFF���� </summary>
		public const int BUFF_FLAG = 0x01;
		/// <summary>
		/// ֻ��BUFF���� </summary>
		public const int DEBUFF_FALG = 0x02;
		/// <summary>
		/// ȫ��BUFF������ </summary>
		public static readonly int BUFF_ALL_FALG = BUFF_FLAG | DEBUFF_FALG;
		/// <summary>
		/// �佫��������Ѫ����1 </summary>
		public const int FIGHTER_ALIVE_MIN_HP = 1;
		/// <summary>
		/// �ɹ����ı�ʶ </summary>
		public const int ATTACK_DIS_FLAG = 0X01;
		/// <summary>
		/// ��ʹ�ü��ܵı�ʶ </summary>
		public const int SKILL_DIS_FLAG = 0X02;
		/// <summary>
		/// ������Ϊ0 </summary>
		public const int ATTACK_ZERO_FLAG = 0X04;
		/// <summary>
		/// ������Ϊ0 </summary>
		public const int SP_MAX_FALG = 0X08;
		/// <summary>
		/// ���� </summary>
		public const int MONSTER_SHIELD_FLAG = 0X10;
		/// <summary>
		/// ȫ�������Զ�����flag </summary>
		public const int DISABLE_FLAG = 0xffff;
		/// <summary>
		/// ��С����ֵΪ1�����Ʒ�ʱʹ�� </summary>
		public const float MIN_ATTACK = 1.0F;
		/// <summary>
		/// ��û�й��ﻤ��ʱ��flag </summary>
		public const int MONSTER_SHIELD_NIL_VALUE = 0;
		/// <summary>
		/// �����ṩ�ļ��˱��� </summary>
		public const float MONSTER_SHIELD_DAMAGE_REDUCE = 2000;

	}

}