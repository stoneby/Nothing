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

namespace Template.Auto.Level
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class LevelUpTemplate : TBase
  {
    private int _id;
    private int _maxExp;
    private int _maxEnergy;
    private int _maxFriend;

    public int Id
    {
      get
      {
        return _id;
      }
      set
      {
        __isset.id = true;
        this._id = value;
      }
    }

    /// <summary>
    /// 经验上限
    /// </summary>
    public int MaxExp
    {
      get
      {
        return _maxExp;
      }
      set
      {
        __isset.maxExp = true;
        this._maxExp = value;
      }
    }

    /// <summary>
    /// 体力上限
    /// </summary>
    public int MaxEnergy
    {
      get
      {
        return _maxEnergy;
      }
      set
      {
        __isset.maxEnergy = true;
        this._maxEnergy = value;
      }
    }

    /// <summary>
    /// 好友上限
    /// </summary>
    public int MaxFriend
    {
      get
      {
        return _maxFriend;
      }
      set
      {
        __isset.maxFriend = true;
        this._maxFriend = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool id;
      public bool maxExp;
      public bool maxEnergy;
      public bool maxFriend;
    }

    public LevelUpTemplate() {
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
              Id = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.I32) {
              MaxExp = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.I32) {
              MaxEnergy = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.I32) {
              MaxFriend = iprot.ReadI32();
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
      TStruct struc = new TStruct("LevelUpTemplate");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.id) {
        field.Name = "id";
        field.Type = TType.I32;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(Id);
        oprot.WriteFieldEnd();
      }
      if (__isset.maxExp) {
        field.Name = "maxExp";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(MaxExp);
        oprot.WriteFieldEnd();
      }
      if (__isset.maxEnergy) {
        field.Name = "maxEnergy";
        field.Type = TType.I32;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(MaxEnergy);
        oprot.WriteFieldEnd();
      }
      if (__isset.maxFriend) {
        field.Name = "maxFriend";
        field.Type = TType.I32;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(MaxFriend);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("LevelUpTemplate(");
      sb.Append("Id: ");
      sb.Append(Id);
      sb.Append(",MaxExp: ");
      sb.Append(MaxExp);
      sb.Append(",MaxEnergy: ");
      sb.Append(MaxEnergy);
      sb.Append(",MaxFriend: ");
      sb.Append(MaxFriend);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
