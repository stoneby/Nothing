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
  public partial class SCHeroList : TBase
  {
    private List<KXSGCodec.HeroInfo> _heroList;
    private List<KXSGCodec.TeamInfo> _teamList;
    private sbyte _orderType;
    private short _heroCountLimit;
    private sbyte _currentTeamIndex;

    public List<KXSGCodec.HeroInfo> HeroList
    {
      get
      {
        return _heroList;
      }
      set
      {
        __isset.heroList = true;
        this._heroList = value;
      }
    }

    public List<KXSGCodec.TeamInfo> TeamList
    {
      get
      {
        return _teamList;
      }
      set
      {
        __isset.teamList = true;
        this._teamList = value;
      }
    }

    public sbyte OrderType
    {
      get
      {
        return _orderType;
      }
      set
      {
        __isset.orderType = true;
        this._orderType = value;
      }
    }

    public short HeroCountLimit
    {
      get
      {
        return _heroCountLimit;
      }
      set
      {
        __isset.heroCountLimit = true;
        this._heroCountLimit = value;
      }
    }

    public sbyte CurrentTeamIndex
    {
      get
      {
        return _currentTeamIndex;
      }
      set
      {
        __isset.currentTeamIndex = true;
        this._currentTeamIndex = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool heroList;
      public bool teamList;
      public bool orderType;
      public bool heroCountLimit;
      public bool currentTeamIndex;
    }

    public SCHeroList() {
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
            if (field.Type == TType.List) {
              {
                HeroList = new List<KXSGCodec.HeroInfo>();
                TList _list0 = iprot.ReadListBegin();
                for( int _i1 = 0; _i1 < _list0.Count; ++_i1)
                {
                  KXSGCodec.HeroInfo _elem2 = new KXSGCodec.HeroInfo();
                  _elem2 = new KXSGCodec.HeroInfo();
                  _elem2.Read(iprot);
                  HeroList.Add(_elem2);
                }
                iprot.ReadListEnd();
              }
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.List) {
              {
                TeamList = new List<KXSGCodec.TeamInfo>();
                TList _list3 = iprot.ReadListBegin();
                for( int _i4 = 0; _i4 < _list3.Count; ++_i4)
                {
                  KXSGCodec.TeamInfo _elem5 = new KXSGCodec.TeamInfo();
                  _elem5 = new KXSGCodec.TeamInfo();
                  _elem5.Read(iprot);
                  TeamList.Add(_elem5);
                }
                iprot.ReadListEnd();
              }
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.Byte) {
              OrderType = iprot.ReadByte();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.I16) {
              HeroCountLimit = iprot.ReadI16();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 5:
            if (field.Type == TType.Byte) {
              CurrentTeamIndex = iprot.ReadByte();
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
      TStruct struc = new TStruct("SCHeroList");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (HeroList != null && __isset.heroList) {
        field.Name = "heroList";
        field.Type = TType.List;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteListBegin(new TList(TType.Struct, HeroList.Count));
          foreach (KXSGCodec.HeroInfo _iter6 in HeroList)
          {
            _iter6.Write(oprot);
          }
          oprot.WriteListEnd();
        }
        oprot.WriteFieldEnd();
      }
      if (TeamList != null && __isset.teamList) {
        field.Name = "teamList";
        field.Type = TType.List;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteListBegin(new TList(TType.Struct, TeamList.Count));
          foreach (KXSGCodec.TeamInfo _iter7 in TeamList)
          {
            _iter7.Write(oprot);
          }
          oprot.WriteListEnd();
        }
        oprot.WriteFieldEnd();
      }
      if (__isset.orderType) {
        field.Name = "orderType";
        field.Type = TType.Byte;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteByte(OrderType);
        oprot.WriteFieldEnd();
      }
      if (__isset.heroCountLimit) {
        field.Name = "heroCountLimit";
        field.Type = TType.I16;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteI16(HeroCountLimit);
        oprot.WriteFieldEnd();
      }
      if (__isset.currentTeamIndex) {
        field.Name = "currentTeamIndex";
        field.Type = TType.Byte;
        field.ID = 5;
        oprot.WriteFieldBegin(field);
        oprot.WriteByte(CurrentTeamIndex);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("SCHeroList(");
      sb.Append("HeroList: ");
      sb.Append(HeroList);
      sb.Append(",TeamList: ");
      sb.Append(TeamList);
      sb.Append(",OrderType: ");
      sb.Append(OrderType);
      sb.Append(",HeroCountLimit: ");
      sb.Append(HeroCountLimit);
      sb.Append(",CurrentTeamIndex: ");
      sb.Append(CurrentTeamIndex);
      sb.Append(")");
      return sb.ToString();
    }

  }

}