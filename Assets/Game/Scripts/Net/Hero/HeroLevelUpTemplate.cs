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
public partial class HeroLevelUpTemplate : TBase
{
    private int _id;
    private long _costSoulStar1;
    private long _costSoulStar2;
    private long _costSoulStar3;
    private long _costSoulStar4;
    private long _costSoulStar5;

    public int Id
    {
        get
        {
            return _id;
        }
        set
        {
            __isset.id = true;
            this._id = value;
        }
    }

    /// <summary>
    /// 1星武将升级消耗武魂
    /// </summary>
    public long CostSoulStar1
    {
        get
        {
            return _costSoulStar1;
        }
        set
        {
            __isset.costSoulStar1 = true;
            this._costSoulStar1 = value;
        }
    }

    /// <summary>
    /// 2星武将升级消耗武魂
    /// </summary>
    public long CostSoulStar2
    {
        get
        {
            return _costSoulStar2;
        }
        set
        {
            __isset.costSoulStar2 = true;
            this._costSoulStar2 = value;
        }
    }

    /// <summary>
    /// 3星武将升级消耗武魂
    /// </summary>
    public long CostSoulStar3
    {
        get
        {
            return _costSoulStar3;
        }
        set
        {
            __isset.costSoulStar3 = true;
            this._costSoulStar3 = value;
        }
    }

    /// <summary>
    /// 4星武将升级消耗武魂
    /// </summary>
    public long CostSoulStar4
    {
        get
        {
            return _costSoulStar4;
        }
        set
        {
            __isset.costSoulStar4 = true;
            this._costSoulStar4 = value;
        }
    }

    /// <summary>
    /// 5星武将升级消耗武魂
    /// </summary>
    public long CostSoulStar5
    {
        get
        {
            return _costSoulStar5;
        }
        set
        {
            __isset.costSoulStar5 = true;
            this._costSoulStar5 = value;
        }
    }


    public Isset __isset;
#if !SILVERLIGHT
    [Serializable]
#endif
    public struct Isset
    {
        public bool id;
        public bool costSoulStar1;
        public bool costSoulStar2;
        public bool costSoulStar3;
        public bool costSoulStar4;
        public bool costSoulStar5;
    }

    public HeroLevelUpTemplate()
    {
    }

    public void Read(TProtocol iprot)
    {
        TField field;
        iprot.ReadStructBegin();
        while (true)
        {
            field = iprot.ReadFieldBegin();
            if (field.Type == TType.Stop)
            {
                break;
            }
            switch (field.ID)
            {
                case 1:
                    if (field.Type == TType.I32)
                    {
                        Id = iprot.ReadI32();
                    }
                    else
                    {
                        TProtocolUtil.Skip(iprot, field.Type);
                    }
                    break;
                case 2:
                    if (field.Type == TType.I64)
                    {
                        CostSoulStar1 = iprot.ReadI64();
                    }
                    else
                    {
                        TProtocolUtil.Skip(iprot, field.Type);
                    }
                    break;
                case 3:
                    if (field.Type == TType.I64)
                    {
                        CostSoulStar2 = iprot.ReadI64();
                    }
                    else
                    {
                        TProtocolUtil.Skip(iprot, field.Type);
                    }
                    break;
                case 4:
                    if (field.Type == TType.I64)
                    {
                        CostSoulStar3 = iprot.ReadI64();
                    }
                    else
                    {
                        TProtocolUtil.Skip(iprot, field.Type);
                    }
                    break;
                case 5:
                    if (field.Type == TType.I64)
                    {
                        CostSoulStar4 = iprot.ReadI64();
                    }
                    else
                    {
                        TProtocolUtil.Skip(iprot, field.Type);
                    }
                    break;
                case 6:
                    if (field.Type == TType.I64)
                    {
                        CostSoulStar5 = iprot.ReadI64();
                    }
                    else
                    {
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

    public void Write(TProtocol oprot)
    {
        TStruct struc = new TStruct("HeroLevelUpTemplate");
        oprot.WriteStructBegin(struc);
        TField field = new TField();
        if (__isset.id)
        {
            field.Name = "id";
            field.Type = TType.I32;
            field.ID = 1;
            oprot.WriteFieldBegin(field);
            oprot.WriteI32(Id);
            oprot.WriteFieldEnd();
        }
        if (__isset.costSoulStar1)
        {
            field.Name = "costSoulStar1";
            field.Type = TType.I64;
            field.ID = 2;
            oprot.WriteFieldBegin(field);
            oprot.WriteI64(CostSoulStar1);
            oprot.WriteFieldEnd();
        }
        if (__isset.costSoulStar2)
        {
            field.Name = "costSoulStar2";
            field.Type = TType.I64;
            field.ID = 3;
            oprot.WriteFieldBegin(field);
            oprot.WriteI64(CostSoulStar2);
            oprot.WriteFieldEnd();
        }
        if (__isset.costSoulStar3)
        {
            field.Name = "costSoulStar3";
            field.Type = TType.I64;
            field.ID = 4;
            oprot.WriteFieldBegin(field);
            oprot.WriteI64(CostSoulStar3);
            oprot.WriteFieldEnd();
        }
        if (__isset.costSoulStar4)
        {
            field.Name = "costSoulStar4";
            field.Type = TType.I64;
            field.ID = 5;
            oprot.WriteFieldBegin(field);
            oprot.WriteI64(CostSoulStar4);
            oprot.WriteFieldEnd();
        }
        if (__isset.costSoulStar5)
        {
            field.Name = "costSoulStar5";
            field.Type = TType.I64;
            field.ID = 6;
            oprot.WriteFieldBegin(field);
            oprot.WriteI64(CostSoulStar5);
            oprot.WriteFieldEnd();
        }
        oprot.WriteFieldStop();
        oprot.WriteStructEnd();
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder("HeroLevelUpTemplate(");
        sb.Append("Id: ");
        sb.Append(Id);
        sb.Append(",CostSoulStar1: ");
        sb.Append(CostSoulStar1);
        sb.Append(",CostSoulStar2: ");
        sb.Append(CostSoulStar2);
        sb.Append(",CostSoulStar3: ");
        sb.Append(CostSoulStar3);
        sb.Append(",CostSoulStar4: ");
        sb.Append(CostSoulStar4);
        sb.Append(",CostSoulStar5: ");
        sb.Append(CostSoulStar5);
        sb.Append(")");
        return sb.ToString();
    }

}

