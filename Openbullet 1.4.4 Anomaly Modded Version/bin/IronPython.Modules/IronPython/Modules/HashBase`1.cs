// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.HashBase`1
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

#nullable disable
namespace IronPython.Modules;

[PythonHidden(new PlatformID[] {})]
public abstract class HashBase<T> : ICloneable where T : HashAlgorithm
{
  protected T _hasher;
  private static MethodInfo _memberwiseClone;
  private static readonly Encoding _raw = Encoding.GetEncoding("iso-8859-1");
  private static readonly byte[] _empty = HashBase<T>._raw.GetBytes(string.Empty);
  public readonly string name;
  public readonly int block_size;
  public readonly int digest_size;
  public readonly int digestsize;

  internal HashBase(string name, int blocksize, int digestsize)
  {
    this.name = name;
    this.block_size = blocksize;
    this.digest_size = this.digestsize = digestsize;
    this.CreateHasher();
  }

  protected abstract void CreateHasher();

  public void update(Bytes newBytes) => this.update((IList<byte>) newBytes);

  public void update(ByteArray newBytes) => this.update((IList<byte>) newBytes);

  [Documentation("update(string) -> None (update digest with string data)")]
  public void update(object newData)
  {
    if (newData is ArrayModule.array array)
      this.update((IList<byte>) array.ToByteArray());
    else
      this.update((IList<byte>) Converter.ConvertToString(newData).MakeByteArray());
  }

  public void update(PythonBuffer buffer) => this.update((IList<byte>) buffer);

  internal void update(IList<byte> newBytes)
  {
    byte[] array = newBytes.ToArray<byte>();
    lock ((object) this._hasher)
      this._hasher.TransformBlock(array, 0, array.Length, array, 0);
  }

  [Documentation("digest() -> int (current digest value)")]
  public string digest()
  {
    T obj = this.CloneHasher();
    obj.TransformFinalBlock(HashBase<T>._empty, 0, 0);
    return ((IList<byte>) obj.Hash).MakeString();
  }

  [Documentation("hexdigest() -> string (current digest as hex digits)")]
  public string hexdigest()
  {
    T obj = this.CloneHasher();
    obj.TransformFinalBlock(HashBase<T>._empty, 0, 0);
    StringBuilder stringBuilder = new StringBuilder(2 * obj.Hash.Length);
    for (int index = 0; index < obj.Hash.Length; ++index)
      stringBuilder.Append(obj.Hash[index].ToString("x2"));
    return stringBuilder.ToString();
  }

  public abstract HashBase<T> copy();

  object ICloneable.Clone() => (object) this.copy();

  protected T CloneHasher()
  {
    T obj = default (T);
    if (HashBase<T>._memberwiseClone == (MethodInfo) null)
      HashBase<T>._memberwiseClone = this._hasher.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    if (HashBase<T>._memberwiseClone != (MethodInfo) null)
    {
      lock ((object) this._hasher)
        obj = (T) HashBase<T>._memberwiseClone.Invoke((object) this._hasher, new object[0]);
    }
    FieldInfo[] fields = this._hasher.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    if (fields != null)
    {
      foreach (FieldInfo fieldInfo in fields)
      {
        if (fieldInfo.FieldType.IsArray)
        {
          lock ((object) this._hasher)
          {
            if (fieldInfo.GetValue((object) this._hasher) is Array array)
              fieldInfo.SetValue((object) obj, array.Clone());
          }
        }
      }
    }
    return obj;
  }
}
