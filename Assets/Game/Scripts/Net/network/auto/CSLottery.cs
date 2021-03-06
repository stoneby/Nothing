/**
 * Autogenerated by Thrift Compiler (0.9.1)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift;
using Thrift.Collections;
using System.Runtime.Serialization;
using Thrift.Protocol;
using Thrift.Transport;

namespace KXSGCodec
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class CSLottery : TBase
  {
    private int _activityId;
    private sbyte _lotteryType;
    private sbyte _lotteryMode;

    public int ActivityId
    {
      get
      {
        return _activityId;
      }
      set
      {
        __isset.activityId = true;
        this._activityId = value;
      }
    }

    /// <summary>
    /// 抽奖类型-1武将抽奖 2道具抽奖
    /// </summary>
    public sbyte LotteryType
    {
      get
      {
        return _lotteryType;
      }
      set
      {
        __isset.lotteryType = true;
        this._lotteryType = value;
      }
    }

    /// <summary>
    /// 抽奖类型 1单次免费抽奖 2单次有费抽奖 3连续10次抽奖
    /// </summary>
    public sbyte LotteryMode
    {
      get
      {
        return _lotteryMode;
      }
      set
      {
        __isset.lotteryMode = true;
        this._lotteryMode = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool activityId;
      public bool lotteryType;
      public bool lotteryMode;
    }

    public CSLottery() {
    }

    public void Read (TProtocol iprot)
    {
      TField field;
      iprot.ReadStructBegin();
      while (true)
      {
        field = iprot.ReadFieldBegin();
        if (field.Type == TType.Stop) { 
          break;
        }
        switch (field.ID)
        {
          case 1:
            if (field.Type == TType.I32) {
              ActivityId = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.Byte) {
              LotteryType = iprot.ReadByte();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.Byte) {
              LotteryMode = iprot.ReadByte();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          default: 
            TProtocolUtil.Skip(iprot, field.Type);
            break;
        }
        iprot.ReadFieldEnd();
      }
      iprot.ReadStructEnd();
    }

    public void Write(TProtocol oprot) {
      TStruct struc = new TStruct("CSLottery");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.activityId) {
        field.Name = "activityId";
        field.Type = TType.I32;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(ActivityId);
        oprot.WriteFieldEnd();
      }
      if (__isset.lotteryType) {
        field.Name = "lotteryType";
        field.Type = TType.Byte;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteByte(LotteryType);
        oprot.WriteFieldEnd();
      }
      if (__isset.lotteryMode) {
        field.Name = "lotteryMode";
        field.Type = TType.Byte;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteByte(LotteryMode);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("CSLottery(");
      sb.Append("ActivityId: ");
      sb.Append(ActivityId);
      sb.Append(",LotteryType: ");
      sb.Append(LotteryType);
      sb.Append(",LotteryMode: ");
      sb.Append(LotteryMode);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
