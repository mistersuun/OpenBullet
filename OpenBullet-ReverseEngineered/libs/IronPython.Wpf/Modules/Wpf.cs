// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.Wpf
// Assembly: IronPython.Wpf, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 35E9DF3F-9727-4D27-864D-27FA48A7F3D4
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Wpf.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Xaml;
using System.Xml;

#nullable disable
namespace IronPython.Modules;

public static class Wpf
{
  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    context.DomainManager.LoadAssembly(typeof (System.Windows.Markup.XamlReader).Assembly);
    context.DomainManager.LoadAssembly(typeof (Clipboard).Assembly);
    context.DomainManager.LoadAssembly(typeof (DependencyProperty).Assembly);
    context.DomainManager.LoadAssembly(typeof (System.Xaml.XamlReader).Assembly);
  }

  public static object LoadComponent(CodeContext context, object self, string filename)
  {
    if (filename == null)
      throw PythonOps.TypeError("expected str, got None");
    if (self == null)
      throw PythonOps.TypeError("expected module, got None");
    return DynamicXamlReader.LoadComponent(self, context.LanguageContext.Operations, filename, System.Windows.Markup.XamlReader.GetWpfSchemaContext());
  }

  public static object LoadComponent(CodeContext context, object self, [NotNull] Stream stream)
  {
    if (self == null)
      throw PythonOps.TypeError("expected module, got None");
    return DynamicXamlReader.LoadComponent(self, context.LanguageContext.Operations, stream, System.Windows.Markup.XamlReader.GetWpfSchemaContext());
  }

  public static object LoadComponent(CodeContext context, object self, [NotNull] XmlReader xmlReader)
  {
    if (self == null)
      throw PythonOps.TypeError("expected module, got None");
    return DynamicXamlReader.LoadComponent(self, context.LanguageContext.Operations, xmlReader, System.Windows.Markup.XamlReader.GetWpfSchemaContext());
  }

  public static object LoadComponent(CodeContext context, object self, [NotNull] TextReader filename)
  {
    if (self == null)
      throw PythonOps.TypeError("expected module, got None");
    return DynamicXamlReader.LoadComponent(self, context.LanguageContext.Operations, filename, System.Windows.Markup.XamlReader.GetWpfSchemaContext());
  }

  public static object LoadComponent(CodeContext context, object self, [NotNull] XamlXmlReader reader)
  {
    if (self == null)
      throw PythonOps.TypeError("expected module, got None");
    return DynamicXamlReader.LoadComponent(self, context.LanguageContext.Operations, reader);
  }
}
