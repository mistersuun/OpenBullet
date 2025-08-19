// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.AssemblyLoadedEventArgs
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting;

public class AssemblyLoadedEventArgs : EventArgs
{
  public AssemblyLoadedEventArgs(Assembly assembly) => this.Assembly = assembly;

  public Assembly Assembly { get; }
}
