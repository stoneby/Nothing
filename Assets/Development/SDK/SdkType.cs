public class SdkType
{
    public static string GetAccessId()
    {
        switch (GameConfig.BundleID)
        {
            case "com.tencent.tmgp.sanguommd":
                return "2100051979";
                break;
            case "cn.kx.sglm.jinshan":
                return "2100052048";
                break;
            case "cn.kx.sglm":
                return "2100052048";
                break;
        }
        return "2100052048";
    }

    public static string GetAccessKey()
    {
        switch (GameConfig.BundleID)
        {
            case "com.tencent.tmgp.sanguommd":
                return "AT3573TR1RWI";
                break;
            case "cn.kx.sglm.jinshan":
                return "A1CIWTN4927N";
                break;
            case "cn.kx.sglm":
                return "A1CIWTN4927N";
                break;
        }
        return "A1CIWTN4927N";
    }
	
}
