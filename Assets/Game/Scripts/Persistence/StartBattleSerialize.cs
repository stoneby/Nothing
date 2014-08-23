using KXSGCodec;
using System;
using System.IO;

/// <summary>
/// Handle storing data when battle start.
/// </summary>
[Serializable]
public class StartBattleSerialize
{
    //Nums count and ClassName.
    private const int FieldCount = 2;
    private const string MissionModelLocatorClassName = "MissionModelLocator:";
    private const string BattleStartMsgClassName = "ScBattlePveStartMsg:";

    public SCBattlePveStartMsg BattleStartMsg = new SCBattlePveStartMsg();

    /// <summary>
    /// Write this whole class to stream.
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="className"></param>
    public void WriteClass(StreamWriter writer, string className)
    {
        writer.Write(className);
        MissionModelLocator.Instance.WriteClass(writer, MissionModelLocatorClassName);
        BattleStartMsg.WriteClass(writer, BattleStartMsgClassName);
    }

    /// <summary>
    /// Read this whole class from string.
    /// </summary>
    /// <param name="value"></param>
    public void ReadClass(string value)
    {
        string[] splitStrings = new string[] { MissionModelLocatorClassName, BattleStartMsgClassName };
        string[] outStrings = value.Split(splitStrings, StringSplitOptions.RemoveEmptyEntries);
        PersistenceFileIOHandler.CheckCount(outStrings, FieldCount);
        MissionModelLocator.Instance.ReadClass(outStrings[0]);
        BattleStartMsg.ReadClass(outStrings[1]);
    }
}
