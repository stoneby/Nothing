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

namespace Template.Auto.Greenhand
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class GreenhandTemplate : TBase
  {
    private int _id;
    private int _greenhandType;
    private int _openRight;
    private List<string> _optionParams;

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
    /// 新手引导类型
    /// </summary>
    public int GreenhandType
    {
      get
      {
        return _greenhandType;
      }
      set
      {
        __isset.greenhandType = true;
        this._greenhandType = value;
      }
    }

    /// <summary>
    /// 开启权限
    /// </summary>
    public int OpenRight
    {
      get
      {
        return _openRight;
      }
      set
      {
        __isset.openRight = true;
        this._openRight = value;
      }
    }

    /// <summary>
    /// 参数
    /// </summary>
    public List<string> OptionParams
    {
      get
      {
        return _optionParams;
      }
      set
      {
        __isset.optionParams = true;
        this._optionParams = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool id;
      public bool greenhandType;
      public bool openRight;
      public bool optionParams;
    }

    public GreenhandTemplate() {
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
              GreenhandType = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.I32) {
              OpenRight = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.List) {
              {
                OptionParams = new List<string>();
                TList _list0 = iprot.ReadListBegin();
                for( int _i1 = 0; _i1 < _list0.Count; ++_i1)
                {
                  string _elem2 = null;
                  _elem2 = iprot.ReadString();
                  OptionParams.Add(_elem2);
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
      TStruct struc = new TStruct("GreenhandTemplate");
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
      if (__isset.greenhandType) {
        field.Name = "greenhandType";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(GreenhandType);
        oprot.WriteFieldEnd();
      }
      if (__isset.openRight) {
        field.Name = "openRight";
        field.Type = TType.I32;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(OpenRight);
        oprot.WriteFieldEnd();
      }
      if (OptionParams != null && __isset.optionParams) {
        field.Name = "optionParams";
        field.Type = TType.List;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteListBegin(new TList(TType.String, OptionParams.Count));
          foreach (string _iter3 in OptionParams)
          {
            oprot.WriteString(_iter3);
          }
          oprot.WriteListEnd();
        }
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("GreenhandTemplate(");
      sb.Append("Id: ");
      sb.Append(Id);
      sb.Append(",GreenhandType: ");
      sb.Append(GreenhandType);
      sb.Append(",OpenRight: ");
      sb.Append(OpenRight);
      sb.Append(",OptionParams: ");
      sb.Append(OptionParams);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
