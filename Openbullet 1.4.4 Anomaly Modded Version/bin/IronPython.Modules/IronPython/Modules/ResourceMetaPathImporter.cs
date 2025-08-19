// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.ResourceMetaPathImporter
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Zlib;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

#nullable disable
namespace IronPython.Modules;

[PythonType]
public class ResourceMetaPathImporter
{
  private readonly ResourceMetaPathImporter.PackedResourceLoader _loader;
  private readonly IDictionary<string, ResourceMetaPathImporter.PackedResourceInfo> _unpackedLibrary;
  private readonly IDictionary<string, ResourceMetaPathImporter.PackedResourceInfo[]> _unpackedModules;
  private readonly string _unpackingError;
  private static readonly Dictionary<string, ResourceMetaPathImporter.ModuleCodeType> SearchOrder = new Dictionary<string, ResourceMetaPathImporter.ModuleCodeType>()
  {
    {
      Path.DirectorySeparatorChar.ToString() + "__init__.py",
      ResourceMetaPathImporter.ModuleCodeType.Package
    },
    {
      ".py",
      ResourceMetaPathImporter.ModuleCodeType.Source
    }
  };

  public ResourceMetaPathImporter(Assembly fromAssembly, string resourceName)
  {
    this._loader = new ResourceMetaPathImporter.PackedResourceLoader(fromAssembly, resourceName);
    if (this._loader.LoadZipDirectory(out this._unpackedLibrary, out this._unpackedModules, out this._unpackingError))
      return;
    this._unpackedLibrary = (IDictionary<string, ResourceMetaPathImporter.PackedResourceInfo>) new Dictionary<string, ResourceMetaPathImporter.PackedResourceInfo>();
    this._unpackedModules = (IDictionary<string, ResourceMetaPathImporter.PackedResourceInfo[]>) new Dictionary<string, ResourceMetaPathImporter.PackedResourceInfo[]>();
    if (!string.IsNullOrEmpty(this._unpackingError))
      throw ResourceMetaPathImporter.MakeError((object) "meta_path importer initialization error: {0}", (object) this._unpackingError);
  }

  [Documentation("find_module(fullname, path=None) -> self or None.\r\n\r\nSearch for a module specified by 'fullname'. 'fullname' must be the\r\nfully qualified (dotted) module name. It returns the importer\r\ninstance itself if the module was found, or None if it wasn't.\r\nThe optional 'path' argument is ignored -- it's there for compatibility\r\nwith the importer protocol.")]
  public object find_module(CodeContext context, string fullname, params object[] args)
  {
    string str = ResourceMetaPathImporter.MakeFilename(fullname);
    foreach (KeyValuePair<string, ResourceMetaPathImporter.ModuleCodeType> keyValuePair in ResourceMetaPathImporter.SearchOrder)
    {
      if (this._unpackedLibrary.ContainsKey(str + keyValuePair.Key))
        return (object) this;
    }
    return (object) null;
  }

  [Documentation("load_module(fullname) -> module.\r\n\r\nLoad the module specified by 'fullname'. 'fullname' must be the\r\nfully qualified (dotted) module name. It returns the imported\r\nmodule, or raises ResourceImportError if it wasn't found.")]
  public object load_module(CodeContext context, string fullname)
  {
    IDictionary<object, object> systemStateModules = context.LanguageContext.SystemStateModules;
    if (systemStateModules.ContainsKey((object) fullname))
      return systemStateModules[(object) fullname];
    bool ispackage;
    string modpath;
    byte[] moduleCode = this.GetModuleCode(context, fullname, out ispackage, out modpath);
    if (moduleCode == null)
      return (object) null;
    PythonContext languageContext = context.LanguageContext;
    ScriptCode scriptCode;
    PythonModule pythonModule = languageContext.CompileModule(modpath, fullname, new SourceUnit((LanguageContext) languageContext, (TextContentProvider) new ZipImportModule.MemoryStreamContentProvider(languageContext, moduleCode, modpath), modpath, SourceCodeKind.File), ModuleOptions.None, out scriptCode);
    PythonDictionary dict = pythonModule.__dict__;
    dict.Add((object) "__name__", (object) fullname);
    dict.Add((object) "__loader__", (object) this);
    dict.Add((object) "__package__", (object) null);
    dict.Add((object) "__file__", (object) "<resource>");
    if (ispackage)
    {
      IronPython.Runtime.List list = PythonOps.MakeList();
      dict.Add((object) "__path__", (object) list);
    }
    systemStateModules.Add((object) fullname, (object) pythonModule);
    try
    {
      scriptCode.Run(pythonModule.Scope);
    }
    catch (Exception ex)
    {
      systemStateModules.Remove((object) fullname);
      throw;
    }
    return (object) pythonModule;
  }

