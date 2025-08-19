// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.IListOfByteOps
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace IronPython.Runtime.Operations;

internal static class IListOfByteOps
{
  internal static int Compare(this IList<byte> self, IList<byte> other)
  {
    for (int index = 0; index < self.Count && index < other.Count; ++index)
    {
      if ((int) self[index] != (int) other[index])
        return (int) self[index] > (int) other[index] ? 1 : -1;
    }
    if (self.Count == other.Count)
      return 0;
    return self.Count <= other.Count ? -1 : 1;
  }

  internal static int Compare(this IList<byte> self, string other)
  {
    for (int index = 0; index < self.Count && index < other.Length; ++index)
    {
      if ((int) self[index] != (int) other[index])
        return (int) self[index] > (int) other[index] ? 1 : -1;
    }
    if (self.Count == other.Length)
      return 0;
    return self.Count <= other.Length ? -1 : 1;
  }

  internal static bool EndsWith(this IList<byte> self, IList<byte> suffix)
  {
    if (self.Count < suffix.Count)
      return false;
    int num = self.Count - suffix.Count;
    for (int index = 0; index < suffix.Count; ++index)
    {
      if ((int) suffix[index] != (int) self[index + num])
        return false;
    }
    return true;
  }

  internal static bool EndsWith(this IList<byte> bytes, IList<byte> suffix, int start)
  {
    int count = bytes.Count;
    if (start > count)
      return false;
    if (start < 0)
    {
      start += count;
      if (start < 0)
        start = 0;
    }
    return bytes.Substring(start).EndsWith(suffix);
  }

  internal static bool EndsWith(this IList<byte> bytes, IList<byte> suffix, int start, int end)
  {
    int count = bytes.Count;
    if (start > count)
      return false;
    if (start < 0)
    {
      start += count;
      if (start < 0)
        start = 0;
    }
    if (end >= count)
      return bytes.Substring(start).EndsWith(suffix);
    if (end < 0)
    {
      end += count;
      if (end < 0)
        return false;
    }
    return end >= start && bytes.Substring(start, end - start).EndsWith(suffix);
  }

  internal static bool EndsWith(this IList<byte> bytes, PythonTuple suffix)
  {
    foreach (object obj in suffix)
    {
      if (bytes.EndsWith(ByteOps.CoerceBytes(obj)))
        return true;
    }
    return false;
  }

  internal static bool EndsWith(this IList<byte> bytes, PythonTuple suffix, int start)
  {
    int count = bytes.Count;
    if (start > count)
      return false;
    if (start < 0)
    {
      start += count;
      if (start < 0)
        start = 0;
    }
    foreach (object obj in suffix)
    {
      if (bytes.Substring(start).EndsWith(ByteOps.CoerceBytes(obj)))
        return true;
    }
    return false;
  }

  internal static bool EndsWith(this IList<byte> bytes, PythonTuple suffix, int start, int end)
  {
    int count = bytes.Count;
    if (start > count)
      return false;
    if (start < 0)
    {
      start += count;
      if (start < 0)
        start = 0;
    }
    if (end >= count)
      end = count;
    else if (end < 0)
    {
      end += count;
      if (end < 0)
        return false;
    }
    if (end < start)
      return false;
    foreach (object obj in suffix)
    {
      if (bytes.Substring(start, end - start).EndsWith(ByteOps.CoerceBytes(obj)))
        return true;
    }
    return false;
  }

  internal static bool StartsWith(this IList<byte> self, IList<byte> prefix)
  {
    if (self.Count < prefix.Count)
      return false;
    for (int index = 0; index < prefix.Count; ++index)
    {
      if ((int) prefix[index] != (int) self[index])
        return false;
    }
    return true;
  }

  internal static int IndexOfAny(this IList<byte> str, IList<byte> separators, int i)
  {
    for (; i < str.Count; ++i)
    {
      for (int index = 0; index < separators.Count; ++index)
      {
        if ((int) str[i] == (int) separators[index])
          return i;
      }
    }
    return -1;
  }

  internal static int IndexOf(this IList<byte> bytes, IList<byte> sub, int start)
  {
    return bytes.IndexOf(sub, start, bytes.Count - start);
  }

  internal static int IndexOf(this IList<byte> self, IList<byte> ssub, int start, int length)
  {
    if (ssub == null)
      throw PythonOps.TypeError("cannot do None in bytes or bytearray");
    if (ssub.Count == 0)
      return 0;
    byte num = ssub[0];
    for (int index1 = start; index1 < start + length; ++index1)
    {
      if ((int) self[index1] == (int) num)
      {
        bool flag = false;
        for (int index2 = 1; index2 < ssub.Count; ++index2)
        {
          if (index2 + index1 == start + length || (int) ssub[index2] != (int) self[index1 + index2])
          {
            flag = true;
            break;
          }
        }
        if (!flag)
          return index1;
      }
    }
    return -1;
  }

