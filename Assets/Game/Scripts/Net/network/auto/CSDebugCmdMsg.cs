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
  public partial class CSDebugCmdMsg : TBase
  {
    private string _cmd;

    public string Cmd
    {
      get
      {
        return _cmd;
      }
      set
      {
        __isset.cmd = true;
        this._cmd = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool cmd;
    }

    public CSDebugCmdMsg() {
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
              Cmd = iprot.ReadString();
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
      TStruct struc = new TStruct("CSDebugCmdMsg");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (Cmd != null && __isset.cmd) {
        field.Name = "cmd";
        field.Type = TType.String;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(Cmd);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("CSDebugCmdMsg(");
      sb.Append("Cmd: ");
      sb.Append(Cmd);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
