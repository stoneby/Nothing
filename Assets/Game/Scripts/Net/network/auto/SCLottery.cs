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
  public partial class SCLottery : TBase
  {
    private int _activityId;
    private sbyte _lotteryType;
    private sbyte _lotteryMode;
    private List<KXSGCodec.RewardItem> _rewardItem;

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
    /// 抽奖类型1武将抽奖2道具抽奖
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

    public List<KXSGCodec.RewardItem> RewardItem
    {
      get
      {
        return _rewardItem;
      }
      set
      {
        __isset.rewardItem = true;
        this._rewardItem = value;
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
      public bool rewardItem;
    }

    public SCLottery() {
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
          case 4:
            if (field.Type == TType.List) {
              {
                RewardItem = new List<KXSGCodec.RewardItem>();
                TList _list4 = iprot.ReadListBegin();
                for( int _i5 = 0; _i5 < _list4.Count; ++_i5)
                {
                  KXSGCodec.RewardItem _elem6 = new KXSGCodec.RewardItem();
                  _elem6 = new KXSGCodec.RewardItem();
                  _elem6.Read(iprot);
                  RewardItem.Add(_elem6);
                }
                iprot.ReadListEnd();
              }
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
      TStruct struc = new TStruct("SCLottery");
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
      if (RewardItem != null && __isset.rewardItem) {
        field.Name = "rewardItem";
        field.Type = TType.List;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteListBegin(new TList(TType.Struct, RewardItem.Count));
          foreach (KXSGCodec.RewardItem _iter7 in RewardItem)
          {
            _iter7.Write(oprot);
          }
          oprot.WriteListEnd();
        }
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("SCLottery(");
      sb.Append("ActivityId: ");
      sb.Append(ActivityId);
      sb.Append(",LotteryType: ");
      sb.Append(LotteryType);
      sb.Append(",LotteryMode: ");
      sb.Append(LotteryMode);
      sb.Append(",RewardItem: ");
      sb.Append(RewardItem);
      sb.Append(")");
      return sb.ToString();
    }

  }

}