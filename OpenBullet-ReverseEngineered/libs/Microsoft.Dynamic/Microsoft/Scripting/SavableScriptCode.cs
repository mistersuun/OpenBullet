// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.SavableScriptCode
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

#nullable disable
namespace Microsoft.Scripting;

public abstract class SavableScriptCode(SourceUnit sourceUnit) : ScriptCode(sourceUnit)
{
  public static void SaveToAssembly(
    string assemblyName,
    IDictionary<string, object> assemblyAttributes,
    params SavableScriptCode[] codes)
  {
    ContractUtils.RequiresNotNull((object) assemblyName, nameof (assemblyName));
    ContractUtils.RequiresNotNullItems<SavableScriptCode>((IList<SavableScriptCode>) codes, nameof (codes));
    string outDir = Path.GetDirectoryName(assemblyName);
    if (string.IsNullOrEmpty(outDir))
      outDir = Environment.CurrentDirectory;
    string withoutExtension = Path.GetFileNameWithoutExtension(assemblyName);
    string extension = Path.GetExtension(assemblyName);
    AssemblyGen myAssembly = new AssemblyGen(new AssemblyName(withoutExtension), outDir, extension, false, assemblyAttributes);
    TypeBuilder myType = myAssembly.DefinePublicType("DLRCachedCode", typeof (object), true);
    TypeGen typeGen = new TypeGen(myAssembly, myType);
    Dictionary<Type, List<SavableScriptCode.CodeInfo>> source = new Dictionary<Type, List<SavableScriptCode.CodeInfo>>();
    foreach (SavableScriptCode code in codes)
    {
      List<SavableScriptCode.CodeInfo> codeInfoList;
      if (!source.TryGetValue(code.LanguageContext.GetType(), out codeInfoList))
        source[code.LanguageContext.GetType()] = codeInfoList = new List<SavableScriptCode.CodeInfo>();
      KeyValuePair<MethodBuilder, Type> keyValuePair = code.CompileForSave(typeGen);
      codeInfoList.Add(new SavableScriptCode.CodeInfo(keyValuePair.Key, (ScriptCode) code, keyValuePair.Value));
    }
    MethodBuilder methodBuilder = myType.DefineMethod("GetScriptCodeInfo", MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.SpecialName, typeof (MutableTuple<Type[], Delegate[][], string[][], string[][]>), ReflectionUtils.EmptyTypes);
    ILGen ilgen = new ILGen(methodBuilder.GetILGenerator());
    KeyValuePair<Type, List<SavableScriptCode.CodeInfo>>[] langsWithBuilders = source.ToArray<KeyValuePair<Type, List<SavableScriptCode.CodeInfo>>>();
    ilgen.EmitArray(typeof (Type), langsWithBuilders.Length, (EmitArrayHelper) (index =>
    {
      ilgen.Emit(OpCodes.Ldtoken, langsWithBuilders[index].Key);
      ilgen.EmitCall(typeof (Type).GetMethod("GetTypeFromHandle", new Type[1]
      {
        typeof (RuntimeTypeHandle)
      }));
    }));
    ilgen.EmitArray(typeof (Delegate[]), langsWithBuilders.Length, (EmitArrayHelper) (index =>
    {
      List<SavableScriptCode.CodeInfo> builders = langsWithBuilders[index].Value;
      ilgen.EmitArray(typeof (Delegate), builders.Count, (EmitArrayHelper) (innerIndex =>
      {
        ilgen.EmitNull();
        ilgen.Emit(OpCodes.Ldftn, (MethodInfo) builders[innerIndex].Builder);
        ilgen.EmitNew(builders[innerIndex].DelegateType, new Type[2]
        {
          typeof (object),
          typeof (IntPtr)
        });
      }));
    }));
    ilgen.EmitArray(typeof (string[]), langsWithBuilders.Length, (EmitArrayHelper) (index =>
    {
      List<SavableScriptCode.CodeInfo> builders = langsWithBuilders[index].Value;
      ilgen.EmitArray(typeof (string), builders.Count, (EmitArrayHelper) (innerIndex => ilgen.EmitString(builders[innerIndex].Code.SourceUnit.Path)));
    }));
    ilgen.EmitArray(typeof (string[]), langsWithBuilders.Length, (EmitArrayHelper) (index =>
    {
      List<SavableScriptCode.CodeInfo> builders = langsWithBuilders[index].Value;
      ilgen.EmitArray(typeof (string), builders.Count, (EmitArrayHelper) (innerIndex =>
      {
        if (builders[innerIndex].Code is ICustomScriptCodeData code2)
          ilgen.EmitString(code2.GetCustomScriptCodeData());
        else
          ilgen.Emit(OpCodes.Ldnull);
      }));
    }));
    ilgen.EmitNew(typeof (MutableTuple<Type[], Delegate[][], string[][], string[][]>), new Type[4]
    {
      typeof (Type[]),
      typeof (Delegate[][]),
      typeof (string[][]),
      typeof (string[][])
    });
    ilgen.Emit(OpCodes.Ret);
    methodBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof (DlrCachedCodeAttribute).GetConstructor(ReflectionUtils.EmptyTypes), ArrayUtils.EmptyObjects));
    typeGen.FinishType();
    myAssembly.SaveAssembly();
  }

  public static void SaveToAssembly(string assemblyName, params SavableScriptCode[] codes)
  {
    SavableScriptCode.SaveToAssembly(assemblyName, (IDictionary<string, object>) null, codes);
  }

  public static ScriptCode[] LoadFromAssembly(ScriptDomainManager runtime, Assembly assembly)
  {
    ContractUtils.RequiresNotNull((object) runtime, nameof (runtime));
    ContractUtils.RequiresNotNull((object) assembly, nameof (assembly));
    Type type = assembly.GetType("DLRCachedCode");
    if (type == (Type) null)
      return new ScriptCode[0];
    List<ScriptCode> scriptCodeList = new List<ScriptCode>();
    MethodInfo method = type.GetMethod("GetScriptCodeInfo");
    if (method.IsSpecialName && method.IsDefined(typeof (DlrCachedCodeAttribute), false))
    {
      MutableTuple<Type[], Delegate[][], string[][], string[][]> mutableTuple = (MutableTuple<Type[], Delegate[][], string[][], string[][]>) method.Invoke((object) null, ArrayUtils.EmptyObjects);
      for (int index1 = 0; index1 < mutableTuple.Item000.Length; ++index1)
      {
        Type providerType = mutableTuple.Item000[index1];
        LanguageContext language = runtime.GetLanguage(providerType);
        Delegate[] delegateArray = mutableTuple.Item001[index1];
        string[] strArray1 = mutableTuple.Item002[index1];
        string[] strArray2 = mutableTuple.Item003[index1];
        for (int index2 = 0; index2 < delegateArray.Length; ++index2)
          scriptCodeList.Add(language.LoadCompiledCode(delegateArray[index2], strArray1[index2], strArray2[index2]));
      }
    }
    return scriptCodeList.ToArray();
  }

  protected LambdaExpression RewriteForSave(TypeGen typeGen, LambdaExpression code)
  {
    return new ToDiskRewriter(typeGen).RewriteLambda(code);
  }

  protected virtual KeyValuePair<MethodBuilder, Type> CompileForSave(TypeGen typeGen)
  {
    throw new NotSupportedException();
  }

  public override string ToString()
  {
    return $"ScriptCode '{this.SourceUnit.Path}' from {this.LanguageContext.GetType().Name}";
  }

  private class CodeInfo
  {
    public readonly MethodBuilder Builder;
    public readonly ScriptCode Code;
    public readonly Type DelegateType;

    public CodeInfo(MethodBuilder builder, ScriptCode code, Type delegateType)
    {
      this.Builder = builder;
      this.Code = code;
      this.DelegateType = delegateType;
    }
  }
}
