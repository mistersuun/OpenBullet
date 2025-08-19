// Decompiled with JetBrains decompiler
// Type: IronPython.Hosting.ErrorCodes
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Hosting;

public static class ErrorCodes
{
  public const int IncompleteMask = 15;
  public const int IncompleteStatement = 1;
  public const int IncompleteToken = 2;
  public const int ErrorMask = 2147483632;
  public const int SyntaxError = 16 /*0x10*/;
  public const int IndentationError = 32 /*0x20*/;
  public const int TabError = 48 /*0x30*/;
  public const int NoCaret = 64 /*0x40*/;
}
