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
  public partial class LotteryInfo : TBase
  {
    private int _activityId;
    private long _startTime;
    private long _endTime;
    private List<int> _icon;
    private string _name;
    private string _desc;
    private bool _firstLotteryGive4Star;
    private bool _tenLotteryGiveElevenHero;

    /// <summary>
    /// 活动id
    /// </summary>
    public int ActivityId
    {
      get
      {
        return _activityId;
      }
      set
      {
        __isset.activityId = true;
        this._activityId = value;
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
    /// 轮播图片
    /// </summary>
    public List<int> Icon
    {
      get
      {
        return _icon;
      }
      set
      {
        __isset.icon = true;
        this._icon = value;
      }
    }

    /// <summary>
    /// 名称
    /// </summary>
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

    /// <summary>
    /// 描述
    /// </summary>
    public string Desc
    {
      get
      {
        return _desc;
      }
      set
      {
        __isset.desc = true;
        this._desc = value;
      }
    }

    /// <summary>
    /// 首抽给4星
    /// </summary>
    public bool FirstLotteryGive4Star
    {
      get
      {
        return _firstLotteryGive4Star;
      }
      set
      {
        __isset.firstLotteryGive4Star = true;
        this._firstLotteryGive4Star = value;
      }
    }

    /// <summary>
    /// 10连抽给11个武将
    /// </summary>
    public bool TenLotteryGiveElevenHero
    {
      get
      {
        return _tenLotteryGiveElevenHero;
      }
      set
      {
        __isset.tenLotteryGiveElevenHero = true;
        this._tenLotteryGiveElevenHero = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool activityId;
      public bool startTime;
      public bool endTime;
      public bool icon;
      public bool name;
      public bool desc;
      public bool firstLotteryGive4Star;
      public bool tenLotteryGiveElevenHero;
    }

    public LotteryInfo() {
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
              ActivityId = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.I64) {
              StartTime = iprot.ReadI64();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.I64) {
              EndTime = iprot.ReadI64();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.List) {
              {
                Icon = new List<int>();
                TList _list0 = iprot.ReadListBegin();
                for( int _i1 = 0; _i1 < _list0.Count; ++_i1)
                {
                  int _elem2 = 0;
                  _elem2 = iprot.ReadI32();
                  Icon.Add(_elem2);
                }
                iprot.ReadListEnd();
              }
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 5:
            if (field.Type == TType.String) {
              Name = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 6:
            if (field.Type == TType.String) {
              Desc = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 7:
            if (field.Type == TType.Bool) {
              FirstLotteryGive4Star = iprot.ReadBool();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 8:
            if (field.Type == TType.Bool) {
              TenLotteryGiveElevenHero = iprot.ReadBool();
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
      TStruct struc = new TStruct("LotteryInfo");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.activityId) {
        field.Name = "activityId";
        field.Type = TType.I32;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(ActivityId);
        oprot.WriteFieldEnd();
      }
      if (__isset.startTime) {
        field.Name = "startTime";
        field.Type = TType.I64;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI64(StartTime);
        oprot.WriteFieldEnd();
      }
      if (__isset.endTime) {
        field.Name = "endTime";
        field.Type = TType.I64;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteI64(EndTime);
        oprot.WriteFieldEnd();
      }
      if (Icon != null && __isset.icon) {
        field.Name = "icon";
        field.Type = TType.List;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteListBegin(new TList(TType.I32, Icon.Count));
          foreach (int _iter3 in Icon)
          {
            oprot.WriteI32(_iter3);
          }
          oprot.WriteListEnd();
        }
        oprot.WriteFieldEnd();
      }
      if (Name != null && __isset.name) {
        field.Name = "name";
        field.Type = TType.String;
        field.ID = 5;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(Name);
        oprot.WriteFieldEnd();
      }
      if (Desc != null && __isset.desc) {
        field.Name = "desc";
        field.Type = TType.String;
        field.ID = 6;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(Desc);
        oprot.WriteFieldEnd();
      }
      if (__isset.firstLotteryGive4Star) {
        field.Name = "firstLotteryGive4Star";
        field.Type = TType.Bool;
        field.ID = 7;
        oprot.WriteFieldBegin(field);
        oprot.WriteBool(FirstLotteryGive4Star);
        oprot.WriteFieldEnd();
      }
      if (__isset.tenLotteryGiveElevenHero) {
        field.Name = "tenLotteryGiveElevenHero";
        field.Type = TType.Bool;
        field.ID = 8;
        oprot.WriteFieldBegin(field);
        oprot.WriteBool(TenLotteryGiveElevenHero);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("LotteryInfo(");
      sb.Append("ActivityId: ");
      sb.Append(ActivityId);
      sb.Append(",StartTime: ");
      sb.Append(StartTime);
      sb.Append(",EndTime: ");
      sb.Append(EndTime);
      sb.Append(",Icon: ");
      sb.Append(Icon);
      sb.Append(",Name: ");
      sb.Append(Name);
      sb.Append(",Desc: ");
      sb.Append(Desc);
      sb.Append(",FirstLotteryGive4Star: ");
      sb.Append(FirstLotteryGive4Star);
      sb.Append(",TenLotteryGiveElevenHero: ");
      sb.Append(TenLotteryGiveElevenHero);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