  internal static bool IsTitle(this IList<byte> bytes)
  {
    if (bytes.Count == 0)
      return false;
    bool flag1 = false;
    bool flag2 = false;
    for (int index = 0; index < bytes.Count; ++index)
    {
      bool flag3;
      if (bytes[index].IsUpper())
      {
        flag2 = true;
        if (flag1)
          return false;
        flag3 = true;
      }
      else if (bytes[index].IsLower())
      {
        if (!flag1)
          return false;
        flag3 = true;
      }
      else
        flag3 = false;
      flag1 = flag3;
    }
    return flag2;
  }

  internal static bool IsUpper(this IList<byte> bytes)
  {
    bool flag = false;
    foreach (byte p in (IEnumerable<byte>) bytes)
    {
      flag = flag || p.IsUpper();
      if (p.IsLower())
        return false;
    }
    return flag;
  }

  internal static List<byte> Title(this IList<byte> self)
  {
    if (self.Count == 0)
      return (List<byte>) null;
    List<byte> byteList = new List<byte>((IEnumerable<byte>) self);
    bool flag1 = false;
    int index = 0;
    do
    {
      bool flag2;
      if (byteList[index].IsUpper() || byteList[index].IsLower())
      {
        byteList[index] = flag1 ? byteList[index].ToLower() : byteList[index].ToUpper();
        flag2 = true;
      }
      else
        flag2 = false;
      ++index;
      flag1 = flag2;
    }
    while (index < byteList.Count);
    return byteList;
  }

  internal static int LastIndexOf(this IList<byte> self, IList<byte> sub, int start, int length)
  {
    byte num1 = sub[sub.Count - 1];
    for (int index1 = start - 1; index1 >= start - length; --index1)
    {
      if ((int) self[index1] == (int) num1)
      {
        bool flag = false;
        if (sub.Count != 1)
        {
          int index2 = sub.Count - 2;
          int num2 = 1;
          while (index2 >= 0)
          {
            if ((int) sub[index2] != (int) self[index1 - num2])
            {
              flag = true;
              break;
            }
            --index2;
            ++num2;
          }
        }
        if (!flag)
          return index1 - sub.Count + 1;
      }
    }
    return -1;
  }

  internal static List<byte>[] Split(
    this IList<byte> str,
    IList<byte> separators,
    int maxComponents,
    StringSplitOptions options)
  {
    ContractUtils.RequiresNotNull((object) str, nameof (str));
    bool flag = (options & StringSplitOptions.RemoveEmptyEntries) != StringSplitOptions.RemoveEmptyEntries;
    if (separators == null)
      return str.SplitOnWhiteSpace(maxComponents);
    List<List<byte>> byteListList = new List<List<byte>>(maxComponents == int.MaxValue ? 1 : maxComponents + 1);
    int num1;
    int num2;
    for (num1 = 0; maxComponents > 1 && num1 < str.Count && (num2 = str.IndexOfAny(separators, num1)) != -1; num1 = num2 + separators.Count)
    {
      if (num2 > num1 | flag)
      {
        byteListList.Add(str.Substring(num1, num2 - num1));
        --maxComponents;
      }
    }
    if (num1 < str.Count | flag)
      byteListList.Add(str.Substring(num1));
    return byteListList.ToArray();
  }

  internal static List<byte>[] SplitOnWhiteSpace(this IList<byte> str, int maxComponents)
  {
    ContractUtils.RequiresNotNull((object) str, nameof (str));
    List<List<byte>> byteListList = new List<List<byte>>(maxComponents == int.MaxValue ? 1 : maxComponents + 1);
    int num1;
    int num2;
    for (num1 = 0; maxComponents > 1 && num1 < str.Count && (num2 = str.IndexOfWhiteSpace(num1)) != -1; num1 = num2 + 1)
    {
      if (num2 > num1)
      {
        byteListList.Add(str.Substring(num1, num2 - num1));
        --maxComponents;
      }
    }
    if (num1 < str.Count)
    {
      while (num1 < str.Count && str[num1].IsWhiteSpace())
        ++num1;
      if (num1 < str.Count)
        byteListList.Add(str.Substring(num1));
    }
    return byteListList.ToArray();
  }

