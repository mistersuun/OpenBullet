// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.SerializationBinderAdapter
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace Newtonsoft.Json.Serialization;

internal class SerializationBinderAdapter : ISerializationBinder
{
  public readonly SerializationBinder SerializationBinder;

  public SerializationBinderAdapter(SerializationBinder serializationBinder)
  {
    this.SerializationBinder = serializationBinder;
  }

  public Type BindToType(string assemblyName, string typeName)
  {
    return this.SerializationBinder.BindToType(assemblyName, typeName);
  }

  public void BindToName(Type serializedType, out string assemblyName, out string typeName)
  {
    this.SerializationBinder.BindToName(serializedType, out assemblyName, out typeName);
  }
}
