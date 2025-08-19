// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MetadataServices
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

#nullable disable
namespace Microsoft.Scripting.Metadata;

public static class MetadataServices
{
  private static Dictionary<Assembly, MetadataTables[]> _metadataCache;
  private static readonly byte[] _ExtensionAttributeNameUtf8 = Encoding.UTF8.GetBytes("ExtensionAttribute");
  private static readonly byte[] _ExtensionAttributeNamespaceUtf8 = Encoding.UTF8.GetBytes("System.Runtime.CompilerServices");

  private static MetadataTables[] GetAsseblyMetadata(Assembly assembly)
  {
    if (MetadataServices._metadataCache == null)
      MetadataServices._metadataCache = new Dictionary<Assembly, MetadataTables[]>();
    lock (MetadataServices._metadataCache)
    {
      MetadataTables[] asseblyMetadata;
      if (!MetadataServices._metadataCache.TryGetValue(assembly, out asseblyMetadata))
      {
        Module[] modules = assembly.GetModules(false);
        asseblyMetadata = new MetadataTables[modules.Length];
        int num = 1;
        foreach (Module module in modules)
        {
          MetadataTables metadataTables = MetadataTables.OpenModule(module);
          if (metadataTables.AssemblyDef.Record.IsNull)
            asseblyMetadata[num++] = MetadataTables.OpenModule(module);
          else
            asseblyMetadata[0] = metadataTables;
        }
        MetadataServices._metadataCache.Add(assembly, asseblyMetadata);
      }
      return asseblyMetadata;
    }
  }

  private static void GetName(
    CustomAttributeDef ca,
    out MetadataName name,
    out MetadataName @namespace)
  {
    MetadataRecord constructor = ca.Constructor;
    if (constructor.IsMemberRef)
    {
      MetadataRecord metadataRecord = constructor.MemberRef.Class;
      if (metadataRecord.IsTypeRef)
      {
        name = metadataRecord.TypeRef.TypeName;
        @namespace = metadataRecord.TypeRef.TypeNamespace;
      }
      else
      {
        name = metadataRecord.TypeDef.Name;
        @namespace = metadataRecord.TypeDef.Namespace;
      }
    }
    else
    {
      TypeDef declaringType = constructor.MethodDef.FindDeclaringType();
      name = declaringType.Name;
      @namespace = declaringType.Namespace;
    }
  }

  private static bool IsExtensionAttribute(CustomAttributeDef ca)
  {
    MetadataName name;
    MetadataName @namespace;
    MetadataServices.GetName(ca, out name, out @namespace);
    return name.Equals(MetadataServices._ExtensionAttributeNameUtf8, 0, MetadataServices._ExtensionAttributeNameUtf8.Length) && @namespace.Equals(MetadataServices._ExtensionAttributeNamespaceUtf8, 0, MetadataServices._ExtensionAttributeNamespaceUtf8.Length);
  }

  private static MetadataRecord GetExtensionAttributeCtor(MetadataTables tables)
  {
    AssemblyDef assemblyDef = tables.AssemblyDef;
    if (!assemblyDef.Record.IsNull)
    {
      foreach (MetadataRecord customAttribute in assemblyDef.CustomAttributes)
      {
        CustomAttributeDef ca = (CustomAttributeDef) customAttribute;
        if (MetadataServices.IsExtensionAttribute(ca))
          return ca.Constructor;
      }
    }
    return new MetadataRecord();
  }

  public static List<KeyValuePair<Module, int>> GetVisibleExtensionMethods(Assembly assembly)
  {
    MetadataTables tables = !(assembly == (Assembly) null) ? MetadataServices.GetAsseblyMetadata(assembly)[0] : throw new ArgumentNullException(nameof (assembly));
    MetadataRecord extensionAttributeCtor = MetadataServices.GetExtensionAttributeCtor(tables);
    List<KeyValuePair<Module, int>> extensionMethods = new List<KeyValuePair<Module, int>>();
    if (!extensionAttributeCtor.IsNull)
    {
      foreach (MetadataRecord customAttribute in tables.CustomAttributes)
      {
        CustomAttributeDef customAttributeDef = (CustomAttributeDef) customAttribute;
        MetadataRecord metadataRecord = customAttributeDef.Constructor;
        if (metadataRecord.Equals(extensionAttributeCtor))
        {
          metadataRecord = customAttributeDef.Parent;
          if (metadataRecord.IsMethodDef)
          {
            metadataRecord = customAttributeDef.Parent;
            MethodDef methodDef = metadataRecord.MethodDef;
            MethodAttributes attributes1 = methodDef.Attributes;
            if ((attributes1 & MethodAttributes.MemberAccessMask) == MethodAttributes.Public && (attributes1 & MethodAttributes.Static) != MethodAttributes.PrivateScope)
            {
              TypeAttributes attributes2 = methodDef.FindDeclaringType().Attributes;
              if (((attributes2 & TypeAttributes.VisibilityMask) == TypeAttributes.Public || (attributes2 & TypeAttributes.VisibilityMask) == TypeAttributes.NestedPublic) && (attributes2 & TypeAttributes.Abstract) != TypeAttributes.NotPublic && (attributes2 & TypeAttributes.Sealed) != TypeAttributes.NotPublic)
              {
                List<KeyValuePair<Module, int>> keyValuePairList = extensionMethods;
                Module module = tables.Module;
                metadataRecord = methodDef.Record;
                int num = metadataRecord.Token.Value;
                KeyValuePair<Module, int> keyValuePair = new KeyValuePair<Module, int>(module, num);
                keyValuePairList.Add(keyValuePair);
              }
            }
          }
        }
      }
    }
    return extensionMethods;
  }

  public static List<MethodInfo> GetVisibleExtensionMethodInfos(Assembly assembly)
  {
    List<KeyValuePair<Module, int>> extensionMethods = MetadataServices.GetVisibleExtensionMethods(assembly);
    List<MethodInfo> extensionMethodInfos = new List<MethodInfo>(extensionMethods.Count);
    foreach (KeyValuePair<Module, int> keyValuePair in extensionMethods)
      extensionMethodInfos.Add((MethodInfo) keyValuePair.Key.ResolveMethod(keyValuePair.Value));
    return extensionMethodInfos;
  }
}
