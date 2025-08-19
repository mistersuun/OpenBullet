// Decompiled with JetBrains decompiler
// Type: System.Windows.TemplateVisualStateAttribute
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

#nullable disable
namespace System.Windows;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class TemplateVisualStateAttribute : Attribute
{
  public string Name { get; set; }

  public string GroupName { get; set; }
}
