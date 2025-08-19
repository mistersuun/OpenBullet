// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.CompareUtil
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using System;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

internal class CompareUtil
{
  [ThreadStatic]
  private static Stack<object> CmpStack;

  internal static bool Check(object o)
  {
    return CompareUtil.CmpStack != null && CompareUtil.CmpStack.Contains(o);
  }

  internal static void Push(object o)
  {
    if (CompareUtil.GetInfiniteCmp().Contains(o))
      throw PythonOps.RuntimeError("maximum recursion depth exceeded in cmp");
    CompareUtil.CmpStack.Push(o);
  }

  internal static void Push(object o1, object o2)
  {
    Stack<object> infiniteCmp = CompareUtil.GetInfiniteCmp();
    TwoObjects twoObjects1 = new TwoObjects(o1, o2);
    TwoObjects twoObjects2 = twoObjects1;
    if (infiniteCmp.Contains((object) twoObjects2))
      throw PythonOps.RuntimeError("maximum recursion depth exceeded in cmp");
    CompareUtil.CmpStack.Push((object) twoObjects1);
  }

  internal static void Pop(object o) => CompareUtil.CmpStack.Pop();

  internal static void Pop(object o1, object o2) => CompareUtil.CmpStack.Pop();

  private static Stack<object> GetInfiniteCmp()
  {
    if (CompareUtil.CmpStack == null)
      CompareUtil.CmpStack = new Stack<object>();
    return CompareUtil.CmpStack;
  }
}
