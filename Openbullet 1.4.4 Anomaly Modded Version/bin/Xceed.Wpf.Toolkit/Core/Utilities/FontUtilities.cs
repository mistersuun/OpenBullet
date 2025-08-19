// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Utilities.FontUtilities
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Utilities;

internal class FontUtilities
{
  internal static IEnumerable<FontFamily> Families
  {
    get
    {
      foreach (FontFamily systemFontFamily in (IEnumerable<FontFamily>) Fonts.SystemFontFamilies)
      {
        try
        {
          LanguageSpecificStringDictionary familyNames = systemFontFamily.FamilyNames;
        }
        catch
        {
          continue;
        }
        yield return systemFontFamily;
      }
    }
  }

  internal static IEnumerable<FontWeight> Weights
  {
    get
    {
      yield return FontWeights.Black;
      yield return FontWeights.Bold;
      yield return FontWeights.ExtraBlack;
      yield return FontWeights.ExtraBold;
      yield return FontWeights.ExtraLight;
      yield return FontWeights.Light;
      yield return FontWeights.Medium;
      yield return FontWeights.Normal;
      yield return FontWeights.SemiBold;
      yield return FontWeights.Thin;
    }
  }

  internal static IEnumerable<FontStyle> Styles
  {
    get
    {
      yield return FontStyles.Italic;
      yield return FontStyles.Normal;
    }
  }

  internal static IEnumerable<FontStretch> Stretches
  {
    get
    {
      yield return FontStretches.Condensed;
      yield return FontStretches.Expanded;
      yield return FontStretches.ExtraCondensed;
      yield return FontStretches.ExtraExpanded;
      yield return FontStretches.Normal;
      yield return FontStretches.SemiCondensed;
      yield return FontStretches.SemiExpanded;
      yield return FontStretches.UltraCondensed;
      yield return FontStretches.UltraExpanded;
    }
  }
}
