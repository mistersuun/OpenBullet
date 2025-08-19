// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Utilities.GeneralUtilities
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows;
using System.Windows.Data;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Utilities;

internal sealed class GeneralUtilities : DependencyObject
{
  internal static readonly DependencyProperty StubValueProperty = DependencyProperty.RegisterAttached("StubValue", typeof (object), typeof (GeneralUtilities), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

  private GeneralUtilities()
  {
  }

  internal static object GetStubValue(DependencyObject obj)
  {
    return obj.GetValue(GeneralUtilities.StubValueProperty);
  }

  internal static void SetStubValue(DependencyObject obj, object value)
  {
    obj.SetValue(GeneralUtilities.StubValueProperty, value);
  }

  public static object GetPathValue(object sourceObject, string path)
  {
    GeneralUtilities target = new GeneralUtilities();
    BindingOperations.SetBinding((DependencyObject) target, GeneralUtilities.StubValueProperty, (BindingBase) new Binding(path)
    {
      Source = sourceObject
    });
    object stubValue = GeneralUtilities.GetStubValue((DependencyObject) target);
    BindingOperations.ClearBinding((DependencyObject) target, GeneralUtilities.StubValueProperty);
    return stubValue;
  }

  public static object GetBindingValue(object sourceObject, Binding binding)
  {
    Binding binding1 = new Binding();
    binding1.BindsDirectlyToSource = binding.BindsDirectlyToSource;
    binding1.Converter = binding.Converter;
    binding1.ConverterCulture = binding.ConverterCulture;
    binding1.ConverterParameter = binding.ConverterParameter;
    binding1.FallbackValue = binding.FallbackValue;
    binding1.Mode = BindingMode.OneTime;
    binding1.Path = binding.Path;
    binding1.StringFormat = binding.StringFormat;
    binding1.TargetNullValue = binding.TargetNullValue;
    binding1.XPath = binding.XPath;
    Binding binding2 = binding1;
    binding2.Source = sourceObject;
    GeneralUtilities target = new GeneralUtilities();
    BindingOperations.SetBinding((DependencyObject) target, GeneralUtilities.StubValueProperty, (BindingBase) binding2);
    object stubValue = GeneralUtilities.GetStubValue((DependencyObject) target);
    BindingOperations.ClearBinding((DependencyObject) target, GeneralUtilities.StubValueProperty);
    return stubValue;
  }

  internal static bool CanConvertValue(object value, object targetType)
  {
    return value != null && !object.Equals((object) value.GetType(), targetType) && !object.Equals(targetType, (object) typeof (object));
  }
}
