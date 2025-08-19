// Decompiled with JetBrains decompiler
// Type: Tesseract.Interop.OSResult
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;

#nullable disable
namespace Tesseract.Interop;

public struct OSResult
{
  private const int kMaxNumberOfScripts = 120;
  private unsafe fixed float _orientations[4];
  private unsafe fixed float _scriptsNA[480];
  private IntPtr _unicharset;
  private OSBestResult _bestResult;

  public unsafe void Init()
  {
    fixed (float* numPtr1 = this._orientations)
      fixed (float* numPtr2 = this._scriptsNA)
      {
        for (int index1 = 0; index1 < 4; ++index1)
        {
          for (int index2 = 0; index2 < 120; ++index2)
            numPtr2[index1 * 120 + index2] = 0.0f;
          numPtr1[index1] = 0.0f;
        }
      }
    this._unicharset = IntPtr.Zero;
    this._bestResult = new OSBestResult();
  }

  public void GetBestOrientation(out Orientation orientation, out float confidence)
  {
    switch (this._bestResult.OrientationId)
    {
      case 0:
        orientation = Orientation.PageUp;
        break;
      case 1:
        orientation = Orientation.PageRight;
        break;
      case 2:
        orientation = Orientation.PageDown;
        break;
      case 3:
        orientation = Orientation.PageLeft;
        break;
      default:
        throw new InvalidOperationException($"Best orientation must be between 0 and 3 but was {(object) this._bestResult.OrientationId}.");
    }
    confidence = this._bestResult.OConfidence;
  }
}