  internal static bool StartsWith(this IList<byte> bytes, IList<byte> prefix, int start, int end)
  {
    int count = bytes.Count;
    if (start > count)
      return false;
    if (start < 0)
    {
      start += count;
      if (start < 0)
        start = 0;
    }
    if (end >= count)
      return bytes.Substring(start).StartsWith(prefix);
    if (end < 0)
    {
      end += count;
      if (end < 0)
        return false;
    }
    return end >= start && bytes.Substring(start, end - start).StartsWith(prefix);
  }

  internal static List<byte> Replace(
    this IList<byte> bytes,
    IList<byte> old,
    IList<byte> @new,
    int count)
  {
    if (@new == null)
      throw PythonOps.TypeError("expected bytes or bytearray, got NoneType");
    if (count == -1)
      count = old.Count + 1;
    if (old.Count == 0)
      return bytes.ReplaceEmpty(@new, count);
    List<byte> byteList = new List<byte>(bytes.Count);
    int start;
    int num;
    for (start = 0; count > 0 && (num = bytes.IndexOf(old, start)) != -1; --count)
    {
      byteList.AddRange((IEnumerable<byte>) bytes.Substring(start, num - start));
      byteList.AddRange((IEnumerable<byte>) @new);
      start = num + old.Count;
    }
    byteList.AddRange((IEnumerable<byte>) bytes.Substring(start));
    return byteList;
  }

  private static List<byte> ReplaceEmpty(this IList<byte> self, IList<byte> @new, int count)
  {
    int num = count > self.Count ? self.Count : count;
    List<byte> byteList = new List<byte>(self.Count * (@new.Count + 1));
    for (int index = 0; index < num; ++index)
    {
      byteList.AddRange((IEnumerable<byte>) @new);
      byteList.Add(self[index]);
    }
    for (int index = num; index < self.Count; ++index)
      byteList.Add(self[index]);
    if (count > num)
      byteList.AddRange((IEnumerable<byte>) @new);
    return byteList;
  }

  internal static int ReverseFind(this IList<byte> bytes, IList<byte> sub, int? start, int? end)
  {
    if (sub == null)
      throw PythonOps.TypeError("expected string, got NoneType");
    int? nullable1 = start;
    int count = bytes.Count;
    if (nullable1.GetValueOrDefault() > count & nullable1.HasValue)
      return -1;
    int num1 = IListOfByteOps.FixStart(bytes, start);
    int start1 = IListOfByteOps.FixEnd(bytes, end);
    if (num1 > start1)
      return -1;
    if (sub.Count == 0)
      return start1;
    int? nullable2 = end;
    int num2 = 0;
    return nullable2.GetValueOrDefault() == num2 & nullable2.HasValue ? -1 : bytes.LastIndexOf(sub, start1, start1 - num1);
  }

  internal static List RightSplit(
    this IList<byte> bytes,
    IList<byte> sep,
    int maxsplit,
    Func<IList<byte>, IList<byte>> ctor)
  {
    IList<byte> byteList = (IList<byte>) bytes.ReverseBytes();
    if (sep != null)
      sep = (IList<byte>) sep.ReverseBytes();
    List list1 = ctor(byteList).Split(sep, maxsplit, (Func<List<byte>, object>) (x => (object) ctor((IList<byte>) x)));
    list1.reverse();
    int capacity = list1.__len__();
    List list2;
    if (capacity != 0)
    {
      list2 = new List(capacity);
      foreach (IList<byte> s in list1)
        list2.AddNoLock((object) ctor((IList<byte>) s.ReverseBytes()));
    }
    else
      list2 = list1;
    return list2;
  }

  internal static int IndexOfWhiteSpace(this IList<byte> str, int start)
  {
    while (start < str.Count && !str[start].IsWhiteSpace())
      ++start;
    return start != str.Count ? start : -1;
  }

  internal static byte[] ReverseBytes(this IList<byte> s)
  {
    byte[] numArray = new byte[s.Count];
    int index1 = s.Count - 1;
    int index2 = 0;
    while (index1 >= 0)
    {
      numArray[index2] = s[index1];
      --index1;
      ++index2;
    }
    return numArray;
  }

  internal static List<byte> Substring(this IList<byte> bytes, int start)
  {
    return bytes.Substring(start, bytes.Count - start);
  }

  internal static List<byte> Substring(this IList<byte> bytes, int start, int len)
  {
    List<byte> byteList = new List<byte>();
    for (int index = start; index < start + len; ++index)
      byteList.Add(bytes[index]);
    return byteList;
  }

