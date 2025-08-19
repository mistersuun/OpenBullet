// Decompiled with JetBrains decompiler
// Type: Tesseract.Internal.Guard
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Diagnostics;

#nullable disable
namespace Tesseract.Internal;

internal static class Guard
{
  [DebuggerHidden]
  public static void Require(string paramName, bool condition)
  {
    if (!condition)
      throw new ArgumentException(string.Empty, paramName);
  }

  [DebuggerHidden]
  public static void Require(string paramName, bool condition, string message)
  {
    if (!condition)
      throw new ArgumentException(message, paramName);
  }

  [DebuggerHidden]
  public static void Require(
    string paramName,
    bool condition,
    string message,
    params object[] args)
  {
    if (!condition)
      throw new ArgumentException(string.Format(message, args), paramName);
  }

  [DebuggerHidden]
  public static void RequireNotNull(string argName, object value)
  {
    if (value == null)
      throw new ArgumentException($"Argument \"{value}\" must not be null.");
  }

  [DebuggerHidden]
  public static void RequireNotNullOrEmpty(string paramName, string value)
  {
    Guard.RequireNotNull(paramName, (object) value);
    if (value.Length == 0)
      throw new ArgumentException(paramName, $"The argument \"{paramName}\" must not be null or empty.");
  }

  [DebuggerHidden]
  public static void Verify(bool condition, string message, params object[] args)
  {
    if (!condition)
      throw new InvalidOperationException(string.Format(message, args));
  }
}
