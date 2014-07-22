using UnityEngine;
using System;
using System.Collections.Generic;


#if UNITY_IPHONE
using LitJson;
using System.Runtime.InteropServices;
#endif


public sealed class MtaService : MonoBehaviour {

	// Use this for initialization
	void Start () {
//		MtaService.setDebugEnable(true);
//		MtaService.setInstallChannel("play");
//		MtaService.startStatServiceWithAppKey("Aqc12345");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	
	/******************** 统计接口开始  ********************************/
	/**
	 * 启动MTA统计，设置APPKEY，APPKEY为MTA分配的key；如果是合作方SDK，则为按MTA规则生成的APPKEY
	 */
	public static bool StartStatServiceWithAppKey(string appkey)
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
#if UNITY_IPHONE
		_startStatServiceWithAppKey (appkey);
		return true;
#elif UNITY_ANDROID
		AndroidJavaClass consts_cls = new AndroidJavaClass("com.tencent.stat.common.StatConstants");
		string ver = consts_cls.GetStatic<string> ("VERSION");
		return MtaServiceImpl.CallStatic<bool> ("startStatService", Context, appkey, ver);
#endif
		}
		return false;
	}
	/**
	 * 页面统计：开始
	 */
	public static void TrackBeginPage(string page_name)
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_trackBeginPage (page_name);
			#elif UNITY_ANDROID
			MtaServiceImpl.CallStatic ("trackBeginPage", Context, page_name);
			#endif
		}

	}
	/**
	 * 页面统计：结果
	 */
	public static void TrackEndPage(string page_name)
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_trackEndPage (page_name);
			#elif UNITY_ANDROID
			MtaServiceImpl.CallStatic ("trackEndPage", Context, page_name);
			#endif
		}
	}
	/**
	 * 带次数统计的自定义K-V事件，event_id需要在前台注册
	 */
	public static void TrackCustomKVEvent(string event_id, Dictionary<string, string> dict)
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_trackCustomKVEvent (event_id, JsonMapper.ToJson(dict));
			#elif UNITY_ANDROID
			MtaServiceImpl.CallStatic ("trackCustomKVEvent", Context, event_id, DictToJavaProperties(dict));
			#endif
		}

	}
	/**
	 * 带时长统计的自定义K-V事件，event_id需要在前台注册
	 * begin和end需匹配使用,event_id和prop要完全一致
	 */
	public static void TrackCustomBeginKVEvent(string event_id, Dictionary<string, string> dict)
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_trackCustomBeginKVEvent (event_id, JsonMapper.ToJson(dict));
			#elif UNITY_ANDROID
			MtaServiceImpl.CallStatic ("trackCustomBeginKVEvent", Context, event_id, DictToJavaProperties(dict));
			#endif
		}

	}
	/**
	 * 带时长统计的自定义K-V事件，event_id需要在前台注册
	 * begin和end需匹配使用,event_id和prop要完全一致
	 */
	public static void TrackCustomEndKVEvent(string event_id, Dictionary<string, string> dict)
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_trackCustomEndKVEvent (event_id, JsonMapper.ToJson(dict));
			#elif UNITY_ANDROID
			MtaServiceImpl.CallStatic ("trackCustomEndKVEvent", Context, event_id, DictToJavaProperties(dict));
			#endif
		}
	}
	/**
	 * 上报QQ号码，用于用户画像(机器人项目)等分析
	 */
	public static void ReportQQ(string qq_account)	
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_reportQQ (qq_account);
			#elif UNITY_ANDROID
			MtaServiceImpl.CallStatic ("reportQQ", Context, qq_account);
			#endif
		}
	}
	/**
	 * 上报监控接口，记得初始化monitor并对所有成员变量赋值！
	 */
	public static void ReportAppMonitorStat(MtaAppMonitor monitor)	
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			Dictionary<string,object> dict = new Dictionary<string,object> ();
			dict.Add ("interfaceName", monitor.InterfaceName);
			dict.Add ("requestPackageSize", monitor.RequestSize);
			dict.Add ("responsePackageSize", monitor.ResponseSize);
			dict.Add ("resultType", monitor.ResultType);
			dict.Add ("millisecondsConsume", monitor.MillisecondsConsume);
			dict.Add ("returnCode", monitor.ReturnCode);
			dict.Add ("sampling", monitor.Sampling);
			
			_reportAppMonitorStat (JsonMapper.ToJson(dict));
			#elif UNITY_ANDROID
			if(monitor == null || monitor.InterfaceName == null || monitor.InterfaceName.Equals("")){
				return;
			}
			AndroidJavaObject obj = new AndroidJavaObject("com.tencent.stat.StatAppMonitor", 
			                                              monitor.InterfaceName, monitor.ResultType,  monitor.ReturnCode,
			                                              monitor.RequestSize, monitor.ResponseSize,
			                                              monitor.MillisecondsConsume, monitor.Sampling);
			MtaServiceImpl.CallStatic ("reportAppMonitorStat", Context, obj);
			#endif
		}
	}
	/**
	 * 上报游戏用户
	 */
	public static void ReportGameUser(MtaGameUser gameUser)	
	{


		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			Dictionary<string,string> dict = new Dictionary<string,string> ();
			dict.Add ("account", gameUser.Account);
			dict.Add ("worldName", gameUser.WorldName);
			dict.Add ("level", gameUser.Level);

			_reportGameUser (JsonMapper.ToJson(dict));
			#elif UNITY_ANDROID
			if(gameUser == null){
				return;
			}
			AndroidJavaObject obj = new AndroidJavaObject("com.tencent.stat.StatGameUser", 
			                                              gameUser.Account, gameUser.WorldName, gameUser.Level);
			MtaServiceImpl.CallStatic ("reportGameUser", Context, obj);
			#endif
		}
	}
	/**
	 * 上报错误
	 */
	public static void ReportError(string error_msg)	
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_reportError (error_msg);
			#elif UNITY_ANDROID
			if(error_msg == null || error_msg.Length == 0){
				return;
			}
			MtaServiceImpl.CallStatic ("reportError", Context, error_msg);
			#endif
		}
	}
	
	/**
	 * 上报本地事件，counts为上报的数量
	 */
	public static void CommitEvents(int counts)	
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_commitEvents (counts);
			#elif UNITY_ANDROID
			MtaServiceImpl.CallStatic ("commitEvents", Context, counts);
			#endif
		}
	}
	/**
	 * 启动新的会话，不建议APP主动调用！
	 */
	public static void StartNewSession()
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_startNewSession();
			#elif UNITY_ANDROID
			MtaServiceImpl.CallStatic ("startNewSession", Context);
			#endif
		}
	}
	/**
	 * 结束当时Session，不建议APP主动调用！
	 */
	public static void StopSession()
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_stopSession();
			#elif UNITY_ANDROID
			MtaServiceImpl.CallStatic ("stopSession");
			#endif
		}
	}
	/******************** 统计接口 结束 ********************************/
	
	
	/******************** 配置接口 开始 ********************************/
	// !!!除getCustomProperty之外的set类型配置接口需要在统计接口前被调用才能及时生效 !!!//
	
	/**
	 * 是否开启debug模式，可在log的MtaSDK标签查看日志，默认关闭
	 * ！！！发布时请关闭掉（设置为false）！！！
	 */
	public static void SetDebugEnable(bool enable)
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_setDebugEnable (enable);
			#elif UNITY_ANDROID
			MtaConfigImpl.CallStatic ("setDebugEnable", enable);
			#endif
		}
	}
	/**
	 * 设置APPKEY，APPKEY为MTA分配的key；如果是合作方SDK，则为按MTA规则生成的APPKEY
	 */
	public static void SetAppKey(string appKey)
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_setAppKey (appKey);
			#elif UNITY_ANDROID
			MtaConfigImpl.CallStatic ("setAppKey", Context, appKey);
			#endif
		}
	}
	/**
	 * 设置APP的下发渠道，如在应用宝发布可填写“应用宝”，appstore则写appstore
	 */
	public static void SetInstallChannel(string channelName)
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_setInstallChannel (channelName);
			#elif UNITY_ANDROID
			MtaConfigImpl.CallStatic ("setInstallChannel", channelName);
			#endif
		}
	}
	/**
	 * 设置上报策略，默认为启动时发送
	 */
	public static void SetStatSendStrategy(MTAStatReportStrategy strategy)
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_setStatSendStrategy ((int)strategy);
			#elif UNITY_ANDROID
			AndroidJavaClass enumClass = new AndroidJavaClass("com.tencent.stat.StatReportStrategy");
			AndroidJavaObject obj = enumClass.CallStatic<AndroidJavaObject>("valueOf", strategy.ToString());
			MtaConfigImpl.CallStatic ("setStatSendStrategy", obj);
			#endif
		}
	}
	/**
	 * 是否开户未处理的java异常捕获，默认开户
	 */
	public static void SetAutoExceptionCaught(bool enable)
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_setAutoExceptionCaught (enable);
			#elif UNITY_ANDROID
			MtaConfigImpl.CallStatic ("setAutoExceptionCaught", enable);
			#endif
		}
	}
	/**
	 * 设置APP自己的用户id
	 */
	public static void SetCustomUserId(string userId)
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_setCustomUserId (userId);
			#elif UNITY_ANDROID
			MtaConfigImpl.CallStatic ("setCustomUserId", Context, userId);
			#endif
		}
	}
	/**
	 * 是否开启wifi下实时发送，默认开启
	 */
	public static void SetEnableSmartReporting(bool enable)
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_setEnableSmartReporting (enable);
			#elif UNITY_ANDROID
			MtaConfigImpl.CallStatic ("setEnableSmartReporting", enable);
			#endif
		}
	}
	/**
	 * 最大的重试次数，默认为3，可根据需要调整
	 * 当上报失败次数超过此值时，本地消息将会被丢弃
	 */
	public static void SetMaxSendRetryCount(int max_count)
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_setMaxSendRetryCount (max_count);
			#elif UNITY_ANDROID
			MtaConfigImpl.CallStatic ("setMaxSendRetryCount", max_count);
			#endif
		}
	}
	/**
	 * 本地缓存日志最大条数，默认为1024
	 * 当达到此值时，新日志将被丢弃
	 */
	public static void SetMaxStoreEventCount(int max_count)
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_setMaxStoreEventCount (max_count);
			#elif UNITY_ANDROID
			MtaConfigImpl.CallStatic ("setMaxStoreEventCount", max_count);
			#endif
		}
	}
	/**
	 * MTA_STRATEGY_PERIOD间隔上报策略间隔时间，单位分钟
	 */
	public static void SetSendPeriodMinutes(int minutes)
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_setSendPeriodMinutes (minutes);
			#elif UNITY_ANDROID
			MtaConfigImpl.CallStatic ("setSendPeriodMinutes", minutes);
			#endif
		}
	}
	/**
	 * session的超时时间，默认为30秒
	 */
	public static void SetSessionTimoutMillis(int ms)
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_setSessionTimoutMillis (ms);
			#elif UNITY_ANDROID
			MtaConfigImpl.CallStatic ("setSessionTimoutMillis", ms);
			#endif
		}
	}
	/**
	 * 进程周期内，最大上报的session个数
	 */
	public static void SetMaxSessionStatReportCount(int max_count)
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_setMaxSendRetryCount (max_count);
			#elif UNITY_ANDROID
			MtaConfigImpl.CallStatic ("setMaxSessionStatReportCount", max_count);
			#endif
		}
	}
	/**
	 * 是否开启MTA，默认为开启
	 * 若为false，MTA服务将不可用
	 */
	public static void SetEnableStatService(bool enable)
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_setEnableStatService (enable);
			#elif UNITY_ANDROID
			MtaConfigImpl.CallStatic ("setEnableStatService", enable);
			#endif
		}
	}
	/**
	 * 批量上报的最大日志数
	 */
	public static void SetMaxBatchReportCount(int count)
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_setMaxBatchReportCount (count);
			#elif UNITY_ANDROID
			MtaConfigImpl.CallStatic ("setMaxBatchReportCount", count);
			#endif
		}
	}
	/**
	 * begin和end类型的统计功能最大并行个数
	 */
	public static void SetMaxParallelTimmingEvents(int max)
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_setMaxParallelTimmingEvents (max);
			#elif UNITY_ANDROID
			MtaConfigImpl.CallStatic ("setMaxParallelTimmingEvents", max);
			#endif
		}
	}
	/**
	 * 单个事件最大的字节数，默认为4096Bytes，即4kB
	 */
	public static void SetMaxReportEventLength(int max_length)
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			_setMaxReportEventLength (max_length);
			#elif UNITY_ANDROID
			MtaConfigImpl.CallStatic ("setMaxReportEventLength", max_length);
			#endif
		}
	}
	/**
	 * 获取前台配置的在线参数，若找不到key则返回NULL
	 */
	public static string GetCustomProperty(string key)
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			return _getCustomProperty (key, null);
			#elif UNITY_ANDROID
			if(key == null || key.Length == 0){
				return null;
			}
			return MtaConfigImpl.CallStatic<string> ("getCustomProperty", key);
			#endif
		}
		return null;
	}
	/**
	 * 获取前台配置的在线参数，若找不到key则返回defaultValue
	 */
	public static string GetCustomProperty(string key,
	                                       string defaultValue)
	{
		if(Application.platform != RuntimePlatform.OSXEditor){
			#if UNITY_IPHONE
			return _getCustomProperty (key, defaultValue);
			#elif UNITY_ANDROID
			if(key == null || key.Length == 0){
				return null;
			}
			return MtaConfigImpl.CallStatic<string> ("getCustomProperty", key, defaultValue);
			#endif
		}
		return defaultValue;
	}
	

	public readonly String VERSION = "1.0.0";
	
	public enum MTAStatReportStrategy{
	 	// 实时发送策略：app产生的消息会马上上报
		INSTANT = 1,
		//WIFI发送策略：只有WIFI时才发送消息
		ONLY_WIFI = 2,
		// 批量发送策略：当消息个数达到某个值时才上报
		BATCH = 3,
		// 应用启动时发送策略：仅当app启动时才上报
		APP_LAUNCH = 4,
		// 开发者模式策略：只有当commitEvents()被显式调用时才上报
		DEVELOPER = 5,
	 	// 间隔时间发送策略：间隔1天（默认值）将之前所有缓存在本地的消息一次性上报至服务器。
		PERIOD = 6
	};
