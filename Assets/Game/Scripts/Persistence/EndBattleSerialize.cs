using KXSGCodec;
using System;
using System.IO;

public class EndBattleSerialize
{
    private const string MissionModelLocatorClassName = "MissionModelLocator:";
    private const string BattleEndMsgClassName = "CSBattlePveFinishMsg:";

    public CSBattlePveFinishMsg BattleEndMsg = new CSBattlePveFinishMsg();

    public void WriteClass(StreamWriter writer, string className)
    {
        writer.Write(className);
        MissionModelLocator.Instance.WriteClass(writer, MissionModelLocatorClassName);
        BattleEndMsg.WriteClass(writer, BattleEndMsgClassName);
    }

    public void ReadClass(string value)
    {
        string[] splitStrings = new string[] { MissionModelLocatorClassName, BattleEndMsgClassName };
        string[] outStrings = value.Split(splitStrings, StringSplitOptions.RemoveEmptyEntries);
        PersistenceFileIOHandler.CheckCount(outStrings, 2);
        MissionModelLocator.Instance.ReadClass(outStrings[0]);
        BattleEndMsg.ReadClass(outStrings[1]);
    }
}
