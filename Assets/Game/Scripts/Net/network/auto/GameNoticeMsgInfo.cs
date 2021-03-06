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
  /// 游戏公告信息
  /// </summary>
  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class GameNoticeMsgInfo : TBase
  {
    private int _uuid;
    private string _title;
    private string _content;
    private long _startTime;
    private long _endTime;
    private sbyte _tag;
    private int _orderNum;

    /// <summary>
    /// 公告ID
    /// </summary>
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

    /// <summary>
    /// 公告title
    /// </summary>
    public string Title
    {
      get
      {
        return _title;
      }
      set
      {
        __isset.title = true;
        this._title = value;
      }
    }

    /// <summary>
    /// 公告内容
    /// </summary>
    public string Content
    {
      get
      {
        return _content;
      }
      set
      {
        __isset.content = true;
        this._content = value;
      }
    }

    /// <summary>
    /// 开始时间
    /// </summary>
    public long StartTime
    {
      get
      {
        return _startTime;
      }
      set
      {
        __isset.startTime = true;
        this._startTime = value;
      }
    }

    /// <summary>
    /// 结束时间
    /// </summary>
    public long EndTime
    {
      get
      {
        return _endTime;
      }
      set
      {
        __isset.endTime = true;
        this._endTime = value;
      }
    }

    /// <summary>
    /// 标识 0:hot 1:new -1:无
    /// </summary>
    public sbyte Tag
    {
      get
      {
        return _tag;
      }
      set
      {
        __isset.tag = true;
        this._tag = value;
      }
    }

    /// <summary>
    /// 排序
    /// </summary>
    public int OrderNum
    {
      get
      {
        return _orderNum;
      }
      set
      {
        __isset.orderNum = true;
        this._orderNum = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool uuid;
      public bool title;
      public bool content;
      public bool startTime;
      public bool endTime;
      public bool tag;
      public bool orderNum;
    }

    public GameNoticeMsgInfo() {
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
            if (field.Type == TType.String) {
              Title = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.String) {
              Content = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.I64) {
              StartTime = iprot.ReadI64();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 5:
            if (field.Type == TType.I64) {
              EndTime = iprot.ReadI64();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 6:
            if (field.Type == TType.Byte) {
              Tag = iprot.ReadByte();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 7:
            if (field.Type == TType.I32) {
              OrderNum = iprot.ReadI32();
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
      TStruct struc = new TStruct("GameNoticeMsgInfo");
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
      if (Title != null && __isset.title) {
        field.Name = "title";
        field.Type = TType.String;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(Title);
        oprot.WriteFieldEnd();
      }
      if (Content != null && __isset.content) {
        field.Name = "content";
        field.Type = TType.String;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(Content);
        oprot.WriteFieldEnd();
      }
      if (__isset.startTime) {
        field.Name = "startTime";
        field.Type = TType.I64;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteI64(StartTime);
        oprot.WriteFieldEnd();
      }
      if (__isset.endTime) {
        field.Name = "endTime";
        field.Type = TType.I64;
        field.ID = 5;
        oprot.WriteFieldBegin(field);
        oprot.WriteI64(EndTime);
        oprot.WriteFieldEnd();
      }
      if (__isset.tag) {
        field.Name = "tag";
        field.Type = TType.Byte;
        field.ID = 6;
        oprot.WriteFieldBegin(field);
        oprot.WriteByte(Tag);
        oprot.WriteFieldEnd();
      }
      if (__isset.orderNum) {
        field.Name = "orderNum";
        field.Type = TType.I32;
        field.ID = 7;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(OrderNum);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("GameNoticeMsgInfo(");
      sb.Append("Uuid: ");
      sb.Append(Uuid);
      sb.Append(",Title: ");
      sb.Append(Title);
      sb.Append(",Content: ");
      sb.Append(Content);
      sb.Append(",StartTime: ");
      sb.Append(StartTime);
      sb.Append(",EndTime: ");
      sb.Append(EndTime);
      sb.Append(",Tag: ");
      sb.Append(Tag);
      sb.Append(",OrderNum: ");
      sb.Append(OrderNum);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
