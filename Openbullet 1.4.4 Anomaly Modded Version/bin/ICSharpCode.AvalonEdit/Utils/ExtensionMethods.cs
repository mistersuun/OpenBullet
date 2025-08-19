// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.ExtensionMethods
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

internal static class ExtensionMethods
{
  public const double Epsilon = 0.01;

  public static bool IsClose(this double d1, double d2) => d1 == d2 || Math.Abs(d1 - d2) < 0.01;

  public static bool IsClose(this System.Windows.Size d1, System.Windows.Size d2)
  {
    return d1.Width.IsClose(d2.Width) && d1.Height.IsClose(d2.Height);
  }

  public static bool IsClose(this Vector d1, Vector d2) => d1.X.IsClose(d2.X) && d1.Y.IsClose(d2.Y);

  public static double CoerceValue(this double value, double minimum, double maximum)
  {
    return Math.Max(Math.Min(value, maximum), minimum);
  }

  public static int CoerceValue(this int value, int minimum, int maximum)
  {
    return Math.Max(Math.Min(value, maximum), minimum);
  }

  public static Typeface CreateTypeface(this FrameworkElement fe)
  {
    return new Typeface((System.Windows.Media.FontFamily) fe.GetValue(TextBlock.FontFamilyProperty), (System.Windows.FontStyle) fe.GetValue(TextBlock.FontStyleProperty), (FontWeight) fe.GetValue(TextBlock.FontWeightProperty), (FontStretch) fe.GetValue(TextBlock.FontStretchProperty));
  }

  public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> elements)
  {
    foreach (T element in elements)
      collection.Add(element);
  }

  public static IEnumerable<T> Sequence<T>(T value)
  {
    yield return value;
  }

  public static string GetAttributeOrNull(this XmlElement element, string attributeName)
  {
    return element.GetAttributeNode(attributeName)?.Value;
  }

  public static bool? GetBoolAttribute(this XmlElement element, string attributeName)
  {
    XmlAttribute attributeNode = element.GetAttributeNode(attributeName);
    return attributeNode == null ? new bool?() : new bool?(XmlConvert.ToBoolean(attributeNode.Value));
  }

  public static bool? GetBoolAttribute(this XmlReader reader, string attributeName)
  {
    string attribute = reader.GetAttribute(attributeName);
    return attribute == null ? new bool?() : new bool?(XmlConvert.ToBoolean(attribute));
  }

  public static Rect TransformToDevice(this Rect rect, Visual visual)
  {
    Matrix transformToDevice = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
    return Rect.Transform(rect, transformToDevice);
  }

  public static Rect TransformFromDevice(this Rect rect, Visual visual)
  {
    Matrix transformFromDevice = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
    return Rect.Transform(rect, transformFromDevice);
  }

  public static System.Windows.Size TransformToDevice(this System.Windows.Size size, Visual visual)
  {
    Matrix transformToDevice = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
    return new System.Windows.Size(size.Width * transformToDevice.M11, size.Height * transformToDevice.M22);
  }

  public static System.Windows.Size TransformFromDevice(this System.Windows.Size size, Visual visual)
  {
    Matrix transformFromDevice = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
    return new System.Windows.Size(size.Width * transformFromDevice.M11, size.Height * transformFromDevice.M22);
  }

  public static System.Windows.Point TransformToDevice(this System.Windows.Point point, Visual visual)
  {
    Matrix transformToDevice = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
    return new System.Windows.Point(point.X * transformToDevice.M11, point.Y * transformToDevice.M22);
  }

  public static System.Windows.Point TransformFromDevice(this System.Windows.Point point, Visual visual)
  {
    Matrix transformFromDevice = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
    return new System.Windows.Point(point.X * transformFromDevice.M11, point.Y * transformFromDevice.M22);
  }

  public static System.Drawing.Point ToSystemDrawing(this System.Windows.Point p)
  {
    return new System.Drawing.Point((int) p.X, (int) p.Y);
  }

  public static System.Windows.Point ToWpf(this System.Drawing.Point p)
  {
    return new System.Windows.Point((double) p.X, (double) p.Y);
  }

  public static System.Windows.Size ToWpf(this System.Drawing.Size s)
  {
    return new System.Windows.Size((double) s.Width, (double) s.Height);
  }

  public static Rect ToWpf(this Rectangle rect)
  {
    return new Rect(rect.Location.ToWpf(), rect.Size.ToWpf());
  }

  public static IEnumerable<DependencyObject> VisualAncestorsAndSelf(this DependencyObject obj)
  {
    while (obj != null)
    {
      yield return obj;
      switch (obj)
      {
        case Visual _:
        case Visual3D _:
          obj = VisualTreeHelper.GetParent(obj);
          continue;
        case FrameworkContentElement _:
          obj = ((FrameworkContentElement) obj).Parent;
          continue;
        default:
          yield break;
      }
    }
  }

  [Conditional("DEBUG")]
  public static void CheckIsFrozen(Freezable f)
  {
    if (f == null)
      return;
    int num = f.IsFrozen ? 1 : 0;
  }

  [Conditional("DEBUG")]
  public static void Log(bool condition, string format, params object[] args)
  {
    if (!condition)
      return;
    Console.WriteLine($"{DateTime.Now.ToString("hh:MM:ss")}: {string.Format(format, args)}{Environment.NewLine}{Environment.StackTrace}");
  }
}
