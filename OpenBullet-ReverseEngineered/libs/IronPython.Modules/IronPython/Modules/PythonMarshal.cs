// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonMarshal
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace IronPython.Modules;

public static class PythonMarshal
{
  public const string __doc__ = "Provides functions for serializing and deserializing primitive data types.";
  public const int version = 2;

  public static void dump(object value, PythonFile file) => PythonMarshal.dump(value, file, 2);

  public static void dump(object value, PythonFile file, int version)
  {
    if (file == null)
      throw PythonOps.TypeError("expected file, found None");
    file.write(PythonMarshal.dumps(value, version));
  }

  public static object load(PythonFile file)
  {
    return file != null ? MarshalOps.GetObject(PythonMarshal.FileEnumerator(file)) : throw PythonOps.TypeError("expected file, found None");
  }

  public static object dumps(object value) => (object) PythonMarshal.dumps(value, 2);

  public static string dumps(object value, int version)
  {
    byte[] bytes = MarshalOps.GetBytes(value, version);
    StringBuilder stringBuilder = new StringBuilder(bytes.Length);
    for (int index = 0; index < bytes.Length; ++index)
      stringBuilder.Append((char) bytes[index]);
    return stringBuilder.ToString();
  }

  public static object loads(string @string)
  {
    return MarshalOps.GetObject(PythonMarshal.StringEnumerator(@string));
  }

  private static IEnumerator<byte> FileEnumerator(PythonFile file)
  {
    while (true)
    {
      string str = file.read(1);
      if (str.Length != 0)
        yield return (byte) str[0];
      else
        break;
    }
  }

  private static IEnumerator<byte> StringEnumerator(string str)
  {
    for (int i = 0; i < str.Length; ++i)
      yield return (byte) str[i];
  }
}
