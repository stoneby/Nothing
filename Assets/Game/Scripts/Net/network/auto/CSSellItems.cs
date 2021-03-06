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
  /// 出售道具
  /// </summary>
  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class CSSellItems : TBase
  {
    private List<short> _sellItemIndexes;

    /// <summary>
    /// 出售道具背包内位置
    /// </summary>
    public List<short> SellItemIndexes
    {
      get
      {
        return _sellItemIndexes;
      }
      set
      {
        __isset.sellItemIndexes = true;
        this._sellItemIndexes = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool sellItemIndexes;
    }

    public CSSellItems() {
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
            if (field.Type == TType.List) {
              {
                SellItemIndexes = new List<short>();
                TList _list4 = iprot.ReadListBegin();
                for( int _i5 = 0; _i5 < _list4.Count; ++_i5)
                {
                  short _elem6 = 0;
                  _elem6 = iprot.ReadI16();
                  SellItemIndexes.Add(_elem6);
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
      TStruct struc = new TStruct("CSSellItems");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (SellItemIndexes != null && __isset.sellItemIndexes) {
        field.Name = "sellItemIndexes";
        field.Type = TType.List;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteListBegin(new TList(TType.I16, SellItemIndexes.Count));
          foreach (short _iter7 in SellItemIndexes)
          {
            oprot.WriteI16(_iter7);
          }
          oprot.WriteListEnd();
        }
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("CSSellItems(");
      sb.Append("SellItemIndexes: ");
      sb.Append(SellItemIndexes);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
