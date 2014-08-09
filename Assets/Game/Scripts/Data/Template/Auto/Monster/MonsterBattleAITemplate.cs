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

namespace Template.Auto.Monster
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class MonsterBattleAITemplate : TBase
  {
    private int _id;
    private int _shieldBuffId;
    private List<SkillRatePairData> _defaultSkills;
    private List<MonsterSkillAIData> _aiSkills;

    /// <summary>
    /// ID
    /// </summary>
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
    /// 护盾ID
    /// </summary>
    public int ShieldBuffId
    {
      get
      {
        return _shieldBuffId;
      }
      set
      {
        __isset.shieldBuffId = true;
        this._shieldBuffId = value;
      }
    }

    /// <summary>
    /// 默认技能
    /// </summary>
    public List<SkillRatePairData> DefaultSkills
    {
      get
      {
        return _defaultSkills;
      }
      set
      {
        __isset.defaultSkills = true;
        this._defaultSkills = value;
      }
    }

    /// <summary>
    /// 怪物AI技能
    /// </summary>
    public List<MonsterSkillAIData> AiSkills
    {
      get
      {
        return _aiSkills;
      }
      set
      {
        __isset.aiSkills = true;
        this._aiSkills = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool id;
      public bool shieldBuffId;
      public bool defaultSkills;
      public bool aiSkills;
    }

    public MonsterBattleAITemplate() {
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
            if (field.Type == TType.I32) {
              Id = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.I32) {
              ShieldBuffId = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.List) {
              {
                DefaultSkills = new List<SkillRatePairData>();
                TList _list0 = iprot.ReadListBegin();
                for( int _i1 = 0; _i1 < _list0.Count; ++_i1)
                {
                  SkillRatePairData _elem2 = new SkillRatePairData();
                  _elem2 = new SkillRatePairData();
                  _elem2.Read(iprot);
                  DefaultSkills.Add(_elem2);
                }
                iprot.ReadListEnd();
              }
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.List) {
              {
                AiSkills = new List<MonsterSkillAIData>();
                TList _list3 = iprot.ReadListBegin();
                for( int _i4 = 0; _i4 < _list3.Count; ++_i4)
                {
                  MonsterSkillAIData _elem5 = new MonsterSkillAIData();
                  _elem5 = new MonsterSkillAIData();
                  _elem5.Read(iprot);
                  AiSkills.Add(_elem5);
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
      TStruct struc = new TStruct("MonsterBattleAITemplate");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.id) {
        field.Name = "id";
        field.Type = TType.I32;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(Id);
        oprot.WriteFieldEnd();
      }
      if (__isset.shieldBuffId) {
        field.Name = "shieldBuffId";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(ShieldBuffId);
        oprot.WriteFieldEnd();
      }
      if (DefaultSkills != null && __isset.defaultSkills) {
        field.Name = "defaultSkills";
        field.Type = TType.List;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteListBegin(new TList(TType.Struct, DefaultSkills.Count));
          foreach (SkillRatePairData _iter6 in DefaultSkills)
          {
            _iter6.Write(oprot);
          }
          oprot.WriteListEnd();
        }
        oprot.WriteFieldEnd();
      }
      if (AiSkills != null && __isset.aiSkills) {
        field.Name = "aiSkills";
        field.Type = TType.List;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteListBegin(new TList(TType.Struct, AiSkills.Count));
          foreach (MonsterSkillAIData _iter7 in AiSkills)
          {
            _iter7.Write(oprot);
          }
          oprot.WriteListEnd();
        }
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("MonsterBattleAITemplate(");
      sb.Append("Id: ");
      sb.Append(Id);
      sb.Append(",ShieldBuffId: ");
      sb.Append(ShieldBuffId);
      sb.Append(",DefaultSkills: ");
      sb.Append(DefaultSkills);
      sb.Append(",AiSkills: ");
      sb.Append(AiSkills);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