  private byte[] GetModuleCode(
    CodeContext context,
    string fullname,
    out bool ispackage,
    out string modpath)
  {
    string str = ResourceMetaPathImporter.MakeFilename(fullname);
    ispackage = false;
    modpath = string.Empty;
    if (string.IsNullOrEmpty(str))
      return (byte[]) null;
    foreach (KeyValuePair<string, ResourceMetaPathImporter.ModuleCodeType> keyValuePair in ResourceMetaPathImporter.SearchOrder)
    {
      string key = str + keyValuePair.Key;
      if (this._unpackedLibrary.ContainsKey(key))
      {
        ResourceMetaPathImporter.PackedResourceInfo tocEntry = this._unpackedLibrary[key];
        ispackage = (keyValuePair.Value & ResourceMetaPathImporter.ModuleCodeType.Package) == ResourceMetaPathImporter.ModuleCodeType.Package;
        byte[] codeFromData = this.GetCodeFromData(context, false, tocEntry);
        if (codeFromData != null)
        {
          modpath = tocEntry.FullName;
          return codeFromData;
        }
      }
    }
    throw ResourceMetaPathImporter.MakeError((object) "can't find module '{0}'", (object) fullname);
  }

  private byte[] GetCodeFromData(
    CodeContext context,
    bool isbytecode,
    ResourceMetaPathImporter.PackedResourceInfo tocEntry)
  {
    byte[] data = this.GetData(tocEntry);
    byte[] codeFromData = (byte[]) null;
    if (data != null && !isbytecode)
      codeFromData = data;
    return codeFromData;
  }

  private byte[] GetData(
    ResourceMetaPathImporter.PackedResourceInfo tocEntry)
  {
    byte[] result;
    string unpackingError;
    if (!this._loader.GetData(tocEntry, out result, out unpackingError))
      throw ResourceMetaPathImporter.MakeError((object) unpackingError);
    return result;
  }

  private static Exception MakeError(params object[] args)
  {
    return PythonOps.CreateThrowable(PythonExceptions.ImportError, args);
  }

  private static string MakeFilename(string name) => name.Replace('.', Path.DirectorySeparatorChar);

  [Flags]
  private enum ModuleCodeType
  {
    Source = 0,
    Package = 1,
  }

  private struct PackedResourceInfo
  {
    private int _fileSize;
    public string FullName;
    public int Compress;
    public int DataSize;
    public int FileOffset;

    public static ResourceMetaPathImporter.PackedResourceInfo Create(
      string fullName,
      int compress,
      int dataSize,
      int fileSize,
      int fileOffset)
    {
      ResourceMetaPathImporter.PackedResourceInfo packedResourceInfo;
      packedResourceInfo.FullName = fullName;
      packedResourceInfo.Compress = compress;
      packedResourceInfo.DataSize = dataSize;
      packedResourceInfo._fileSize = fileSize;
      packedResourceInfo.FileOffset = fileOffset;
      return packedResourceInfo;
    }
  }

  private class PackedResourceLoader
  {
    private readonly Assembly _fromAssembly;
    private readonly string _resourceNameBase;
    private const int MaxPathLen = 256 /*0x0100*/;

    public PackedResourceLoader(Assembly fromAssembly, string resourceName)
    {
      this._fromAssembly = fromAssembly;
      this._resourceNameBase = resourceName;
    }

