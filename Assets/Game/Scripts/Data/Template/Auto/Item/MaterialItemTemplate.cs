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
  public partial class MaterialItemTemplate : TBase
  {
    private BaseItemTemplate _baseTmpl;
    private sbyte _fitType;
    private sbyte _fitJobType;
    private sbyte _type;

    /// <summary>
    /// 基础模板信息
    /// </summary>
    public BaseItemTemplate BaseTmpl
    {
      get
      {
        return _baseTmpl;
      }
      set
      {
        __isset.baseTmpl = true;
        this._baseTmpl = value;
      }
    }

    /// <summary>
    /// 适用于武器还是防具
    /// </summary>
    public sbyte FitType
    {
      get
      {
        return _fitType;
      }
      set
      {
        __isset.fitType = true;
        this._fitType = value;
      }
    }

    /// <summary>
    /// 适用职业类型
    /// </summary>
    public sbyte FitJobType
    {
      get
      {
        return _fitJobType;
      }
      set
      {
        __isset.fitJobType = true;
        this._fitJobType = value;
      }
    }

    /// <summary>
    /// 材料类型
    /// </summary>
    public sbyte Type
    {
      get
      {
        return _type;
      }
      set
      {
        __isset.type = true;
        this._type = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool baseTmpl;
      public bool fitType;
      public bool fitJobType;
      public bool type;
    }

    public MaterialItemTemplate() {
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
            if (field.Type == TType.Struct) {
              BaseTmpl = new BaseItemTemplate();
              BaseTmpl.Read(iprot);
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.Byte) {
              FitType = iprot.ReadByte();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.Byte) {
              FitJobType = iprot.ReadByte();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.Byte) {
              Type = iprot.ReadByte();
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
      TStruct struc = new TStruct("MaterialItemTemplate");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (BaseTmpl != null && __isset.baseTmpl) {
        field.Name = "baseTmpl";
        field.Type = TType.Struct;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        BaseTmpl.Write(oprot);
        oprot.WriteFieldEnd();
      }
      if (__isset.fitType) {
        field.Name = "fitType";
        field.Type = TType.Byte;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteByte(FitType);
        oprot.WriteFieldEnd();
      }
      if (__isset.fitJobType) {
        field.Name = "fitJobType";
        field.Type = TType.Byte;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteByte(FitJobType);
        oprot.WriteFieldEnd();
      }
      if (__isset.type) {
        field.Name = "type";
        field.Type = TType.Byte;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteByte(Type);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("MaterialItemTemplate(");
      sb.Append("BaseTmpl: ");
      sb.Append(BaseTmpl== null ? "<null>" : BaseTmpl.ToString());
      sb.Append(",FitType: ");
      sb.Append(FitType);
      sb.Append(",FitJobType: ");
      sb.Append(FitJobType);
      sb.Append(",Type: ");
      sb.Append(Type);
      sb.Append(")");
      return sb.ToString();
    }

  }

}