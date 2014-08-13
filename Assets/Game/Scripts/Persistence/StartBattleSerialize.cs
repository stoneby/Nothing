using KXSGCodec;
using System;
using System.IO;

[Serializable]
public class StartBattleSerialize
{
    private const int FieldCount = 2;
    private const string MissionModelLocatorClassName = "MissionModelLocator:";
    private const string BattleStartMsgClassName = "ScBattlePveStartMsg:";

    public SCBattlePveStartMsg BattleStartMsg=new SCBattlePveStartMsg();

    public void WriteClass(StreamWriter writer, string className)
    {
        writer.Write(className);
        MissionModelLocator.Instance.WriteClass(writer, MissionModelLocatorClassName);
        BattleStartMsg.WriteClass(writer, BattleStartMsgClassName);
    }

    public void ReadClass(string value)
    {
        string[] splitStrings = new string[] { MissionModelLocatorClassName, BattleStartMsgClassName };
        string[] outStrings = value.Split(splitStrings, StringSplitOptions.RemoveEmptyEntries);
        PersistenceFileIOHandler.CheckCount(outStrings, FieldCount);
        MissionModelLocator.Instance.ReadClass(outStrings[0]);
        BattleStartMsg.ReadClass(outStrings[1]);
    }
}