#if UNITY_IPHONE
	[DllImport ("__Internal")]
	public static extern void  _reportError(String error_msg);
	
	[DllImport ("__Internal")]
	public static extern void _trackBeginPage(String page_name);
	
	[DllImport ("__Internal")]
	public static extern void _trackEndPage(String page_name);
	
	[DllImport ("__Internal")]
	public static extern void _reportQQ(String qq_account);
	
	[DllImport ("__Internal")]
	public static extern void _startNewSession();
	
	[DllImport ("__Internal")]
	public static extern bool _startStatServiceWithAppKey(String appkey);
	
	[DllImport ("__Internal")]
	public static extern void _stopSession();
	
	[DllImport ("__Internal")]
	public static extern void _commitEvents(int counts);
	
	[DllImport ("__Internal")]
	public static extern void _reportGameUser(String jsonGameUser);
	
	[DllImport ("__Internal")]
	public static extern void _reportAppMonitorStat(String jsonMonitor);
	
	[DllImport ("__Internal")]
	public static extern void _trackCustomKVEvent(String event_id, String jsonProp);
	
	[DllImport ("__Internal")]
	public static extern void _trackCustomBeginKVEvent(String event_id, String jsonProp);
	
	[DllImport ("__Internal")]
	public static extern void _trackCustomEndKVEvent(String event_id, String jsonProp);
	
	[DllImport ("__Internal")]
	public static extern void _setAppKey(String appKey);
	
	[DllImport ("__Internal")]
	public static extern void _setAutoExceptionCaught(bool enable);
	
	[DllImport ("__Internal")]
	public static extern void _setCustomUserId(String userId);
	
	[DllImport ("__Internal")]
	public static extern void _setDebugEnable(bool enable);
	
	[DllImport ("__Internal")]
	public static extern void _setEnableSmartReporting(bool enable);
	
	[DllImport ("__Internal")]
	public static extern void _setInstallChannel(String channelName);
	
	[DllImport ("__Internal")]
	public static extern void _setStatSendStrategy(int strategy);
	
	[DllImport ("__Internal")]
	public static extern void _setMaxSendRetryCount(int max_count);
	
	[DllImport ("__Internal")]
	public static extern void _setMaxStoreEventCount(int max_count);
	
	[DllImport ("__Internal")]
	public static extern void _setSendPeriodMinutes(int minutes);
	
	[DllImport ("__Internal")]
	public static extern void _setSessionTimoutMillis(int ms);
	
	[DllImport ("__Internal")]
	public static extern void _setMaxSessionStatReportCount(int max_count);
	
	[DllImport ("__Internal")]
	public static extern void _setEnableStatService(bool enable);
	
	[DllImport ("__Internal")]
	public static extern void _setMaxBatchReportCount(int count);
	
	[DllImport ("__Internal")]
	public static extern void _setMaxParallelTimmingEvents(int max);
	
	[DllImport ("__Internal")]
	public static extern void _setMaxReportEventLength(int max_length);
	
	[DllImport ("__Internal")]
	public static extern String  _getCustomProperty(String key, String defaultValue);
