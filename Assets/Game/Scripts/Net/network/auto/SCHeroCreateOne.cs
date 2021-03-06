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
  public partial class SCHeroCreateOne : TBase
  {
    private KXSGCodec.HeroInfo _newHero;

    public KXSGCodec.HeroInfo NewHero
    {
      get
      {
        return _newHero;
      }
      set
      {
        __isset.newHero = true;
        this._newHero = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool newHero;
    }

    public SCHeroCreateOne() {
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
            if (field.Type == TType.Struct) {
              NewHero = new KXSGCodec.HeroInfo();
              NewHero.Read(iprot);
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
      TStruct struc = new TStruct("SCHeroCreateOne");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (NewHero != null && __isset.newHero) {
        field.Name = "newHero";
        field.Type = TType.Struct;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        NewHero.Write(oprot);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("SCHeroCreateOne(");
      sb.Append("NewHero: ");
      sb.Append(NewHero== null ? "<null>" : NewHero.ToString());
      sb.Append(")");
      return sb.ToString();
    }

  }

}
