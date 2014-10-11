//
//  FD_interface.h
//  FeiDou_SDK
//
//  Created by kaixin on 14-3-19.
//  Copyright (c) 2014年 kaixin. All rights reserved.
//


/**** 这是个接口类 ****/
#import "FD_interface.h"
//头文件
#import <Foundation/Foundation.h>


//productid 根据自己情况修改
#define ProductID_IAP_1 @"test.kaixin.1"
#define ProductID_IAP_2 @"test.kaixin.10"
#define ProductID_IAP_3 @"test.kaixin.10"
#define ProductID_IAP_4 @"test.kaixin.10"
#define ProductID_IAP_5 @"test.kaixin.10"


/*定义自己的NSLog*/
#ifdef DEBUG
#define MLog(format,name) NSLog(format,name)
#else
#define MLog(format,name)
#endif

/*注册的通知*/
#define LOGIN_NOTIFICATION_SUCCESS                      (@"Login_Notification_Success")
#define LOGOUT_NOTIFICATION_SUCCESS                     (@"Logout_Notification_Success")
#define INITGAMESTART_NOTIFICATION                      (@"InitGameStart_Notification")
#define ADDROLE_NOTIFICATION_SUCCESS                    (@"AddRole_Notification_Success")
#define IOSPAY_NOTIFICATION_SUCCESS                     (@"IosPay_Notification_Success")
#define IN_APP_PURCHASE_NOTIFICATION                    (@"In_App_Purchase_Notification")
#define CLOSELOGINVIEWACTION                            (@"closeLoginViewAction")
#define CLOSEPAYVIEWACTION                              (@"closePayViewAction")



/*字段*/
#define USERNAME                         (@"username")
#define PASSWORDS                        (@"password")
#define USERNAME_ARRAY                   (@"username_array")
#define COOKIE_ARRAY                     (@"cookie_array")
#define COOKIE                           (@"cookie")
#define TIMESTAMP                        (@"timestamp")
#define GAMEID                           (@"gameid")
#define USERID                           (@"userid")
#define ROLEID                           (@"roleid")
#define SERVERID                         (@"serverid")
#define LOGINTYPE                        (@"logintype")
#define SDKTITLE                         (@"sdktitle")
#define SDKVERSION                       (@"sdkversion")
#define DEVICE                           (@"device")
#define KEY                              (@"key")
#define PLATFORM                         (@"platform")
#define PRICES                           (@"prices")
#define PRODUCTIDS                       (@"products")
#define PAYBEFORE_ARRAY                  (@"paybefore_Array")
#define PAYBEFORE_ARRAYBIG               (@"paybefore_ArrayBig")
#define DEVICEIDFA                       (@"deviceidfa")
#define TRANSACTIONRECEIPT               (@"transactionReceipt")
#define GOLDS                            (@"golds")


//平台充值字段
#define CURRENCY                         (@"currency")
#define PRODUCTIDSPLATFORM               (@"productsPlatform")


/*同步数据*/
#define NSUSERDEFAULT_SYNCHRONIZE [[NSUserDefaults standardUserDefaults] synchronize]
#define NSUSERDEFAULT_COOKIEARRAY [[NSUserDefaults standardUserDefaults] objectForKey:COOKIE_ARRAY]

/*MD5加密自己长度设定*/
#define CC_MD5_DIGEST_LENGTH    32 /* digest length in bytes */

/*获取时间戳*/
#define GET_TIMESTAMP [[NSString stringWithFormat:@"%ld", (long)[[NSDate date] timeIntervalSince1970]] intValue]

/*获取当前设备*/
#define GET_DEVICE_TYPE [[[[[[[UIDevice currentDevice] systemName]lowercaseString] stringByAppendingString:@" "]stringByAppendingString:[[UIDevice currentDevice] systemVersion]] stringByAppendingString:@" "] stringByAppendingString:[[[self class] rawSystemInfoString]lowercaseString]]

/*获取当前设备*/
#define GET_DEVICE_VERSION [[[UIDevice currentDevice] systemVersion]lowercaseString]




/****定义回调block****/

//MFblock_iospay BLOCK
//typedef void(^MFblock_Init)(NSString *  state) ;
//
////MFblock_Login BLOCK
//typedef void(^MFblock_Login)(NSString *  state) ;
//
////MFblock_Register BLOCK
//typedef void(^MFblock_Register)(NSString *  state) ;
//
////MFblock_Exit BLOCK
//typedef void(^MFblock_Exit)(NSString *  state) ;
//
////MFblock_startGame BLOCK
//typedef void(^MFblock_startGame)(NSString *  state) ;
//
////MFblock_addrole BLOCK
//typedef void(^MFblock_addrole)(NSString *  state) ;
//
////MFblock_iospay BLOCK
//typedef void(^MFblock_iospay)(NSString *  state) ;
//
////MFblock_InAppPurchase BLOCK
//typedef void(^MFblock_InAppPurchase)(NSString *  state) ;



/****定义回调block * end ***/



@interface FD_interface : NSObject

+(FD_interface *)shareInstance;

/**** 外部调用初始化方法 并接收返回信息的接口 ****/
+(void)InitWithGameid:(int)gameid appversion:(NSString *)appversion  f:(NSString *)f baseView:(UIView *)baseView extradata:(NSString *)extradata;



/**** 外部调用登录界面 并接收返回信息的接口 ****/
+ (void)loginWithServerid:(int)serverid extradata:(NSString *)extradata;



/**** 退出游戏调用接口 并接收返回信息的接口 ****/
+ (void)logoutWithUserid:(int)userid serverid:(int)serverid extradata:(NSString *)extradata;





/**** 添加角色调用接口 并接收返回信息的接口 ****/
+(void)addroleWithUserid:(int)userid roleid:(NSString *)roleid rolename:(NSString *)rolename serverid:(int)serverid extradata:(NSString *)extradata;




/**** 第三方充值调用接口 并接收返回信息的接口 ****/
+(void)iospayWithUserid:(int)userid roleid:(NSString *)roleid currency:(NSString *)currency amount:(NSString *)amount itemid:(NSString *)itemid itemname:(NSString *)itemname itemprice:(int)itemprice coin:(NSString *)coin orderid:(NSString *)orderid channel:(NSString *)channel serverid:(int)serverid extradata:(NSString *)extradata;


/**** 获取充值/官网/论坛/防沉迷等链接 接口 ****/
+(void)weburlWithuserid:(int)userid urltype:(int)urltype url:(NSString *)url serverid:(int)serverid;




//支付
+(void)payWithViewcontroller:(UIViewController *)viewC remark:(NSString *)remark  userid:(int)userid serverid:(int)serverid roleid:(NSString *)roleid extradata:(NSString *)extradata;

//设置是否debug状态， 默认为NO;
+(void)isDebug:(BOOL)Debug;


//是否预加载和处理ios支付(本sdk支付接口)掉单信息;
+(void)prepareLoad:(BOOL)loadstate;

//平台支付
+(void)platformPayWithViewcontroller:(UIViewController *)viewC remark:(NSString *)remark  userid:(int)userid serverid:(int)serverid roleid:(NSString *)roleid extradata:(NSString *)extradata url:(NSString *)url;

//无界面支付
+(void)payWithProductid:(NSString *)Productid price:(float)price gold:(int)gold remark:(NSString *)remark  userid:(int)userid serverid:(int)serverid roleid:(NSString *)roleid extradata:(NSString *)extradata;



//FB推广和登录跳转回调
-(void)FaceBookDidBecomeActiveWithID:(NSString *)appid;
-(BOOL)FaceBookhandleOpenURL:(NSURL *)url;



@end