  internal static List<byte> Multiply(this IList<byte> self, int count)
  {
    if (count <= 0)
      return new List<byte>();
    List<byte> byteList = new List<byte>(checked (self.Count * count));
    for (int index = 0; index < count; ++index)
      byteList.AddRange((IEnumerable<byte>) self);
    return byteList;
  }

  internal static List<byte> Capitalize(this IList<byte> bytes)
  {
    List<byte> byteList = new List<byte>((IEnumerable<byte>) bytes);
    if (byteList.Count > 0)
    {
      byteList[0] = byteList[0].ToUpper();
      for (int index = 1; index < byteList.Count; ++index)
        byteList[index] = byteList[index].ToLower();
    }
    return byteList;
  }

  internal static List<byte> TryCenter(this IList<byte> bytes, int width, int fillchar)
  {
    int num = width - bytes.Count;
    if (num <= 0)
      return (List<byte>) null;
    byte byteChecked = fillchar.ToByteChecked();
    List<byte> byteList = new List<byte>();
    for (int index = 0; index < num / 2; ++index)
      byteList.Add(byteChecked);
    byteList.AddRange((IEnumerable<byte>) bytes);
    for (int index = 0; index < (num + 1) / 2; ++index)
      byteList.Add(byteChecked);
    return byteList;
  }

  internal static int CountOf(this IList<byte> bytes, IList<byte> ssub, int start, int end)
  {
    if (ssub == null)
      throw PythonOps.TypeError("expected bytes or byte array, got NoneType");
    if (start > bytes.Count)
      return 0;
    start = PythonOps.FixSliceIndex(start, bytes.Count);
    end = PythonOps.FixSliceIndex(end, bytes.Count);
    if (ssub.Count == 0)
      return Math.Max(end - start + 1, 0);
    int num1 = 0;
    int num2;
    for (; end > start; start = num2 + ssub.Count)
    {
      num2 = bytes.IndexOf(ssub, start, end - start);
      if (num2 != -1)
        ++num1;
      else
        break;
    }
    return num1;
  }

  internal static List<byte> ExpandTabs(this IList<byte> bytes, int tabsize)
  {
    List<byte> byteList = new List<byte>(bytes.Count * 2);
    int num1 = 0;
    for (int index1 = 0; index1 < bytes.Count; ++index1)
    {
      byte num2 = bytes[index1];
      switch (num2)
      {
        case 9:
          if (tabsize > 0)
          {
            int num3 = tabsize - num1 % tabsize;
            int capacity = byteList.Capacity;
            byteList.Capacity = checked (capacity + num3);
            for (int index2 = 0; index2 < num3; ++index2)
              byteList.Add((byte) 32 /*0x20*/);
            num1 = 0;
            break;
          }
          break;
        case 10:
        case 13:
          num1 = 0;
          byteList.Add(num2);
          break;
        default:
          ++num1;
          byteList.Add(num2);
          break;
      }
    }
    return byteList;
  }

  internal static int IndexOfByte(this IList<byte> bytes, int item, int start, int stop)
  {
    start = PythonOps.FixSliceIndex(start, bytes.Count);
    stop = PythonOps.FixSliceIndex(stop, bytes.Count);
    for (int index = start; index < Math.Min(stop, bytes.Count); ++index)
    {
      if ((int) bytes[index] == item)
        return index;
    }
    throw PythonOps.ValueError("bytearray.index(item): item not in bytearray");
  }

