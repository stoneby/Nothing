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

namespace Template.Auto.Monster
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class SkillRatePairData : TBase
  {
    private int _skillId;
    private int _rouletteRate;

    public int SkillId
    {
      get
      {
        return _skillId;
      }
      set
      {
        __isset.skillId = true;
        this._skillId = value;
      }
    }

    public int RouletteRate
    {
      get
      {
        return _rouletteRate;
      }
      set
      {
        __isset.rouletteRate = true;
        this._rouletteRate = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool skillId;
      public bool rouletteRate;
    }

    public SkillRatePairData() {
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
              SkillId = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.I32) {
              RouletteRate = iprot.ReadI32();
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
      TStruct struc = new TStruct("SkillRatePairData");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.skillId) {
        field.Name = "skillId";
        field.Type = TType.I32;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(SkillId);
        oprot.WriteFieldEnd();
      }
      if (__isset.rouletteRate) {
        field.Name = "rouletteRate";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(RouletteRate);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("SkillRatePairData(");
      sb.Append("SkillId: ");
      sb.Append(SkillId);
      sb.Append(",RouletteRate: ");
      sb.Append(RouletteRate);
      sb.Append(")");
      return sb.ToString();
    }

  }

}