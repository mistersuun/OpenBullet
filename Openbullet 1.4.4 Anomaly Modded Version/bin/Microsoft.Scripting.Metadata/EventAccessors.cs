// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.EventAccessors
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct EventAccessors
{
  private readonly EventDef m_event;
  private readonly MetadataToken m_add;
  private readonly MetadataToken m_remove;
  private readonly MetadataToken m_fire;

  internal EventAccessors(
    EventDef eventDef,
    MetadataToken add,
    MetadataToken remove,
    MetadataToken fire)
  {
    this.m_add = add;
    this.m_remove = remove;
    this.m_fire = fire;
    this.m_event = eventDef;
  }

  public EventDef DeclaringEvent => this.m_event;

  public bool HasAdd => !this.m_add.IsNull;

  public bool HasRemove => !this.m_remove.IsNull;

  public bool HasFire => !this.m_fire.IsNull;

  public MethodDef Add => new MetadataRecord(this.m_add, this.m_event.Record.Tables).MethodDef;

  public MethodDef Remove
  {
    get => new MetadataRecord(this.m_remove, this.m_event.Record.Tables).MethodDef;
  }

  public MethodDef Fire => new MetadataRecord(this.m_fire, this.m_event.Record.Tables).MethodDef;

  public IEnumerable<MethodDef> Others => throw new NotImplementedException();
}
