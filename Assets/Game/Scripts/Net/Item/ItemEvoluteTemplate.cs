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


#if !SILVERLIGHT
[Serializable]
#endif
public partial class ItemEvoluteTemplate : TBase
{
  private int _id;
  private int _targetItemId;
  private int _costGold;
  private int _needMaterialId1;
  private int _needMaterialCount1;
  private int _needMaterialId2;
  private int _needMaterialCount2;
  private int _needMaterialId3;
  private int _needMaterialCount3;
  private int _needMaterialId4;
  private int _needMaterialCount4;

  public int Id
  {
    get
    {
      return _id;
    }
    set
    {
      __isset.id = true;
      this._id = value;
    }
  }

  public int TargetItemId
  {
    get
    {
      return _targetItemId;
    }
    set
    {
      __isset.targetItemId = true;
      this._targetItemId = value;
    }
  }

  public int CostGold
  {
    get
    {
      return _costGold;
    }
    set
    {
      __isset.costGold = true;
      this._costGold = value;
    }
  }

  public int NeedMaterialId1
  {
    get
    {
      return _needMaterialId1;
    }
    set
    {
      __isset.needMaterialId1 = true;
      this._needMaterialId1 = value;
    }
  }

  public int NeedMaterialCount1
  {
    get
    {
      return _needMaterialCount1;
    }
    set
    {
      __isset.needMaterialCount1 = true;
      this._needMaterialCount1 = value;
    }
  }

  public int NeedMaterialId2
  {
    get
    {
      return _needMaterialId2;
    }
    set
    {
      __isset.needMaterialId2 = true;
      this._needMaterialId2 = value;
    }
  }

  public int NeedMaterialCount2
  {
    get
    {
      return _needMaterialCount2;
    }
    set
    {
      __isset.needMaterialCount2 = true;
      this._needMaterialCount2 = value;
    }
  }

  public int NeedMaterialId3
  {
    get
    {
      return _needMaterialId3;
    }
    set
    {
      __isset.needMaterialId3 = true;
      this._needMaterialId3 = value;
    }
  }

  public int NeedMaterialCount3
  {
    get
    {
      return _needMaterialCount3;
    }
    set
    {
      __isset.needMaterialCount3 = true;
      this._needMaterialCount3 = value;
    }
  }

  public int NeedMaterialId4
  {
    get
    {
      return _needMaterialId4;
    }
    set
    {
      __isset.needMaterialId4 = true;
      this._needMaterialId4 = value;
    }
  }

  public int NeedMaterialCount4
  {
    get
    {
      return _needMaterialCount4;
    }
    set
    {
      __isset.needMaterialCount4 = true;
      this._needMaterialCount4 = value;
    }
  }


  public Isset __isset;
  #if !SILVERLIGHT
  [Serializable]
  #endif
  public struct Isset {
    public bool id;
    public bool targetItemId;
    public bool costGold;
    public bool needMaterialId1;
    public bool needMaterialCount1;
    public bool needMaterialId2;
    public bool needMaterialCount2;
    public bool needMaterialId3;
    public bool needMaterialCount3;
    public bool needMaterialId4;
    public bool needMaterialCount4;
  }

