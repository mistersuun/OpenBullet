// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.ThrowUtil
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Globalization;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

internal static class ThrowUtil
{
  public static T CheckNotNull<T>(T val, string parameterName) where T : class
  {
    return (object) val != null ? val : throw new ArgumentNullException(parameterName);
  }

  public static int CheckNotNegative(int val, string parameterName)
  {
    return val >= 0 ? val : throw new ArgumentOutOfRangeException(parameterName, (object) val, "value must not be negative");
  }

  public static int CheckInRangeInclusive(int val, string parameterName, int lower, int upper)
  {
    if (val < lower || val > upper)
      throw new ArgumentOutOfRangeException(parameterName, (object) val, $"Expected: {lower.ToString((IFormatProvider) CultureInfo.InvariantCulture)} <= {parameterName} <= {upper.ToString((IFormatProvider) CultureInfo.InvariantCulture)}");
    return val;
  }

  public static InvalidOperationException NoDocumentAssigned()
  {
    return new InvalidOperationException("Document is null");
  }

  public static InvalidOperationException NoValidCaretPosition()
  {
    return new InvalidOperationException("Could not find a valid caret position in the line");
  }
}
