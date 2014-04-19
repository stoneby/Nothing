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
  public partial class BattleMsgFighter : TBase
  {
    private int _uuid;
    private int _index;
    private int _templateId;
    private Dictionary<int, int> _propties;
    private List<int> _leaderSkill;
    private List<int> _activeSkill;
    private List<int> _giftSkill;
    private List<int> _warriorsSkill;
    private List<int> _weaponSkill;

    public int Uuid
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

    public int Index
    {
      get
      {
        return _index;
      }
      set
      {
        __isset.index = true;
        this._index = value;
      }
    }

    public int TemplateId
    {
      get
      {
        return _templateId;
      }
      set
      {
        __isset.templateId = true;
        this._templateId = value;
      }
    }

    public Dictionary<int, int> Propties
    {
      get
      {
        return _propties;
      }
      set
      {
        __isset.propties = true;
        this._propties = value;
      }
    }

    public List<int> LeaderSkill
    {
      get
      {
        return _leaderSkill;
      }
      set
      {
        __isset.leaderSkill = true;
        this._leaderSkill = value;
      }
    }

    public List<int> ActiveSkill
    {
      get
      {
        return _activeSkill;
      }
      set
      {
        __isset.activeSkill = true;
        this._activeSkill = value;
      }
    }

    public List<int> GiftSkill
    {
      get
      {
        return _giftSkill;
      }
      set
      {
        __isset.giftSkill = true;
        this._giftSkill = value;
      }
    }

    public List<int> WarriorsSkill
    {
      get
      {
        return _warriorsSkill;
      }
      set
      {
        __isset.warriorsSkill = true;
        this._warriorsSkill = value;
      }
    }

    public List<int> WeaponSkill
    {
      get
      {
        return _weaponSkill;
      }
      set
      {
        __isset.weaponSkill = true;
        this._weaponSkill = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool uuid;
      public bool index;
      public bool templateId;
      public bool propties;
      public bool leaderSkill;
      public bool activeSkill;
      public bool giftSkill;
      public bool warriorsSkill;
      public bool weaponSkill;
    }

    public BattleMsgFighter() {
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
              Uuid = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.I32) {
              Index = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.I32) {
              TemplateId = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.Map) {
              {
                Propties = new Dictionary<int, int>();
                TMap _map0 = iprot.ReadMapBegin();
                for( int _i1 = 0; _i1 < _map0.Count; ++_i1)
                {
                  int _key2;
                  int _val3;
                  _key2 = iprot.ReadI32();
                  _val3 = iprot.ReadI32();
                  Propties[_key2] = _val3;
                }
                iprot.ReadMapEnd();
              }
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 5:
            if (field.Type == TType.List) {
              {
                LeaderSkill = new List<int>();
                TList _list4 = iprot.ReadListBegin();
                for( int _i5 = 0; _i5 < _list4.Count; ++_i5)
                {
                  int _elem6 = 0;
                  _elem6 = iprot.ReadI32();
                  LeaderSkill.Add(_elem6);
                }
                iprot.ReadListEnd();
              }
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 6:
            if (field.Type == TType.List) {
              {
                ActiveSkill = new List<int>();
                TList _list7 = iprot.ReadListBegin();
                for( int _i8 = 0; _i8 < _list7.Count; ++_i8)
                {
                  int _elem9 = 0;
                  _elem9 = iprot.ReadI32();
                  ActiveSkill.Add(_elem9);
                }
                iprot.ReadListEnd();
              }
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 7:
            if (field.Type == TType.List) {
              {
                GiftSkill = new List<int>();
                TList _list10 = iprot.ReadListBegin();
                for( int _i11 = 0; _i11 < _list10.Count; ++_i11)
                {
                  int _elem12 = 0;
                  _elem12 = iprot.ReadI32();
                  GiftSkill.Add(_elem12);
                }
                iprot.ReadListEnd();
              }
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 8:
            if (field.Type == TType.List) {
              {
                WarriorsSkill = new List<int>();
                TList _list13 = iprot.ReadListBegin();
                for( int _i14 = 0; _i14 < _list13.Count; ++_i14)
                {
                  int _elem15 = 0;
                  _elem15 = iprot.ReadI32();
                  WarriorsSkill.Add(_elem15);
                }
                iprot.ReadListEnd();
              }
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 9:
            if (field.Type == TType.List) {
              {
                WeaponSkill = new List<int>();
                TList _list16 = iprot.ReadListBegin();
                for( int _i17 = 0; _i17 < _list16.Count; ++_i17)
                {
                  int _elem18 = 0;
                  _elem18 = iprot.ReadI32();
                  WeaponSkill.Add(_elem18);
                }
                iprot.ReadListEnd();
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
      TStruct struc = new TStruct("BattleMsgFighter");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.uuid) {
        field.Name = "uuid";
        field.Type = TType.I32;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(Uuid);
        oprot.WriteFieldEnd();
      }
      if (__isset.index) {
        field.Name = "index";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(Index);
        oprot.WriteFieldEnd();
      }
      if (__isset.templateId) {
        field.Name = "templateId";
        field.Type = TType.I32;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(TemplateId);
        oprot.WriteFieldEnd();
      }
      if (Propties != null && __isset.propties) {
        field.Name = "propties";
        field.Type = TType.Map;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteMapBegin(new TMap(TType.I32, TType.I32, Propties.Count));
          foreach (int _iter19 in Propties.Keys)
          {
            oprot.WriteI32(_iter19);
            oprot.WriteI32(Propties[_iter19]);
          }
          oprot.WriteMapEnd();
        }
        oprot.WriteFieldEnd();
      }
      if (LeaderSkill != null && __isset.leaderSkill) {
        field.Name = "leaderSkill";
        field.Type = TType.List;
        field.ID = 5;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteListBegin(new TList(TType.I32, LeaderSkill.Count));
          foreach (int _iter20 in LeaderSkill)
          {
            oprot.WriteI32(_iter20);
          }
          oprot.WriteListEnd();
        }
        oprot.WriteFieldEnd();
      }
      if (ActiveSkill != null && __isset.activeSkill) {
        field.Name = "activeSkill";
        field.Type = TType.List;
        field.ID = 6;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteListBegin(new TList(TType.I32, ActiveSkill.Count));
          foreach (int _iter21 in ActiveSkill)
          {
            oprot.WriteI32(_iter21);
          }
          oprot.WriteListEnd();
        }
        oprot.WriteFieldEnd();
      }
      if (GiftSkill != null && __isset.giftSkill) {
        field.Name = "giftSkill";
        field.Type = TType.List;
        field.ID = 7;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteListBegin(new TList(TType.I32, GiftSkill.Count));
          foreach (int _iter22 in GiftSkill)
          {
            oprot.WriteI32(_iter22);
          }
          oprot.WriteListEnd();
        }
        oprot.WriteFieldEnd();
      }
      if (WarriorsSkill != null && __isset.warriorsSkill) {
        field.Name = "warriorsSkill";
        field.Type = TType.List;
        field.ID = 8;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteListBegin(new TList(TType.I32, WarriorsSkill.Count));
          foreach (int _iter23 in WarriorsSkill)
          {
            oprot.WriteI32(_iter23);
          }
          oprot.WriteListEnd();
        }
        oprot.WriteFieldEnd();
      }
      if (WeaponSkill != null && __isset.weaponSkill) {
        field.Name = "weaponSkill";
        field.Type = TType.List;
        field.ID = 9;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteListBegin(new TList(TType.I32, WeaponSkill.Count));
          foreach (int _iter24 in WeaponSkill)
          {
            oprot.WriteI32(_iter24);
          }
          oprot.WriteListEnd();
        }
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("BattleMsgFighter(");
      sb.Append("Uuid: ");
      sb.Append(Uuid);
      sb.Append(",Index: ");
      sb.Append(Index);
      sb.Append(",TemplateId: ");
      sb.Append(TemplateId);
      sb.Append(",Propties: ");
      sb.Append(Propties);
      sb.Append(",LeaderSkill: ");
      sb.Append(LeaderSkill);
      sb.Append(",ActiveSkill: ");
      sb.Append(ActiveSkill);
      sb.Append(",GiftSkill: ");
      sb.Append(GiftSkill);
      sb.Append(",WarriorsSkill: ");
      sb.Append(WarriorsSkill);
      sb.Append(",WeaponSkill: ");
      sb.Append(WeaponSkill);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
