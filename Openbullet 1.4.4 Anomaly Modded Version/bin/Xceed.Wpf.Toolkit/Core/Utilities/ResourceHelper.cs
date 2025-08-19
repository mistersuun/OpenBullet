// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Utilities.ResourceHelper
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.IO;
using System.Reflection;
using System.Resources;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Utilities;

internal class ResourceHelper
{
  internal static Stream LoadResourceStream(Assembly assembly, string resId)
  {
    ResourceManager resourceManager = new ResourceManager(Path.GetFileNameWithoutExtension(assembly.ManifestModule.Name) + ".g", assembly);
    resId = resId.ToLower();
    resId = resId.Replace('\\', '/');
    string name = resId;
    return resourceManager.GetObject(name) as Stream;
  }
}
