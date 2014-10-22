
using UnityEngine;

public class XgManager 
{
    #if UNITY_ANDROID
    private static AndroidJavaClass _xinggeInstance;
    private static AndroidJavaObject _contex;
#endif
    public static void init()
    {
#if UNITY_ANDROID

        

        using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))    
        {   
   
            using (_contex = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) 
            {   
   
                _xinggeInstance = new AndroidJavaClass("com.kx.xg.KxXgSdk");
                _xinggeInstance.CallStatic("initXg", _contex, "SDKResponse", "XgMessage", SdkType.GetAccessId(), SdkType.GetAccessKey());
   
            }   
        }   
#endif
    }

    public static void unregisterPush()
    {
#if UNITY_IPHONE
	  
#elif UNITY_ANDROID
        _xinggeInstance.CallStatic("unregisterPush");
#endif

    }

    public static void setTag(string tagname)
    {
#if UNITY_IPHONE
	  
#elif UNITY_ANDROID
        _xinggeInstance.CallStatic("setTag", tagname);
#endif

    }

    //deleteTag(string tagname)
    public static void deleteTag(string tagname)
    {
#if UNITY_IPHONE
	  
#elif UNITY_ANDROID
        _xinggeInstance.CallStatic("deleteTag", tagname);
#endif

    }

    //setKeyValueTag(string tagname, string tagvalue)
    public static void setKeyValueTag(string tagname, string tagvalue)
    {
#if UNITY_IPHONE
	  
#elif UNITY_ANDROID
        _xinggeInstance.CallStatic("setKeyValueTag", tagname, tagvalue);
#endif

    }

    //deleteKeyValueTag(string tagname, string tagvalue)
    public static void deleteKeyValueTag(string tagname, string tagvalue)
    {
#if UNITY_IPHONE
	  
#elif UNITY_ANDROID
        _xinggeInstance.CallStatic("deleteKeyValueTag", tagname, tagvalue);
#endif

    }

    //msg.setTitle("我是标题");
    //msg.setContent("我是内容");
    //msg.setDate("20140909");
    //msg.setHour("11");
    //msg.setMin("50");
    public static void addLocalNotification(string title, string content, string date, string hour, string minute)
    {
#if UNITY_IPHONE
	  
#elif UNITY_ANDROID
        _xinggeInstance.CallStatic("addLocalNotification", title, content, date, hour, minute);
#endif

    }

    public static void clearLocalNotification()
    {
#if UNITY_IPHONE
	  
#elif UNITY_ANDROID
         _xinggeInstance.CallStatic("clearLocalNotification");
#endif

    }
    
}