  public ItemEvoluteTemplate() {
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
            Id = iprot.ReadI32();
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 2:
          if (field.Type == TType.I32) {
            TargetItemId = iprot.ReadI32();
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 3:
          if (field.Type == TType.I32) {
            CostGold = iprot.ReadI32();
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 4:
          if (field.Type == TType.I32) {
            NeedMaterialId1 = iprot.ReadI32();
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 5:
          if (field.Type == TType.I32) {
            NeedMaterialCount1 = iprot.ReadI32();
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 6:
          if (field.Type == TType.I32) {
            NeedMaterialId2 = iprot.ReadI32();
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 7:
          if (field.Type == TType.I32) {
            NeedMaterialCount2 = iprot.ReadI32();
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 8:
          if (field.Type == TType.I32) {
            NeedMaterialId3 = iprot.ReadI32();
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 9:
          if (field.Type == TType.I32) {
            NeedMaterialCount3 = iprot.ReadI32();
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 10:
          if (field.Type == TType.I32) {
            NeedMaterialId4 = iprot.ReadI32();
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 11:
          if (field.Type == TType.I32) {
            NeedMaterialCount4 = iprot.ReadI32();
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
    TStruct struc = new TStruct("ItemEvoluteTemplate");
    oprot.WriteStructBegin(struc);
    TField field = new TField();
    if (__isset.id) {
      field.Name = "id";
      field.Type = TType.I32;
      field.ID = 1;
      oprot.WriteFieldBegin(field);
      oprot.WriteI32(Id);
      oprot.WriteFieldEnd();
    }
    if (__isset.targetItemId) {
      field.Name = "targetItemId";
      field.Type = TType.I32;
      field.ID = 2;
      oprot.WriteFieldBegin(field);
      oprot.WriteI32(TargetItemId);
      oprot.WriteFieldEnd();
    }
    if (__isset.costGold) {
      field.Name = "costGold";
      field.Type = TType.I32;
      field.ID = 3;
      oprot.WriteFieldBegin(field);
      oprot.WriteI32(CostGold);
      oprot.WriteFieldEnd();
    }
    if (__isset.needMaterialId1) {
      field.Name = "needMaterialId1";
      field.Type = TType.I32;
      field.ID = 4;
      oprot.WriteFieldBegin(field);
      oprot.WriteI32(NeedMaterialId1);
      oprot.WriteFieldEnd();
    }
    if (__isset.needMaterialCount1) {
      field.Name = "needMaterialCount1";
      field.Type = TType.I32;
      field.ID = 5;
      oprot.WriteFieldBegin(field);
      oprot.WriteI32(NeedMaterialCount1);
      oprot.WriteFieldEnd();
    }
    if (__isset.needMaterialId2) {
      field.Name = "needMaterialId2";
      field.Type = TType.I32;
      field.ID = 6;
      oprot.WriteFieldBegin(field);
      oprot.WriteI32(NeedMaterialId2);
      oprot.WriteFieldEnd();
    }
    if (__isset.needMaterialCount2) {
      field.Name = "needMaterialCount2";
      field.Type = TType.I32;
      field.ID = 7;
      oprot.WriteFieldBegin(field);
      oprot.WriteI32(NeedMaterialCount2);
      oprot.WriteFieldEnd();
    }
    if (__isset.needMaterialId3) {
      field.Name = "needMaterialId3";
      field.Type = TType.I32;
      field.ID = 8;
      oprot.WriteFieldBegin(field);
      oprot.WriteI32(NeedMaterialId3);
      oprot.WriteFieldEnd();
    }
    if (__isset.needMaterialCount3) {
      field.Name = "needMaterialCount3";
      field.Type = TType.I32;
      field.ID = 9;
      oprot.WriteFieldBegin(field);
      oprot.WriteI32(NeedMaterialCount3);
      oprot.WriteFieldEnd();
    }
    if (__isset.needMaterialId4) {
      field.Name = "needMaterialId4";
      field.Type = TType.I32;
      field.ID = 10;
      oprot.WriteFieldBegin(field);
      oprot.WriteI32(NeedMaterialId4);
      oprot.WriteFieldEnd();
    }
    if (__isset.needMaterialCount4) {
      field.Name = "needMaterialCount4";
      field.Type = TType.I32;
      field.ID = 11;
      oprot.WriteFieldBegin(field);
      oprot.WriteI32(NeedMaterialCount4);
      oprot.WriteFieldEnd();
    }
    oprot.WriteFieldStop();
    oprot.WriteStructEnd();
  }

  public override string ToString() {
    StringBuilder sb = new StringBuilder("ItemEvoluteTemplate(");
    sb.Append("Id: ");
    sb.Append(Id);
    sb.Append(",TargetItemId: ");
    sb.Append(TargetItemId);
    sb.Append(",CostGold: ");
    sb.Append(CostGold);
    sb.Append(",NeedMaterialId1: ");
    sb.Append(NeedMaterialId1);
    sb.Append(",NeedMaterialCount1: ");
    sb.Append(NeedMaterialCount1);
    sb.Append(",NeedMaterialId2: ");
    sb.Append(NeedMaterialId2);
    sb.Append(",NeedMaterialCount2: ");
    sb.Append(NeedMaterialCount2);
    sb.Append(",NeedMaterialId3: ");
    sb.Append(NeedMaterialId3);
    sb.Append(",NeedMaterialCount3: ");
    sb.Append(NeedMaterialCount3);
    sb.Append(",NeedMaterialId4: ");
    sb.Append(NeedMaterialId4);
    sb.Append(",NeedMaterialCount4: ");
    sb.Append(NeedMaterialCount4);
    sb.Append(")");
    return sb.ToString();
  }

}
