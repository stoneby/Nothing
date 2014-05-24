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
public partial class SkillTemplate : TBase
{
  private int _id;
  private string _name;
  private string _desc;
  private string _skillTypeDesc;
  private int _maxLvl;
  private int _costMp;
  private sbyte _occorRate;

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

  /// <summary>
  /// 技能名称
  /// </summary>
  public string Name
  {
    get
    {
      return _name;
    }
    set
    {
      __isset.name = true;
      this._name = value;
    }
  }

  /// <summary>
  /// 技能描述
  /// </summary>
  public string Desc
  {
    get
    {
      return _desc;
    }
    set
    {
      __isset.desc = true;
      this._desc = value;
    }
  }

  /// <summary>
  /// 技能类型描述
  /// </summary>
  public string SkillTypeDesc
  {
    get
    {
      return _skillTypeDesc;
    }
    set
    {
      __isset.skillTypeDesc = true;
      this._skillTypeDesc = value;
    }
  }

  /// <summary>
  /// 最高等级
  /// </summary>
  public int MaxLvl
  {
    get
    {
      return _maxLvl;
    }
    set
    {
      __isset.maxLvl = true;
      this._maxLvl = value;
    }
  }

  /// <summary>
  /// 消耗气力
  /// </summary>
  public int CostMp
  {
    get
    {
      return _costMp;
    }
    set
    {
      __isset.costMp = true;
      this._costMp = value;
    }
  }

  public sbyte OccorRate
  {
    get
    {
      return _occorRate;
    }
    set
    {
      __isset.occorRate = true;
      this._occorRate = value;
    }
  }


  public Isset __isset;
  #if !SILVERLIGHT
  [Serializable]
  #endif
  public struct Isset {
    public bool id;
    public bool name;
    public bool desc;
    public bool skillTypeDesc;
    public bool maxLvl;
    public bool costMp;
    public bool occorRate;
  }

  public SkillTemplate() {
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
          if (field.Type == TType.String) {
            Name = iprot.ReadString();
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 3:
          if (field.Type == TType.String) {
            Desc = iprot.ReadString();
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 4:
          if (field.Type == TType.String) {
            SkillTypeDesc = iprot.ReadString();
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 5:
          if (field.Type == TType.I32) {
            MaxLvl = iprot.ReadI32();
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 8:
          if (field.Type == TType.I32) {
            CostMp = iprot.ReadI32();
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 10:
          if (field.Type == TType.Byte) {
            OccorRate = iprot.ReadByte();
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
    TStruct struc = new TStruct("SkillTemplate");
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
    if (Name != null && __isset.name) {
      field.Name = "name";
      field.Type = TType.String;
      field.ID = 2;
      oprot.WriteFieldBegin(field);
      oprot.WriteString(Name);
      oprot.WriteFieldEnd();
    }
    if (Desc != null && __isset.desc) {
      field.Name = "desc";
      field.Type = TType.String;
      field.ID = 3;
      oprot.WriteFieldBegin(field);
      oprot.WriteString(Desc);
      oprot.WriteFieldEnd();
    }
    if (SkillTypeDesc != null && __isset.skillTypeDesc) {
      field.Name = "skillTypeDesc";
      field.Type = TType.String;
      field.ID = 4;
      oprot.WriteFieldBegin(field);
      oprot.WriteString(SkillTypeDesc);
      oprot.WriteFieldEnd();
    }
    if (__isset.maxLvl) {
      field.Name = "maxLvl";
      field.Type = TType.I32;
      field.ID = 5;
      oprot.WriteFieldBegin(field);
      oprot.WriteI32(MaxLvl);
      oprot.WriteFieldEnd();
    }
    if (__isset.costMp) {
      field.Name = "costMp";
      field.Type = TType.I32;
      field.ID = 8;
      oprot.WriteFieldBegin(field);
      oprot.WriteI32(CostMp);
      oprot.WriteFieldEnd();
    }
    if (__isset.occorRate) {
      field.Name = "occorRate";
      field.Type = TType.Byte;
      field.ID = 10;
      oprot.WriteFieldBegin(field);
      oprot.WriteByte(OccorRate);
      oprot.WriteFieldEnd();
    }
    oprot.WriteFieldStop();
    oprot.WriteStructEnd();
  }

  public override string ToString() {
    StringBuilder sb = new StringBuilder("SkillTemplate(");
    sb.Append("Id: ");
    sb.Append(Id);
    sb.Append(",Name: ");
    sb.Append(Name);
    sb.Append(",Desc: ");
    sb.Append(Desc);
    sb.Append(",SkillTypeDesc: ");
    sb.Append(SkillTypeDesc);
    sb.Append(",MaxLvl: ");
    sb.Append(MaxLvl);
    sb.Append(",CostMp: ");
    sb.Append(CostMp);
    sb.Append(",OccorRate: ");
    sb.Append(OccorRate);
    sb.Append(")");
    return sb.ToString();
  }

}
