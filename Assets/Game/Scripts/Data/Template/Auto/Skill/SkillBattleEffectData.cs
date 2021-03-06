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
  public partial class SkillBattleEffectData : TBase
  {
    private int _battleEffectType;
    private int _battleEffectRatio;
    private string _battleEffectParam1;
    private string _battleEffectParam2;
    private string _battleEffectParam3;

    public int BattleEffectType
    {
      get
      {
        return _battleEffectType;
      }
      set
      {
        __isset.battleEffectType = true;
        this._battleEffectType = value;
      }
    }

    public int BattleEffectRatio
    {
      get
      {
        return _battleEffectRatio;
      }
      set
      {
        __isset.battleEffectRatio = true;
        this._battleEffectRatio = value;
      }
    }

    public string BattleEffectParam1
    {
      get
      {
        return _battleEffectParam1;
      }
      set
      {
        __isset.battleEffectParam1 = true;
        this._battleEffectParam1 = value;
      }
    }

    public string BattleEffectParam2
    {
      get
      {
        return _battleEffectParam2;
      }
      set
      {
        __isset.battleEffectParam2 = true;
        this._battleEffectParam2 = value;
      }
    }

    public string BattleEffectParam3
    {
      get
      {
        return _battleEffectParam3;
      }
      set
      {
        __isset.battleEffectParam3 = true;
        this._battleEffectParam3 = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool battleEffectType;
      public bool battleEffectRatio;
      public bool battleEffectParam1;
      public bool battleEffectParam2;
      public bool battleEffectParam3;
    }

    public SkillBattleEffectData() {
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
              BattleEffectType = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.I32) {
              BattleEffectRatio = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.String) {
              BattleEffectParam1 = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.String) {
              BattleEffectParam2 = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 5:
            if (field.Type == TType.String) {
              BattleEffectParam3 = iprot.ReadString();
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
      TStruct struc = new TStruct("SkillBattleEffectData");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.battleEffectType) {
        field.Name = "battleEffectType";
        field.Type = TType.I32;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(BattleEffectType);
        oprot.WriteFieldEnd();
      }
      if (__isset.battleEffectRatio) {
        field.Name = "battleEffectRatio";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(BattleEffectRatio);
        oprot.WriteFieldEnd();
      }
      if (BattleEffectParam1 != null && __isset.battleEffectParam1) {
        field.Name = "battleEffectParam1";
        field.Type = TType.String;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(BattleEffectParam1);
        oprot.WriteFieldEnd();
      }
      if (BattleEffectParam2 != null && __isset.battleEffectParam2) {
        field.Name = "battleEffectParam2";
        field.Type = TType.String;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(BattleEffectParam2);
        oprot.WriteFieldEnd();
      }
      if (BattleEffectParam3 != null && __isset.battleEffectParam3) {
        field.Name = "battleEffectParam3";
        field.Type = TType.String;
        field.ID = 5;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(BattleEffectParam3);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("SkillBattleEffectData(");
      sb.Append("BattleEffectType: ");
      sb.Append(BattleEffectType);
      sb.Append(",BattleEffectRatio: ");
      sb.Append(BattleEffectRatio);
      sb.Append(",BattleEffectParam1: ");
      sb.Append(BattleEffectParam1);
      sb.Append(",BattleEffectParam2: ");
      sb.Append(BattleEffectParam2);
      sb.Append(",BattleEffectParam3: ");
      sb.Append(BattleEffectParam3);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
