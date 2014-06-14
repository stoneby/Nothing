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

namespace Template
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class Hero : TBase
  {
    private Dictionary<int, HeroTemplate> _heroTmpl;
    private Dictionary<int, HeroLevelUpTemplate> _lvlUpTmpl;
    private Dictionary<int, HeroBaseTemplate> _baseTmpl;

    public Dictionary<int, HeroTemplate> HeroTmpl
    {
      get
      {
        return _heroTmpl;
      }
      set
      {
        __isset.heroTmpl = true;
        this._heroTmpl = value;
      }
    }

    public Dictionary<int, HeroLevelUpTemplate> LvlUpTmpl
    {
      get
      {
        return _lvlUpTmpl;
      }
      set
      {
        __isset.lvlUpTmpl = true;
        this._lvlUpTmpl = value;
      }
    }

    public Dictionary<int, HeroBaseTemplate> BaseTmpl
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


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool heroTmpl;
      public bool lvlUpTmpl;
      public bool baseTmpl;
    }

    public Hero() {
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
            if (field.Type == TType.Map) {
              {
                HeroTmpl = new Dictionary<int, HeroTemplate>();
                TMap _map0 = iprot.ReadMapBegin();
                for( int _i1 = 0; _i1 < _map0.Count; ++_i1)
                {
                  int _key2;
                  HeroTemplate _val3;
                  _key2 = iprot.ReadI32();
                  _val3 = new HeroTemplate();
                  _val3.Read(iprot);
                  HeroTmpl[_key2] = _val3;
                }
                iprot.ReadMapEnd();
              }
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.Map) {
              {
                LvlUpTmpl = new Dictionary<int, HeroLevelUpTemplate>();
                TMap _map4 = iprot.ReadMapBegin();
                for( int _i5 = 0; _i5 < _map4.Count; ++_i5)
                {
                  int _key6;
                  HeroLevelUpTemplate _val7;
                  _key6 = iprot.ReadI32();
                  _val7 = new HeroLevelUpTemplate();
                  _val7.Read(iprot);
                  LvlUpTmpl[_key6] = _val7;
                }
                iprot.ReadMapEnd();
              }
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.Map) {
              {
                BaseTmpl = new Dictionary<int, HeroBaseTemplate>();
                TMap _map8 = iprot.ReadMapBegin();
                for( int _i9 = 0; _i9 < _map8.Count; ++_i9)
                {
                  int _key10;
                  HeroBaseTemplate _val11;
                  _key10 = iprot.ReadI32();
                  _val11 = new HeroBaseTemplate();
                  _val11.Read(iprot);
                  BaseTmpl[_key10] = _val11;
                }
                iprot.ReadMapEnd();
              }
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
      TStruct struc = new TStruct("Hero");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (HeroTmpl != null && __isset.heroTmpl) {
        field.Name = "heroTmpl";
        field.Type = TType.Map;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteMapBegin(new TMap(TType.I32, TType.Struct, HeroTmpl.Count));
          foreach (int _iter12 in HeroTmpl.Keys)
          {
            oprot.WriteI32(_iter12);
            HeroTmpl[_iter12].Write(oprot);
          }
          oprot.WriteMapEnd();
        }
        oprot.WriteFieldEnd();
      }
      if (LvlUpTmpl != null && __isset.lvlUpTmpl) {
        field.Name = "lvlUpTmpl";
        field.Type = TType.Map;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteMapBegin(new TMap(TType.I32, TType.Struct, LvlUpTmpl.Count));
          foreach (int _iter13 in LvlUpTmpl.Keys)
          {
            oprot.WriteI32(_iter13);
            LvlUpTmpl[_iter13].Write(oprot);
          }
          oprot.WriteMapEnd();
        }
        oprot.WriteFieldEnd();
      }
      if (BaseTmpl != null && __isset.baseTmpl) {
        field.Name = "baseTmpl";
        field.Type = TType.Map;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteMapBegin(new TMap(TType.I32, TType.Struct, BaseTmpl.Count));
          foreach (int _iter14 in BaseTmpl.Keys)
          {
            oprot.WriteI32(_iter14);
            BaseTmpl[_iter14].Write(oprot);
          }
          oprot.WriteMapEnd();
        }
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("Hero(");
      sb.Append("HeroTmpl: ");
      sb.Append(HeroTmpl);
      sb.Append(",LvlUpTmpl: ");
      sb.Append(LvlUpTmpl);
      sb.Append(",BaseTmpl: ");
      sb.Append(BaseTmpl);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
