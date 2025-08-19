// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.DocumentationOperations
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting;

#nullable disable
namespace Microsoft.Scripting.Hosting;

public sealed class DocumentationOperations : MarshalByRefObject
{
  private readonly DocumentationProvider _provider;

  internal DocumentationOperations(DocumentationProvider provider) => this._provider = provider;

  public ICollection<MemberDoc> GetMembers(object value) => this._provider.GetMembers(value);

  public ICollection<OverloadDoc> GetOverloads(object value) => this._provider.GetOverloads(value);

  public ICollection<MemberDoc> GetMembers(ObjectHandle value)
  {
    return this._provider.GetMembers(value.Unwrap());
  }

  public ICollection<OverloadDoc> GetOverloads(ObjectHandle value)
  {
    return this._provider.GetOverloads(value.Unwrap());
  }

  public override object InitializeLifetimeService() => (object) null;
}
