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
  public partial class RaidAddtionInfo : TBase
  {
    private sbyte _raidType;
    private sbyte _addtionType;
    private List<int> _raidTemplateId;

    public sbyte RaidType
    {
      get
      {
        return _raidType;
      }
      set
      {
        __isset.raidType = true;
        this._raidType = value;
      }
    }

    public sbyte AddtionType
    {
      get
      {
        return _addtionType;
      }
      set
      {
        __isset.addtionType = true;
        this._addtionType = value;
      }
    }

    public List<int> RaidTemplateId
    {
      get
      {
        return _raidTemplateId;
      }
      set
      {
        __isset.raidTemplateId = true;
        this._raidTemplateId = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool raidType;
      public bool addtionType;
      public bool raidTemplateId;
    }

    public RaidAddtionInfo() {
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
              RaidType = iprot.ReadByte();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.Byte) {
              AddtionType = iprot.ReadByte();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.List) {
              {
                RaidTemplateId = new List<int>();
                TList _list4 = iprot.ReadListBegin();
                for( int _i5 = 0; _i5 < _list4.Count; ++_i5)
                {
                  int _elem6 = 0;
                  _elem6 = iprot.ReadI32();
                  RaidTemplateId.Add(_elem6);
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
      TStruct struc = new TStruct("RaidAddtionInfo");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.raidType) {
        field.Name = "raidType";
        field.Type = TType.Byte;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteByte(RaidType);
        oprot.WriteFieldEnd();
      }
      if (__isset.addtionType) {
        field.Name = "addtionType";
        field.Type = TType.Byte;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteByte(AddtionType);
        oprot.WriteFieldEnd();
      }
      if (RaidTemplateId != null && __isset.raidTemplateId) {
        field.Name = "raidTemplateId";
        field.Type = TType.List;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteListBegin(new TList(TType.I32, RaidTemplateId.Count));
          foreach (int _iter7 in RaidTemplateId)
          {
            oprot.WriteI32(_iter7);
          }
          oprot.WriteListEnd();
        }
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("RaidAddtionInfo(");
      sb.Append("RaidType: ");
      sb.Append(RaidType);
      sb.Append(",AddtionType: ");
      sb.Append(AddtionType);
      sb.Append(",RaidTemplateId: ");
      sb.Append(RaidTemplateId);
      sb.Append(")");
      return sb.ToString();
    }

  }

}