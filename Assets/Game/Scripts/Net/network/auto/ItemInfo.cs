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
  public partial class ItemInfo : TBase
  {
    private string _id;
    private short _bagIndex;
    private int _tmplId;
    private sbyte _bindStatus;
    private sbyte _equipStatus;
    private short _level;
    private int _curExp;
    private short _maxLvl;
    private sbyte _upVal;
    private int _contribExp;
    private long _createdTime;
    private short _count;
    private long _expireTime;

    /// <summary>
    /// 实例id
    /// </summary>
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

    /// <summary>
    /// 所在背包位置
    /// </summary>
    public short BagIndex
    {
      get
      {
        return _bagIndex;
      }
      set
      {
        __isset.bagIndex = true;
        this._bagIndex = value;
      }
    }

    /// <summary>
    /// 模板id
    /// </summary>
    public int TmplId
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

    /// <summary>
    /// 绑定状态 0-未绑定 1-已绑定
    /// </summary>
    public sbyte BindStatus
    {
      get
      {
        return _bindStatus;
      }
      set
      {
        __isset.bindStatus = true;
        this._bindStatus = value;
      }
    }

    /// <summary>
    /// 装备状态 0-未装备 1-当前武将装备 2-非当前队伍武将装备
    /// </summary>
    public sbyte EquipStatus
    {
      get
      {
        return _equipStatus;
      }
      set
      {
        __isset.equipStatus = true;
        this._equipStatus = value;
      }
    }

    public short Level
    {
      get
      {
        return _level;
      }
      set
      {
        __isset.level = true;
        this._level = value;
      }
    }

    /// <summary>
    /// 当前经验值
    /// </summary>
    public int CurExp
    {
      get
      {
        return _curExp;
      }
      set
      {
        __isset.curExp = true;
        this._curExp = value;
      }
    }

    /// <summary>
    /// 最大等级
    /// </summary>
    public short MaxLvl
    {
      get
      {
        return _maxLvl;
      }
      set
      {
        __isset.maxLvl = true;
        this._maxLvl = value;
      }
    }

    /// <summary>
    /// 当前上限值
    /// </summary>
    public sbyte UpVal
    {
      get
      {
        return _upVal;
      }
      set
      {
        __isset.upVal = true;
        this._upVal = value;
      }
    }

    /// <summary>
    /// 贡献经验值
    /// </summary>
    public int ContribExp
    {
      get
      {
        return _contribExp;
      }
      set
      {
        __isset.contribExp = true;
        this._contribExp = value;
      }
    }

    /// <summary>
    /// 创建时间
    /// </summary>
    public long CreatedTime
    {
      get
      {
        return _createdTime;
      }
      set
      {
        __isset.createdTime = true;
        this._createdTime = value;
      }
    }

    public short Count
    {
      get
      {
        return _count;
      }
      set
      {
        __isset.count = true;
        this._count = value;
      }
    }

    /// <summary>
    /// 过期时间
    /// </summary>
    public long ExpireTime
    {
      get
      {
        return _expireTime;
      }
      set
      {
        __isset.expireTime = true;
        this._expireTime = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool id;
      public bool bagIndex;
      public bool tmplId;
      public bool bindStatus;
      public bool equipStatus;
      public bool level;
      public bool curExp;
      public bool maxLvl;
      public bool upVal;
      public bool contribExp;
      public bool createdTime;
      public bool count;
      public bool expireTime;
    }

    public ItemInfo() {
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
            if (field.Type == TType.I16) {
              BagIndex = iprot.ReadI16();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.I32) {
              TmplId = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.Byte) {
              BindStatus = iprot.ReadByte();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 5:
            if (field.Type == TType.Byte) {
              EquipStatus = iprot.ReadByte();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 6:
            if (field.Type == TType.I16) {
              Level = iprot.ReadI16();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 7:
            if (field.Type == TType.I32) {
              CurExp = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 8:
            if (field.Type == TType.I16) {
              MaxLvl = iprot.ReadI16();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 9:
            if (field.Type == TType.Byte) {
              UpVal = iprot.ReadByte();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 10:
            if (field.Type == TType.I32) {
              ContribExp = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 11:
            if (field.Type == TType.I64) {
              CreatedTime = iprot.ReadI64();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 12:
            if (field.Type == TType.I16) {
              Count = iprot.ReadI16();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 13:
            if (field.Type == TType.I64) {
              ExpireTime = iprot.ReadI64();
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
      TStruct struc = new TStruct("ItemInfo");
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
      if (__isset.bagIndex) {
        field.Name = "bagIndex";
        field.Type = TType.I16;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI16(BagIndex);
        oprot.WriteFieldEnd();
      }
      if (__isset.tmplId) {
        field.Name = "tmplId";
        field.Type = TType.I32;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(TmplId);
        oprot.WriteFieldEnd();
      }
      if (__isset.bindStatus) {
        field.Name = "bindStatus";
        field.Type = TType.Byte;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteByte(BindStatus);
        oprot.WriteFieldEnd();
      }
      if (__isset.equipStatus) {
        field.Name = "equipStatus";
        field.Type = TType.Byte;
        field.ID = 5;
        oprot.WriteFieldBegin(field);
        oprot.WriteByte(EquipStatus);
        oprot.WriteFieldEnd();
      }
      if (__isset.level) {
        field.Name = "level";
        field.Type = TType.I16;
        field.ID = 6;
        oprot.WriteFieldBegin(field);
        oprot.WriteI16(Level);
        oprot.WriteFieldEnd();
      }
      if (__isset.curExp) {
        field.Name = "curExp";
        field.Type = TType.I32;
        field.ID = 7;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(CurExp);
        oprot.WriteFieldEnd();
      }
      if (__isset.maxLvl) {
        field.Name = "maxLvl";
        field.Type = TType.I16;
        field.ID = 8;
        oprot.WriteFieldBegin(field);
        oprot.WriteI16(MaxLvl);
        oprot.WriteFieldEnd();
      }
      if (__isset.upVal) {
        field.Name = "upVal";
        field.Type = TType.Byte;
        field.ID = 9;
        oprot.WriteFieldBegin(field);
        oprot.WriteByte(UpVal);
        oprot.WriteFieldEnd();
      }
      if (__isset.contribExp) {
        field.Name = "contribExp";
        field.Type = TType.I32;
        field.ID = 10;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(ContribExp);
        oprot.WriteFieldEnd();
      }
      if (__isset.createdTime) {
        field.Name = "createdTime";
        field.Type = TType.I64;
        field.ID = 11;
        oprot.WriteFieldBegin(field);
        oprot.WriteI64(CreatedTime);
        oprot.WriteFieldEnd();
      }
      if (__isset.count) {
        field.Name = "count";
        field.Type = TType.I16;
        field.ID = 12;
        oprot.WriteFieldBegin(field);
        oprot.WriteI16(Count);
        oprot.WriteFieldEnd();
      }
      if (__isset.expireTime) {
        field.Name = "expireTime";
        field.Type = TType.I64;
        field.ID = 13;
        oprot.WriteFieldBegin(field);
        oprot.WriteI64(ExpireTime);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("ItemInfo(");
      sb.Append("Id: ");
      sb.Append(Id);
      sb.Append(",BagIndex: ");
      sb.Append(BagIndex);
      sb.Append(",TmplId: ");
      sb.Append(TmplId);
      sb.Append(",BindStatus: ");
      sb.Append(BindStatus);
      sb.Append(",EquipStatus: ");
      sb.Append(EquipStatus);
      sb.Append(",Level: ");
      sb.Append(Level);
      sb.Append(",CurExp: ");
      sb.Append(CurExp);
      sb.Append(",MaxLvl: ");
      sb.Append(MaxLvl);
      sb.Append(",UpVal: ");
      sb.Append(UpVal);
      sb.Append(",ContribExp: ");
      sb.Append(ContribExp);
      sb.Append(",CreatedTime: ");
      sb.Append(CreatedTime);
      sb.Append(",Count: ");
      sb.Append(Count);
      sb.Append(",ExpireTime: ");
      sb.Append(ExpireTime);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
