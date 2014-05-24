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
  /// 增加道具
  /// </summary>
  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class SCAddItem : TBase
  {
    private KXSGCodec.ItemInfo _info;

    /// <summary>
    /// 道具基本信息
    /// </summary>
    public KXSGCodec.ItemInfo Info
    {
      get
      {
        return _info;
      }
      set
      {
        __isset.info = true;
        this._info = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool info;
    }

    public SCAddItem() {
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
            if (field.Type == TType.Struct) {
              Info = new KXSGCodec.ItemInfo();
              Info.Read(iprot);
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
      TStruct struc = new TStruct("SCAddItem");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (Info != null && __isset.info) {
        field.Name = "info";
        field.Type = TType.Struct;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        Info.Write(oprot);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("SCAddItem(");
      sb.Append("Info: ");
      sb.Append(Info== null ? "<null>" : Info.ToString());
      sb.Append(")");
      return sb.ToString();
    }

  }

}
