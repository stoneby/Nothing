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
  /// 扩展好友上限
  /// </summary>
  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class CSFriendExtend : TBase
  {
    private sbyte _extendTimes;

    public sbyte ExtendTimes
    {
      get
      {
        return _extendTimes;
      }
      set
      {
        __isset.extendTimes = true;
        this._extendTimes = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool extendTimes;
    }

    public CSFriendExtend() {
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
              ExtendTimes = iprot.ReadByte();
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
      TStruct struc = new TStruct("CSFriendExtend");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.extendTimes) {
        field.Name = "extendTimes";
        field.Type = TType.Byte;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteByte(ExtendTimes);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("CSFriendExtend(");
      sb.Append("ExtendTimes: ");
      sb.Append(ExtendTimes);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
