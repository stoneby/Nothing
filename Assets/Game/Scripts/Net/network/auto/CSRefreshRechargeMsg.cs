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
  /// 刷新充值结果
  /// </summary>
  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class CSRefreshRechargeMsg : TBase
  {
    private string _orderId;

    /// <summary>
    /// 平台订单号
    /// </summary>
    public string OrderId
    {
      get
      {
        return _orderId;
      }
      set
      {
        __isset.orderId = true;
        this._orderId = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool orderId;
    }

    public CSRefreshRechargeMsg() {
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
              OrderId = iprot.ReadString();
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
      TStruct struc = new TStruct("CSRefreshRechargeMsg");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (OrderId != null && __isset.orderId) {
        field.Name = "orderId";
        field.Type = TType.String;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(OrderId);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("CSRefreshRechargeMsg(");
      sb.Append("OrderId: ");
      sb.Append(OrderId);
      sb.Append(")");
      return sb.ToString();
    }

  }

}