// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.DelegateUtils
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Utils;

internal static class DelegateUtils
{
  private static AssemblyBuilder _assembly;
  private static ModuleBuilder _modBuilder;
  private static int _typeCount;
  private static readonly Type[] _DelegateCtorSignature = new Type[2]
  {
    typeof (object),
    typeof (IntPtr)
  };
  public const char GenericArityDelimiter = '`';

  private static TypeBuilder DefineDelegateType(string name)
  {
    if ((Assembly) DelegateUtils._assembly == (Assembly) null)
    {
      AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DynamicDelegates"), AssemblyBuilderAccess.Run);
      Interlocked.CompareExchange<AssemblyBuilder>(ref DelegateUtils._assembly, assemblyBuilder, (AssemblyBuilder) null);
      lock (DelegateUtils._assembly)
      {
        if ((Module) DelegateUtils._modBuilder == (Module) null)
          DelegateUtils._modBuilder = DelegateUtils._assembly.DefineDynamicModule("DynamicDelegates");
      }
    }
    return DelegateUtils._modBuilder.DefineType(name + (object) Interlocked.Increment(ref DelegateUtils._typeCount), TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.AutoClass, typeof (MulticastDelegate));
  }

  internal static Type EmitCallSiteDelegateType(int paramCount)
  {
    Type[] parameterTypes = new Type[paramCount + 2];
    parameterTypes[0] = typeof (CallSite);
    parameterTypes[1] = typeof (object);
    for (int index = 0; index < paramCount; ++index)
      parameterTypes[index + 2] = typeof (object);
    TypeBuilder typeBuilder = DelegateUtils.DefineDelegateType("Delegate");
    typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName, CallingConventions.Standard, DelegateUtils._DelegateCtorSignature).SetImplementationFlags(MethodImplAttributes.CodeTypeMask);
    typeBuilder.DefineMethod("Invoke", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask, typeof (object), parameterTypes).SetImplementationFlags(MethodImplAttributes.CodeTypeMask);
    return typeBuilder.CreateType();
  }
}
