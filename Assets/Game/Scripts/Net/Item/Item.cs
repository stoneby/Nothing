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
public partial class Item : TBase
{
  private Dictionary<int, EquipItemTemplate> _equipTmpl;
  private Dictionary<int, ArmorItemTemplate> _armorTmpl;
  private Dictionary<int, MaterialItemTemplate> _materialTmpl;

  public Dictionary<int, EquipItemTemplate> EquipTmpl
  {
    get
    {
      return _equipTmpl;
    }
    set
    {
      __isset.equipTmpl = true;
      this._equipTmpl = value;
    }
  }

  public Dictionary<int, ArmorItemTemplate> ArmorTmpl
  {
    get
    {
      return _armorTmpl;
    }
    set
    {
      __isset.armorTmpl = true;
      this._armorTmpl = value;
    }
  }

  public Dictionary<int, MaterialItemTemplate> MaterialTmpl
  {
    get
    {
      return _materialTmpl;
    }
    set
    {
      __isset.materialTmpl = true;
      this._materialTmpl = value;
    }
  }


  public Isset __isset;
  #if !SILVERLIGHT
  [Serializable]
  #endif
  public struct Isset {
    public bool equipTmpl;
    public bool armorTmpl;
    public bool materialTmpl;
  }

  public Item() {
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
              EquipTmpl = new Dictionary<int, EquipItemTemplate>();
              TMap _map0 = iprot.ReadMapBegin();
              for( int _i1 = 0; _i1 < _map0.Count; ++_i1)
              {
                int _key2;
                EquipItemTemplate _val3;
                _key2 = iprot.ReadI32();
                _val3 = new EquipItemTemplate();
                _val3.Read(iprot);
                EquipTmpl[_key2] = _val3;
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
              ArmorTmpl = new Dictionary<int, ArmorItemTemplate>();
              TMap _map4 = iprot.ReadMapBegin();
              for( int _i5 = 0; _i5 < _map4.Count; ++_i5)
              {
                int _key6;
                ArmorItemTemplate _val7;
                _key6 = iprot.ReadI32();
                _val7 = new ArmorItemTemplate();
                _val7.Read(iprot);
                ArmorTmpl[_key6] = _val7;
              }
              iprot.ReadMapEnd();
            }
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 3:
          if (field.Type == TType.Map) {
            {
              MaterialTmpl = new Dictionary<int, MaterialItemTemplate>();
              TMap _map8 = iprot.ReadMapBegin();
              for( int _i9 = 0; _i9 < _map8.Count; ++_i9)
              {
                int _key10;
                MaterialItemTemplate _val11;
                _key10 = iprot.ReadI32();
                _val11 = new MaterialItemTemplate();
                _val11.Read(iprot);
                MaterialTmpl[_key10] = _val11;
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
    TStruct struc = new TStruct("Item");
    oprot.WriteStructBegin(struc);
    TField field = new TField();
    if (EquipTmpl != null && __isset.equipTmpl) {
      field.Name = "equipTmpl";
      field.Type = TType.Map;
      field.ID = 1;
      oprot.WriteFieldBegin(field);
      {
        oprot.WriteMapBegin(new TMap(TType.I32, TType.Struct, EquipTmpl.Count));
        foreach (int _iter12 in EquipTmpl.Keys)
        {
          oprot.WriteI32(_iter12);
          EquipTmpl[_iter12].Write(oprot);
        }
        oprot.WriteMapEnd();
      }
      oprot.WriteFieldEnd();
    }
    if (ArmorTmpl != null && __isset.armorTmpl) {
      field.Name = "armorTmpl";
      field.Type = TType.Map;
      field.ID = 2;
      oprot.WriteFieldBegin(field);
      {
        oprot.WriteMapBegin(new TMap(TType.I32, TType.Struct, ArmorTmpl.Count));
        foreach (int _iter13 in ArmorTmpl.Keys)
        {
          oprot.WriteI32(_iter13);
          ArmorTmpl[_iter13].Write(oprot);
        }
        oprot.WriteMapEnd();
      }
      oprot.WriteFieldEnd();
    }
    if (MaterialTmpl != null && __isset.materialTmpl) {
      field.Name = "materialTmpl";
      field.Type = TType.Map;
      field.ID = 3;
      oprot.WriteFieldBegin(field);
      {
        oprot.WriteMapBegin(new TMap(TType.I32, TType.Struct, MaterialTmpl.Count));
        foreach (int _iter14 in MaterialTmpl.Keys)
        {
          oprot.WriteI32(_iter14);
          MaterialTmpl[_iter14].Write(oprot);
        }
        oprot.WriteMapEnd();
      }
      oprot.WriteFieldEnd();
    }
    oprot.WriteFieldStop();
    oprot.WriteStructEnd();
  }

  public override string ToString() {
    StringBuilder sb = new StringBuilder("Item(");
    sb.Append("EquipTmpl: ");
    sb.Append(EquipTmpl);
    sb.Append(",ArmorTmpl: ");
    sb.Append(ArmorTmpl);
    sb.Append(",MaterialTmpl: ");
    sb.Append(MaterialTmpl);
    sb.Append(")");
    return sb.ToString();
  }

}
