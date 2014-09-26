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
  public partial class CSPasswdLoginMsg : TBase
  {
    private sbyte _deviceType;
    private string _accountName;
    private string _passwd;
    private string _deviceId;
    private string _deviceModel;
    private short _serverId;

    public sbyte DeviceType
    {
      get
      {
        return _deviceType;
      }
      set
      {
        __isset.deviceType = true;
        this._deviceType = value;
      }
    }

    public string AccountName
    {
      get
      {
        return _accountName;
      }
      set
      {
        __isset.accountName = true;
        this._accountName = value;
      }
    }

    public string Passwd
    {
      get
      {
        return _passwd;
      }
      set
      {
        __isset.passwd = true;
        this._passwd = value;
      }
    }

    public string DeviceId
    {
      get
      {
        return _deviceId;
      }
      set
      {
        __isset.deviceId = true;
        this._deviceId = value;
      }
    }

    public string DeviceModel
    {
      get
      {
        return _deviceModel;
      }
      set
      {
        __isset.deviceModel = true;
        this._deviceModel = value;
      }
    }

    /// <summary>
    /// 服务器序号
    /// </summary>
    public short ServerId
    {
      get
      {
        return _serverId;
      }
      set
      {
        __isset.serverId = true;
        this._serverId = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool deviceType;
      public bool accountName;
      public bool passwd;
      public bool deviceId;
      public bool deviceModel;
      public bool serverId;
    }

    public CSPasswdLoginMsg() {
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
            if (field.Type == TType.Byte) {
              DeviceType = iprot.ReadByte();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.String) {
              AccountName = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.String) {
              Passwd = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.String) {
              DeviceId = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 5:
            if (field.Type == TType.String) {
              DeviceModel = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 6:
            if (field.Type == TType.I16) {
              ServerId = iprot.ReadI16();
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
      TStruct struc = new TStruct("CSPasswdLoginMsg");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.deviceType) {
        field.Name = "deviceType";
        field.Type = TType.Byte;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteByte(DeviceType);
        oprot.WriteFieldEnd();
      }
      if (AccountName != null && __isset.accountName) {
        field.Name = "accountName";
        field.Type = TType.String;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(AccountName);
        oprot.WriteFieldEnd();
      }
      if (Passwd != null && __isset.passwd) {
        field.Name = "passwd";
        field.Type = TType.String;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(Passwd);
        oprot.WriteFieldEnd();
      }
      if (DeviceId != null && __isset.deviceId) {
        field.Name = "deviceId";
        field.Type = TType.String;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(DeviceId);
        oprot.WriteFieldEnd();
      }
      if (DeviceModel != null && __isset.deviceModel) {
        field.Name = "deviceModel";
        field.Type = TType.String;
        field.ID = 5;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(DeviceModel);
        oprot.WriteFieldEnd();
      }
      if (__isset.serverId) {
        field.Name = "serverId";
        field.Type = TType.I16;
        field.ID = 6;
        oprot.WriteFieldBegin(field);
        oprot.WriteI16(ServerId);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("CSPasswdLoginMsg(");
      sb.Append("DeviceType: ");
      sb.Append(DeviceType);
      sb.Append(",AccountName: ");
      sb.Append(AccountName);
      sb.Append(",Passwd: ");
      sb.Append(Passwd);
      sb.Append(",DeviceId: ");
      sb.Append(DeviceId);
      sb.Append(",DeviceModel: ");
      sb.Append(DeviceModel);
      sb.Append(",ServerId: ");
      sb.Append(ServerId);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