    public bool LoadZipDirectory(
      out IDictionary<string, ResourceMetaPathImporter.PackedResourceInfo> files,
      out IDictionary<string, ResourceMetaPathImporter.PackedResourceInfo[]> modules,
      out string unpackingError)
    {
      if (!this.ReadZipDirectory(out files, out unpackingError))
      {
        modules = (IDictionary<string, ResourceMetaPathImporter.PackedResourceInfo[]>) null;
        return false;
      }
      try
      {
        IEnumerable<\u003C\u003Ef__AnonymousType7<string, ResourceMetaPathImporter.PackedResourceInfo[]>> source1 = files.Values.Select(entry => new
        {
          entry = entry,
          isPyFile = entry.FullName.EndsWith(".py", StringComparison.OrdinalIgnoreCase)
        }).Where(_param1 => _param1.isPyFile).Select(_param1 => new
        {
          \u003C\u003Eh__TransparentIdentifier0 = _param1,
          name = _param1.entry.FullName.Substring(0, _param1.entry.FullName.Length - 3)
        }).Select(_param1 => new
        {
          \u003C\u003Eh__TransparentIdentifier1 = _param1,
          dottedName = _param1.name.Replace('\\', '.').Replace('/', '.')
        }).Select(_param1 => new
        {
          \u003C\u003Eh__TransparentIdentifier2 = _param1,
          lineage = _param1.dottedName.Split('.')
        }).Select(_param1 => new
        {
          \u003C\u003Eh__TransparentIdentifier3 = _param1,
          fileName = ((IEnumerable<string>) _param1.lineage).Last<string>()
        }).Select(_param1 => new
        {
          \u003C\u003Eh__TransparentIdentifier4 = _param1,
          path = ((IEnumerable<string>) _param1.\u003C\u003Eh__TransparentIdentifier3.lineage).Take<string>(_param1.\u003C\u003Eh__TransparentIdentifier3.lineage.Length - 1).ToArray<string>()
        }).OrderBy(_param1 => _param1.\u003C\u003Eh__TransparentIdentifier4.fileName).Select(_param1 => new
        {
          name = _param1.\u003C\u003Eh__TransparentIdentifier4.fileName,
          path = _param1.path,
          dottedPath = string.Join(".", _param1.path),
          entry = _param1.\u003C\u003Eh__TransparentIdentifier4.\u003C\u003Eh__TransparentIdentifier3.\u003C\u003Eh__TransparentIdentifier2.\u003C\u003Eh__TransparentIdentifier1.\u003C\u003Eh__TransparentIdentifier0.entry
        }).OrderBy(source => source.dottedPath).GroupBy(source => source.dottedPath).Select(moduleGroup => new
        {
          Key = moduleGroup.Key,
          Items = moduleGroup.Select(item => item.entry).ToArray<ResourceMetaPathImporter.PackedResourceInfo>()
        });
        modules = (IDictionary<string, ResourceMetaPathImporter.PackedResourceInfo[]>) source1.ToDictionary(moduleGroup => moduleGroup.Key, moduleGroup => moduleGroup.Items);
        return true;
      }
      catch (Exception ex)
      {
        files = (IDictionary<string, ResourceMetaPathImporter.PackedResourceInfo>) null;
        modules = (IDictionary<string, ResourceMetaPathImporter.PackedResourceInfo[]>) null;
        unpackingError = $"{ex.GetType().Name}: {ex.Message}";
        return false;
      }
    }

    private Stream GetZipArchive()
    {
      string compareName = this._resourceNameBase.ToLowerInvariant();
      string name1 = ((IEnumerable<string>) this._fromAssembly.GetManifestResourceNames()).Where<string>((Func<string, bool>) (name => name.ToLowerInvariant().EndsWith(compareName))).FirstOrDefault<string>();
      return !string.IsNullOrEmpty(name1) ? this._fromAssembly.GetManifestResourceStream(name1) : (Stream) null;
    }

    private bool ReadZipDirectory(
      out IDictionary<string, ResourceMetaPathImporter.PackedResourceInfo> result,
      out string unpackingError)
    {
      unpackingError = (string) null;
      result = (IDictionary<string, ResourceMetaPathImporter.PackedResourceInfo>) null;
      try
      {
        Stream zipArchive = this.GetZipArchive();
        if (zipArchive == null)
        {
          unpackingError = "Resource not found.";
          return false;
        }
        using (BinaryReader reader = new BinaryReader(zipArchive))
        {
          if (reader.BaseStream.Length < 2L)
          {
            unpackingError = "Can't read ZIP resource: Empty Resource.";
            return false;
          }
          byte[] buffer = new byte[22];
          reader.BaseStream.Seek(-22L, SeekOrigin.End);
          int position = (int) reader.BaseStream.Position;
          if (reader.Read(buffer, 0, 22) != 22)
          {
            unpackingError = "Can't read ZIP resource: Invalid ZIP Directory.";
            return false;
          }
          if (BitConverter.ToUInt32(buffer, 0) != 101010256U)
          {
            unpackingError = "Can't read ZIP resource: Not a ZIP file.";
            return false;
          }
          int int32_1 = BitConverter.ToInt32(buffer, 12);
          int int32_2 = BitConverter.ToInt32(buffer, 16 /*0x10*/);
          int arcoffset = position - int32_2 - int32_1;
          int headerOffset = int32_2 + arcoffset;
          IEnumerable<ResourceMetaPathImporter.PackedResourceInfo> source = ResourceMetaPathImporter.PackedResourceLoader.ReadZipDirectory(reader, headerOffset, arcoffset);
          result = (IDictionary<string, ResourceMetaPathImporter.PackedResourceInfo>) source.OrderBy<ResourceMetaPathImporter.PackedResourceInfo, string>((Func<ResourceMetaPathImporter.PackedResourceInfo, string>) (entry => entry.FullName)).ToDictionary<ResourceMetaPathImporter.PackedResourceInfo, string>((Func<ResourceMetaPathImporter.PackedResourceInfo, string>) (entry => entry.FullName));
          return true;
        }
      }
      catch (Exception ex)
      {
        unpackingError = $"{ex.GetType().Name}: {ex.Message}";
        return false;
      }
    }

