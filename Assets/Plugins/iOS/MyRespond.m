//
//  MyRespond.m
//  Unity-iPhone
//
//  Created by kaixin on 14-7-9.
//
//

#import "MyRespond.h"

@implementation MyRespond

static MyRespond* myrespond;

void PressInitialize(int gameid,const char * appversion,const char * f,const char * extradata)
{
    myrespond=[[MyRespond alloc]init];
    myrespond.myinterface=[FD_interface shareInstance];
    [FD_interface isDebug:YES];
    
    //添加接口属性
    [[NSNotificationCenter defaultCenter] addObserver:MyRespond.class selector:@selector(login_Success:) name:LOGIN_NOTIFICATION_SUCCESS object:nil];
    [[NSNotificationCenter defaultCenter] addObserver:MyRespond.class selector:@selector(logout_Success:) name:LOGOUT_NOTIFICATION_SUCCESS object:nil];
    [[NSNotificationCenter defaultCenter] addObserver:MyRespond.class selector:@selector(initGameStart_Success:) name:INITGAMESTART_NOTIFICATION_SUCCESS object:nil];
    [[NSNotificationCenter defaultCenter] addObserver:MyRespond.class selector:@selector(addRole_Success:) name:ADDROLE_NOTIFICATION_SUCCESS object:nil];
    [[NSNotificationCenter defaultCenter] addObserver:MyRespond.class selector:@selector(iosPay_Success:) name:IOSPAY_NOTIFICATION_SUCCESS object:nil];
    [[NSNotificationCenter defaultCenter] addObserver:MyRespond.class selector:@selector(Pay_Success:) name:IN_APP_PURCHASE_NOTIFICATION_SUCCESS object:nil];
    NSLog(@"NSNotification complete");
    
    [FD_interface InitWithGameid:gameid appversion:[NSString stringWithUTF8String:appversion] f:[NSString stringWithUTF8String:f] baseView:[UIApplication sharedApplication].keyWindow.rootViewController.view extradata:[NSString stringWithUTF8String:extradata]];
    NSLog(@"initialize complete");
}
void PressLogin(int serverid, const char * extradata)
{
    NSString * str = [NSString stringWithUTF8String:extradata];
    NSLog(@"login extra = %@", str);
    [FD_interface loginWithServerid:serverid extradata:str];
    NSLog(@"login complete");
}

void PressLogout(int userid,int serverid,const char * extradata)
{
    [FD_interface logoutWithUserid:userid serverid:serverid extradata:[NSString stringWithUTF8String:extradata]];
    NSLog(@"logout complete");
}

void PressAddrole(int userid,const char * roleid,const char * rolename,int serverid,const char * extradata)
{
    [FD_interface addroleWithUserid:userid roleid:[NSString stringWithUTF8String:roleid] rolename:[NSString stringWithUTF8String:rolename] serverid:serverid extradata:[NSString stringWithUTF8String:extradata]];
    NSLog(@"addrole complete");
}

void PressIospay(int userid,const char * roleid,const char * currency,const char * amount,const char * itemid,
                  const char * itemname,int itemprice,const char * coin,const char * orderid,const char * channel,
                  int serverid,const char * extradata)
{
    [FD_interface iospayWithUserid:userid roleid:[NSString stringWithUTF8String:roleid] currency:[NSString stringWithUTF8String:currency] amount:[NSString stringWithUTF8String:amount] itemid:[NSString stringWithUTF8String:itemid] itemname:[NSString stringWithUTF8String:itemname] itemprice:itemprice coin:[NSString stringWithUTF8String:coin] orderid:[NSString stringWithUTF8String:orderid] channel:[NSString stringWithUTF8String:channel] serverid:serverid extradata:[NSString stringWithUTF8String:extradata]];
    NSLog(@"iospay complete");
}

void PressWeburl(int userid,int urltype,const char * url,int serverid)
{
    [FD_interface weburlWithuserid:userid urltype:urltype url:[NSString stringWithUTF8String:url] serverid:serverid];
    NSLog(@"weburl complete");
}

void PressPay(const char * remark, int userid, int serverid, const char * roleid, const char * extradata)
{
    [FD_interface payWithViewcontroller:[UIApplication sharedApplication].keyWindow.rootViewController remark:[NSString stringWithUTF8String:remark] userid:userid serverid:serverid roleid:[NSString stringWithUTF8String:roleid] extradata:[NSString stringWithUTF8String:extradata]];
    NSLog(@"pay complete");
}

+(void) initGameStart_Success:(NSNotification*) message
{
    NSLog(@"initialize respond");
    NSString* str=@"";
    str=[[str stringByAppendingString:@"initGameStart:"]stringByAppendingString:(NSString*)[message object]];
    const char* result=[str cStringUsingEncoding:NSASCIIStringEncoding];
    UnitySendMessage("Global", "Respond", result);
    NSLog(@"Sending message to Unity.");
    
}

+(void) login_Success:(NSNotification*) message
{
    NSLog(@"login respond");
    NSString* str=@"";
    str=[[str stringByAppendingString:@"login:"]stringByAppendingString:(NSString*)[message object]];
    const char* result=[str cStringUsingEncoding:NSASCIIStringEncoding];
    UnitySendMessage("Global", "Respond", result);
    NSLog(@"Sending message to Unity.");
}

+(void) logout_Success:(NSNotification*) message
{
    NSLog(@"logout respond");
    NSString* str=@"";
    str=[[str stringByAppendingString:@"logout:"]stringByAppendingString:(NSString*)[message object]];
    const char* result=[str cStringUsingEncoding:NSASCIIStringEncoding];
    UnitySendMessage("Global", "Respond", result);
}

+(void) addRole_Success:(NSNotification*) message
{
    NSLog(@"addrole respond");
    NSString* str=@"";
    str=[[str stringByAppendingString:@"addRole:"]stringByAppendingString:(NSString*)[message object]];
    const char* result=[str cStringUsingEncoding:NSASCIIStringEncoding];
    UnitySendMessage("Global", "Respond", result);
}

+(void) iosPay_Success:(NSNotification*) message
{
    NSLog(@"iospay respond");
    NSString* str=@"";
    str=[[str stringByAppendingString:@"iosPay:"]stringByAppendingString:(NSString*)[message object]];
    const char* result=[str cStringUsingEncoding:NSASCIIStringEncoding];
    UnitySendMessage("Global", "Respond", result);
}

+(void) Pay_Success:(NSNotification*) message
{
    NSLog(@"pay respond");
    NSString* str=@"";
    str=[[str stringByAppendingString:@"Pay:"]stringByAppendingString:(NSString*)[message object]];
    const char* result=[str cStringUsingEncoding:NSASCIIStringEncoding];
    UnitySendMessage("Global", "Respond", result);
}

-(BOOL)shouldAutorotate
{
    return YES;
}

-(NSUInteger)supportedInterfaceOrientations
{
    return UIInterfaceOrientationMaskLandscape;
}

@end
