// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.EventDef
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct EventDef
{
  private readonly MetadataRecord m_record;

  internal EventDef(MetadataRecord record) => this.m_record = record;

  public EventAttributes Attributes
  {
    get
    {
      MetadataRecord record = this.m_record;
      EventTable eventTable = record.Import.EventTable;
      record = this.m_record;
      int rid = record.Rid;
      return eventTable.GetFlags(rid);
    }
  }

  public MetadataName Name
  {
    get
    {
      MetadataRecord record = this.m_record;
      MetadataTables tables = record.Tables;
      record = this.m_record;
      EventTable eventTable = record.Import.EventTable;
      record = this.m_record;
      int rid = record.Rid;
      int name = (int) eventTable.GetName(rid);
      return tables.ToMetadataName((uint) name);
    }
  }

  public MetadataRecord EventType
  {
    get
    {
      MetadataRecord record = this.m_record;
      EventTable eventTable = record.Import.EventTable;
      record = this.m_record;
      int rid = record.Rid;
      MetadataToken eventType = eventTable.GetEventType(rid);
      record = this.m_record;
      MetadataTables tables = record.Tables;
      return new MetadataRecord(eventType, tables);
    }
  }

  public EventAccessors GetAccessors()
  {
    MetadataImport import = this.m_record.Import;
    int methodCount;
    int semanticMethodsForEvent = import.MethodSemanticsTable.FindSemanticMethodsForEvent(this.m_record.Rid, out methodCount);
    uint rowId1 = 0;
    uint rowId2 = 0;
    uint rowId3 = 0;
    for (ushort index = 0; (int) index < methodCount; ++index)
    {
      switch (import.MethodSemanticsTable.GetFlags(semanticMethodsForEvent))
      {
        case MethodSemanticsFlags.AddOn:
          rowId1 = import.MethodSemanticsTable.GetMethodRid(semanticMethodsForEvent);
          break;
        case MethodSemanticsFlags.RemoveOn:
          rowId2 = import.MethodSemanticsTable.GetMethodRid(semanticMethodsForEvent);
          break;
        case MethodSemanticsFlags.Fire:
          rowId3 = import.MethodSemanticsTable.GetMethodRid(semanticMethodsForEvent);
          break;
      }
      ++semanticMethodsForEvent;
    }
    return new EventAccessors(this, new MetadataToken(MetadataTokenType.MethodDef, rowId1), new MetadataToken(MetadataTokenType.MethodDef, rowId2), new MetadataToken(MetadataTokenType.MethodDef, rowId3));
  }

  public TypeDef FindDeclaringType()
  {
    MetadataRecord record = this.m_record;
    EventMapTable eventMapTable = record.Import.EventMapTable;
    record = this.m_record;
    int rid = record.Rid;
    record = this.m_record;
    int numberOfRows = record.Import.EventTable.NumberOfRows;
    return new MetadataRecord(new MetadataToken(MetadataTokenType.TypeDef, eventMapTable.FindTypeContainingEvent(rid, numberOfRows)), this.m_record.Tables).TypeDef;
  }

  public static implicit operator MetadataRecord(EventDef eventDef) => eventDef.m_record;

  public static explicit operator EventDef(MetadataRecord record) => record.EventDef;

  public MetadataRecord Record => this.m_record;

  public MetadataTableView CustomAttributes
  {
    get => new MetadataTableView(this.m_record, MetadataTokenType.CustomAttribute);
  }
}
