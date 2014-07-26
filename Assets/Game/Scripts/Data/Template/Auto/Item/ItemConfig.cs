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

namespace Template.Auto.Item
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class ItemConfig : TBase
  {
    private Dictionary<int, ItemLevelTemplate> _itemLvlTmpls;
    private Dictionary<int, ItemEvoluteTemplate> _itemEvoluteTmpls;

    public Dictionary<int, ItemLevelTemplate> ItemLvlTmpls
    {
      get
      {
        return _itemLvlTmpls;
      }
      set
      {
        __isset.itemLvlTmpls = true;
        this._itemLvlTmpls = value;
      }
    }

    public Dictionary<int, ItemEvoluteTemplate> ItemEvoluteTmpls
    {
      get
      {
        return _itemEvoluteTmpls;
      }
      set
      {
        __isset.itemEvoluteTmpls = true;
        this._itemEvoluteTmpls = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool itemLvlTmpls;
      public bool itemEvoluteTmpls;
    }

    public ItemConfig() {
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
            if (field.Type == TType.Map) {
              {
                ItemLvlTmpls = new Dictionary<int, ItemLevelTemplate>();
                TMap _map4 = iprot.ReadMapBegin();
                for( int _i5 = 0; _i5 < _map4.Count; ++_i5)
                {
                  int _key6;
                  ItemLevelTemplate _val7;
                  _key6 = iprot.ReadI32();
                  _val7 = new ItemLevelTemplate();
                  _val7.Read(iprot);
                  ItemLvlTmpls[_key6] = _val7;
                }
                iprot.ReadMapEnd();
              }
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.Map) {
              {
                ItemEvoluteTmpls = new Dictionary<int, ItemEvoluteTemplate>();
                TMap _map8 = iprot.ReadMapBegin();
                for( int _i9 = 0; _i9 < _map8.Count; ++_i9)
                {
                  int _key10;
                  ItemEvoluteTemplate _val11;
                  _key10 = iprot.ReadI32();
                  _val11 = new ItemEvoluteTemplate();
                  _val11.Read(iprot);
                  ItemEvoluteTmpls[_key10] = _val11;
                }
                iprot.ReadMapEnd();
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
      TStruct struc = new TStruct("ItemConfig");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (ItemLvlTmpls != null && __isset.itemLvlTmpls) {
        field.Name = "itemLvlTmpls";
        field.Type = TType.Map;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteMapBegin(new TMap(TType.I32, TType.Struct, ItemLvlTmpls.Count));
          foreach (int _iter12 in ItemLvlTmpls.Keys)
          {
            oprot.WriteI32(_iter12);
            ItemLvlTmpls[_iter12].Write(oprot);
          }
          oprot.WriteMapEnd();
        }
        oprot.WriteFieldEnd();
      }
      if (ItemEvoluteTmpls != null && __isset.itemEvoluteTmpls) {
        field.Name = "itemEvoluteTmpls";
        field.Type = TType.Map;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteMapBegin(new TMap(TType.I32, TType.Struct, ItemEvoluteTmpls.Count));
          foreach (int _iter13 in ItemEvoluteTmpls.Keys)
          {
            oprot.WriteI32(_iter13);
            ItemEvoluteTmpls[_iter13].Write(oprot);
          }
          oprot.WriteMapEnd();
        }
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("ItemConfig(");
      sb.Append("ItemLvlTmpls: ");
      sb.Append(ItemLvlTmpls);
      sb.Append(",ItemEvoluteTmpls: ");
      sb.Append(ItemEvoluteTmpls);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
