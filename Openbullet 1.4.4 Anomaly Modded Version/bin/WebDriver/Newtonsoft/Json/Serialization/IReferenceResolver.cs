// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.IReferenceResolver
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace Newtonsoft.Json.Serialization;

internal interface IReferenceResolver
{
  object ResolveReference(object context, string reference);

  string GetReference(object context, object value);

  bool IsReferenced(object context, object value);

  void AddReference(object context, string reference, object value);
}
