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


#if !SILVERLIGHT
[Serializable]
#endif
public partial class ItemConfig : TBase
{
  private Dictionary<int, ItemLevelTemplate> _itemLvlTmpl;
  private Dictionary<int, ItemEvoluteTemplate> _itemEvoluteTmpl;

  public Dictionary<int, ItemLevelTemplate> ItemLvlTmpl
  {
    get
    {
      return _itemLvlTmpl;
    }
    set
    {
      __isset.itemLvlTmpl = true;
      this._itemLvlTmpl = value;
    }
  }

  public Dictionary<int, ItemEvoluteTemplate> ItemEvoluteTmpl
  {
    get
    {
      return _itemEvoluteTmpl;
    }
    set
    {
      __isset.itemEvoluteTmpl = true;
      this._itemEvoluteTmpl = value;
    }
  }


  public Isset __isset;
  #if !SILVERLIGHT
  [Serializable]
  #endif
  public struct Isset {
    public bool itemLvlTmpl;
    public bool itemEvoluteTmpl;
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
              ItemLvlTmpl = new Dictionary<int, ItemLevelTemplate>();
              TMap _map0 = iprot.ReadMapBegin();
              for( int _i1 = 0; _i1 < _map0.Count; ++_i1)
              {
                int _key2;
                ItemLevelTemplate _val3;
                _key2 = iprot.ReadI32();
                _val3 = new ItemLevelTemplate();
                _val3.Read(iprot);
                ItemLvlTmpl[_key2] = _val3;
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
              ItemEvoluteTmpl = new Dictionary<int, ItemEvoluteTemplate>();
              TMap _map4 = iprot.ReadMapBegin();
              for( int _i5 = 0; _i5 < _map4.Count; ++_i5)
              {
                int _key6;
                ItemEvoluteTemplate _val7;
                _key6 = iprot.ReadI32();
                _val7 = new ItemEvoluteTemplate();
                _val7.Read(iprot);
                ItemEvoluteTmpl[_key6] = _val7;
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
    if (ItemLvlTmpl != null && __isset.itemLvlTmpl) {
      field.Name = "itemLvlTmpl";
      field.Type = TType.Map;
      field.ID = 1;
      oprot.WriteFieldBegin(field);
      {
        oprot.WriteMapBegin(new TMap(TType.I32, TType.Struct, ItemLvlTmpl.Count));
        foreach (int _iter8 in ItemLvlTmpl.Keys)
        {
          oprot.WriteI32(_iter8);
          ItemLvlTmpl[_iter8].Write(oprot);
        }
        oprot.WriteMapEnd();
      }
      oprot.WriteFieldEnd();
    }
    if (ItemEvoluteTmpl != null && __isset.itemEvoluteTmpl) {
      field.Name = "itemEvoluteTmpl";
      field.Type = TType.Map;
      field.ID = 2;
      oprot.WriteFieldBegin(field);
      {
        oprot.WriteMapBegin(new TMap(TType.I32, TType.Struct, ItemEvoluteTmpl.Count));
        foreach (int _iter9 in ItemEvoluteTmpl.Keys)
        {
          oprot.WriteI32(_iter9);
          ItemEvoluteTmpl[_iter9].Write(oprot);
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
    sb.Append("ItemLvlTmpl: ");
    sb.Append(ItemLvlTmpl);
    sb.Append(",ItemEvoluteTmpl: ");
    sb.Append(ItemEvoluteTmpl);
    sb.Append(")");
    return sb.ToString();
  }

}
