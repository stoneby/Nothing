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
  public partial class CSBattlePveFinishMsg : TBase
  {
    private long _uuid;
    private int _battleResult;
    private sbyte _star;
    private string _checkCode;

    public long Uuid
    {
      get
      {
        return _uuid;
      }
      set
      {
        __isset.uuid = true;
        this._uuid = value;
      }
    }

    public int BattleResult
    {
      get
      {
        return _battleResult;
      }
      set
      {
        __isset.battleResult = true;
        this._battleResult = value;
      }
    }

    public sbyte Star
    {
      get
      {
        return _star;
      }
      set
      {
        __isset.star = true;
        this._star = value;
      }
    }

    public string CheckCode
    {
      get
      {
        return _checkCode;
      }
      set
      {
        __isset.checkCode = true;
        this._checkCode = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool uuid;
      public bool battleResult;
      public bool star;
      public bool checkCode;
    }

    public CSBattlePveFinishMsg() {
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
            if (field.Type == TType.I64) {
              Uuid = iprot.ReadI64();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.I32) {
              BattleResult = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.Byte) {
              Star = iprot.ReadByte();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.String) {
              CheckCode = iprot.ReadString();
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
      TStruct struc = new TStruct("CSBattlePveFinishMsg");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.uuid) {
        field.Name = "uuid";
        field.Type = TType.I64;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI64(Uuid);
        oprot.WriteFieldEnd();
      }
      if (__isset.battleResult) {
        field.Name = "battleResult";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(BattleResult);
        oprot.WriteFieldEnd();
      }
      if (__isset.star) {
        field.Name = "star";
        field.Type = TType.Byte;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteByte(Star);
        oprot.WriteFieldEnd();
      }
      if (CheckCode != null && __isset.checkCode) {
        field.Name = "checkCode";
        field.Type = TType.String;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(CheckCode);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("CSBattlePveFinishMsg(");
      sb.Append("Uuid: ");
      sb.Append(Uuid);
      sb.Append(",BattleResult: ");
      sb.Append(BattleResult);
      sb.Append(",Star: ");
      sb.Append(Star);
      sb.Append(",CheckCode: ");
      sb.Append(CheckCode);
      sb.Append(")");
      return sb.ToString();
    }

  }

}