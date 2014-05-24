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
  public partial class SkillInfo : TBase
  {
    private string _id;
    private string _tmplId;
    private short _lvl;
    private short _pos;
    private SkillState _state;
    private short _leftTime;

    public string Id
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

    public string TmplId
    {
      get
      {
        return _tmplId;
      }
      set
      {
        __isset.tmplId = true;
        this._tmplId = value;
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

    public short Pos
    {
      get
      {
        return _pos;
      }
      set
      {
        __isset.pos = true;
        this._pos = value;
      }
    }

    /// <summary>
    /// 
    /// <seealso cref="SkillState"/>
    /// </summary>
    public SkillState State
    {
      get
      {
        return _state;
      }
      set
      {
        __isset.state = true;
        this._state = value;
      }
    }

    public short LeftTime
    {
      get
      {
        return _leftTime;
      }
      set
      {
        __isset.leftTime = true;
        this._leftTime = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool id;
      public bool tmplId;
      public bool lvl;
      public bool pos;
      public bool state;
      public bool leftTime;
    }

    public SkillInfo() {
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
            if (field.Type == TType.String) {
              Id = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.String) {
              TmplId = iprot.ReadString();
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
            if (field.Type == TType.I16) {
              Pos = iprot.ReadI16();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 5:
            if (field.Type == TType.I32) {
              State = (SkillState)iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 6:
            if (field.Type == TType.I16) {
              LeftTime = iprot.ReadI16();
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
      TStruct struc = new TStruct("SkillInfo");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (Id != null && __isset.id) {
        field.Name = "id";
        field.Type = TType.String;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(Id);
        oprot.WriteFieldEnd();
      }
      if (TmplId != null && __isset.tmplId) {
        field.Name = "tmplId";
        field.Type = TType.String;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(TmplId);
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
      if (__isset.pos) {
        field.Name = "pos";
        field.Type = TType.I16;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteI16(Pos);
        oprot.WriteFieldEnd();
      }
      if (__isset.state) {
        field.Name = "state";
        field.Type = TType.I32;
        field.ID = 5;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32((int)State);
        oprot.WriteFieldEnd();
      }
      if (__isset.leftTime) {
        field.Name = "leftTime";
        field.Type = TType.I16;
        field.ID = 6;
        oprot.WriteFieldBegin(field);
        oprot.WriteI16(LeftTime);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("SkillInfo(");
      sb.Append("Id: ");
      sb.Append(Id);
      sb.Append(",TmplId: ");
      sb.Append(TmplId);
      sb.Append(",Lvl: ");
      sb.Append(Lvl);
      sb.Append(",Pos: ");
      sb.Append(Pos);
      sb.Append(",State: ");
      sb.Append(State);
      sb.Append(",LeftTime: ");
      sb.Append(LeftTime);
      sb.Append(")");
      return sb.ToString();
    }

  }

}