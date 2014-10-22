
public class ResourcePath
{
    public const string BundlePath = "Bundles";
    public const string TemplatePath = "Templates/Templates.zip";
    public const string BaseTempatePath = "AssetBundles/TextAssets/Templates";

    public const string FileBag = BaseTempatePath + "/Bag";
    public const string FileHero = BaseTempatePath + "/Hero";
    public const string FileItem = BaseTempatePath + "/Item";
    public const string FileSkill = BaseTempatePath + "/Skill";
    public const string FileLevelUp = BaseTempatePath + "/LevelUp";
    public const string FileRaid = BaseTempatePath + "/Raid";
    public const string FileItemConfig = BaseTempatePath + "/ItemConfig";
    public const string FileMonsterConfig = BaseTempatePath + "/Monster";
    public const string FileBuffConfig = BaseTempatePath + "/Buff";
    public const string FileGreenhandConfig = BaseTempatePath + "/Greenhand";
    public const string FileSign = BaseTempatePath + "/Sign";
    public const string FileQuest = BaseTempatePath + "/Quest";
    public const string FileReward = BaseTempatePath + "/Reward";

    public static string[] ByteFiles = { FileBag, FileHero, FileItem, FileItemConfig, FileLevelUp, 
                                           FileRaid, FileSkill, FileMonsterConfig, FileBuffConfig,
                                           FileGreenhandConfig, FileSign, FileQuest , FileReward};
    public const string MainBgClipPath = "Sounds/mainbg";
}
