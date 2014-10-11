using System.Runtime.Serialization.Formatters;
using KXSGCodec;
using UnityEngine;

public class SDKResponse : MonoBehaviour
{
    #region Public Fields

    /// <summary>
    /// Verifying SDK initialized or not.
    /// </summary>
    public static bool IsInitialized = false;

    /// <summary>
    /// Go to which function after receive the response from SDK.
    /// </summary>
    public delegate void Responses();
    public static Responses WhichResponse;
    public static string TokenString;

    #endregion

    #region Public Methods

#if UNITY_ANDROID

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
                TokenString = split[1];
                WindowManager.Instance.Show<LoadingWaitWindow>(false);
                var msg = new CSTokenLoginMsg() { DeviceType = 0, DeviceId = "", DeviceModel = SystemInfo.deviceModel, Token = TokenString };
                NetManager.SendMessage(msg);
                //PopTextManager.PopTip("平台登录成功，开始调用服务器进行token登录");
            }
            else if (split[0] == "logoutResult is")
            {

            }
            else if (split[0] == "addroleResult is")
            {

            }
            else if (split[0] == "iospayResult is")
            {

            }
            else if (split[0] == "platformBillingResult is")
            {
                Debug.Log("Response platformBillingResult succeed!Sending message to server.");
                var msg = new CSRefreshRechargeMsg() { OrderId = split[1] };
                NetManager.SendMessage(msg);
                //SDKPayManager.IsPaying = false;
                WindowManager.Instance.GetWindow<UIMainScreenWindow>()
                    .GetComponentInChildren<SDKPayManager>()
                    .GetComponent<UIButton>()
                    .isEnabled = true;
            }
            else
            {
                Debug.LogError("Android SDK Message check fail! Check the message responded above.");
            }
        }
    }
#endif
#if UNITY_IPHONE

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
                //SDKPayManager.IsPaying = false;
                WindowManager.Instance.GetWindow<UIMainScreenWindow>()
                .GetComponentInChildren<SDKPayManager>()
                .GetComponent<UIButton>()
                .isEnabled = true;
            }
            else if (split[0] == "CloseLogin")
            {
                Debug.Log("Response close login succeed.");
                GlobalDimmerController.Instance.Show(false);
                WindowManager.Instance.GetWindow<UIMainScreenWindow>()
                .GetComponentInChildren<SDKPayManager>()
                .GetComponent<UIButton>()
                .isEnabled = true;
            }
            else if (split[0] == "ClosePay")
            {
                Debug.Log("Response close pay succeed.");
                GlobalDimmerController.Instance.Show(false);
                WindowManager.Instance.GetWindow<UIMainScreenWindow>()
                .GetComponentInChildren<SDKPayManager>()
                .GetComponent<UIButton>()
                .isEnabled = true;
            }
        }
    }
#endif

    #endregion
}
