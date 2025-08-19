// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.ModuleChangeEventArgs
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public class ModuleChangeEventArgs : EventArgs
{
  public ModuleChangeEventArgs(string name, ModuleChangeType changeType)
  {
    this.Name = name;
    this.ChangeType = changeType;
  }

  public ModuleChangeEventArgs(string name, ModuleChangeType changeType, object value)
  {
    this.Name = name;
    this.ChangeType = changeType;
    this.Value = value;
  }

  public string Name { get; }

  public ModuleChangeType ChangeType { get; }

  public object Value { get; }
}
