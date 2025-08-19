// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.CharOps
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class CharOps
{
  public static string __repr__(char self)
  {
    return self.ToString((IFormatProvider) CultureInfo.InvariantCulture);
  }

  public static bool __eq__(char self, char other) => (int) self == (int) other;

  [SpecialName]
  public static bool __ne__(char self, char other) => (int) self != (int) other;

  public static int __hash__(char self) => new string(self, 1).GetHashCode();

  public static int __index__(char self) => (int) self;

  [return: MaybeNotImplemented]
  public static object __cmp__(char self, object other)
  {
    switch (other)
    {
      case char ch:
        int num1 = (int) self - (int) ch;
        return (object) (num1 > 0 ? 1 : (num1 < 0 ? -1 : 0));
      case string str:
        if (str.Length == 1)
        {
          int num2 = (int) self - (int) str[0];
          return (object) (num2 > 0 ? 1 : (num2 < 0 ? -1 : 0));
        }
        break;
    }
    return (object) NotImplementedType.Value;
  }

  public static bool __contains__(char self, char other) => (int) self == (int) other;

  public static bool __contains__(char self, string other)
  {
    return other.Length == 1 && (int) other[0] == (int) self;
  }

  [ImplicitConversionMethod]
  [SpecialName]
  public static string ConvertToString(char self) => new string(self, 1);

  [ExplicitConversionMethod]
  [SpecialName]
  public static char ConvertToChar(int value)
  {
    return value >= 0 && value <= (int) ushort.MaxValue ? (char) value : throw new OverflowException();
  }
}
