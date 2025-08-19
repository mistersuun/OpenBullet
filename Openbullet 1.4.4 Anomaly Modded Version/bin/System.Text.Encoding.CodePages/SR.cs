// Decompiled with JetBrains decompiler
// Type: System.SR
// Assembly: System.Text.Encoding.CodePages, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D2B4F262-31A4-4E80-9CFB-26A2249A735E
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Text.Encoding.CodePages.dll

using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace System;

internal static class SR
{
  private static ResourceManager s_resourceManager;

  private static ResourceManager ResourceManager
  {
    get => SR.s_resourceManager ?? (SR.s_resourceManager = new ResourceManager(SR.ResourceType));
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private static bool UsingResourceKeys() => false;

  internal static string GetResourceString(string resourceKey, string defaultString)
  {
    string str = (string) null;
    try
    {
      str = SR.ResourceManager.GetString(resourceKey);
    }
    catch (MissingManifestResourceException ex)
    {
    }
    return defaultString != null && resourceKey.Equals(str, StringComparison.Ordinal) ? defaultString : str;
  }

  internal static string Format(string resourceFormat, params object[] args)
  {
    if (args == null)
      return resourceFormat;
    return SR.UsingResourceKeys() ? resourceFormat + string.Join(", ", args) : string.Format(resourceFormat, args);
  }

  internal static string Format(string resourceFormat, object p1)
  {
    if (!SR.UsingResourceKeys())
      return string.Format(resourceFormat, p1);
    return string.Join(", ", (object) resourceFormat, p1);
  }

  internal static string Format(string resourceFormat, object p1, object p2)
  {
    if (!SR.UsingResourceKeys())
      return string.Format(resourceFormat, p1, p2);
    return string.Join(", ", (object) resourceFormat, p1, p2);
  }

  internal static string Format(string resourceFormat, object p1, object p2, object p3)
  {
    if (!SR.UsingResourceKeys())
      return string.Format(resourceFormat, p1, p2, p3);
    return string.Join(", ", (object) resourceFormat, p1, p2, p3);
  }

  internal static Type ResourceType { get; } = typeof (FxResources.System.Text.Encoding.CodePages.SR);

  internal static string ArgumentNull_Array
  {
    get => SR.GetResourceString(nameof (ArgumentNull_Array), (string) null);
  }

  internal static string ArgumentOutOfRange_NeedNonNegNum
  {
    get => SR.GetResourceString(nameof (ArgumentOutOfRange_NeedNonNegNum), (string) null);
  }

  internal static string ArgumentOutOfRange_IndexCount
  {
    get => SR.GetResourceString(nameof (ArgumentOutOfRange_IndexCount), (string) null);
  }

  internal static string ArgumentOutOfRange_IndexCountBuffer
  {
    get => SR.GetResourceString(nameof (ArgumentOutOfRange_IndexCountBuffer), (string) null);
  }

  internal static string NotSupported_NoCodepageData
  {
    get => SR.GetResourceString(nameof (NotSupported_NoCodepageData), (string) null);
  }

  internal static string Argument_EncodingConversionOverflowBytes
  {
    get => SR.GetResourceString(nameof (Argument_EncodingConversionOverflowBytes), (string) null);
  }

  internal static string Argument_InvalidCharSequenceNoIndex
  {
    get => SR.GetResourceString(nameof (Argument_InvalidCharSequenceNoIndex), (string) null);
  }

  internal static string ArgumentOutOfRange_GetByteCountOverflow
  {
    get => SR.GetResourceString(nameof (ArgumentOutOfRange_GetByteCountOverflow), (string) null);
  }

  internal static string Argument_EncodingConversionOverflowChars
  {
    get => SR.GetResourceString(nameof (Argument_EncodingConversionOverflowChars), (string) null);
  }

  internal static string ArgumentOutOfRange_GetCharCountOverflow
  {
    get => SR.GetResourceString(nameof (ArgumentOutOfRange_GetCharCountOverflow), (string) null);
  }

  internal static string Argument_EncoderFallbackNotEmpty
  {
    get => SR.GetResourceString(nameof (Argument_EncoderFallbackNotEmpty), (string) null);
  }

  internal static string Argument_RecursiveFallback
  {
    get => SR.GetResourceString(nameof (Argument_RecursiveFallback), (string) null);
  }

  internal static string Argument_RecursiveFallbackBytes
  {
    get => SR.GetResourceString(nameof (Argument_RecursiveFallbackBytes), (string) null);
  }

  internal static string ArgumentOutOfRange_Range
  {
    get => SR.GetResourceString(nameof (ArgumentOutOfRange_Range), (string) null);
  }

  internal static string Argument_CodepageNotSupported
  {
    get => SR.GetResourceString(nameof (Argument_CodepageNotSupported), (string) null);
  }

  internal static string ArgumentOutOfRange_Index
  {
    get => SR.GetResourceString(nameof (ArgumentOutOfRange_Index), (string) null);
  }

  internal static string MissingEncodingNameResource
  {
    get => SR.GetResourceString(nameof (MissingEncodingNameResource), (string) null);
  }

  internal static string Globalization_cp_37
  {
    get => SR.GetResourceString(nameof (Globalization_cp_37), (string) null);
  }

  internal static string Globalization_cp_437
  {
    get => SR.GetResourceString(nameof (Globalization_cp_437), (string) null);
  }

  internal static string Globalization_cp_500
  {
    get => SR.GetResourceString(nameof (Globalization_cp_500), (string) null);
  }

  internal static string Globalization_cp_708
  {
    get => SR.GetResourceString(nameof (Globalization_cp_708), (string) null);
  }

  internal static string Globalization_cp_720
  {
    get => SR.GetResourceString(nameof (Globalization_cp_720), (string) null);
  }

  internal static string Globalization_cp_737
  {
    get => SR.GetResourceString(nameof (Globalization_cp_737), (string) null);
  }

  internal static string Globalization_cp_775
  {
    get => SR.GetResourceString(nameof (Globalization_cp_775), (string) null);
  }

  internal static string Globalization_cp_850
  {
    get => SR.GetResourceString(nameof (Globalization_cp_850), (string) null);
  }

  internal static string Globalization_cp_852
  {
    get => SR.GetResourceString(nameof (Globalization_cp_852), (string) null);
  }

  internal static string Globalization_cp_855
  {
    get => SR.GetResourceString(nameof (Globalization_cp_855), (string) null);
  }

  internal static string Globalization_cp_857
  {
    get => SR.GetResourceString(nameof (Globalization_cp_857), (string) null);
  }

  internal static string Globalization_cp_858
  {
    get => SR.GetResourceString(nameof (Globalization_cp_858), (string) null);
  }

  internal static string Globalization_cp_860
  {
    get => SR.GetResourceString(nameof (Globalization_cp_860), (string) null);
  }

  internal static string Globalization_cp_861
  {
    get => SR.GetResourceString(nameof (Globalization_cp_861), (string) null);
  }

  internal static string Globalization_cp_862
  {
    get => SR.GetResourceString(nameof (Globalization_cp_862), (string) null);
  }

  internal static string Globalization_cp_863
  {
    get => SR.GetResourceString(nameof (Globalization_cp_863), (string) null);
  }

  internal static string Globalization_cp_864
  {
    get => SR.GetResourceString(nameof (Globalization_cp_864), (string) null);
  }

  internal static string Globalization_cp_865
  {
    get => SR.GetResourceString(nameof (Globalization_cp_865), (string) null);
  }

  internal static string Globalization_cp_866
  {
    get => SR.GetResourceString(nameof (Globalization_cp_866), (string) null);
  }

  internal static string Globalization_cp_869
  {
    get => SR.GetResourceString(nameof (Globalization_cp_869), (string) null);
  }

  internal static string Globalization_cp_870
  {
    get => SR.GetResourceString(nameof (Globalization_cp_870), (string) null);
  }

  internal static string Globalization_cp_874
  {
    get => SR.GetResourceString(nameof (Globalization_cp_874), (string) null);
  }

  internal static string Globalization_cp_875
  {
    get => SR.GetResourceString(nameof (Globalization_cp_875), (string) null);
  }

  internal static string Globalization_cp_932
  {
    get => SR.GetResourceString(nameof (Globalization_cp_932), (string) null);
  }

  internal static string Globalization_cp_936
  {
    get => SR.GetResourceString(nameof (Globalization_cp_936), (string) null);
  }

  internal static string Globalization_cp_949
  {
    get => SR.GetResourceString(nameof (Globalization_cp_949), (string) null);
  }

  internal static string Globalization_cp_950
  {
    get => SR.GetResourceString(nameof (Globalization_cp_950), (string) null);
  }

  internal static string Globalization_cp_1026
  {
    get => SR.GetResourceString(nameof (Globalization_cp_1026), (string) null);
  }

  internal static string Globalization_cp_1047
  {
    get => SR.GetResourceString(nameof (Globalization_cp_1047), (string) null);
  }

  internal static string Globalization_cp_1140
  {
    get => SR.GetResourceString(nameof (Globalization_cp_1140), (string) null);
  }

  internal static string Globalization_cp_1141
  {
    get => SR.GetResourceString(nameof (Globalization_cp_1141), (string) null);
  }

  internal static string Globalization_cp_1142
  {
    get => SR.GetResourceString(nameof (Globalization_cp_1142), (string) null);
  }

  internal static string Globalization_cp_1143
  {
    get => SR.GetResourceString(nameof (Globalization_cp_1143), (string) null);
  }

  internal static string Globalization_cp_1144
  {
    get => SR.GetResourceString(nameof (Globalization_cp_1144), (string) null);
  }

  internal static string Globalization_cp_1145
  {
    get => SR.GetResourceString(nameof (Globalization_cp_1145), (string) null);
  }

  internal static string Globalization_cp_1146
  {
    get => SR.GetResourceString(nameof (Globalization_cp_1146), (string) null);
  }

  internal static string Globalization_cp_1147
  {
    get => SR.GetResourceString(nameof (Globalization_cp_1147), (string) null);
  }

  internal static string Globalization_cp_1148
  {
    get => SR.GetResourceString(nameof (Globalization_cp_1148), (string) null);
  }

  internal static string Globalization_cp_1149
  {
    get => SR.GetResourceString(nameof (Globalization_cp_1149), (string) null);
  }

  internal static string Globalization_cp_1250
  {
    get => SR.GetResourceString(nameof (Globalization_cp_1250), (string) null);
  }

  internal static string Globalization_cp_1251
  {
    get => SR.GetResourceString(nameof (Globalization_cp_1251), (string) null);
  }

  internal static string Globalization_cp_1252
  {
    get => SR.GetResourceString(nameof (Globalization_cp_1252), (string) null);
  }

  internal static string Globalization_cp_1253
  {
    get => SR.GetResourceString(nameof (Globalization_cp_1253), (string) null);
  }

  internal static string Globalization_cp_1254
  {
    get => SR.GetResourceString(nameof (Globalization_cp_1254), (string) null);
  }

  internal static string Globalization_cp_1255
  {
    get => SR.GetResourceString(nameof (Globalization_cp_1255), (string) null);
  }

  internal static string Globalization_cp_1256
  {
    get => SR.GetResourceString(nameof (Globalization_cp_1256), (string) null);
  }

  internal static string Globalization_cp_1257
  {
    get => SR.GetResourceString(nameof (Globalization_cp_1257), (string) null);
  }

  internal static string Globalization_cp_1258
  {
    get => SR.GetResourceString(nameof (Globalization_cp_1258), (string) null);
  }

  internal static string Globalization_cp_1361
  {
    get => SR.GetResourceString(nameof (Globalization_cp_1361), (string) null);
  }

  internal static string Globalization_cp_10000
  {
    get => SR.GetResourceString(nameof (Globalization_cp_10000), (string) null);
  }

  internal static string Globalization_cp_10001
  {
    get => SR.GetResourceString(nameof (Globalization_cp_10001), (string) null);
  }

  internal static string Globalization_cp_10002
  {
    get => SR.GetResourceString(nameof (Globalization_cp_10002), (string) null);
  }

  internal static string Globalization_cp_10003
  {
    get => SR.GetResourceString(nameof (Globalization_cp_10003), (string) null);
  }

  internal static string Globalization_cp_10004
  {
    get => SR.GetResourceString(nameof (Globalization_cp_10004), (string) null);
  }

  internal static string Globalization_cp_10005
  {
    get => SR.GetResourceString(nameof (Globalization_cp_10005), (string) null);
  }

  internal static string Globalization_cp_10006
  {
    get => SR.GetResourceString(nameof (Globalization_cp_10006), (string) null);
  }

  internal static string Globalization_cp_10007
  {
    get => SR.GetResourceString(nameof (Globalization_cp_10007), (string) null);
  }

  internal static string Globalization_cp_10008
  {
    get => SR.GetResourceString(nameof (Globalization_cp_10008), (string) null);
  }

  internal static string Globalization_cp_10010
  {
    get => SR.GetResourceString(nameof (Globalization_cp_10010), (string) null);
  }

  internal static string Globalization_cp_10017
  {
    get => SR.GetResourceString(nameof (Globalization_cp_10017), (string) null);
  }

  internal static string Globalization_cp_10021
  {
    get => SR.GetResourceString(nameof (Globalization_cp_10021), (string) null);
  }

  internal static string Globalization_cp_10029
  {
    get => SR.GetResourceString(nameof (Globalization_cp_10029), (string) null);
  }

  internal static string Globalization_cp_10079
  {
    get => SR.GetResourceString(nameof (Globalization_cp_10079), (string) null);
  }

  internal static string Globalization_cp_10081
  {
    get => SR.GetResourceString(nameof (Globalization_cp_10081), (string) null);
  }

  internal static string Globalization_cp_10082
  {
    get => SR.GetResourceString(nameof (Globalization_cp_10082), (string) null);
  }

  internal static string Globalization_cp_20000
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20000), (string) null);
  }

  internal static string Globalization_cp_20001
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20001), (string) null);
  }

  internal static string Globalization_cp_20002
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20002), (string) null);
  }

  internal static string Globalization_cp_20003
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20003), (string) null);
  }

  internal static string Globalization_cp_20004
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20004), (string) null);
  }

  internal static string Globalization_cp_20005
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20005), (string) null);
  }

  internal static string Globalization_cp_20105
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20105), (string) null);
  }

  internal static string Globalization_cp_20106
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20106), (string) null);
  }

  internal static string Globalization_cp_20107
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20107), (string) null);
  }

  internal static string Globalization_cp_20108
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20108), (string) null);
  }

  internal static string Globalization_cp_20261
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20261), (string) null);
  }

  internal static string Globalization_cp_20269
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20269), (string) null);
  }

  internal static string Globalization_cp_20273
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20273), (string) null);
  }

  internal static string Globalization_cp_20277
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20277), (string) null);
  }

  internal static string Globalization_cp_20278
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20278), (string) null);
  }

  internal static string Globalization_cp_20280
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20280), (string) null);
  }

  internal static string Globalization_cp_20284
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20284), (string) null);
  }

  internal static string Globalization_cp_20285
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20285), (string) null);
  }

  internal static string Globalization_cp_20290
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20290), (string) null);
  }

  internal static string Globalization_cp_20297
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20297), (string) null);
  }

  internal static string Globalization_cp_20420
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20420), (string) null);
  }

  internal static string Globalization_cp_20423
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20423), (string) null);
  }

  internal static string Globalization_cp_20424
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20424), (string) null);
  }

  internal static string Globalization_cp_20833
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20833), (string) null);
  }

  internal static string Globalization_cp_20838
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20838), (string) null);
  }

  internal static string Globalization_cp_20866
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20866), (string) null);
  }

  internal static string Globalization_cp_20871
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20871), (string) null);
  }

  internal static string Globalization_cp_20880
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20880), (string) null);
  }

  internal static string Globalization_cp_20905
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20905), (string) null);
  }

  internal static string Globalization_cp_20924
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20924), (string) null);
  }

  internal static string Globalization_cp_20932
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20932), (string) null);
  }

  internal static string Globalization_cp_20936
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20936), (string) null);
  }

  internal static string Globalization_cp_20949
  {
    get => SR.GetResourceString(nameof (Globalization_cp_20949), (string) null);
  }

  internal static string Globalization_cp_21025
  {
    get => SR.GetResourceString(nameof (Globalization_cp_21025), (string) null);
  }

  internal static string Globalization_cp_21027
  {
    get => SR.GetResourceString(nameof (Globalization_cp_21027), (string) null);
  }

  internal static string Globalization_cp_21866
  {
    get => SR.GetResourceString(nameof (Globalization_cp_21866), (string) null);
  }

  internal static string Globalization_cp_28592
  {
    get => SR.GetResourceString(nameof (Globalization_cp_28592), (string) null);
  }

  internal static string Globalization_cp_28593
  {
    get => SR.GetResourceString(nameof (Globalization_cp_28593), (string) null);
  }

  internal static string Globalization_cp_28594
  {
    get => SR.GetResourceString(nameof (Globalization_cp_28594), (string) null);
  }

  internal static string Globalization_cp_28595
  {
    get => SR.GetResourceString(nameof (Globalization_cp_28595), (string) null);
  }

  internal static string Globalization_cp_28596
  {
    get => SR.GetResourceString(nameof (Globalization_cp_28596), (string) null);
  }

  internal static string Globalization_cp_28597
  {
    get => SR.GetResourceString(nameof (Globalization_cp_28597), (string) null);
  }

  internal static string Globalization_cp_28598
  {
    get => SR.GetResourceString(nameof (Globalization_cp_28598), (string) null);
  }

  internal static string Globalization_cp_28599
  {
    get => SR.GetResourceString(nameof (Globalization_cp_28599), (string) null);
  }

  internal static string Globalization_cp_28603
  {
    get => SR.GetResourceString(nameof (Globalization_cp_28603), (string) null);
  }

  internal static string Globalization_cp_28605
  {
    get => SR.GetResourceString(nameof (Globalization_cp_28605), (string) null);
  }

  internal static string Globalization_cp_29001
  {
    get => SR.GetResourceString(nameof (Globalization_cp_29001), (string) null);
  }

  internal static string Globalization_cp_38598
  {
    get => SR.GetResourceString(nameof (Globalization_cp_38598), (string) null);
  }

  internal static string Globalization_cp_50000
  {
    get => SR.GetResourceString(nameof (Globalization_cp_50000), (string) null);
  }

  internal static string Globalization_cp_50220
  {
    get => SR.GetResourceString(nameof (Globalization_cp_50220), (string) null);
  }

  internal static string Globalization_cp_50221
  {
    get => SR.GetResourceString(nameof (Globalization_cp_50221), (string) null);
  }

  internal static string Globalization_cp_50222
  {
    get => SR.GetResourceString(nameof (Globalization_cp_50222), (string) null);
  }

  internal static string Globalization_cp_50225
  {
    get => SR.GetResourceString(nameof (Globalization_cp_50225), (string) null);
  }

  internal static string Globalization_cp_50227
  {
    get => SR.GetResourceString(nameof (Globalization_cp_50227), (string) null);
  }

  internal static string Globalization_cp_50229
  {
    get => SR.GetResourceString(nameof (Globalization_cp_50229), (string) null);
  }

  internal static string Globalization_cp_50930
  {
    get => SR.GetResourceString(nameof (Globalization_cp_50930), (string) null);
  }

  internal static string Globalization_cp_50931
  {
    get => SR.GetResourceString(nameof (Globalization_cp_50931), (string) null);
  }

  internal static string Globalization_cp_50933
  {
    get => SR.GetResourceString(nameof (Globalization_cp_50933), (string) null);
  }

  internal static string Globalization_cp_50935
  {
    get => SR.GetResourceString(nameof (Globalization_cp_50935), (string) null);
  }

  internal static string Globalization_cp_50937
  {
    get => SR.GetResourceString(nameof (Globalization_cp_50937), (string) null);
  }

  internal static string Globalization_cp_50939
  {
    get => SR.GetResourceString(nameof (Globalization_cp_50939), (string) null);
  }

  internal static string Globalization_cp_51932
  {
    get => SR.GetResourceString(nameof (Globalization_cp_51932), (string) null);
  }

  internal static string Globalization_cp_51936
  {
    get => SR.GetResourceString(nameof (Globalization_cp_51936), (string) null);
  }

  internal static string Globalization_cp_51949
  {
    get => SR.GetResourceString(nameof (Globalization_cp_51949), (string) null);
  }

  internal static string Globalization_cp_52936
  {
    get => SR.GetResourceString(nameof (Globalization_cp_52936), (string) null);
  }

  internal static string Globalization_cp_54936
  {
    get => SR.GetResourceString(nameof (Globalization_cp_54936), (string) null);
  }

  internal static string Globalization_cp_57002
  {
    get => SR.GetResourceString(nameof (Globalization_cp_57002), (string) null);
  }

  internal static string Globalization_cp_57003
  {
    get => SR.GetResourceString(nameof (Globalization_cp_57003), (string) null);
  }

  internal static string Globalization_cp_57004
  {
    get => SR.GetResourceString(nameof (Globalization_cp_57004), (string) null);
  }

  internal static string Globalization_cp_57005
  {
    get => SR.GetResourceString(nameof (Globalization_cp_57005), (string) null);
  }

  internal static string Globalization_cp_57006
  {
    get => SR.GetResourceString(nameof (Globalization_cp_57006), (string) null);
  }

  internal static string Globalization_cp_57007
  {
    get => SR.GetResourceString(nameof (Globalization_cp_57007), (string) null);
  }

  internal static string Globalization_cp_57008
  {
    get => SR.GetResourceString(nameof (Globalization_cp_57008), (string) null);
  }

  internal static string Globalization_cp_57009
  {
    get => SR.GetResourceString(nameof (Globalization_cp_57009), (string) null);
  }

  internal static string Globalization_cp_57010
  {
    get => SR.GetResourceString(nameof (Globalization_cp_57010), (string) null);
  }

  internal static string Globalization_cp_57011
  {
    get => SR.GetResourceString(nameof (Globalization_cp_57011), (string) null);
  }
}