#elif UNITY_ANDROID
	static class MtaServiceJava{
		public static AndroidJavaClass serviceInstance;
		public static AndroidJavaClass configInstance;
		public static AndroidJavaObject contextInstance;
		
		static MtaServiceJava()
		{
			configInstance = new AndroidJavaClass("com.tencent.stat.StatConfig");
			serviceInstance = new AndroidJavaClass("com.tencent.stat.StatService");
			using(AndroidJavaClass context_cls = new AndroidJavaClass("com.unity3d.player.UnityPlayer")){
				contextInstance = context_cls.GetStatic<AndroidJavaObject>("currentActivity");
			}
		}
	}

	static AndroidJavaClass MtaServiceImpl
	{
		get{
			return MtaServiceJava.serviceInstance;
		}
	}

	static AndroidJavaClass MtaConfigImpl
	{
		get{
			return MtaServiceJava.configInstance;
		}
	}

	static AndroidJavaObject Context
	{
		get{
			return MtaServiceJava.contextInstance;
		}
	}

	private static AndroidJavaObject DictToJavaProperties(Dictionary<string, string> dict)
	{
		var hashMap = new AndroidJavaObject( "java.util.Properties" );
		var putMethod = AndroidJNIHelper.GetMethodID( hashMap.GetRawClass(), "setProperty", "(Ljava/lang/String;Ljava/lang/String;)Ljava/lang/Object;" );
		var arguments = new object[2];
		foreach( var entry in dict )
		{
			using( var key = new AndroidJavaObject( "java.lang.String", entry.Key ) )
			{
				using( var val = new AndroidJavaObject( "java.lang.String", entry.Value ) )
				{
					arguments[0] = key;
					arguments[1] = val;
					AndroidJNI.CallObjectMethod( hashMap.GetRawObject(), putMethod, AndroidJNIHelper.CreateJNIArgArray( arguments ) );
				}
			} 
		} 
		
		return hashMap;
	}

	private static AndroidJavaObject ToJavaHashMap(Dictionary<string,int> dic)
	{
		AndroidJavaObject map = new AndroidJavaObject( "java.util.HashMap" );
		IntPtr put = AndroidJNIHelper.GetMethodID(map.GetRawClass(), 
		                                                 "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
		
//		object[] args = new object[2];
//		foreach (KeyValuePair<string, object> kvp in dic) {
//			AndroidJavaObject k = new AndroidJavaObject("java.lang.String", kvp.Key);
//			args[0] = k;
//			args[1] = kvp.Value;
//			AndroidJNI.CallObjectMethod(map.GetRawObject(), put, AndroidJNIHelper.CreateJNIArgArray(args));
//		}

		var arguments = new object[2];
		foreach( var entry in dic )
		{
			using( var key = new AndroidJavaObject( "java.lang.String", entry.Key ) )
			{
				using( var val = new AndroidJavaObject( "java.lang.Integer", entry.Value ) )
				{
					arguments[0] = key;
					arguments[1] = val;
					AndroidJNI.CallObjectMethod( map.GetRawObject(), put, AndroidJNIHelper.CreateJNIArgArray( arguments ) );
				}
			}
		} 
		return map;
	}


	/**
	 * 测试domain_map列表里域名（ip）的连接速度、成功/失败等
	 */
	public static void TestSpeed(Dictionary<string, int> domain_map)
	{
		MtaServiceImpl.CallStatic ("testSpeed", Context, ToJavaHashMap(domain_map));
	}
	/**
	 * 测试MTA前台配置的固定域名的连接速度等情况，不建议主动调用
	 */
	public static void TestSpeed()
	{
		MtaServiceImpl.CallStatic ("testSpeed", Context);
	}
	
	/**
	 * 开启NativeCrash上报，调用前请确保已经添加相关的.so文件
	 */
	public static void InitNativeCrashReport(string tombstone_dir)
	{
		MtaConfigImpl.CallStatic ("initNativeCrashReport", Context, tombstone_dir);
	}
	/**
	 * 是否开启多进程
	 */
	public static void SetEnableConcurrentProcess(bool enable)
	{
		MtaConfigImpl.CallStatic ("setEnableConcurrentProcess", enable);
	}
	/**
	 * 每天/每进程周期内最大的session个数
	 */
	public static void SetMaxDaySessionNumbers(int number)
	{
		MtaConfigImpl.CallStatic ("setMaxDaySessionNumbers", number);
	}
	
	/**
	 * 获取MID（Mobile ID），MTA会对每台设备分配唯一的id，作为该设备的标识
	 */
	public static string GetMid()
	{
		return MtaConfigImpl.CallStatic<string> ("getMid", Context);
	}
	
	/**
	 * 获取APPKEY
	 */
	public static string GetAppKey()
	{
		return MtaConfigImpl.CallStatic<string> ("getAppKey", Context);
	}
	/**
	 * 获取自定义用户ID
	 */
	public static string GetCustomUserId()
	{
		return MtaConfigImpl.CallStatic<string> ("getCustomUserId", Context);
	}
	/**
	 * 获取配置/设置的安装渠道
	 */
	public static string GetInstallChannel()
	{
		return MtaConfigImpl.CallStatic<string> ("getInstallChannel", Context);
	}

#endif


}
