// Decompiled with JetBrains decompiler
// Type: AngleSharp.Text.StringSourceExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

#nullable disable
namespace AngleSharp.Text;

public static class StringSourceExtensions
{
  public static char SkipSpaces(this StringSource source)
  {
    char c = source.Current;
    while (c.IsSpaceCharacter())
      c = source.Next();
    return c;
  }

  public static char Next(this StringSource source, int n)
  {
    for (int index = 0; index < n; ++index)
    {
      int num = (int) source.Next();
    }
    return source.Current;
  }

  public static char Back(this StringSource source, int n)
  {
    for (int index = 0; index < n; ++index)
    {
      int num = (int) source.Back();
    }
    return source.Current;
  }

  public static char Peek(this StringSource source)
  {
    int num1 = (int) source.Next();
    int num2 = (int) source.Back();
    return (char) num1;
  }
}
