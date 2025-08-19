// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.ZipImportModule
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using IronPython.Zlib;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime;

public static class ZipImportModule
{
  public const string __doc__ = "zipimport provides support for importing Python modules from Zip archives.\r\n\r\nThis module exports three objects:\r\n- zipimporter: a class; its constructor takes a path to a Zip archive.\r\n- ZipImportError: exception raised by zipimporter objects. It's a\r\nsubclass of ImportError, so it can be caught as ImportError, too.\r\n- _zip_directory_cache: a dict, mapping archive paths to zip directory\r\ninfo dicts, as used in zipimporter._files.\r\n\r\nIt is usually not needed to use the zipimport module explicitly; it is\r\nused by the builtin import mechanism for sys.path items that are paths\r\nto Zip archives.";
  private static readonly object _zip_directory_cache_key = new object();

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    if (!context.HasModuleState(ZipImportModule._zip_directory_cache_key))
      context.SetModuleState(ZipImportModule._zip_directory_cache_key, (object) new PythonDictionary());
    dict[(object) "_zip_directory_cache"] = context.GetModuleState(ZipImportModule._zip_directory_cache_key);
    ZipImportModule.InitModuleExceptions(context, dict);
  }

  public static PythonType get_ZipImportError(CodeContext context)
  {
    return (PythonType) context.LanguageContext.GetModuleState((object) "zipimport.ZipImportError");
  }

  internal static Exception MakeError(CodeContext context, params object[] args)
  {
    return PythonOps.CreateThrowable(ZipImportModule.get_ZipImportError(context), args);
  }

  private static void InitModuleExceptions(PythonContext context, PythonDictionary dict)
  {
    context.EnsureModuleException((object) "zipimport.ZipImportError", PythonExceptions.ImportError, typeof (PythonExceptions.BaseException), dict, "ZipImportError", "zipimport", (Func<string, Exception>) (msg => (Exception) new ImportException(msg)));
  }

  [PythonType]
  public class zipimporter
  {
    private const int MAXPATHLEN = 256 /*0x0100*/;
    private string _archive;
    private string _prefix;
    private PythonDictionary __files;
    private static readonly Dictionary<string, ZipImportModule.zipimporter.ModuleCodeType> _search_order = new Dictionary<string, ZipImportModule.zipimporter.ModuleCodeType>()
    {
      {
        Path.DirectorySeparatorChar.ToString() + "__init__.py",
        ZipImportModule.zipimporter.ModuleCodeType.Package
      },
      {
        ".py",
        ZipImportModule.zipimporter.ModuleCodeType.Source
      }
    };

    public zipimporter(CodeContext context, object pathObj, [ParamDictionary] IDictionary<object, object> kwArgs)
    {
      PlatformAdaptationLayer platform = context.LanguageContext.DomainManager.Platform;
      if (pathObj == null)
        throw PythonOps.TypeError("must be string, not None");
      if (!(pathObj is string))
        throw PythonOps.TypeError("must be string, not {0}", (object) pathObj.GetType());
      if (kwArgs.Count > 0)
        throw PythonOps.TypeError("zipimporter() does not take keyword arguments");
      string str1 = pathObj as string;
      if (str1.Length == 0)
        throw ZipImportModule.MakeError(context, (object) "archive path is empty");
      if (str1.Length > 256 /*0x0100*/)
        throw ZipImportModule.MakeError(context, (object) "archive path too long");
      string path = str1.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
      string str2 = path;
      string str3 = string.Empty;
      string empty = string.Empty;
      for (; !string.IsNullOrEmpty(path); path = Path.GetDirectoryName(path))
      {
        if (platform.FileExists(path))
        {
          str3 = path;
          break;
        }
      }
      if (!string.IsNullOrEmpty(str3))
      {
        if (context.LanguageContext.GetModuleState(ZipImportModule._zip_directory_cache_key) is PythonDictionary moduleState && moduleState.ContainsKey((object) str3))
        {
          this._files = moduleState[(object) str3] as PythonDictionary;
        }
        else
        {
          this._files = this.ReadDirectory(context, str3);
          moduleState.Add((object) str3, (object) this._files);
        }
        this._prefix = str2.Replace(str3, string.Empty);
        if (!string.IsNullOrEmpty(this._prefix) && !this._prefix.EndsWith(Path.DirectorySeparatorChar.ToString()))
        {
          this._prefix = this._prefix.Substring(1);
          this._prefix += Path.DirectorySeparatorChar.ToString();
        }
        this._archive = str3;
      }
      else
        throw ZipImportModule.MakeError(context, (object) "not a Zip file");
    }

    public PythonDictionary _files
    {
      get => this.__files;
      private set => this.__files = value;
    }

    public string archive => this._archive;

    public string prefix => this._prefix;

    public string __repr__()
    {
      string str1 = string.IsNullOrEmpty(this._archive) ? "???" : this._archive;
      string str2 = string.IsNullOrEmpty(this._prefix) ? string.Empty : this._prefix;
      string empty = string.Empty;
      return string.IsNullOrEmpty(str2) ? $"<zipimporter object \"{str1}\">" : $"<zipimporter object \"{str1}{Path.DirectorySeparatorChar}{str2}\">";
    }

    [Documentation("find_module(fullname, path=None) -> self or None.\r\n\r\nSearch for a module specified by 'fullname'. 'fullname' must be the\r\nfully qualified (dotted) module name. It returns the zipimporter\r\ninstance itself if the module was found, or None if it wasn't.\r\nThe optional 'path' argument is ignored -- it's there for compatibility\r\nwith the importer protocol.")]
    public object find_module(CodeContext context, string fullname, params object[] args)
    {
      switch (this.GetModuleInfo(context, fullname))
      {
        case ZipImportModule.zipimporter.ModuleStatus.Error:
        case ZipImportModule.zipimporter.ModuleStatus.NotFound:
          return (object) null;
        default:
          return (object) this;
      }
    }

    [Documentation("load_module(fullname) -> module.\r\n\r\nLoad the module specified by 'fullname'. 'fullname' must be the\r\nfully qualified (dotted) module name. It returns the imported\r\nmodule, or raises ZipImportError if it wasn't found.")]
    public object load_module(CodeContext context, string fullname)
    {
      PythonContext languageContext = context.LanguageContext;
      ScriptCode scriptCode = (ScriptCode) null;
      bool ispackage;
      string modpath;
      byte[] moduleCode = this.GetModuleCode(context, fullname, out ispackage, out modpath);
      if (moduleCode == null)
        return (object) null;
      PythonModule pythonModule = languageContext.CompileModule(modpath, fullname, new SourceUnit((LanguageContext) languageContext, (TextContentProvider) new ZipImportModule.MemoryStreamContentProvider(languageContext, moduleCode, modpath), modpath, SourceCodeKind.File), ModuleOptions.None, out scriptCode);
      PythonDictionary dict = pythonModule.__dict__;
      dict.Add((object) "__name__", (object) fullname);
      dict.Add((object) "__loader__", (object) this);
      dict.Add((object) "__package__", (object) null);
      if (ispackage)
      {
        string subName = this.GetSubName(fullname);
        List list = PythonOps.MakeList((object) $"{this._archive}{Path.DirectorySeparatorChar}{(this._prefix.Length > 0 ? (object) this._prefix : (object) string.Empty)}{subName}");
        dict.Add((object) "__path__", (object) list);
      }
      scriptCode.Run(pythonModule.Scope);
      return (object) pythonModule;
    }

    [Documentation("get_filename(fullname) -> filename string.\r\n\r\nReturn the filename for the specified module.")]
    public string get_filename(CodeContext context, string fullname)
    {
      string modpath;
      return this.GetModuleCode(context, fullname, out bool _, out modpath) == null ? (string) null : modpath;
    }

    [Documentation("is_package(fullname) -> bool.\r\n\r\nReturn True if the module specified by fullname is a package.\r\nRaise ZipImportError if the module couldn't be found.")]
    public bool is_package(CodeContext context, string fullname)
    {
      ZipImportModule.zipimporter.ModuleStatus moduleInfo = this.GetModuleInfo(context, fullname);
      if (moduleInfo == ZipImportModule.zipimporter.ModuleStatus.NotFound)
        throw ZipImportModule.MakeError(context, (object) "can't find module '{0}'", (object) fullname);
      return moduleInfo == ZipImportModule.zipimporter.ModuleStatus.Package;
    }

    [Documentation("get_data(pathname) -> string with file data.\r\n\r\nReturn the data associated with 'pathname'. Raise IOError if\r\nthe file wasn't found.")]
    public string get_data(CodeContext context, string path)
    {
      if (path.Length >= 256 /*0x0100*/)
        throw ZipImportModule.MakeError(context, (object) "path too long");
      path = path.Replace(this._archive, string.Empty).TrimStart(Path.DirectorySeparatorChar);
      if (!this.__files.ContainsKey((object) path))
        throw PythonOps.IOError(path);
      byte[] data = this.GetData(context, this._archive, this.__files[(object) path] as PythonTuple);
      return PythonAsciiEncoding.Instance.GetString(data, 0, data.Length);
    }

    [Documentation("get_code(fullname) -> code object.\r\n\r\nReturn the code object for the specified module. Raise ZipImportError\r\nif the module couldn't be found.")]
    public string get_code(CodeContext context, string fullname) => string.Empty;

    [Documentation("get_source(fullname) -> source string.\r\n\r\nReturn the source code for the specified module. Raise ZipImportError\r\nif the module couldn't be found, return None if the archive does\r\ncontain the module, but has no source for it.")]
    public string get_source(CodeContext context, string fullname)
    {
      ZipImportModule.zipimporter.ModuleStatus moduleInfo = this.GetModuleInfo(context, fullname);
      string source = (string) null;
      PythonContext languageContext = context.LanguageContext;
      if (moduleInfo == ZipImportModule.zipimporter.ModuleStatus.Error)
        return (string) null;
      if (moduleInfo == ZipImportModule.zipimporter.ModuleStatus.NotFound)
        throw ZipImportModule.MakeError(context, (object) "can't find module '{0}'", (object) fullname);
      string subName = this.GetSubName(fullname);
      string str = this.MakeFilename(context, this._prefix, subName);
      if (string.IsNullOrEmpty(str))
        return (string) null;
      string key = moduleInfo != ZipImportModule.zipimporter.ModuleStatus.Package ? str + ".py" : $"{str}{Path.DirectorySeparatorChar.ToString()}__init__.py";
      if (this.__files.ContainsKey((object) key))
      {
        byte[] data = this.GetData(context, this._archive, this.__files[(object) key] as PythonTuple);
        source = languageContext.DefaultEncoding.GetString(data, 0, data.Length);
      }
      return source;
    }

    private byte[] GetModuleCode(
      CodeContext context,
      string fullname,
      out bool ispackage,
      out string modpath)
    {
      string subName = this.GetSubName(fullname);
      string str = this.MakeFilename(context, this._prefix, subName);
      ispackage = false;
      modpath = string.Empty;
      if (string.IsNullOrEmpty(str))
        return (byte[]) null;
      foreach (KeyValuePair<string, ZipImportModule.zipimporter.ModuleCodeType> keyValuePair in ZipImportModule.zipimporter._search_order)
      {
        string key = str + keyValuePair.Key;
        if (this.__files.ContainsKey((object) key))
        {
          PythonTuple file = (PythonTuple) this.__files[(object) key];
          ispackage = (keyValuePair.Value & ZipImportModule.zipimporter.ModuleCodeType.Package) == ZipImportModule.zipimporter.ModuleCodeType.Package;
          int num = (int) keyValuePair.Value;
          byte[] codeFromData = this.GetCodeFromData(context, ispackage, false, 0, file);
          if (codeFromData != null)
          {
            modpath = (string) file[0];
            return codeFromData;
          }
        }
      }
      throw ZipImportModule.MakeError(context, (object) "can't find module '{0}'", (object) fullname);
    }

    private byte[] GetData(CodeContext context, string archive, PythonTuple toc_entry)
    {
      int offset1 = (int) toc_entry[4];
      int index = (int) toc_entry[2];
      int num1 = (int) toc_entry[1];
      BinaryReader binaryReader = (BinaryReader) null;
      try
      {
        try
        {
          binaryReader = new BinaryReader((Stream) new FileStream(archive, FileMode.Open, FileAccess.Read));
        }
        catch
        {
          throw PythonOps.IOError("zipimport: can not open file {0}", (object) archive);
        }
        binaryReader.BaseStream.Seek((long) offset1, SeekOrigin.Begin);
        if (binaryReader.ReadInt32() != 67324752)
          throw ZipImportModule.MakeError(context, (object) "bad local file header in {0}", (object) archive);
        binaryReader.BaseStream.Seek((long) (offset1 + 26), SeekOrigin.Begin);
        int num2 = 30 + (int) binaryReader.ReadInt16() + (int) binaryReader.ReadInt16();
        int offset2 = offset1 + num2;
        binaryReader.BaseStream.Seek((long) offset2, SeekOrigin.Begin);
        byte[] input;
        try
        {
          input = binaryReader.ReadBytes(num1 == 0 ? index : index + 1);
        }
        catch
        {
          throw PythonOps.IOError("zipimport: can't read data");
        }
        if (num1 != 0)
        {
          input[index] = (byte) 90;
          int num3 = index + 1;
        }
        return num1 == 0 ? input : ZlibModule.Decompress(input, -15);
      }
      finally
      {
        binaryReader?.Dispose();
      }
    }

    private byte[] GetCodeFromData(
      CodeContext context,
      bool ispackage,
      bool isbytecode,
      int mtime,
      PythonTuple toc_entry)
    {
      byte[] data = this.GetData(context, this._archive, toc_entry);
      string str = (string) toc_entry[0];
      byte[] codeFromData = (byte[]) null;
      if (data != null && !isbytecode)
        codeFromData = data;
      return codeFromData;
    }

    private PythonDictionary ReadDirectory(CodeContext context, string archive)
    {
      string empty1 = string.Empty;
      BinaryReader binaryReader = (BinaryReader) null;
      PythonDictionary pythonDictionary = (PythonDictionary) null;
      byte[] buffer = new byte[22];
      string str1 = archive.Length <= 256 /*0x0100*/ ? archive : throw PythonOps.OverflowError("Zip path name is too long");
      try
      {
        try
        {
          binaryReader = new BinaryReader((Stream) new FileStream(archive, FileMode.Open, FileAccess.Read));
        }
        catch
        {
          throw ZipImportModule.MakeError(context, (object) "can't open Zip file: '{0}'", (object) archive);
        }
        if (binaryReader.BaseStream.Length < 22L)
          throw ZipImportModule.MakeError(context, (object) "can't read Zip file: '{0}'", (object) archive);
        binaryReader.BaseStream.Seek(-22L, SeekOrigin.End);
        int position = (int) binaryReader.BaseStream.Position;
        if (binaryReader.Read(buffer, 0, 22) != 22)
          throw ZipImportModule.MakeError(context, (object) "can't read Zip file: '{0}'", (object) archive);
        if (BitConverter.ToUInt32(buffer, 0) != 101010256U)
        {
          binaryReader.Dispose();
          throw ZipImportModule.MakeError(context, (object) "not a Zip file: '{0}'", (object) archive);
        }
        int int32_1 = BitConverter.ToInt32(buffer, 12);
        int int32_2 = BitConverter.ToInt32(buffer, 16 /*0x10*/);
        int num1 = position - int32_2 - int32_1;
        int offset = int32_2 + num1;
        pythonDictionary = new PythonDictionary();
        string str2 = str1 + Path.DirectorySeparatorChar.ToString();
        int num2 = 0;
        while (true)
        {
          string empty2 = string.Empty;
          binaryReader.BaseStream.Seek((long) offset, SeekOrigin.Begin);
          if (binaryReader.ReadInt32() == 33639248)
          {
            binaryReader.BaseStream.Seek((long) (offset + 10), SeekOrigin.Begin);
            int num3 = (int) binaryReader.ReadInt16();
            int num4 = (int) binaryReader.ReadInt16();
            int num5 = (int) binaryReader.ReadInt16();
            int num6 = binaryReader.ReadInt32();
            int num7 = binaryReader.ReadInt32();
            int num8 = binaryReader.ReadInt32();
            int num9 = (int) binaryReader.ReadInt16();
            int num10 = 46 + num9 + (int) binaryReader.ReadInt16() + (int) binaryReader.ReadInt16();
            binaryReader.BaseStream.Seek((long) (offset + 42), SeekOrigin.Begin);
            int num11 = binaryReader.ReadInt32() + num1;
            if (num9 > 256 /*0x0100*/)
              num9 = 256 /*0x0100*/;
            for (int index = 0; index < num9; ++index)
            {
              char ch = binaryReader.ReadChar();
              if (ch == '/')
                ch = Path.DirectorySeparatorChar;
              empty2 += ch.ToString();
            }
            offset += num10;
            PythonTuple pythonTuple = PythonOps.MakeTuple((object) (str2 + empty2), (object) num3, (object) num7, (object) num8, (object) num11, (object) num4, (object) num5, (object) num6);
            pythonDictionary.Add((object) empty2, (object) pythonTuple);
            ++num2;
          }
          else
            break;
        }
      }
      finally
      {
        binaryReader?.Dispose();
      }
      return pythonDictionary;
    }

    private string GetSubName(string fullname)
    {
      string[] strArray = fullname.Split(new char[1]{ '.' }, StringSplitOptions.RemoveEmptyEntries);
      return strArray[strArray.Length - 1];
    }

    private string MakeFilename(CodeContext context, string prefix, string name)
    {
      if (prefix.Length + name.Length + 13 >= 256 /*0x0100*/)
        throw ZipImportModule.MakeError(context, (object) "path to long");
      return (prefix + name).Replace('.', Path.DirectorySeparatorChar);
    }

    private ZipImportModule.zipimporter.ModuleStatus GetModuleInfo(
      CodeContext context,
      string fullname)
    {
      string subName = this.GetSubName(fullname);
      string str = this.MakeFilename(context, this._prefix, subName);
      if (string.IsNullOrEmpty(str))
        return ZipImportModule.zipimporter.ModuleStatus.Error;
      foreach (KeyValuePair<string, ZipImportModule.zipimporter.ModuleCodeType> keyValuePair in ZipImportModule.zipimporter._search_order)
      {
        if (this._files.ContainsKey((object) (str + keyValuePair.Key)))
          return (keyValuePair.Value & ZipImportModule.zipimporter.ModuleCodeType.Package) == ZipImportModule.zipimporter.ModuleCodeType.Package ? ZipImportModule.zipimporter.ModuleStatus.Package : ZipImportModule.zipimporter.ModuleStatus.Module;
      }
      return ZipImportModule.zipimporter.ModuleStatus.NotFound;
    }

    [Flags]
    private enum ModuleCodeType
    {
      Source = 0,
      ByteCode = 1,
      Package = 2,
    }

    private enum ModuleStatus
    {
      Error,
      NotFound,
      Module,
      Package,
    }
  }

  [Serializable]
  internal sealed class MemoryStreamContentProvider : TextContentProvider
  {
    private readonly PythonContext _context;
    private readonly MemoryStream _stream;
    private readonly string _path;

    internal MemoryStreamContentProvider(PythonContext context, byte[] data, string path)
    {
      ContractUtils.RequiresNotNull((object) context, nameof (context));
      ContractUtils.RequiresNotNull((object) data, nameof (data));
      this._context = context;
      this._stream = new MemoryStream(data);
      this._path = path;
    }

    public override SourceCodeReader GetReader()
    {
      return this._context.GetSourceReader((Stream) this._stream, this._context.DefaultEncoding, this._path);
    }
  }
}
