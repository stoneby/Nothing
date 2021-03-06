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

namespace Template.Auto.Item
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class ItemEvoluteParam : TBase
  {
    private int _needMaterialId;
    private int _needMaterialCount;

    /// <summary>
    /// 消耗材料id
    /// </summary>
    public int NeedMaterialId
    {
      get
      {
        return _needMaterialId;
      }
      set
      {
        __isset.needMaterialId = true;
        this._needMaterialId = value;
      }
    }

    /// <summary>
    /// 消耗材料数量
    /// </summary>
    public int NeedMaterialCount
    {
      get
      {
        return _needMaterialCount;
      }
      set
      {
        __isset.needMaterialCount = true;
        this._needMaterialCount = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool needMaterialId;
      public bool needMaterialCount;
    }

    public ItemEvoluteParam() {
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
              NeedMaterialId = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.I32) {
              NeedMaterialCount = iprot.ReadI32();
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
      TStruct struc = new TStruct("ItemEvoluteParam");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.needMaterialId) {
        field.Name = "needMaterialId";
        field.Type = TType.I32;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(NeedMaterialId);
        oprot.WriteFieldEnd();
      }
      if (__isset.needMaterialCount) {
        field.Name = "needMaterialCount";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(NeedMaterialCount);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("ItemEvoluteParam(");
      sb.Append("NeedMaterialId: ");
      sb.Append(NeedMaterialId);
      sb.Append(",NeedMaterialCount: ");
      sb.Append(NeedMaterialCount);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
