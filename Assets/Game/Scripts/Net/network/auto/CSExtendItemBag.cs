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
  /// 扩展道具背包
  /// </summary>
  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class CSExtendItemBag : TBase
  {
    private short _extendSize;

    /// <summary>
    /// 增加数量
    /// </summary>
    public short ExtendSize
    {
      get
      {
        return _extendSize;
      }
      set
      {
        __isset.extendSize = true;
        this._extendSize = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool extendSize;
    }

    public CSExtendItemBag() {
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
              ExtendSize = iprot.ReadI16();
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
      TStruct struc = new TStruct("CSExtendItemBag");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.extendSize) {
        field.Name = "extendSize";
        field.Type = TType.I16;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI16(ExtendSize);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("CSExtendItemBag(");
      sb.Append("ExtendSize: ");
      sb.Append(ExtendSize);
      sb.Append(")");
      return sb.ToString();
    }

  }

}