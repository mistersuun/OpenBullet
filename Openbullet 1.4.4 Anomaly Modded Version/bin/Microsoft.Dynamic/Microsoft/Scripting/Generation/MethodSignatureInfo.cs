// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Generation.MethodSignatureInfo
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Generation;

public class MethodSignatureInfo
{
  private readonly ParameterInfo[] _pis;
  private readonly bool _isStatic;
  private readonly int _genericArity;

  public MethodSignatureInfo(MethodInfo info)
    : this(info.IsStatic, info.GetParameters(), info.IsGenericMethodDefinition ? info.GetGenericArguments().Length : 0)
  {
  }

  public MethodSignatureInfo(bool isStatic, ParameterInfo[] pis, int genericArity)
  {
    this._isStatic = isStatic;
    this._pis = pis;
    this._genericArity = genericArity;
  }

  public override bool Equals(object obj)
  {
    if (!(obj is MethodSignatureInfo methodSignatureInfo) || methodSignatureInfo._isStatic != this._isStatic || methodSignatureInfo._pis.Length != this._pis.Length || methodSignatureInfo._genericArity != this._genericArity)
      return false;
    for (int index = 0; index < this._pis.Length; ++index)
    {
      if (this._pis[index].ParameterType != methodSignatureInfo._pis[index].ParameterType)
        return false;
    }
    return true;
  }

  public override int GetHashCode()
  {
    int hashCode = 6551 ^ (this._isStatic ? 79234 : 3123) ^ this._genericArity;
    foreach (ParameterInfo pi in this._pis)
      hashCode ^= pi.ParameterType.GetHashCode();
    return hashCode;
  }
}