    private static IEnumerable<ResourceMetaPathImporter.PackedResourceInfo> ReadZipDirectory(
      BinaryReader reader,
      int headerOffset,
      int arcoffset)
    {
      while (true)
      {
        string empty = string.Empty;
        reader.BaseStream.Seek((long) headerOffset, SeekOrigin.Begin);
        if (reader.ReadInt32() == 33639248)
        {
          reader.BaseStream.Seek((long) (headerOffset + 10), SeekOrigin.Begin);
          short compress = reader.ReadInt16();
          int num1 = (int) reader.ReadInt16();
          int num2 = (int) reader.ReadInt16();
          reader.ReadInt32();
          int dataSize = reader.ReadInt32();
          int fileSize = reader.ReadInt32();
          short num3 = reader.ReadInt16();
          int num4 = 46 + (int) num3 + (int) reader.ReadInt16() + (int) reader.ReadInt16();
          reader.BaseStream.Seek((long) (headerOffset + 42), SeekOrigin.Begin);
          int fileOffset = reader.ReadInt32() + arcoffset;
          if (num3 > (short) 256 /*0x0100*/)
            num3 = (short) 256 /*0x0100*/;
          for (int index = 0; index < (int) num3; ++index)
          {
            char ch = reader.ReadChar();
            if (ch == '/')
              ch = Path.DirectorySeparatorChar;
            empty += ch.ToString();
          }
          headerOffset += num4;
          yield return ResourceMetaPathImporter.PackedResourceInfo.Create(empty, (int) compress, dataSize, fileSize, fileOffset);
        }
        else
          break;
      }
    }

    public bool GetData(
      ResourceMetaPathImporter.PackedResourceInfo tocEntry,
      out byte[] result,
      out string unpackingError)
    {
      unpackingError = (string) null;
      result = (byte[]) null;
      int fileOffset = tocEntry.FileOffset;
      int dataSize = tocEntry.DataSize;
      int compress = tocEntry.Compress;
      try
      {
        Stream zipArchive = this.GetZipArchive();
        if (zipArchive == null)
        {
          unpackingError = "Resource not found.";
          return false;
        }
        using (BinaryReader binaryReader = new BinaryReader(zipArchive))
        {
          binaryReader.BaseStream.Seek((long) fileOffset, SeekOrigin.Begin);
          if (binaryReader.ReadInt32() != 67324752)
          {
            unpackingError = "Bad local file header in ZIP resource.";
            return false;
          }
          binaryReader.BaseStream.Seek((long) (fileOffset + 26), SeekOrigin.Begin);
          int num = 30 + (int) binaryReader.ReadInt16() + (int) binaryReader.ReadInt16();
          int offset = fileOffset + num;
          binaryReader.BaseStream.Seek((long) offset, SeekOrigin.Begin);
          byte[] input;
          try
          {
            input = binaryReader.ReadBytes(compress == 0 ? dataSize : dataSize + 1);
          }
          catch
          {
            unpackingError = "Can't read data";
            return false;
          }
          if (compress != 0)
            input[dataSize] = (byte) 90;
          result = compress == 0 ? input : ZlibModule.Decompress(input, -15);
          return true;
        }
      }
      catch (Exception ex)
      {
        unpackingError = $"{ex.GetType().Name}: {ex.Message}";
        return false;
      }
    }
  }
}
