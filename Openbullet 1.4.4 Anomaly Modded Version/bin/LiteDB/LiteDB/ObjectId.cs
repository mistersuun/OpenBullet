// Decompiled with JetBrains decompiler
// Type: LiteDB.ObjectId
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;

#nullable disable
namespace LiteDB;

public class ObjectId : IComparable<ObjectId>, IEquatable<ObjectId>
{
  public static readonly ObjectId Empty = new ObjectId();
  private static int _machine = ObjectId.GetMachineHash() + AppDomain.CurrentDomain.Id & 16777215 /*0xFFFFFF*/;
  private static short _pid;
  private static int _increment = new Random().Next();

  public int Timestamp { get; private set; }

  public int Machine { get; private set; }

  public short Pid { get; private set; }

  public int Increment { get; private set; }

  public DateTime CreationTime => BsonValue.UnixEpoch.AddSeconds((double) this.Timestamp);

  public ObjectId()
  {
    this.Timestamp = 0;
    this.Machine = 0;
    this.Pid = (short) 0;
    this.Increment = 0;
  }

  public ObjectId(int timestamp, int machine, short pid, int increment)
  {
    this.Timestamp = timestamp;
    this.Machine = machine;
    this.Pid = pid;
    this.Increment = increment;
  }

  public ObjectId(ObjectId from)
  {
    this.Timestamp = from.Timestamp;
    this.Machine = from.Machine;
    this.Pid = from.Pid;
    this.Increment = from.Increment;
  }

  public ObjectId(string value)
    : this(ObjectId.FromHex(value))
  {
  }

  public ObjectId(byte[] bytes)
  {
    if (bytes == null)
      throw new ArgumentNullException(nameof (bytes));
    if (bytes.Length != 12)
      throw new ArgumentException(nameof (bytes), "Byte array must be 12 bytes long");
    this.Timestamp = ((int) bytes[0] << 24) + ((int) bytes[1] << 16 /*0x10*/) + ((int) bytes[2] << 8) + (int) bytes[3];
    this.Machine = ((int) bytes[4] << 16 /*0x10*/) + ((int) bytes[5] << 8) + (int) bytes[6];
    this.Pid = (short) (((int) bytes[7] << 8) + (int) bytes[8]);
    this.Increment = ((int) bytes[9] << 16 /*0x10*/) + ((int) bytes[10] << 8) + (int) bytes[11];
  }

  private static byte[] FromHex(string value)
  {
    if (string.IsNullOrEmpty(value))
      throw new ArgumentNullException(nameof (value));
    if (value.Length != 24)
      throw new ArgumentException($"ObjectId strings should be 24 hex characters, got {value.Length} : \"{value}\"");
    byte[] numArray = new byte[12];
    for (int startIndex = 0; startIndex < 24; startIndex += 2)
      numArray[startIndex / 2] = Convert.ToByte(value.Substring(startIndex, 2), 16 /*0x10*/);
    return numArray;
  }

  public bool Equals(ObjectId other)
  {
    return other != (ObjectId) null && this.Timestamp == other.Timestamp && this.Machine == other.Machine && (int) this.Pid == (int) other.Pid && this.Increment == other.Increment;
  }

  public override bool Equals(object other) => this.Equals(other as ObjectId);

  public override int GetHashCode()
  {
    return 37 * (37 * (37 * (37 * 17 + this.Timestamp.GetHashCode()) + this.Machine.GetHashCode()) + this.Pid.GetHashCode()) + this.Increment.GetHashCode();
  }

  public int CompareTo(ObjectId other)
  {
    int num1 = this.Timestamp.CompareTo(other.Timestamp);
    if (num1 != 0)
      return num1;
    int num2 = this.Machine.CompareTo(other.Machine);
    if (num2 != 0)
      return num2;
    int num3 = this.Pid.CompareTo(other.Pid);
    if (num3 == 0)
      return this.Increment.CompareTo(other.Increment);
    return num3 >= 0 ? 1 : -1;
  }

  public byte[] ToByteArray()
  {
    return new byte[12]
    {
      (byte) (this.Timestamp >> 24),
      (byte) (this.Timestamp >> 16 /*0x10*/),
      (byte) (this.Timestamp >> 8),
      (byte) this.Timestamp,
      (byte) (this.Machine >> 16 /*0x10*/),
      (byte) (this.Machine >> 8),
      (byte) this.Machine,
      (byte) ((uint) this.Pid >> 8),
      (byte) this.Pid,
      (byte) (this.Increment >> 16 /*0x10*/),
      (byte) (this.Increment >> 8),
      (byte) this.Increment
    };
  }

  public override string ToString()
  {
    return BitConverter.ToString(this.ToByteArray()).Replace("-", "").ToLower();
  }

  public static bool operator ==(ObjectId lhs, ObjectId rhs)
  {
    if ((object) lhs == null)
      return (object) rhs == null;
    return (object) rhs != null && lhs.Equals(rhs);
  }

  public static bool operator !=(ObjectId lhs, ObjectId rhs) => !(lhs == rhs);

  public static bool operator >=(ObjectId lhs, ObjectId rhs) => lhs.CompareTo(rhs) >= 0;

  public static bool operator >(ObjectId lhs, ObjectId rhs) => lhs.CompareTo(rhs) > 0;

  public static bool operator <(ObjectId lhs, ObjectId rhs) => lhs.CompareTo(rhs) < 0;

  public static bool operator <=(ObjectId lhs, ObjectId rhs) => lhs.CompareTo(rhs) <= 0;

  static ObjectId()
  {
    try
    {
      ObjectId._pid = (short) ObjectId.GetCurrentProcessId();
    }
    catch (SecurityException ex)
    {
      ObjectId._pid = (short) 0;
    }
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private static int GetCurrentProcessId() => Process.GetCurrentProcess().Id;

  private static int GetMachineHash()
  {
    return 16777215 /*0xFFFFFF*/ & Environment.MachineName.GetHashCode();
  }

  public static ObjectId NewObjectId()
  {
    long timestamp = (long) Math.Floor((DateTime.UtcNow - BsonValue.UnixEpoch).TotalSeconds);
    int increment = Interlocked.Increment(ref ObjectId._increment) & 16777215 /*0xFFFFFF*/;
    return new ObjectId((int) timestamp, ObjectId._machine, ObjectId._pid, increment);
  }
}
