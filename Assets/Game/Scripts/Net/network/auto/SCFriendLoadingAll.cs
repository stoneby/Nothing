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

  /// <summary>
  /// 请求所有好友信息
  /// </summary>
  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class SCFriendLoadingAll : TBase
  {
    private short _friendLimit;
    private short _friendLimitExtendTimes;
    private List<KXSGCodec.FriendInfo> _friendList;
    private KXSGCodec.FriendInfo _myself;

    public short FriendLimit
    {
      get
      {
        return _friendLimit;
      }
      set
      {
        __isset.friendLimit = true;
        this._friendLimit = value;
      }
    }

    public short FriendLimitExtendTimes
    {
      get
      {
        return _friendLimitExtendTimes;
      }
      set
      {
        __isset.friendLimitExtendTimes = true;
        this._friendLimitExtendTimes = value;
      }
    }

    public List<KXSGCodec.FriendInfo> FriendList
    {
      get
      {
        return _friendList;
      }
      set
      {
        __isset.friendList = true;
        this._friendList = value;
      }
    }

    public KXSGCodec.FriendInfo Myself
    {
      get
      {
        return _myself;
      }
      set
      {
        __isset.myself = true;
        this._myself = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool friendLimit;
      public bool friendLimitExtendTimes;
      public bool friendList;
      public bool myself;
    }

    public SCFriendLoadingAll() {
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
              FriendLimit = iprot.ReadI16();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.I16) {
              FriendLimitExtendTimes = iprot.ReadI16();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.List) {
              {
                FriendList = new List<KXSGCodec.FriendInfo>();
                TList _list0 = iprot.ReadListBegin();
                for( int _i1 = 0; _i1 < _list0.Count; ++_i1)
                {
                  KXSGCodec.FriendInfo _elem2 = new KXSGCodec.FriendInfo();
                  _elem2 = new KXSGCodec.FriendInfo();
                  _elem2.Read(iprot);
                  FriendList.Add(_elem2);
                }
                iprot.ReadListEnd();
              }
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.Struct) {
              Myself = new KXSGCodec.FriendInfo();
              Myself.Read(iprot);
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
      TStruct struc = new TStruct("SCFriendLoadingAll");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.friendLimit) {
        field.Name = "friendLimit";
        field.Type = TType.I16;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI16(FriendLimit);
        oprot.WriteFieldEnd();
      }
      if (__isset.friendLimitExtendTimes) {
        field.Name = "friendLimitExtendTimes";
        field.Type = TType.I16;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI16(FriendLimitExtendTimes);
        oprot.WriteFieldEnd();
      }
      if (FriendList != null && __isset.friendList) {
        field.Name = "friendList";
        field.Type = TType.List;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteListBegin(new TList(TType.Struct, FriendList.Count));
          foreach (KXSGCodec.FriendInfo _iter3 in FriendList)
          {
            _iter3.Write(oprot);
          }
          oprot.WriteListEnd();
        }
        oprot.WriteFieldEnd();
      }
      if (Myself != null && __isset.myself) {
        field.Name = "myself";
        field.Type = TType.Struct;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        Myself.Write(oprot);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("SCFriendLoadingAll(");
      sb.Append("FriendLimit: ");
      sb.Append(FriendLimit);
      sb.Append(",FriendLimitExtendTimes: ");
      sb.Append(FriendLimitExtendTimes);
      sb.Append(",FriendList: ");
      sb.Append(FriendList);
      sb.Append(",Myself: ");
      sb.Append(Myself== null ? "<null>" : Myself.ToString());
      sb.Append(")");
      return sb.ToString();
    }

  }

}
