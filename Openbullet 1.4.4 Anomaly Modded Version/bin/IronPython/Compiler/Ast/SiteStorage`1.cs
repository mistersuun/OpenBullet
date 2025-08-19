﻿// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.SiteStorage`1
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler.Ast;

public static class SiteStorage<T> where T : class
{
  public static int SiteCount;
  public static CallSite<T>[] Sites = new CallSite<T>[64 /*0x40*/];

  public static Type SiteStorageType(int index)
  {
    switch (index / 50)
    {
      case 0:
        return typeof (SiteStorage000<T>);
      case 1:
        return typeof (SiteStorage001<T>);
      case 2:
        return typeof (SiteStorage002<T>);
      case 3:
        return typeof (SiteStorage003<T>);
      case 4:
        return typeof (SiteStorage004<T>);
      case 5:
        return typeof (SiteStorage005<T>);
      case 6:
        return typeof (SiteStorage006<T>);
      case 7:
        return typeof (SiteStorage007<T>);
      case 8:
        return typeof (SiteStorage008<T>);
      case 9:
        return typeof (SiteStorage009<T>);
      case 10:
        return typeof (SiteStorage010<T>);
      case 11:
        return typeof (SiteStorage011<T>);
      case 12:
        return typeof (SiteStorage012<T>);
      case 13:
        return typeof (SiteStorage013<T>);
      case 14:
        return typeof (SiteStorage014<T>);
      case 15:
        return typeof (SiteStorage015<T>);
      case 16 /*0x10*/:
        return typeof (SiteStorage016<T>);
      case 17:
        return typeof (SiteStorage017<T>);
      case 18:
        return typeof (SiteStorage018<T>);
      case 19:
        return typeof (SiteStorage019<T>);
      case 20:
        return typeof (SiteStorage020<T>);
      case 21:
        return typeof (SiteStorage021<T>);
      case 22:
        return typeof (SiteStorage022<T>);
      case 23:
        return typeof (SiteStorage023<T>);
      case 24:
        return typeof (SiteStorage024<T>);
      case 25:
        return typeof (SiteStorage025<T>);
      case 26:
        return typeof (SiteStorage026<T>);
      case 27:
        return typeof (SiteStorage027<T>);
      case 28:
        return typeof (SiteStorage028<T>);
      case 29:
        return typeof (SiteStorage029<T>);
      default:
        int num = checked (index - 1500 + 1);
        if (SiteStorage<T>.Sites.Length < num)
        {
          int length = SiteStorage<T>.Sites.Length;
          while (length < num)
            length *= 2;
          Array.Resize<CallSite<T>>(ref SiteStorage<T>.Sites, length);
        }
        return typeof (SiteStorage<T>);
    }
  }
}
