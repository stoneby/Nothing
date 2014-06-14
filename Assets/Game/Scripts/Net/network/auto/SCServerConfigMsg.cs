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
  public partial class SCServerConfigMsg : TBase
  {
    private sbyte _expConvertRaito;
    private sbyte _sameTypeExpTimes;
    private short _bindItemLimit;
    private sbyte _sellItemPriceRatio;
    private sbyte _buybackItemPriceRatio;
    private sbyte _sellItemSaveHours;

    /// <summary>
    /// 升级经验折算系数(%)
    /// </summary>
    public sbyte ExpConvertRaito
    {
      get
      {
        return _expConvertRaito;
      }
      set
      {
        __isset.expConvertRaito = true;
        this._expConvertRaito = value;
      }
    }

    /// <summary>
    /// 升级经验同类型道具经验倍数
    /// </summary>
    public sbyte SameTypeExpTimes
    {
      get
      {
        return _sameTypeExpTimes;
      }
      set
      {
        __isset.sameTypeExpTimes = true;
        this._sameTypeExpTimes = value;
      }
    }

    /// <summary>
    /// 可绑定道具数量上限
    /// </summary>
    public short BindItemLimit
    {
      get
      {
        return _bindItemLimit;
      }
      set
      {
        __isset.bindItemLimit = true;
        this._bindItemLimit = value;
      }
    }

    /// <summary>
    /// 出售道具折扣系数(%)
    /// </summary>
    public sbyte SellItemPriceRatio
    {
      get
      {
        return _sellItemPriceRatio;
      }
      set
      {
        __isset.sellItemPriceRatio = true;
        this._sellItemPriceRatio = value;
      }
    }

    /// <summary>
    /// 回购道具折扣系数(%)
    /// </summary>
    public sbyte BuybackItemPriceRatio
    {
      get
      {
        return _buybackItemPriceRatio;
      }
      set
      {
        __isset.buybackItemPriceRatio = true;
        this._buybackItemPriceRatio = value;
      }
    }

    /// <summary>
    /// 出售道具在回购背包保留时间（小时）
    /// </summary>
    public sbyte SellItemSaveHours
    {
      get
      {
        return _sellItemSaveHours;
      }
      set
      {
        __isset.sellItemSaveHours = true;
        this._sellItemSaveHours = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool expConvertRaito;
      public bool sameTypeExpTimes;
      public bool bindItemLimit;
      public bool sellItemPriceRatio;
      public bool buybackItemPriceRatio;
      public bool sellItemSaveHours;
    }

    public SCServerConfigMsg() {
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
            if (field.Type == TType.Byte) {
              ExpConvertRaito = iprot.ReadByte();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.Byte) {
              SameTypeExpTimes = iprot.ReadByte();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.I16) {
              BindItemLimit = iprot.ReadI16();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.Byte) {
              SellItemPriceRatio = iprot.ReadByte();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 5:
            if (field.Type == TType.Byte) {
              BuybackItemPriceRatio = iprot.ReadByte();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 6:
            if (field.Type == TType.Byte) {
              SellItemSaveHours = iprot.ReadByte();
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
      TStruct struc = new TStruct("SCServerConfigMsg");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.expConvertRaito) {
        field.Name = "expConvertRaito";
        field.Type = TType.Byte;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteByte(ExpConvertRaito);
        oprot.WriteFieldEnd();
      }
      if (__isset.sameTypeExpTimes) {
        field.Name = "sameTypeExpTimes";
        field.Type = TType.Byte;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteByte(SameTypeExpTimes);
        oprot.WriteFieldEnd();
      }
      if (__isset.bindItemLimit) {
        field.Name = "bindItemLimit";
        field.Type = TType.I16;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteI16(BindItemLimit);
        oprot.WriteFieldEnd();
      }
      if (__isset.sellItemPriceRatio) {
        field.Name = "sellItemPriceRatio";
        field.Type = TType.Byte;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteByte(SellItemPriceRatio);
        oprot.WriteFieldEnd();
      }
      if (__isset.buybackItemPriceRatio) {
        field.Name = "buybackItemPriceRatio";
        field.Type = TType.Byte;
        field.ID = 5;
        oprot.WriteFieldBegin(field);
        oprot.WriteByte(BuybackItemPriceRatio);
        oprot.WriteFieldEnd();
      }
      if (__isset.sellItemSaveHours) {
        field.Name = "sellItemSaveHours";
        field.Type = TType.Byte;
        field.ID = 6;
        oprot.WriteFieldBegin(field);
        oprot.WriteByte(SellItemSaveHours);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("SCServerConfigMsg(");
      sb.Append("ExpConvertRaito: ");
      sb.Append(ExpConvertRaito);
      sb.Append(",SameTypeExpTimes: ");
      sb.Append(SameTypeExpTimes);
      sb.Append(",BindItemLimit: ");
      sb.Append(BindItemLimit);
      sb.Append(",SellItemPriceRatio: ");
      sb.Append(SellItemPriceRatio);
      sb.Append(",BuybackItemPriceRatio: ");
      sb.Append(BuybackItemPriceRatio);
      sb.Append(",SellItemSaveHours: ");
      sb.Append(SellItemSaveHours);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
