// Decompiled with JetBrains decompiler
// Type: AngleSharp.Text.StringBuilderPool
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace AngleSharp.Text;

public static class StringBuilderPool
{
  private static readonly Stack<StringBuilder> _builder = new Stack<StringBuilder>();
  private static readonly object _lock = new object();
  private static int _count = 4;
  private static int _limit = 85000;

  public static int MaxCount
  {
    get => StringBuilderPool._count;
    set => StringBuilderPool._count = Math.Max(1, value);
  }

  public static int SizeLimit
  {
    get => StringBuilderPool._limit;
    set => StringBuilderPool._limit = Math.Max(1024 /*0x0400*/, value);
  }

  public static StringBuilder Obtain()
  {
    lock (StringBuilderPool._lock)
      return StringBuilderPool._builder.Count == 0 ? new StringBuilder(1024 /*0x0400*/) : StringBuilderPool._builder.Pop().Clear();
  }

  public static string ToPool(this StringBuilder sb)
  {
    string pool = sb.ToString();
    lock (StringBuilderPool._lock)
    {
      int count = StringBuilderPool._builder.Count;
      if (sb.Capacity <= StringBuilderPool._limit)
      {
        if (count == StringBuilderPool._count)
        {
          StringBuilderPool.DropMinimum(sb);
        }
        else
        {
          if (count >= Math.Min(2, StringBuilderPool._count))
          {
            if (StringBuilderPool._builder.Peek().Capacity >= sb.Capacity)
              goto label_10;
          }
          StringBuilderPool._builder.Push(sb);
        }
      }
    }
label_10:
    return pool;
  }

  private static void DropMinimum(StringBuilder sb)
  {
    int capacity = sb.Capacity;
    StringBuilder[] array = StringBuilderPool._builder.ToArray();
    int index = StringBuilderPool.FindIndex(array, capacity);
    if (index <= -1)
      return;
    StringBuilderPool.RebuildPool(sb, array, index);
  }

  private static void RebuildPool(StringBuilder sb, StringBuilder[] instances, int index)
  {
    StringBuilderPool._builder.Clear();
    int num = instances.Length - 1;
    while (num > index)
      StringBuilderPool._builder.Push(instances[num--]);
    while (num > 0)
      StringBuilderPool._builder.Push(instances[--num]);
    StringBuilderPool._builder.Push(sb);
  }

  private static int FindIndex(StringBuilder[] instances, int minimum)
  {
    int index1 = -1;
    for (int index2 = 0; index2 < instances.Length; ++index2)
    {
      int capacity = instances[index2].Capacity;
      if (capacity < minimum)
      {
        minimum = capacity;
        index1 = index2;
      }
    }
    return index1;
  }
}
