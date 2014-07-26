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
  public partial class MonsterTemplate : TBase
  {
    private int _id;
    private string _monsterName;
    private int _level;
    private int _icon;
    private int _monsterType;
    private int _weakJob;
    private int _attack;
    private int _defense;
    private int _HP;
    private int _CD;
    private int _shield;
    private int _shieldCount;
    private int _aiID;
    private int _dropGroupId;
    private int _dropGroupRate;

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
    /// 怪物名称
    /// </summary>
    public string MonsterName
    {
      get
      {
        return _monsterName;
      }
      set
      {
        __isset.monsterName = true;
        this._monsterName = value;
      }
    }

    /// <summary>
    /// 怪物等级
    /// </summary>
    public int Level
    {
      get
      {
        return _level;
      }
      set
      {
        __isset.level = true;
        this._level = value;
      }
    }

    /// <summary>
    /// 怪物外形
    /// </summary>
    public int Icon
    {
      get
      {
        return _icon;
      }
      set
      {
        __isset.icon = true;
        this._icon = value;
      }
    }

    /// <summary>
    /// 怪物类型
    /// </summary>
    public int MonsterType
    {
      get
      {
        return _monsterType;
      }
      set
      {
        __isset.monsterType = true;
        this._monsterType = value;
      }
    }

    /// <summary>
    /// 弱职业性
    /// </summary>
    public int WeakJob
    {
      get
      {
        return _weakJob;
      }
      set
      {
        __isset.weakJob = true;
        this._weakJob = value;
      }
    }

    /// <summary>
    /// 攻击力
    /// </summary>
    public int Attack
    {
      get
      {
        return _attack;
      }
      set
      {
        __isset.attack = true;
        this._attack = value;
      }
    }

    /// <summary>
    /// 防御力
    /// </summary>
    public int Defense
    {
      get
      {
        return _defense;
      }
      set
      {
        __isset.defense = true;
        this._defense = value;
      }
    }

    /// <summary>
    /// 血量
    /// </summary>
    public int HP
    {
      get
      {
        return _HP;
      }
      set
      {
        __isset.HP = true;
        this._HP = value;
      }
    }

    /// <summary>
    /// 出手CD
    /// </summary>
    public int CD
    {
      get
      {
        return _CD;
      }
      set
      {
        __isset.CD = true;
        this._CD = value;
      }
    }

    /// <summary>
    /// 盾牌种类
    /// </summary>
    public int Shield
    {
      get
      {
        return _shield;
      }
      set
      {
        __isset.shield = true;
        this._shield = value;
      }
    }

    /// <summary>
    /// 带盾个数
    /// </summary>
    public int ShieldCount
    {
      get
      {
        return _shieldCount;
      }
      set
      {
        __isset.shieldCount = true;
        this._shieldCount = value;
      }
    }

    /// <summary>
    /// AI的ID
    /// </summary>
    public int AiID
    {
      get
      {
        return _aiID;
      }
      set
      {
        __isset.aiID = true;
        this._aiID = value;
      }
    }

    /// <summary>
    /// 掉落组ID
    /// </summary>
    public int DropGroupId
    {
      get
      {
        return _dropGroupId;
      }
      set
      {
        __isset.dropGroupId = true;
        this._dropGroupId = value;
      }
    }

    /// <summary>
    /// 掉落组概率
    /// </summary>
    public int DropGroupRate
    {
      get
      {
        return _dropGroupRate;
      }
      set
      {
        __isset.dropGroupRate = true;
        this._dropGroupRate = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool id;
      public bool monsterName;
      public bool level;
      public bool icon;
      public bool monsterType;
      public bool weakJob;
      public bool attack;
      public bool defense;
      public bool HP;
      public bool CD;
      public bool shield;
      public bool shieldCount;
      public bool aiID;
      public bool dropGroupId;
      public bool dropGroupRate;
    }

    public MonsterTemplate() {
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
            if (field.Type == TType.String) {
              MonsterName = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.I32) {
              Level = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.I32) {
              Icon = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 5:
            if (field.Type == TType.I32) {
              MonsterType = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 6:
            if (field.Type == TType.I32) {
              WeakJob = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 7:
            if (field.Type == TType.I32) {
              Attack = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 8:
            if (field.Type == TType.I32) {
              Defense = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 9:
            if (field.Type == TType.I32) {
              HP = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 10:
            if (field.Type == TType.I32) {
              CD = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 11:
            if (field.Type == TType.I32) {
              Shield = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 12:
            if (field.Type == TType.I32) {
              ShieldCount = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 13:
            if (field.Type == TType.I32) {
              AiID = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 14:
            if (field.Type == TType.I32) {
              DropGroupId = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 15:
            if (field.Type == TType.I32) {
              DropGroupRate = iprot.ReadI32();
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
      TStruct struc = new TStruct("MonsterTemplate");
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
      if (MonsterName != null && __isset.monsterName) {
        field.Name = "monsterName";
        field.Type = TType.String;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(MonsterName);
        oprot.WriteFieldEnd();
      }
      if (__isset.level) {
        field.Name = "level";
        field.Type = TType.I32;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(Level);
        oprot.WriteFieldEnd();
      }
      if (__isset.icon) {
        field.Name = "icon";
        field.Type = TType.I32;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(Icon);
        oprot.WriteFieldEnd();
      }
      if (__isset.monsterType) {
        field.Name = "monsterType";
        field.Type = TType.I32;
        field.ID = 5;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(MonsterType);
        oprot.WriteFieldEnd();
      }
      if (__isset.weakJob) {
        field.Name = "weakJob";
        field.Type = TType.I32;
        field.ID = 6;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(WeakJob);
        oprot.WriteFieldEnd();
      }
      if (__isset.attack) {
        field.Name = "attack";
        field.Type = TType.I32;
        field.ID = 7;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(Attack);
        oprot.WriteFieldEnd();
      }
      if (__isset.defense) {
        field.Name = "defense";
        field.Type = TType.I32;
        field.ID = 8;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(Defense);
        oprot.WriteFieldEnd();
      }
      if (__isset.HP) {
        field.Name = "HP";
        field.Type = TType.I32;
        field.ID = 9;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(HP);
        oprot.WriteFieldEnd();
      }
      if (__isset.CD) {
        field.Name = "CD";
        field.Type = TType.I32;
        field.ID = 10;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(CD);
        oprot.WriteFieldEnd();
      }
      if (__isset.shield) {
        field.Name = "shield";
        field.Type = TType.I32;
        field.ID = 11;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(Shield);
        oprot.WriteFieldEnd();
      }
      if (__isset.shieldCount) {
        field.Name = "shieldCount";
        field.Type = TType.I32;
        field.ID = 12;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(ShieldCount);
        oprot.WriteFieldEnd();
      }
      if (__isset.aiID) {
        field.Name = "aiID";
        field.Type = TType.I32;
        field.ID = 13;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(AiID);
        oprot.WriteFieldEnd();
      }
      if (__isset.dropGroupId) {
        field.Name = "dropGroupId";
        field.Type = TType.I32;
        field.ID = 14;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(DropGroupId);
        oprot.WriteFieldEnd();
      }
      if (__isset.dropGroupRate) {
        field.Name = "dropGroupRate";
        field.Type = TType.I32;
        field.ID = 15;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(DropGroupRate);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("MonsterTemplate(");
      sb.Append("Id: ");
      sb.Append(Id);
      sb.Append(",MonsterName: ");
      sb.Append(MonsterName);
      sb.Append(",Level: ");
      sb.Append(Level);
      sb.Append(",Icon: ");
      sb.Append(Icon);
      sb.Append(",MonsterType: ");
      sb.Append(MonsterType);
      sb.Append(",WeakJob: ");
      sb.Append(WeakJob);
      sb.Append(",Attack: ");
      sb.Append(Attack);
      sb.Append(",Defense: ");
      sb.Append(Defense);
      sb.Append(",HP: ");
      sb.Append(HP);
      sb.Append(",CD: ");
      sb.Append(CD);
      sb.Append(",Shield: ");
      sb.Append(Shield);
      sb.Append(",ShieldCount: ");
      sb.Append(ShieldCount);
      sb.Append(",AiID: ");
      sb.Append(AiID);
      sb.Append(",DropGroupId: ");
      sb.Append(DropGroupId);
      sb.Append(",DropGroupRate: ");
      sb.Append(DropGroupRate);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
