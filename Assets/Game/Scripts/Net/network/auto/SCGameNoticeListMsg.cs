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
  /// 游戏公告列表
  /// </summary>
  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class SCGameNoticeListMsg : TBase
  {
    private List<KXSGCodec.GameNoticeMsgInfo> _noticeList;

    /// <summary>
    /// 公告列表
    /// </summary>
    public List<KXSGCodec.GameNoticeMsgInfo> NoticeList
    {
      get
      {
        return _noticeList;
      }
      set
      {
        __isset.noticeList = true;
        this._noticeList = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool noticeList;
    }

    public SCGameNoticeListMsg() {
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
                NoticeList = new List<KXSGCodec.GameNoticeMsgInfo>();
                TList _list0 = iprot.ReadListBegin();
                for( int _i1 = 0; _i1 < _list0.Count; ++_i1)
                {
                  KXSGCodec.GameNoticeMsgInfo _elem2 = new KXSGCodec.GameNoticeMsgInfo();
                  _elem2 = new KXSGCodec.GameNoticeMsgInfo();
                  _elem2.Read(iprot);
                  NoticeList.Add(_elem2);
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
      TStruct struc = new TStruct("SCGameNoticeListMsg");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (NoticeList != null && __isset.noticeList) {
        field.Name = "noticeList";
        field.Type = TType.List;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteListBegin(new TList(TType.Struct, NoticeList.Count));
          foreach (KXSGCodec.GameNoticeMsgInfo _iter3 in NoticeList)
          {
            _iter3.Write(oprot);
          }
          oprot.WriteListEnd();
        }
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("SCGameNoticeListMsg(");
      sb.Append("NoticeList: ");
      sb.Append(NoticeList);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
