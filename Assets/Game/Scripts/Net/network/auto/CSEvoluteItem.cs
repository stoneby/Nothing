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
  /// 进化道具
  /// </summary>
  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class CSEvoluteItem : TBase
  {
    private short _operItemIndex;

    /// <summary>
    /// 进化道具背包内位置
    /// </summary>
    public short OperItemIndex
    {
      get
      {
        return _operItemIndex;
      }
      set
      {
        __isset.operItemIndex = true;
        this._operItemIndex = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool operItemIndex;
    }

    public CSEvoluteItem() {
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
              OperItemIndex = iprot.ReadI16();
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
      TStruct struc = new TStruct("CSEvoluteItem");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.operItemIndex) {
        field.Name = "operItemIndex";
        field.Type = TType.I16;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI16(OperItemIndex);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("CSEvoluteItem(");
      sb.Append("OperItemIndex: ");
      sb.Append(OperItemIndex);
      sb.Append(")");
      return sb.ToString();
    }

  }

}