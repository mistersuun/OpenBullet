// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.SerializationBinderAdapter
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

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