  internal static string BytesRepr(this IList<byte> bytes)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("b'");
    for (int index = 0; index < bytes.Count; ++index)
    {
      byte num = bytes[index];
      switch (num)
      {
        case 9:
          stringBuilder.Append("\\t");
          break;
        case 10:
          stringBuilder.Append("\\n");
          break;
        case 13:
          stringBuilder.Append("\\r");
          break;
        case 39:
          stringBuilder.Append('\\');
          stringBuilder.Append('\'');
          break;
        case 92:
          stringBuilder.Append("\\\\");
          break;
        default:
          if (num < (byte) 32 /*0x20*/ || num >= (byte) 127 /*0x7F*/ && num <= byte.MaxValue)
          {
            stringBuilder.AppendFormat("\\x{0:x2}", (object) num);
            break;
          }
          stringBuilder.Append((char) num);
          break;
      }
    }
    stringBuilder.Append("'");
    return stringBuilder.ToString();
  }

  internal static List<byte> ZeroFill(this IList<byte> bytes, int width, int spaces)
  {
    List<byte> byteList = new List<byte>(width);
    if (bytes.Count > 0 && bytes[0].IsSign())
    {
      byteList.Add(bytes[0]);
      for (int index = 0; index < spaces; ++index)
        byteList.Add((byte) 48 /*0x30*/);
      for (int index = 1; index < bytes.Count; ++index)
        byteList.Add(bytes[index]);
    }
    else
    {
      for (int index = 0; index < spaces; ++index)
        byteList.Add((byte) 48 /*0x30*/);
      byteList.AddRange((IEnumerable<byte>) bytes);
    }
    return byteList;
  }

  internal static List<byte> ToLower(this IList<byte> bytes)
  {
    List<byte> lower = new List<byte>();
    for (int index = 0; index < bytes.Count; ++index)
      lower.Add(bytes[index].ToLower());
    return lower;
  }

  internal static List<byte> ToUpper(this IList<byte> bytes)
  {
    List<byte> upper = new List<byte>();
    for (int index = 0; index < bytes.Count; ++index)
      upper.Add(bytes[index].ToUpper());
    return upper;
  }

  internal static List<byte> Translate(
    this IList<byte> bytes,
    IList<byte> table,
    IList<byte> deletechars)
  {
    List<byte> byteList = new List<byte>();
    for (int index = 0; index < bytes.Count; ++index)
    {
      if (deletechars == null || !deletechars.Contains(bytes[index]))
      {
        if (table == null)
          byteList.Add(bytes[index]);
        else
          byteList.Add(table[(int) bytes[index]]);
      }
    }
    return byteList;
  }

  internal static List<byte> RightStrip(this IList<byte> bytes)
  {
    int index1 = bytes.Count - 1;
    while (index1 >= 0 && bytes[index1].IsWhiteSpace())
      --index1;
    if (index1 == bytes.Count - 1)
      return (List<byte>) null;
    List<byte> byteList = new List<byte>();
    for (int index2 = 0; index2 <= index1; ++index2)
      byteList.Add(bytes[index2]);
    return byteList;
  }

  internal static List<byte> RightStrip(this IList<byte> bytes, IList<byte> chars)
  {
    int index1 = bytes.Count - 1;
    while (index1 >= 0 && chars.Contains(bytes[index1]))
      --index1;
    if (index1 == bytes.Count - 1)
      return (List<byte>) null;
    List<byte> byteList = new List<byte>();
    for (int index2 = 0; index2 <= index1; ++index2)
      byteList.Add(bytes[index2]);
    return byteList;
  }

  internal static List SplitLines(
    this IList<byte> bytes,
    bool keepends,
    Func<List<byte>, object> ctor)
  {
    List list = new List();
    int index = 0;
    int start = 0;
    for (; index < bytes.Count; ++index)
    {
      if (bytes[index] == (byte) 10 || bytes[index] == (byte) 13)
      {
        if (index < bytes.Count - 1 && bytes[index] == (byte) 13 && bytes[index + 1] == (byte) 10)
        {
          if (keepends)
            list.AddNoLock(ctor(bytes.Substring(start, index - start + 2)));
          else
            list.AddNoLock(ctor(bytes.Substring(start, index - start)));
          start = index + 2;
          ++index;
        }
        else
        {
          if (keepends)
            list.AddNoLock(ctor(bytes.Substring(start, index - start + 1)));
          else
            list.AddNoLock(ctor(bytes.Substring(start, index - start)));
          start = index + 1;
        }
      }
    }
    if (index - start != 0)
      list.AddNoLock(ctor(bytes.Substring(start, index - start)));
    return list;
  }

  internal static List<byte> LeftStrip(this IList<byte> bytes)
  {
    int index = 0;
    while (index < bytes.Count && bytes[index].IsWhiteSpace())
      ++index;
    if (index == 0)
      return (List<byte>) null;
    List<byte> byteList = new List<byte>();
    for (; index < bytes.Count; ++index)
      byteList.Add(bytes[index]);
    return byteList;
  }

  internal static List<byte> LeftStrip(this IList<byte> bytes, IList<byte> chars)
  {
    int index = 0;
    while (index < bytes.Count && chars.Contains(bytes[index]))
      ++index;
    if (index == 0)
      return (List<byte>) null;
    List<byte> byteList = new List<byte>();
    for (; index < bytes.Count; ++index)
      byteList.Add(bytes[index]);
    return byteList;
  }

  internal static List Split(
    this IList<byte> bytes,
    IList<byte> sep,
    int maxsplit,
    Func<List<byte>, object> ctor)
  {
    if (sep == null)
    {
      if (maxsplit != 0)
        return IListOfByteOps.SplitInternal(bytes, (byte[]) null, maxsplit, ctor);
      List list = PythonOps.MakeEmptyList(1);
      list.AddNoLock(ctor(bytes.LeftStrip() ?? (bytes is List<byte> byteList ? byteList : new List<byte>((IEnumerable<byte>) bytes))));
      return list;
    }
    if (sep.Count == 0)
      throw PythonOps.ValueError("empty separator");
    if (sep.Count != 1)
      return bytes.SplitInternal(sep, maxsplit, ctor);
    return IListOfByteOps.SplitInternal(bytes, new byte[1]
    {
      sep[0]
    }, maxsplit, ctor);
  }

  internal static List SplitInternal(
    IList<byte> bytes,
    byte[] seps,
    int maxsplit,
    Func<List<byte>, object> ctor)
  {
    if (bytes.Count == 0)
      return IListOfByteOps.SplitEmptyString(seps != null, ctor);
    List<byte>[] byteListArray = bytes.Split((IList<byte>) seps, maxsplit < 0 ? int.MaxValue : maxsplit + 1, IListOfByteOps.GetStringSplitOptions((IList<byte>) seps));
    List list = PythonOps.MakeEmptyList(byteListArray.Length);
    foreach (List<byte> byteList in byteListArray)
      list.AddNoLock(ctor(byteList));
    return list;
  }

  private static StringSplitOptions GetStringSplitOptions(IList<byte> seps)
  {
    return seps != null ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries;
  }

  internal static List SplitInternal(
    this IList<byte> bytes,
    IList<byte> separator,
    int maxsplit,
    Func<List<byte>, object> ctor)
  {
    if (bytes.Count == 0)
      return IListOfByteOps.SplitEmptyString(separator != null, ctor);
    List<byte>[] byteListArray = bytes.Split(separator, maxsplit < 0 ? int.MaxValue : maxsplit + 1, IListOfByteOps.GetStringSplitOptions(separator));
    List list = PythonOps.MakeEmptyList(byteListArray.Length);
    foreach (List<byte> byteList in byteListArray)
      list.AddNoLock(ctor(byteList));
    return list;
  }

  private static List SplitEmptyString(bool separators, Func<List<byte>, object> ctor)
  {
    List list = PythonOps.MakeEmptyList(1);
    if (separators)
      list.AddNoLock(ctor(new List<byte>(0)));
    return list;
  }

  internal static List<byte> Strip(this IList<byte> bytes)
  {
    int index1 = 0;
    while (index1 < bytes.Count && bytes[index1].IsWhiteSpace())
      ++index1;
    int index2 = bytes.Count - 1;
    while (index2 >= 0 && bytes[index2].IsWhiteSpace())
      --index2;
    if (index1 == 0 && index2 == bytes.Count - 1)
      return (List<byte>) null;
    List<byte> byteList = new List<byte>();
    for (int index3 = index1; index3 <= index2; ++index3)
      byteList.Add(bytes[index3]);
    return byteList;
  }

  internal static List<byte> Strip(this IList<byte> bytes, IList<byte> chars)
  {
    int index1 = 0;
    while (index1 < bytes.Count && chars.Contains(bytes[index1]))
      ++index1;
    int index2 = bytes.Count - 1;
    while (index2 >= 0 && chars.Contains(bytes[index2]))
      --index2;
    if (index1 == 0 && index2 == bytes.Count - 1)
      return (List<byte>) null;
    List<byte> byteList = new List<byte>();
    for (int index3 = index1; index3 <= index2; ++index3)
      byteList.Add(bytes[index3]);
    return byteList;
  }

  internal static List<byte> Slice(this IList<byte> bytes, IronPython.Runtime.Slice slice)
  {
    if (slice == null)
      throw PythonOps.TypeError("indices must be slices or integers");
    int ostart;
    int ostop;
    int ostep;
    slice.indices(bytes.Count, out ostart, out ostop, out ostep);
    if (ostep == 1)
      return ostop <= ostart ? (List<byte>) null : bytes.Substring(ostart, ostop - ostart);
    List<byte> byteList;
    if (ostep > 0)
    {
      if (ostart > ostop)
        return (List<byte>) null;
      byteList = new List<byte>((ostop - ostart + ostep - 1) / ostep);
      for (int index = ostart; index < ostop; index += ostep)
        byteList.Add(bytes[index]);
    }
    else
    {
      if (ostart < ostop)
        return (List<byte>) null;
      byteList = new List<byte>((ostop - ostart + ostep + 1) / ostep);
      for (int index = ostart; index > ostop; index += ostep)
        byteList.Add(bytes[index]);
    }
    return byteList;
  }

  internal static List<byte> SwapCase(this IList<byte> bytes)
  {
    List<byte> byteList = new List<byte>((IEnumerable<byte>) bytes);
    for (int index = 0; index < bytes.Count; ++index)
    {
      byte p = byteList[index];
      if (p.IsUpper())
        byteList[index] = p.ToLower();
      else if (p.IsLower())
        byteList[index] = p.ToUpper();
    }
    return byteList;
  }

  internal static bool StartsWith(this IList<byte> bytes, PythonTuple prefix, int start, int end)
  {
    int count = bytes.Count;
    if (start > count)
      return false;
    if (start < 0)
    {
      start += count;
      if (start < 0)
        start = 0;
    }
    if (end >= count)
      end = count;
    else if (end < 0)
    {
      end += count;
      if (end < 0)
        return false;
    }
    if (end < start)
      return false;
    foreach (object obj in prefix)
    {
      if (bytes.Substring(start, end - start).StartsWith(ByteOps.CoerceBytes(obj)))
        return true;
    }
    return false;
  }

  internal static bool StartsWith(this IList<byte> bytes, PythonTuple prefix, int start)
  {
    int count = bytes.Count;
    if (start > count)
      return false;
    if (start < 0)
    {
      start += count;
      if (start < 0)
        start = 0;
    }
    foreach (object obj in prefix)
    {
      if (bytes.Substring(start).StartsWith(ByteOps.CoerceBytes(obj)))
        return true;
    }
    return false;
  }

  internal static bool StartsWith(this IList<byte> bytes, PythonTuple prefix)
  {
    foreach (object obj in prefix)
    {
      if (bytes.StartsWith(ByteOps.CoerceBytes(obj)))
        return true;
    }
    return false;
  }

  internal static bool IsWhiteSpace(this IList<byte> bytes)
  {
    if (bytes.Count == 0)
      return false;
    foreach (byte b in (IEnumerable<byte>) bytes)
    {
      if (!b.IsWhiteSpace())
        return false;
    }
    return true;
  }

  internal static bool IsLower(this IList<byte> bytes)
  {
    bool flag = false;
    foreach (byte p in (IEnumerable<byte>) bytes)
    {
      flag = flag || p.IsLower();
      if (p.IsUpper())
        return false;
    }
    return flag;
  }

  internal static bool IsDigit(this IList<byte> bytes)
  {
    if (bytes.Count == 0)
      return false;
    foreach (byte b in (IEnumerable<byte>) bytes)
    {
      if (!b.IsDigit())
        return false;
    }
    return true;
  }

  internal static bool IsLetter(this IList<byte> bytes)
  {
    if (bytes.Count == 0)
      return false;
    foreach (byte b in (IEnumerable<byte>) bytes)
    {
      if (!b.IsLetter())
        return false;
    }
    return true;
  }

  internal static bool IsAlphaNumeric(this IList<byte> bytes)
  {
    if (bytes.Count == 0)
      return false;
    foreach (byte b in (IEnumerable<byte>) bytes)
    {
      if (!b.IsDigit() && !b.IsLetter())
        return false;
    }
    return true;
  }

  internal static int Find(this IList<byte> bytes, IList<byte> sub)
  {
    return sub != null ? bytes.IndexOf(sub, 0) : throw PythonOps.TypeError("expected byte or byte array, got NoneType");
  }

  internal static int Find(this IList<byte> bytes, IList<byte> sub, int? start)
  {
    if (sub == null)
      throw PythonOps.TypeError("expected byte or byte array, got NoneType");
    int? nullable = start;
    int count = bytes.Count;
    if (nullable.GetValueOrDefault() > count & nullable.HasValue)
      return -1;
    int start1 = !start.HasValue ? 0 : PythonOps.FixSliceIndex(start.Value, bytes.Count);
    return bytes.IndexOf(sub, start1);
  }

  internal static int Find(this IList<byte> bytes, IList<byte> sub, int? start, int? end)
  {
    if (sub == null)
      throw PythonOps.TypeError("expected byte or byte array, got NoneType");
    int? nullable = start;
    int count = bytes.Count;
    if (nullable.GetValueOrDefault() > count & nullable.HasValue)
      return -1;
    int start1 = IListOfByteOps.FixStart(bytes, start);
    int num = IListOfByteOps.FixEnd(bytes, end);
    return num < start1 ? -1 : bytes.IndexOf(sub, start1, num - start1);
  }

  private static int FixEnd(IList<byte> bytes, int? end)
  {
    return !end.HasValue ? bytes.Count : PythonOps.FixSliceIndex(end.Value, bytes.Count);
  }

  private static int FixStart(IList<byte> bytes, int? start)
  {
    return !start.HasValue ? 0 : PythonOps.FixSliceIndex(start.Value, bytes.Count);
  }

  internal static byte ToByte(this string self, string name, int pos)
  {
    return self.Length == 1 && self[0] < 'Ā' ? (byte) self[0] : throw PythonOps.TypeError($"{name}() argument {(object) pos} must be char < 256, not string");
  }

  internal static byte ToByte(this IList<byte> self, string name, int pos)
  {
    if (self == null)
      throw PythonOps.TypeError($"{name}() argument {(object) pos} must be char < 256, not None");
    return self.Count == 1 ? self[0] : throw PythonOps.TypeError($"{name}() argument {(object) pos} must be char < 256, not bytearray or bytes");
  }

  internal static List<byte> FromHex(string @string)
  {
    if (@string == null)
      throw PythonOps.TypeError("expected str, got NoneType");
    List<byte> byteList = new List<byte>();
    for (int index = 0; index < @string.Length; ++index)
    {
      char c1 = @string[index];
      int num1;
      if (char.IsDigit(c1))
        num1 = ((int) c1 - 48 /*0x30*/) * 16 /*0x10*/;
      else if (c1 >= 'A' && c1 <= 'F')
        num1 = ((int) c1 - 65 + 10) * 16 /*0x10*/;
      else if (c1 >= 'a' && c1 <= 'f')
      {
        num1 = ((int) c1 - 97 + 10) * 16 /*0x10*/;
      }
      else
      {
        if (c1 != ' ')
          throw PythonOps.ValueError("non-hexadecimal number found in fromhex() arg at position {0}", (object) index);
        continue;
      }
      ++index;
      if (index == @string.Length)
        throw PythonOps.ValueError("non-hexadecimal number found in fromhex() arg at position {0}", (object) (index - 1));
      char c2 = @string[index];
      int num2;
      if (char.IsDigit(c2))
        num2 = num1 + ((int) c2 - 48 /*0x30*/);
      else if (c2 >= 'A' && c2 <= 'F')
        num2 = num1 + ((int) c2 - 65 + 10);
      else if (c2 >= 'a' && c2 <= 'f')
        num2 = num1 + ((int) c2 - 97 + 10);
      else
        throw PythonOps.ValueError("non-hexadecimal number found in fromhex() arg at position {0}", (object) index);
      byteList.Add((byte) num2);
    }
    return byteList;
  }

  internal static IEnumerable BytesEnumerable(IList<byte> bytes)
  {
    return (IEnumerable) new IListOfByteOps.PythonBytesEnumerator<Bytes>(bytes, (Func<byte, Bytes>) (b => Bytes.Make(new byte[1]
    {
      b
    })));
  }

  internal static IEnumerable BytesIntEnumerable(IList<byte> bytes)
  {
    return (IEnumerable) new IListOfByteOps.PythonBytesEnumerator<int>(bytes, (Func<byte, int>) (b => (int) b));
  }

  internal static IEnumerator<Bytes> BytesEnumerator(IList<byte> bytes)
  {
    return (IEnumerator<Bytes>) new IListOfByteOps.PythonBytesEnumerator<Bytes>(bytes, (Func<byte, Bytes>) (b => Bytes.Make(new byte[1]
    {
      b
    })));
  }

  internal static IEnumerator<int> BytesIntEnumerator(IList<byte> bytes)
  {
    return (IEnumerator<int>) new IListOfByteOps.PythonBytesEnumerator<int>(bytes, (Func<byte, int>) (b => (int) b));
  }

  [PythonType("bytes_iterator")]
  private class PythonBytesEnumerator<T> : IEnumerable, IEnumerator<T>, IDisposable, IEnumerator
  {
    private readonly IList<byte> _bytes;
    private readonly Func<byte, T> _conversion;
    private int _index;

    public PythonBytesEnumerator(IList<byte> bytes, Func<byte, T> conversion)
    {
      this._bytes = bytes;
      this._conversion = conversion;
      this._index = -1;
    }

    public T Current
    {
      get
      {
        if (this._index < 0)
          throw PythonOps.SystemError("Enumeration has not started. Call MoveNext.");
        if (this._index >= this._bytes.Count)
          throw PythonOps.SystemError("Enumeration already finished.");
        return this._conversion(this._bytes[this._index]);
      }
    }

    public void Dispose()
    {
    }

    object IEnumerator.Current => (object) this.Current;

    public bool MoveNext()
    {
      if (this._index >= this._bytes.Count)
        return false;
      ++this._index;
      return this._index != this._bytes.Count;
    }

    public void Reset() => this._index = -1;

    public IEnumerator GetEnumerator() => (IEnumerator) this;
  }
}
