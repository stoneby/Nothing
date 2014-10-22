using System;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using KXSGCodec;
using UnityEngine;

public class SDKResponseManager : MonoBehaviour
{
    #region Public Fields

    /// <summary>
    /// Verifying SDK initialized or not.
    /// </summary>
    public static bool IsInitialized = false;
    public static bool IsLogin = false;

    /// <summary>
    /// Go to which function after receive the response from SDK.
    /// </summary>
    public delegate void Responses();
    public static Responses WhichResponse;
    public static string TokenString;

    #endregion

    #region Private Fields

    private const float ShieldTime = 3.0f;

    #endregion

    #region Public Methods

//#if UNITY_IPHONE
    /// <summary>
    /// Process the string from IOS SDK.
    /// ':'is the key char.
    /// </summary>
    /// <param name="str"></param>
    public void Respond(string str)
    {
        if (Application.platform != RuntimePlatform.OSXEditor)
        {
            Logger.Log("message is:" + str);
            string[] split = str.Split(new char[] { ':' });
            if (split[0] == "initGameStart")
            {
                if (split[1] == "ok")
                {
                    Debug.Log("Setting isInitialized true.");
                    IsInitialized = true;

                    WhichResponse();
                }
            }
            else if (split[0] == "login")
            {
                Logger.Log("Response login succeed, Sending message to server.");
                GlobalDimmerController.Instance.Show(false);
                var msg = new CSTokenLoginMsg() { DeviceType = 0, DeviceId = "", DeviceModel = SystemInfo.deviceModel, Token = split[1] };
                NetManager.SendMessage(msg);
            }
            else if (split[0] == "logout")
            {
                Debug.Log("Response logout succeed.");
            }
            else if (split[0] == "addRole")
            {
                Debug.Log("Response addRole succeed.");
            }
            else if (split[0] == "iosPay")
            {
                Debug.Log("Response iosPay succeed.");
            }
            else if (split[0] == "Pay")
            {
                Debug.Log("Response Pay succeed, sending message to server.");
                GlobalDimmerController.Instance.Show(false);
                var msg = new CSRefreshRechargeMsg() { OrderId = split[1] };
                NetManager.SendMessage(msg);
            }
            else if (split[0] == "CloseLogin")
            {
                Debug.Log("Response close login succeed.");
                GlobalDimmerController.Instance.Show(false);
            }
            else if (split[0] == "ClosePay")
            {
                Debug.Log("Response close pay succeed.");
                GlobalDimmerController.Instance.Show(false);
            }
        }
    }
//#endif

    public void XgMessage(string msg)
    {
        if (Application.platform != RuntimePlatform.WindowsEditor)
        {
            Debug.Log("xinge:" + msg);
            PopTextManager.PopTip("信鸽:" + msg);
            string[] arr = msg.Split(':');
            if (arr == null || arr.Length <= 0) return;
            switch (arr[0])
            {
                case "init":
                    if (arr[1] == "success")
                    {
                        Debug.Log("xinge token:" + arr[2]);
                        PopTextManager.PopTip("xinge token:" + arr[2]);
                        MtaManager.ReportError("xinge token:" + arr[2]);
                        var d = GetTimeStr(DateTime.Now.Year) + GetTimeStr(DateTime.Now.Month) +
                                GetTimeStr(DateTime.Now.Day);
                        var h = DateTime.Now.Hour;
                        var m = DateTime.Now.Minute + 5;
                        if (m > 60)
                        {
                            m -= 60;
                            h++;
                        }
                        PopTextManager.PopTip("Date:" + d + ", Hour:" + h + ", Minute:" + m);
                        XgManager.clearLocalNotification();
                        XgManager.addLocalNotification("推送标题", "这里显示的都是本地推送内容", d, h.ToString(), m.ToString());
                    }
                    else
                    {
                        Debug.Log("xinge init fail:" + arr[2]);
                    }
                    break;
                case "unregister":
                    Debug.Log("xinge:" + msg);
                    break;
                default:
                    Debug.Log("xinge:" + msg);
                    break;
            }
        }
    }

    private string GetTimeStr(int v)
    {
        if (v < 10)
        {
            return "0" + v;
        }
        else
        {
            return v.ToString();
        }
    }

//#if UNITY_ANDROID
    /// <summary>
    /// Process the string from Android SDK.
    /// ':'is the key char.
    /// </summary>
    /// <param name="str"></param>
    public void Message(string str)
    {
        if (Application.platform != RuntimePlatform.WindowsEditor)
        {
            Debug.Log("message is:" + str);
            string[] split = str.Split(new char[] { ':' });
            if (split[0] == "startsResult is")
            {
                Debug.Log("Response startsResult succeed!");
                Debug.Log("Setting isInitialized true.");
                IsInitialized = true;

                WhichResponse();
            }
            else if (split[0] == "loginResult is")
            {
                Debug.Log("Response loginResult succeed!Sending message to server.");
                Debug.Log("Setting IsLogin true.");
                IsLogin = true;
                TokenString = split[1];
                WindowManager.Instance.Show<LoadingWaitWindow>(false);
                var msg = new CSTokenLoginMsg() { DeviceType = 0, DeviceId = "", DeviceModel = SystemInfo.deviceModel, Token = TokenString };
                NetManager.SendMessage(msg);
                //PopTextManager.PopTip("平台登录成功，开始调用服务器进行token登录");
            }
            else if (split[0] == "logoutResult is")
            {
                Debug.Log("Response logoutResult succeed!");
                Debug.Log("Setting IsLogin false.");
                IsLogin = false;

                WhichResponse();
            }
            else if (split[0] == "addroleResult is")
            {

            }
            else if (split[0] == "iospayResult is")
            {

            }
            else if (split[0] == "platformBillingResult is")
            {
                Debug.Log("Set SDKPayManager.GameID null.");
                SDKPayManager.GameId = null;

                Debug.Log("Response platformBillingResult succeed!Sending message to server.");
                var msg = new CSRefreshRechargeMsg() { OrderId = split[1] };
                NetManager.SendMessage(msg);
            }
            else
            {
                Debug.LogError("Android SDK Message check fail! Check the message responded above.");
            }
        }
    }
//#endif

    #endregion

    #region GlobalDimmer Methods

    public void ShieldUI(bool isAutomaticlyOpen)
    {
        GlobalDimmerController.Instance.Transparent = true;
        GlobalDimmerController.Instance.DetectObject = null;
        Debug.Log("Shield all UI.");
        GlobalDimmerController.Instance.Show(true);

        if (isAutomaticlyOpen)
        {
            StartCoroutine(DoOpenUI());
        }
    }

    public void ShieldButton(params UIButton[] buttons)
    {
        Debug.Log("Deactive buttons, count:" + buttons.Count());
        foreach (var item in buttons)
        {
            item.isEnabled = false;
        }

        StartCoroutine(DoOpenButton(buttons));
    }

    private IEnumerator DoOpenUI()
    {
        yield return new WaitForSeconds(ShieldTime);

        Debug.Log("Open all UI.");
        GlobalDimmerController.Instance.Show(false);
    }

    private IEnumerator DoOpenButton(params UIButton[] buttons)
    {
        yield return new WaitForSeconds(ShieldTime);

        Debug.Log("Active buttons, count:" + buttons.Count());
        foreach (var item in buttons)
        {
            item.isEnabled = true;
        }
    }

    #endregion
}
