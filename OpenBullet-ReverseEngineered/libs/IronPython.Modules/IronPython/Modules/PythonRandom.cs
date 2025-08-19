// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonRandom
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Utils;
using System;
using System.Linq;
using System.Numerics;

#nullable disable
namespace IronPython.Modules;

public static class PythonRandom
{
  public const string __doc__ = "implements a random number generator";

  [PythonType]
  public class Random
  {
    private PythonRandom.RandomGen _rnd;

    public Random(object seed = null) => this.seed(seed);

    public object getrandbits(int bits)
    {
      if (bits <= 0)
        throw PythonOps.ValueError("number of bits must be greater than zero");
      lock (this)
        return (object) MathUtils.GetRandBits(new Action<byte[]>(this._rnd.NextBytes), bits);
    }

    public PythonTuple getstate()
    {
      object[] state;
      lock (this)
        state = this._rnd.GetState();
      return PythonTuple.MakeTuple(state);
    }

    public void jumpahead(int count)
    {
      lock (this)
        this._rnd.NextBytes(new byte[4096 /*0x1000*/]);
    }

    public void jumpahead(BigInteger count)
    {
      lock (this)
        this._rnd.NextBytes(new byte[4096 /*0x1000*/]);
    }

    public void jumpahead(object count)
    {
      throw PythonOps.TypeError("jumpahead requires an integer, not '{0}'", (object) PythonOps.GetPythonTypeName(count));
    }

    public object random()
    {
      lock (this)
      {
        byte[] buffer1 = new byte[4];
        byte[] buffer2 = new byte[4];
        this._rnd.NextBytes(buffer1);
        this._rnd.NextBytes(buffer2);
        return (object) (((double) (BitConverter.ToUInt32(buffer1, 0) >> 5) * 67108864.0 + (double) (BitConverter.ToUInt32(buffer2, 0) >> 6)) * 1.1102230246251565E-16);
      }
    }

    public void seed(object s = null)
    {
      object obj = s;
      int Seed;
      if (obj != null)
      {
        if (obj is int num)
          Seed = num;
        else
          Seed = PythonContext.IsHashable(s) ? s.GetHashCode() : throw PythonOps.TypeError("unhashable type: '{0}'", (object) PythonOps.GetPythonTypeName(s));
      }
      else
        Seed = DateTime.Now.GetHashCode();
      lock (this)
        this._rnd = new PythonRandom.RandomGen(Seed);
    }

    public void setstate(PythonTuple state)
    {
      int[] state1 = state.Count == 58 ? state._data.Cast<int>().ToArray<int>() : throw PythonOps.ValueError("state vector is the wrong size");
      lock (this)
        this._rnd.SetState(state1);
    }
  }

  private class RandomGen
  {
    private const int MBIG = 2147483647 /*0x7FFFFFFF*/;
    private const int MSEED = 161803398;
    private int _inext;
    private int _inextp;
    private int[] _seedArray = new int[56];

    public RandomGen(int Seed)
    {
      int index1 = 0;
      int num1 = 161803398 - (Seed == int.MinValue ? int.MaxValue : Math.Abs(Seed));
      this._seedArray[55] = num1;
      int num2 = 1;
      for (int index2 = 1; index2 < 55; ++index2)
      {
        if ((index1 += 21) >= 55)
          index1 -= 55;
        this._seedArray[index1] = num2;
        num2 = num1 - num2;
        if (num2 < 0)
          num2 += int.MaxValue;
        num1 = this._seedArray[index1];
      }
      for (int index3 = 1; index3 < 5; ++index3)
      {
        for (int index4 = 1; index4 < 56; ++index4)
        {
          int num3 = index4 + 30;
          if (num3 >= 55)
            num3 -= 55;
          this._seedArray[index4] -= this._seedArray[1 + num3];
          if (this._seedArray[index4] < 0)
            this._seedArray[index4] += int.MaxValue;
        }
      }
      this._inext = 0;
      this._inextp = 21;
      Seed = 1;
    }

    private int InternalSample()
    {
      int inext = this._inext;
      int inextp = this._inextp;
      int index1;
      if ((index1 = inext + 1) >= 56)
        index1 = 1;
      int index2;
      if ((index2 = inextp + 1) >= 56)
        index2 = 1;
      int num = this._seedArray[index1] - this._seedArray[index2];
      if (num == int.MaxValue)
        --num;
      if (num < 0)
        num += int.MaxValue;
      this._seedArray[index1] = num;
      this._inext = index1;
      this._inextp = index2;
      return num;
    }

    public void NextBytes(byte[] buffer)
    {
      if (buffer == null)
        throw new ArgumentNullException(nameof (buffer));
      for (int index = 0; index < buffer.Length; ++index)
        buffer[index] = (byte) this.InternalSample();
    }

    internal object[] GetState()
    {
      object[] destinationArray = new object[58];
      Array.Copy((Array) this._seedArray, (Array) destinationArray, this._seedArray.Length);
      destinationArray[56] = (object) this._inext;
      destinationArray[57] = (object) this._inextp;
      return destinationArray;
    }

    internal void SetState(int[] state)
    {
      Array.Copy((Array) state, (Array) this._seedArray, this._seedArray.Length);
      this._inext = state[56];
      this._inextp = state[57];
    }
  }
}
