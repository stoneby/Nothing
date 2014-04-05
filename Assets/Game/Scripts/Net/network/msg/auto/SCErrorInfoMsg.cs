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
  public partial class SCErrorInfoMsg : TBase
  {
    private sbyte _errorCode;

    public sbyte ErrorCode
    {
      get
      {
        return _errorCode;
      }
      set
      {
        __isset.errorCode = true;
        this._errorCode = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool errorCode;
    }

    public SCErrorInfoMsg() {
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
              ErrorCode = iprot.ReadByte();
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
      TStruct struc = new TStruct("SCErrorInfoMsg");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.errorCode) {
        field.Name = "errorCode";
        field.Type = TType.Byte;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteByte(ErrorCode);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("SCErrorInfoMsg(");
      sb.Append("ErrorCode: ");
      sb.Append(ErrorCode);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
