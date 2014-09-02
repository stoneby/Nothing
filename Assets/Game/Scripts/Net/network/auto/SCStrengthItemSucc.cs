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

  /// <summary>
  /// 道具升级成功
  /// </summary>
  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class SCStrengthItemSucc : TBase
  {
    private short _operItemIndex;
    private int _updateContribExp;
    private SCDeleteItems _delteItems;

    /// <summary>
    /// 升级道具背包内位置
    /// </summary>
    public short OperItemIndex
    {
      get
      {
        return _operItemIndex;
      }
      set
      {
        __isset.operItemIndex = true;
        this._operItemIndex = value;
      }
    }

    /// <summary>
    /// 升级后贡献经验值
    /// </summary>
    public int UpdateContribExp
    {
      get
      {
        return _updateContribExp;
      }
      set
      {
        __isset.updateContribExp = true;
        this._updateContribExp = value;
      }
    }

    public SCDeleteItems DelteItems
    {
      get
      {
        return _delteItems;
      }
      set
      {
        __isset.delteItems = true;
        this._delteItems = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool operItemIndex;
      public bool updateContribExp;
      public bool delteItems;
    }

    public SCStrengthItemSucc() {
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
            if (field.Type == TType.I16) {
              OperItemIndex = iprot.ReadI16();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.I32) {
              UpdateContribExp = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.Struct) {
              DelteItems = new SCDeleteItems();
              DelteItems.Read(iprot);
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
      TStruct struc = new TStruct("SCStrengthItemSucc");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.operItemIndex) {
        field.Name = "operItemIndex";
        field.Type = TType.I16;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI16(OperItemIndex);
        oprot.WriteFieldEnd();
      }
      if (__isset.updateContribExp) {
        field.Name = "updateContribExp";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(UpdateContribExp);
        oprot.WriteFieldEnd();
      }
      if (DelteItems != null && __isset.delteItems) {
        field.Name = "delteItems";
        field.Type = TType.Struct;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        DelteItems.Write(oprot);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("SCStrengthItemSucc(");
      sb.Append("OperItemIndex: ");
      sb.Append(OperItemIndex);
      sb.Append(",UpdateContribExp: ");
      sb.Append(UpdateContribExp);
      sb.Append(",DelteItems: ");
      sb.Append(DelteItems== null ? "<null>" : DelteItems.ToString());
      sb.Append(")");
      return sb.ToString();
    }

  }

}
