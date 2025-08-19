// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.ExceptionUtils
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Utils;

public static class ExceptionUtils
{
  public static ArgumentOutOfRangeException MakeArgumentOutOfRangeException(
    string paramName,
    object actualValue,
    string message)
  {
    return new ArgumentOutOfRangeException(paramName, actualValue, message);
  }

  public static ArgumentNullException MakeArgumentItemNullException(int index, string arrayName)
  {
    return new ArgumentNullException($"{arrayName}[{index}]");
  }

  public static object GetData(this Exception e, object key) => e.Data[key];

  public static void SetData(this Exception e, object key, object data) => e.Data[key] = data;

  public static void RemoveData(this Exception e, object key) => e.Data.Remove(key);
}
