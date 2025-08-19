// Decompiled with JetBrains decompiler
// Type: Standard.Assert
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;

#nullable disable
namespace Standard;

internal static class Assert
{
  private static void _Break() => Debugger.Break();

  [Conditional("DEBUG")]
  public static void Evaluate(Assert.EvaluateFunction argument) => argument();

  [Obsolete("Use Assert.AreEqual instead of Assert.Equals", false)]
  [Conditional("DEBUG")]
  public static void Equals<T>(T expected, T actual)
  {
  }

  [Conditional("DEBUG")]
  public static void AreEqual<T>(T expected, T actual)
  {
    if ((object) expected == null)
    {
      if ((object) actual == null || actual.Equals((object) expected))
        return;
      Assert._Break();
    }
    else
    {
      if (expected.Equals((object) actual))
        return;
      Assert._Break();
    }
  }

  [Conditional("DEBUG")]
  public static void AreNotEqual<T>(T notExpected, T actual)
  {
    if ((object) notExpected == null)
    {
      if ((object) actual != null && !actual.Equals((object) notExpected))
        return;
      Assert._Break();
    }
    else
    {
      if (!notExpected.Equals((object) actual))
        return;
      Assert._Break();
    }
  }

  [Conditional("DEBUG")]
  public static void Implies(bool condition, bool result)
  {
    if (!condition || result)
      return;
    Assert._Break();
  }

  [Conditional("DEBUG")]
  public static void Implies(bool condition, Assert.ImplicationFunction result)
  {
    if (!condition || result())
      return;
    Assert._Break();
  }

  [Conditional("DEBUG")]
  public static void IsNeitherNullNorEmpty(string value)
  {
  }

  [Conditional("DEBUG")]
  public static void IsNeitherNullNorWhitespace(string value)
  {
    if (string.IsNullOrEmpty(value))
      Assert._Break();
    if (value.Trim().Length != 0)
      return;
    Assert._Break();
  }

  [Conditional("DEBUG")]
  public static void IsNotNull<T>(T value) where T : class
  {
    if ((object) value != null)
      return;
    Assert._Break();
  }

  [Conditional("DEBUG")]
  public static void IsDefault<T>(T value) where T : struct => value.Equals((object) default (T));

  [Conditional("DEBUG")]
  public static void IsNotDefault<T>(T value) where T : struct
  {
    value.Equals((object) default (T));
  }

  [Conditional("DEBUG")]
  public static void IsFalse(bool condition)
  {
    if (!condition)
      return;
    Assert._Break();
  }

  [Conditional("DEBUG")]
  public static void IsFalse(bool condition, string message)
  {
    if (!condition)
      return;
    Assert._Break();
  }

  [Conditional("DEBUG")]
  public static void IsTrue(bool condition)
  {
    if (condition)
      return;
    Assert._Break();
  }

  [Conditional("DEBUG")]
  public static void IsTrue(bool condition, string message)
  {
    if (condition)
      return;
    Assert._Break();
  }

  [Conditional("DEBUG")]
  public static void Fail() => Assert._Break();

  [Conditional("DEBUG")]
  public static void Fail(string message) => Assert._Break();

  [Conditional("DEBUG")]
  public static void IsNull<T>(T item) where T : class
  {
    if ((object) item == null)
      return;
    Assert._Break();
  }

  [Conditional("DEBUG")]
  public static void BoundedDoubleInc(
    double lowerBoundInclusive,
    double value,
    double upperBoundInclusive)
  {
    if (value >= lowerBoundInclusive && value <= upperBoundInclusive)
      return;
    Assert._Break();
  }

  [Conditional("DEBUG")]
  public static void BoundedInteger(int lowerBoundInclusive, int value, int upperBoundExclusive)
  {
    if (value >= lowerBoundInclusive && value < upperBoundExclusive)
      return;
    Assert._Break();
  }

  [Conditional("DEBUG")]
  public static void IsApartmentState(ApartmentState expectedState)
  {
    if (Thread.CurrentThread.GetApartmentState() == expectedState)
      return;
    Assert._Break();
  }

  [Conditional("DEBUG")]
  public static void NullableIsNotNull<T>(T? value) where T : struct
  {
    if (value.HasValue)
      return;
    Assert._Break();
  }

  [Conditional("DEBUG")]
  public static void NullableIsNull<T>(T? value) where T : struct
  {
    if (!value.HasValue)
      return;
    Assert._Break();
  }

  [Conditional("DEBUG")]
  public static void IsNotOnMainThread()
  {
    if (!Application.Current.Dispatcher.CheckAccess())
      return;
    Assert._Break();
  }

  public delegate void EvaluateFunction();

  public delegate bool ImplicationFunction();
}
