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

namespace Template.Auto.Skill
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class SkillTargetGetterData : TBase
  {
    private int _targetType;
    private int _targetValue;

    public int TargetType
    {
      get
      {
        return _targetType;
      }
      set
      {
        __isset.targetType = true;
        this._targetType = value;
      }
    }

    public int TargetValue
    {
      get
      {
        return _targetValue;
      }
      set
      {
        __isset.targetValue = true;
        this._targetValue = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool targetType;
      public bool targetValue;
    }

    public SkillTargetGetterData() {
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
              TargetType = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.I32) {
              TargetValue = iprot.ReadI32();
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
      TStruct struc = new TStruct("SkillTargetGetterData");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.targetType) {
        field.Name = "targetType";
        field.Type = TType.I32;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(TargetType);
        oprot.WriteFieldEnd();
      }
      if (__isset.targetValue) {
        field.Name = "targetValue";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(TargetValue);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("SkillTargetGetterData(");
      sb.Append("TargetType: ");
      sb.Append(TargetType);
      sb.Append(",TargetValue: ");
      sb.Append(TargetValue);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
