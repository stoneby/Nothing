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
  public partial class Property : TBase
  {
    private int _ATK;
    private int _HP;
    private int _RECOVER;
    private int _MP;

    public int ATK
    {
      get
      {
        return _ATK;
      }
      set
      {
        __isset.ATK = true;
        this._ATK = value;
      }
    }

    public int HP
    {
      get
      {
        return _HP;
      }
      set
      {
        __isset.HP = true;
        this._HP = value;
      }
    }

    public int RECOVER
    {
      get
      {
        return _RECOVER;
      }
      set
      {
        __isset.RECOVER = true;
        this._RECOVER = value;
      }
    }

    public int MP
    {
      get
      {
        return _MP;
      }
      set
      {
        __isset.MP = true;
        this._MP = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool ATK;
      public bool HP;
      public bool RECOVER;
      public bool MP;
    }

    public Property() {
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
              ATK = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.I32) {
              HP = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.I32) {
              RECOVER = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.I32) {
              MP = iprot.ReadI32();
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
      TStruct struc = new TStruct("Property");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.ATK) {
        field.Name = "ATK";
        field.Type = TType.I32;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(ATK);
        oprot.WriteFieldEnd();
      }
      if (__isset.HP) {
        field.Name = "HP";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(HP);
        oprot.WriteFieldEnd();
      }
      if (__isset.RECOVER) {
        field.Name = "RECOVER";
        field.Type = TType.I32;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(RECOVER);
        oprot.WriteFieldEnd();
      }
      if (__isset.MP) {
        field.Name = "MP";
        field.Type = TType.I32;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(MP);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("Property(");
      sb.Append("ATK: ");
      sb.Append(ATK);
      sb.Append(",HP: ");
      sb.Append(HP);
      sb.Append(",RECOVER: ");
      sb.Append(RECOVER);
      sb.Append(",MP: ");
      sb.Append(MP);
      sb.Append(")");
      return sb.ToString();
    }

  }

}