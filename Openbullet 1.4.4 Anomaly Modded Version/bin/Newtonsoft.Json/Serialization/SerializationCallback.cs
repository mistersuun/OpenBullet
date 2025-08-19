// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.SerializationCallback
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

using System.Runtime.Serialization;

#nullable disable
namespace Newtonsoft.Json.Serialization;

public delegate void SerializationCallback(object o, StreamingContext context);
