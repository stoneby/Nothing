using KXSGCodec;
using System;
using System.IO;

public class EndBattleSerialize
{
    //Nums count and ClassName.
    private const string MissionModelLocatorClassName = "MissionModelLocator:";
    private const string BattleEndMsgClassName = "CSBattlePveFinishMsg:";

    public CSBattlePveFinishMsg BattleEndMsg = new CSBattlePveFinishMsg();

    /// <summary>
    /// Write this whole class to stream.
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="className"></param>
    public void WriteClass(StreamWriter writer, string className)
    {
        writer.Write(className);
        MissionModelLocator.Instance.WriteClass(writer, MissionModelLocatorClassName);
        BattleEndMsg.WriteClass(writer, BattleEndMsgClassName);
    }

    /// <summary>
    /// Read this whole class from string.
    /// </summary>
    /// <param name="value"></param>
    public void ReadClass(string value)
    {
        string[] splitStrings = new string[] { MissionModelLocatorClassName, BattleEndMsgClassName };
        string[] outStrings = value.Split(splitStrings, StringSplitOptions.RemoveEmptyEntries);
        PersistenceFileIOHandler.CheckCount(outStrings, 2);
        MissionModelLocator.Instance.ReadClass(outStrings[0]);
        BattleEndMsg.ReadClass(outStrings[1]);
    }
}
