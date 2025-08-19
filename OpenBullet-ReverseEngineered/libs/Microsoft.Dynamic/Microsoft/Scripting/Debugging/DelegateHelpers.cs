// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Debugging.DelegateHelpers
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Reflection;
using System.Reflection.Emit;

#nullable disable
namespace Microsoft.Scripting.Debugging;

internal static class DelegateHelpers
{
  private static ModuleBuilder _moduleBuilder;
  private const MethodAttributes CtorAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName;
  private const MethodImplAttributes ImplAttributes = MethodImplAttributes.CodeTypeMask;
  private const MethodAttributes InvokeAttributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask;
  private static readonly Type[] _DelegateCtorSignature = new Type[2]
  {
    typeof (object),
    typeof (IntPtr)
  };

  internal static Type MakeNewCustomDelegateType(Type[] types)
  {
    Type type = types[types.Length - 1];
    Type[] parameterTypes = types.RemoveLast<Type>();
    TypeBuilder typeBuilder = DelegateHelpers.DefineDelegateType("Delegate_" + Guid.NewGuid().ToString());
    typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName, CallingConventions.Standard, DelegateHelpers._DelegateCtorSignature).SetImplementationFlags(MethodImplAttributes.CodeTypeMask);
    typeBuilder.DefineMethod("Invoke", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask, type, parameterTypes).SetImplementationFlags(MethodImplAttributes.CodeTypeMask);
    return typeBuilder.CreateType();
  }

  private static TypeBuilder DefineDelegateType(string name)
  {
    return DelegateHelpers.GetModule().DefineType(name, TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.AutoClass, typeof (MulticastDelegate));
  }

  private static ModuleBuilder GetModule()
  {
    lock (DelegateHelpers._DelegateCtorSignature)
    {
      if ((Module) DelegateHelpers._moduleBuilder == (Module) null)
        DelegateHelpers._moduleBuilder = ReflectionUtils.DefineDynamicAssembly(new AssemblyName("Snippets.Microsoft.Scripting.Debugging"), AssemblyBuilderAccess.Run).DefineDynamicModule("Snippets.Microsoft.Scripting.Debugging", true);
    }
    return DelegateHelpers._moduleBuilder;
  }
}
