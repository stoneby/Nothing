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
  public partial class SCPlayerInfoMsg : TBase
  {
    private short _heroId;
    private string _name;
    private short _lvl;
    private int _headIconId;
    private int _exp;
    private long _diamond;
    private long _gold;

    /// <summary>
    /// 对应武将id
    /// </summary>
    public short HeroId
    {
      get
      {
        return _heroId;
      }
      set
      {
        __isset.heroId = true;
        this._heroId = value;
      }
    }

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

    public short Lvl
    {
      get
      {
        return _lvl;
      }
      set
      {
        __isset.lvl = true;
        this._lvl = value;
      }
    }

    public int HeadIconId
    {
      get
      {
        return _headIconId;
      }
      set
      {
        __isset.headIconId = true;
        this._headIconId = value;
      }
    }

    public int Exp
    {
      get
      {
        return _exp;
      }
      set
      {
        __isset.exp = true;
        this._exp = value;
      }
    }

    public long Diamond
    {
      get
      {
        return _diamond;
      }
      set
      {
        __isset.diamond = true;
        this._diamond = value;
      }
    }

    public long Gold
    {
      get
      {
        return _gold;
      }
      set
      {
        __isset.gold = true;
        this._gold = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool heroId;
      public bool name;
      public bool lvl;
      public bool headIconId;
      public bool exp;
      public bool diamond;
      public bool gold;
    }

    public SCPlayerInfoMsg() {
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
              HeroId = iprot.ReadI16();
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
            if (field.Type == TType.I16) {
              Lvl = iprot.ReadI16();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.I32) {
              HeadIconId = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 5:
            if (field.Type == TType.I32) {
              Exp = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 6:
            if (field.Type == TType.I64) {
              Diamond = iprot.ReadI64();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 7:
            if (field.Type == TType.I64) {
              Gold = iprot.ReadI64();
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
      TStruct struc = new TStruct("SCPlayerInfoMsg");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.heroId) {
        field.Name = "heroId";
        field.Type = TType.I16;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI16(HeroId);
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
      if (__isset.lvl) {
        field.Name = "lvl";
        field.Type = TType.I16;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteI16(Lvl);
        oprot.WriteFieldEnd();
      }
      if (__isset.headIconId) {
        field.Name = "headIconId";
        field.Type = TType.I32;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(HeadIconId);
        oprot.WriteFieldEnd();
      }
      if (__isset.exp) {
        field.Name = "exp";
        field.Type = TType.I32;
        field.ID = 5;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(Exp);
        oprot.WriteFieldEnd();
      }
      if (__isset.diamond) {
        field.Name = "diamond";
        field.Type = TType.I64;
        field.ID = 6;
        oprot.WriteFieldBegin(field);
        oprot.WriteI64(Diamond);
        oprot.WriteFieldEnd();
      }
      if (__isset.gold) {
        field.Name = "gold";
        field.Type = TType.I64;
        field.ID = 7;
        oprot.WriteFieldBegin(field);
        oprot.WriteI64(Gold);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("SCPlayerInfoMsg(");
      sb.Append("HeroId: ");
      sb.Append(HeroId);
      sb.Append(",Name: ");
      sb.Append(Name);
      sb.Append(",Lvl: ");
      sb.Append(Lvl);
      sb.Append(",HeadIconId: ");
      sb.Append(HeadIconId);
      sb.Append(",Exp: ");
      sb.Append(Exp);
      sb.Append(",Diamond: ");
      sb.Append(Diamond);
      sb.Append(",Gold: ");
      sb.Append(Gold);
      sb.Append(")");
      return sb.ToString();
    }

  }

}