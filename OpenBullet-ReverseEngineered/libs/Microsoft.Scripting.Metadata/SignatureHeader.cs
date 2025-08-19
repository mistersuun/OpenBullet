// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.SignatureHeader
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

public static class SignatureHeader
{
  public const byte DefaultCall = 0;
  public const byte CCall = 1;
  public const byte StdCall = 2;
  public const byte ThisCall = 3;
  public const byte FastCall = 4;
  public const byte VarArgCall = 5;
  public const byte Field = 6;
  public const byte LocalVar = 7;
  public const byte Property = 8;
  public const byte GenericInstance = 10;
  public const byte Max = 12;
  public const byte CallingConventionMask = 15;
  public const byte HasThis = 32 /*0x20*/;
  public const byte ExplicitThis = 64 /*0x40*/;
  public const byte Generic = 16 /*0x10*/;

  public static bool IsMethodSignature(byte signatureHeader) => ((int) signatureHeader & 15) <= 5;

  public static bool IsVarArgCallSignature(byte signatureHeader)
  {
    return ((int) signatureHeader & 15) == 5;
  }

  public static bool IsFieldSignature(byte signatureHeader) => ((int) signatureHeader & 15) == 6;

  public static bool IsLocalVarSignature(byte signatureHeader) => ((int) signatureHeader & 15) == 7;

  public static bool IsPropertySignature(byte signatureHeader) => ((int) signatureHeader & 15) == 8;

  public static bool IsGenericInstanceSignature(byte signatureHeader)
  {
    return ((int) signatureHeader & 15) == 10;
  }

  public static bool IsExplicitThis(byte signatureHeader)
  {
    return ((int) signatureHeader & 64 /*0x40*/) == 64 /*0x40*/;
  }

  public static bool IsGeneric(byte signatureHeader)
  {
    return ((int) signatureHeader & 16 /*0x10*/) == 16 /*0x10*/;
  }
}
