// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Generation.Snippets
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Generation;

public sealed class Snippets
{
  public static readonly Snippets Shared = new Snippets();
  private int _methodNameIndex;
  private AssemblyGen _assembly;
  private AssemblyGen _debugAssembly;
  private string _snippetsDirectory;
  private bool _saveSnippets;
  private static readonly Type[] _DelegateCtorSignature = new Type[2]
  {
    typeof (object),
    typeof (IntPtr)
  };

  private Snippets()
  {
  }

  public string SnippetsDirectory => this._snippetsDirectory;

  public bool SaveSnippets => this._saveSnippets;

  private AssemblyGen GetAssembly(bool emitSymbols)
  {
    return !emitSymbols ? this.GetOrCreateAssembly(emitSymbols, ref this._assembly) : this.GetOrCreateAssembly(emitSymbols, ref this._debugAssembly);
  }

  private AssemblyGen GetOrCreateAssembly(bool emitSymbols, ref AssemblyGen assembly)
  {
    if (assembly == null)
    {
      string nameSuffix = (emitSymbols ? ".debug" : "") + ".scripting";
      Interlocked.CompareExchange<AssemblyGen>(ref assembly, this.CreateNewAssembly(nameSuffix, emitSymbols), (AssemblyGen) null);
    }
    return assembly;
  }

  private AssemblyGen CreateNewAssembly(string nameSuffix, bool emitSymbols)
  {
    string outDir = (string) null;
    if (this._saveSnippets)
      outDir = this._snippetsDirectory ?? Directory.GetCurrentDirectory();
    return new AssemblyGen(new AssemblyName(nameof (Snippets) + nameSuffix), outDir, ".dll", emitSymbols);
  }

  public static void SetSaveAssemblies(bool enable, string directory)
  {
    Snippets.Shared.ConfigureSaveAssemblies(enable, directory);
  }

  private void ConfigureSaveAssemblies(bool enable, string directory)
  {
    this._saveSnippets = enable;
    this._snippetsDirectory = directory;
  }

  public static void SaveAndVerifyAssemblies()
  {
    if (!Snippets.Shared.SaveSnippets)
      return;
    Type type = typeof (Expression).Assembly.GetType(typeof (Expression).Namespace + ".Compiler.AssemblyGen");
    string[] strArray1 = (string[]) null;
    if (type != (Type) null)
    {
      MethodInfo method = type.GetMethod("SaveAssembliesToDisk", BindingFlags.Static | BindingFlags.NonPublic);
      if (method != (MethodInfo) null)
        strArray1 = (string[]) method.Invoke((object) null, (object[]) null);
    }
    string[] strArray2 = Snippets.Shared.SaveAssemblies();
    if (strArray1 != null)
    {
      foreach (string fileLocation in strArray1)
        AssemblyGen.PeVerifyAssemblyFile(fileLocation);
    }
    foreach (string fileLocation in strArray2)
      AssemblyGen.PeVerifyAssemblyFile(fileLocation);
  }

  private string[] SaveAssemblies()
  {
    if (!this.SaveSnippets)
      return new string[0];
    List<string> stringList = new List<string>();
    if (this._assembly != null)
    {
      string str = this._assembly.SaveAssembly();
      if (str != null)
        stringList.Add(str);
      this._assembly = (AssemblyGen) null;
    }
    if (this._debugAssembly != null)
    {
      string str = this._debugAssembly.SaveAssembly();
      if (str != null)
        stringList.Add(str);
      this._debugAssembly = (AssemblyGen) null;
    }
    return stringList.ToArray();
  }

  public TypeBuilder DefinePublicType(string name, Type parent)
  {
    return this.GetAssembly(false).DefinePublicType(name, parent, false);
  }

  public TypeGen DefineType(string name, Type parent, bool preserveName, bool emitDebugSymbols)
  {
    AssemblyGen assembly = this.GetAssembly(emitDebugSymbols);
    return new TypeGen(assembly, assembly.DefinePublicType(name, parent, preserveName));
  }

  public TypeBuilder DefineDelegateType(string name)
  {
    return this.GetAssembly(false).DefineType(name, typeof (MulticastDelegate), TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.AutoClass, false);
  }

  public Type DefineDelegate(string name, Type returnType, params Type[] argTypes)
  {
    TypeBuilder typeBuilder = this.DefineDelegateType(name);
    typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName, CallingConventions.Standard, Snippets._DelegateCtorSignature).SetImplementationFlags(MethodImplAttributes.CodeTypeMask);
    typeBuilder.DefineMethod("Invoke", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask, returnType, argTypes).SetImplementationFlags(MethodImplAttributes.CodeTypeMask);
    return typeBuilder.CreateType();
  }

  public bool IsSnippetsAssembly(Assembly asm)
  {
    if (this._assembly != null && asm == (Assembly) this._assembly.AssemblyBuilder)
      return true;
    return this._debugAssembly != null && asm == (Assembly) this._debugAssembly.AssemblyBuilder;
  }

  public DynamicILGen CreateDynamicMethod(
    string methodName,
    Type returnType,
    Type[] parameterTypes,
    bool isDebuggable)
  {
    ContractUtils.RequiresNotEmpty(methodName, nameof (methodName));
    ContractUtils.RequiresNotNull((object) returnType, nameof (returnType));
    ContractUtils.RequiresNotNullItems<Type>((IList<Type>) parameterTypes, nameof (parameterTypes));
    if (Snippets.Shared.SaveSnippets)
    {
      TypeBuilder tb = this.GetAssembly(isDebuggable).DefinePublicType(methodName, typeof (object), false);
      MethodBuilder mb = tb.DefineMethod(methodName, CompilerHelpers.PublicStatic, returnType, parameterTypes);
      return (DynamicILGen) new DynamicILGenType(tb, mb, mb.GetILGenerator());
    }
    DynamicMethod dynamicMethod = ReflectionUtils.RawCreateDynamicMethod(methodName, returnType, parameterTypes);
    return (DynamicILGen) new DynamicILGenMethod(dynamicMethod, dynamicMethod.GetILGenerator());
  }

  internal DynamicMethod CreateDynamicMethod(string name, Type returnType, Type[] parameterTypes)
  {
    return ReflectionUtils.RawCreateDynamicMethod($"{name}##{(object) Interlocked.Increment(ref this._methodNameIndex)}", returnType, parameterTypes);
  }
}
