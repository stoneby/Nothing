using Assets.Game.Scripts.Net.network;
using KXSGCodec;
using UnityEngine;

namespace Assets.Game.Scripts.Net.handler
{
    class SystemHandler
    {
        public static void OnSystemInfo(ThriftSCMessage msg)
        {
            var sysmsg = msg.GetContent() as SCSystemInfoMsg;
            if (sysmsg != null && sysmsg.Info != null && sysmsg.Info != "")
            {
                Logger.Log("服务端系统消息1：" + sysmsg.Info);
                PopTextManager.PopTip(sysmsg.Info, false);
            }
            else
            {
                var clientmsg = msg as ClientSCMessage;
                Logger.Log("服务端系统消息2：" + clientmsg.Info);
                //PopTextManager.PopTip(clientmsg.Info);
                Alert.Show(AssertionWindow.Type.Ok, "系统提示", clientmsg.Info, AlertHandler);
            }
        }

        private static void AlertHandler(GameObject sender = null)
        {
            //WindowManager.Instance.Show(typeof(MainMenuBarWindow), false);

            WindowManager.Instance.Show(false);
            WindowManager.Instance.Show(typeof(LoginWindow), true);
            WindowManager.Instance.Show(typeof(LoginMainWindow), true);
        }

        public static void OnErrorInfo(ThriftSCMessage msg)
        {
            var errmsg = msg.GetContent() as SCErrorInfoMsg;
            if (errmsg != null)
            {
                Logger.Log("服务端系统消息error：" + errmsg.ErrorCode);
                string str = "系统提示：";
                switch (errmsg.ErrorCode)
                {
                    case (short)ErrorType.ACCOUNT_NOT_ACTIVE:
                        str += "账号未激活";
                        break;
                    case (short)ErrorType.CREATE_CHAR_FAIL:
                        str += "创建角色失败";
                        break;
                    case (short)ErrorType.DECODE_EXCEPTION:
                        str += "解码异常";
                        break;
                    case (short)ErrorType.ILLEGAL_REQUEST:
                        str += "非法请求";
                        break;
                    case (short)ErrorType.LOGIN_CHECK_FAIL:
                        str += "登录信息检查未通过";
                        break;
                    case (short)ErrorType.LOGIN_EXPIRED:
                        str += "登录超时";
                        break;
                    case (short)ErrorType.LOGIN_INVALID:
                        str += "登录失效";
                        AlertHandler();//登录失效时返回登录页面
                        break;
                    case (short)ErrorType.MSG_EXEC_EXCEPTION:
                        str += "消息执行异常";
                        break;
                    case (short)ErrorType.NAME_EXISTS:
                        str += "名字已存在";
                        break;
                    case (short)ErrorType.NO_ACCOUNT_OR_ERR_PWD:
                        str += "帐号不存在或密码错误";
                        break;
                    case (short)ErrorType.SERVER_NOT_OPEN:
                        str += "服务器未开放";
                        break;
                    case (short)ErrorType.USER_LOCKED:
                        str += "帐号被锁定";
                        break;
                    default:
                        str = "未处理的ErrorCode：" + errmsg.ErrorCode;
                        break;
                }
                PopTextManager.PopTip(str, false);
            }
        }


        public static void OnNoticeList(ThriftSCMessage msg)
        {
            var greenHand = GreenHandGuideHandler.Instance;
            if (!GreenHandGuideHandler.Instance.IsGreenHandGuideFinish) return;
            if (!greenHand.BattleFinishFlag) return;

            //Shield Notice if greenHand not finish
            if (!(greenHand.GiveHeroFinishFlag && greenHand.RaidFinishFlag && greenHand.SummitFinishFlag &&
                greenHand.TeamFinishFlag)) return;

            var sysmsg = msg.GetContent() as SCGameNoticeListMsg;
            //PopTextManager.PopTip("公告" + sysmsg.NoticeList.Count);
            //Logger.Log("公告" + sysmsg.NoticeList.Count);
            if (sysmsg != null && sysmsg.NoticeList != null && sysmsg.NoticeList.Count > 0)
            {
                SystemModelLocator.Instance.NoticeListMsg = sysmsg;
                WindowManager.Instance.Show(typeof(NoticeWindow), true);
            }
            else
            {
                Logger.Log("Notice list message should not be null.");
            }
        }

        public static void OnNoticeDetail(ThriftSCMessage msg)
        {
            var sysmsg = msg.GetContent() as SCGameNoticeDetailMsg;

            if (sysmsg != null)
            {
                SystemModelLocator.Instance.NoticeDetailMsg = sysmsg;
                SystemModelLocator.Instance.NoticeItem.SetContent(sysmsg.Content);
                //WindowManager.Instance.Show(typeof(NoticeWindow), true);
            }
            else
            {
                Logger.LogError("Notice list message should not be null.");
            }
        }
    }
}
