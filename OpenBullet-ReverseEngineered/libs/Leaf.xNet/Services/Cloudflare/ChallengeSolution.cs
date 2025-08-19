// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.Services.Cloudflare.ChallengeSolution
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;
using System.Globalization;

#nullable disable
namespace Leaf.xNet.Services.Cloudflare;

public struct ChallengeSolution(
  string clearancePage,
  string verificationCode,
  string pass,
  double answer,
  string s,
  bool containsIntegerTag) : IEquatable<ChallengeSolution>
{
  public string ClearancePage { get; } = clearancePage;

  public string VerificationCode { get; } = verificationCode;

  public string Pass { get; } = pass;

  public double Answer { get; } = answer;

  public string S { get; } = s;

  public bool ContainsIntegerTag { get; } = containsIntegerTag;

  public string ClearanceQuery
  {
    get
    {
      return string.IsNullOrEmpty(this.S) ? $"{this.ClearancePage}?jschl_vc={this.VerificationCode}&pass={this.Pass}&jschl_answer={this.Answer.ToString("R", (IFormatProvider) CultureInfo.InvariantCulture)}" : $"{this.ClearancePage}?s={Uri.EscapeDataString(this.S)}&jschl_vc={this.VerificationCode}&pass={this.Pass}&jschl_answer={this.Answer.ToString("R", (IFormatProvider) CultureInfo.InvariantCulture)}";
    }
  }

  public static bool operator ==(ChallengeSolution solutionA, ChallengeSolution solutionB)
  {
    return solutionA.Equals(solutionB);
  }

  public static bool operator !=(ChallengeSolution solutionA, ChallengeSolution solutionB)
  {
    return !(solutionA == solutionB);
  }

  public override bool Equals(object obj) => obj is ChallengeSolution other && this.Equals(other);

  public override int GetHashCode() => this.ClearanceQuery.GetHashCode();

  public bool Equals(ChallengeSolution other) => other.ClearanceQuery == this.ClearanceQuery;
}
