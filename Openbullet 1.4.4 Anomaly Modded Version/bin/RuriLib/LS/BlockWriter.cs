// Decompiled with JetBrains decompiler
// Type: RuriLib.LS.BlockWriter
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace RuriLib.LS;

public class BlockWriter : StringWriter
{
  private bool Indented { get; set; }

  private Type Type { get; set; }

  private object Block { get; set; }

  private bool Disabled { get; set; }

  public BlockWriter(Type blockType, bool indented = true, bool disabled = false)
  {
    this.Type = blockType;
    this.Block = Activator.CreateInstance(blockType);
    this.Indented = indented;
    this.Disabled = disabled;
    if (!this.Disabled)
      return;
    this.Write("!");
  }

  public BlockWriter Token(object token, string property = "")
  {
    // ISSUE: reference to a compiler-generated field
    if (BlockWriter.\u003C\u003Eo__17.\u003C\u003Ep__2 == null)
    {
      // ISSUE: reference to a compiler-generated field
      BlockWriter.\u003C\u003Eo__17.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (BlockWriter), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
      {
        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
      }));
    }
    // ISSUE: reference to a compiler-generated field
    Func<CallSite, object, bool> target1 = BlockWriter.\u003C\u003Eo__17.\u003C\u003Ep__2.Target;
    // ISSUE: reference to a compiler-generated field
    CallSite<Func<CallSite, object, bool>> p2 = BlockWriter.\u003C\u003Eo__17.\u003C\u003Ep__2;
    bool flag = property != "";
    object obj1;
    if (flag)
    {
      // ISSUE: reference to a compiler-generated field
      if (BlockWriter.\u003C\u003Eo__17.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        BlockWriter.\u003C\u003Eo__17.\u003C\u003Ep__1 = CallSite<Func<CallSite, bool, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.And, typeof (BlockWriter), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, bool, object, object> target2 = BlockWriter.\u003C\u003Eo__17.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, bool, object, object>> p1 = BlockWriter.\u003C\u003Eo__17.\u003C\u003Ep__1;
      int num = flag ? 1 : 0;
      // ISSUE: reference to a compiler-generated field
      if (BlockWriter.\u003C\u003Eo__17.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        BlockWriter.\u003C\u003Eo__17.\u003C\u003Ep__0 = CallSite<Func<CallSite, BlockWriter, object, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, "CheckDefault", (IEnumerable<Type>) null, typeof (BlockWriter), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = BlockWriter.\u003C\u003Eo__17.\u003C\u003Ep__0.Target((CallSite) BlockWriter.\u003C\u003Eo__17.\u003C\u003Ep__0, this, token, property);
      obj1 = target2((CallSite) p1, num != 0, obj2);
    }
    else
      obj1 = (object) flag;
    if (target1((CallSite) p2, obj1))
      return this;
    // ISSUE: reference to a compiler-generated field
    if (BlockWriter.\u003C\u003Eo__17.\u003C\u003Ep__3 == null)
    {
      // ISSUE: reference to a compiler-generated field
      BlockWriter.\u003C\u003Eo__17.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", (IEnumerable<Type>) null, typeof (BlockWriter), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
      {
        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
      }));
    }
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this.Write($"{BlockWriter.\u003C\u003Eo__17.\u003C\u003Ep__3.Target((CallSite) BlockWriter.\u003C\u003Eo__17.\u003C\u003Ep__3, token)} ");
    return this;
  }

  public BlockWriter Integer(int integer, string property = "")
  {
    if (property != "" && this.CheckDefault((object) integer, property))
      return this;
    this.Write($"{integer} ");
    return this;
  }

  public BlockWriter Literal(string literal, string property = "")
  {
    if (property != "" && this.CheckDefault((object) literal, property))
      return this;
    this.Write($"\"{literal.Replace("\\", "\\\\").Replace("\"", "\\\"")}\" ");
    return this;
  }

  public BlockWriter Float(float floatVar, string property = "")
  {
    if (property != "" && this.CheckDefault((object) floatVar, property))
      return this;
    this.Write($"{floatVar} ");
    return this;
  }

  public BlockWriter Arrow()
  {
    this.Write("-> ");
    return this;
  }

  public BlockWriter Label(string label)
  {
    if (this.CheckDefault((object) label, nameof (Label)))
      return this;
    this.Write($"#{label.Replace(" ", "_")} ");
    return this;
  }

  public BlockWriter Boolean(bool boolean, string property)
  {
    if (property != "" && this.CheckDefault((object) boolean, property))
      return this;
    this.Write($"{property}={boolean.ToString().ToUpper()} ");
    return this;
  }

  public BlockWriter Indent(int spacing = 1)
  {
    if (this.Indented)
    {
      this.WriteLine();
      if (this.Disabled)
        this.Write("!");
      for (int index = 0; index < spacing; ++index)
        this.Write("  ");
    }
    return this;
  }

  public BlockWriter Return()
  {
    this.WriteLine();
    return this;
  }

  public bool CheckDefault(object value, string property)
  {
    object obj = this.Type.GetProperty(property).GetValue(this.Block);
    return value.Equals(obj);
  }
}
