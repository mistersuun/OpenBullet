// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Generation.AssemblyGen
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using System.Text;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Generation;

public sealed class AssemblyGen
{
  private readonly AssemblyBuilder _myAssembly;
  private readonly ModuleBuilder _myModule;
  private readonly bool _isDebuggable;
  private readonly string _outFileName;
  private readonly string _outDir;
  private const string peverify_exe = "peverify.exe";
  private int _index;
  private const MethodAttributes CtorAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName;
  private const MethodImplAttributes ImplAttributes = MethodImplAttributes.CodeTypeMask;
  private const MethodAttributes InvokeAttributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask;
  private const TypeAttributes DelegateAttributes = TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.AutoClass;
  private static readonly Type[] _DelegateCtorSignature = new Type[2]
  {
    typeof (object),
    typeof (IntPtr)
  };

  internal bool IsDebuggable => this._isDebuggable;

  public AssemblyGen(
    AssemblyName name,
    string outDir,
    string outFileExtension,
    bool isDebuggable,
    IDictionary<string, object> attrs = null)
  {
    ContractUtils.RequiresNotNull((object) name, nameof (name));
    if (outFileExtension == null)
      outFileExtension = ".dll";
    if (outDir != null)
    {
      try
      {
        outDir = Path.GetFullPath(outDir);
      }
      catch (Exception ex)
      {
        throw Error.InvalidOutputDir();
      }
      try
      {
        Path.Combine(outDir, name.Name + outFileExtension);
      }
      catch (ArgumentException ex)
      {
        throw Error.InvalidAsmNameOrExtension();
      }
      this._outFileName = name.Name + outFileExtension;
      this._outDir = outDir;
    }
    List<CustomAttributeBuilder> assemblyAttributes = new List<CustomAttributeBuilder>()
    {
      new CustomAttributeBuilder(typeof (SecurityTransparentAttribute).GetConstructor(ReflectionUtils.EmptyTypes), new object[0]),
      new CustomAttributeBuilder(typeof (SecurityRulesAttribute).GetConstructor(new Type[1]
      {
        typeof (SecurityRuleSet)
      }), new object[1]{ (object) SecurityRuleSet.Level1 })
    };
    if (attrs != null)
    {
      foreach (KeyValuePair<string, object> attr in (IEnumerable<KeyValuePair<string, object>>) attrs)
      {
        if (attr.Value is string str && !string.IsNullOrWhiteSpace(str))
        {
          ConstructorInfo con = (ConstructorInfo) null;
          switch (attr.Key)
          {
            case "assemblyFileVersion":
              con = typeof (AssemblyFileVersionAttribute).GetConstructor(new Type[1]
              {
                typeof (string)
              });
              break;
            case "copyright":
              con = typeof (AssemblyCopyrightAttribute).GetConstructor(new Type[1]
              {
                typeof (string)
              });
              break;
            case "productName":
              con = typeof (AssemblyProductAttribute).GetConstructor(new Type[1]
              {
                typeof (string)
              });
              break;
            case "productVersion":
              con = typeof (AssemblyInformationalVersionAttribute).GetConstructor(new Type[1]
              {
                typeof (string)
              });
              break;
          }
          if (con != (ConstructorInfo) null)
            assemblyAttributes.Add(new CustomAttributeBuilder(con, new object[1]
            {
              (object) str
            }));
        }
      }
    }
    if (outDir != null)
    {
      this._myAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndSave, outDir, false, (IEnumerable<CustomAttributeBuilder>) assemblyAttributes);
      this._myModule = this._myAssembly.DefineDynamicModule(name.Name, this._outFileName, isDebuggable);
    }
    else
    {
      this._myAssembly = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run, (IEnumerable<CustomAttributeBuilder>) assemblyAttributes);
      this._myModule = this._myAssembly.DefineDynamicModule(name.Name, isDebuggable);
    }
    this._myAssembly.DefineVersionInfoResource();
    this._isDebuggable = isDebuggable;
    if (!isDebuggable)
      return;
    this.SetDebuggableAttributes();
  }

  internal void SetDebuggableAttributes()
  {
    DebuggableAttribute.DebuggingModes debuggingModes = DebuggableAttribute.DebuggingModes.Default | DebuggableAttribute.DebuggingModes.DisableOptimizations | DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints;
    Type[] types = new Type[1]
    {
      typeof (DebuggableAttribute.DebuggingModes)
    };
    object[] constructorArgs = new object[1]
    {
      (object) debuggingModes
    };
    ConstructorInfo constructor = typeof (DebuggableAttribute).GetConstructor(types);
    this._myAssembly.SetCustomAttribute(new CustomAttributeBuilder(constructor, constructorArgs));
    this._myModule.SetCustomAttribute(new CustomAttributeBuilder(constructor, constructorArgs));
  }

  public string SaveAssembly()
  {
    this._myAssembly.Save(this._outFileName, PortableExecutableKinds.ILOnly, ImageFileMachine.I386);
    return Path.Combine(this._outDir, this._outFileName);
  }

  internal void Verify() => this.PeVerifyThis();

  internal static void PeVerifyAssemblyFile(string fileLocation)
  {
    string directoryName1 = Path.GetDirectoryName(fileLocation);
    string fileName = Path.GetFileName(fileLocation);
    string peverify = AssemblyGen.FindPeverify();
    if (peverify == null)
      return;
    string strOut = (string) null;
    string str = (string) null;
    int num1;
    try
    {
      string directoryName2 = new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName;
      string lower = Path.Combine(directoryName1, fileName).ToLower(CultureInfo.InvariantCulture);
      string withoutExtension = Path.GetFileNameWithoutExtension(fileName);
      string extension = Path.GetExtension(fileName);
      Random random = new Random();
      int num2 = 0;
      while (true)
      {
        string path2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}_{2}{3}", (object) withoutExtension, (object) num2, (object) random.Next(1, 100), (object) extension);
        string destFileName = Path.Combine(Path.GetTempPath(), path2);
        try
        {
          File.Copy(lower, destFileName);
          str = destFileName;
          break;
        }
        catch (IOException ex)
        {
        }
        ++num2;
      }
      AssemblyGen.CopyFilesCreatedSinceStart(Path.GetTempPath(), Environment.CurrentDirectory, fileName);
      AssemblyGen.CopyDirectory(Path.GetTempPath(), directoryName2);
      if (Snippets.Shared.SnippetsDirectory != null && Snippets.Shared.SnippetsDirectory != Path.GetTempPath())
        AssemblyGen.CopyFilesCreatedSinceStart(Path.GetTempPath(), Snippets.Shared.SnippetsDirectory, fileName);
      Process proc = Process.Start(new ProcessStartInfo(peverify, $"/IGNORE=80070002 \"{str}\"")
      {
        UseShellExecute = false,
        RedirectStandardOutput = true
      });
      Thread thread = new Thread((ThreadStart) (() =>
      {
        using (StreamReader standardOutput = proc.StandardOutput)
          strOut = standardOutput.ReadToEnd();
      }));
      thread.Start();
      proc.WaitForExit();
      thread.Join();
      num1 = proc.ExitCode;
      proc.Close();
    }
    catch (Exception ex)
    {
      strOut = "Unexpected exception: " + (object) ex;
      num1 = 1;
    }
    if (num1 != 0)
    {
      Console.WriteLine("Verification failed w/ exit code {0}: {1}", (object) num1, (object) strOut);
      throw Error.VerificationException((object) fileName, (object) str, (object) (strOut ?? ""));
    }
    if (str == null)
      return;
    File.Delete(str);
  }

  internal static string FindPeverify()
  {
    string environmentVariable = Environment.GetEnvironmentVariable("PATH");
    char[] chArray = new char[1]{ ';' };
    foreach (string path1 in environmentVariable.Split(chArray))
    {
      string path = Path.Combine(path1, "peverify.exe");
      if (File.Exists(path))
        return path;
    }
    return (string) null;
  }

  private void PeVerifyThis()
  {
    AssemblyGen.PeVerifyAssemblyFile(Path.Combine(this._outDir, this._outFileName));
  }

  private static void CopyFilesCreatedSinceStart(string pythonPath, string dir, string outFileName)
  {
    DateTime startTime = Process.GetCurrentProcess().StartTime;
    foreach (string file in Directory.GetFiles(dir))
    {
      FileInfo fileInfo = new FileInfo(file);
      if (fileInfo.Name != outFileName)
      {
        if (fileInfo.LastWriteTime - startTime >= TimeSpan.Zero)
        {
          try
          {
            File.Copy(file, Path.Combine(pythonPath, fileInfo.Name), true);
          }
          catch (Exception ex)
          {
            Console.WriteLine("Error copying {0}: {1}", (object) file, (object) ex.Message);
          }
        }
      }
    }
  }

  private static void CopyDirectory(string to, string from)
  {
    foreach (string file in Directory.GetFiles(from))
    {
      FileInfo fileInfo1 = new FileInfo(file);
      string str = Path.Combine(to, fileInfo1.Name);
      FileInfo fileInfo2 = new FileInfo(str);
      if ((fileInfo1.Extension.ToLowerInvariant() == ".dll" || fileInfo1.Extension.ToLowerInvariant() == ".exe") && (!File.Exists(str) || fileInfo2.CreationTime != fileInfo1.CreationTime))
      {
        try
        {
          File.Copy(file, str, true);
        }
        catch (Exception ex)
        {
          Console.WriteLine("Error copying {0}: {1}", (object) file, (object) ex.Message);
        }
      }
    }
  }

  public TypeBuilder DefinePublicType(string name, Type parent, bool preserveName)
  {
    return this.DefineType(name, parent, TypeAttributes.Public, preserveName);
  }

  internal TypeBuilder DefineType(
    string name,
    Type parent,
    TypeAttributes attr,
    bool preserveName)
  {
    ContractUtils.RequiresNotNull((object) name, nameof (name));
    ContractUtils.RequiresNotNull((object) parent, nameof (parent));
    StringBuilder stringBuilder = new StringBuilder(name);
    if (!preserveName)
    {
      int num = Interlocked.Increment(ref this._index);
      stringBuilder.Append("$");
      stringBuilder.Append(num);
    }
    stringBuilder.Replace('+', '_').Replace('[', '_').Replace(']', '_').Replace('*', '_').Replace('&', '_').Replace(',', '_').Replace('\\', '_');
    name = stringBuilder.ToString();
    return this._myModule.DefineType(name, attr, parent);
  }

  internal void SetEntryPoint(MethodInfo mi, PEFileKinds kind)
  {
    this._myAssembly.SetEntryPoint(mi, kind);
  }

  public AssemblyBuilder AssemblyBuilder => this._myAssembly;

  public ModuleBuilder ModuleBuilder => this._myModule;

  public Type MakeDelegateType(string name, Type[] parameters, Type returnType)
  {
    TypeBuilder typeBuilder = this.DefineType(name, typeof (MulticastDelegate), TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.AutoClass, false);
    typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName, CallingConventions.Standard, AssemblyGen._DelegateCtorSignature).SetImplementationFlags(MethodImplAttributes.CodeTypeMask);
    typeBuilder.DefineMethod("Invoke", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask, returnType, parameters).SetImplementationFlags(MethodImplAttributes.CodeTypeMask);
    return typeBuilder.CreateType();
  }
}
