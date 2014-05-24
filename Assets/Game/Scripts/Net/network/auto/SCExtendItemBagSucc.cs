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
  public partial class SCExtendItemBagSucc : TBase
  {
    private short _capacity;

    /// <summary>
    /// 背包容量
    /// </summary>
    public short Capacity
    {
      get
      {
        return _capacity;
      }
      set
      {
        __isset.capacity = true;
        this._capacity = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool capacity;
    }

    public SCExtendItemBagSucc() {
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
              Capacity = iprot.ReadI16();
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
      TStruct struc = new TStruct("SCExtendItemBagSucc");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.capacity) {
        field.Name = "capacity";
        field.Type = TType.I16;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI16(Capacity);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("SCExtendItemBagSucc(");
      sb.Append("Capacity: ");
      sb.Append(Capacity);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
