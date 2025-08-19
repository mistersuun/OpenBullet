// Decompiled with JetBrains decompiler
// Type: System.Net.Http.StreamToStreamCopy
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.IO;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace System.Net.Http;

internal static class StreamToStreamCopy
{
  [ThreadStatic]
  private static byte[] t_singleByteArray;

  public static Task CopyAsync(
    Stream source,
    Stream destination,
    int bufferSize,
    bool disposeSource,
    CancellationToken cancellationToken = default (CancellationToken))
  {
    if (cancellationToken.IsCancellationRequested)
      return Task.FromCanceled(cancellationToken);
    try
    {
      ArraySegment<byte> buffer1;
      if (source is MemoryStream source1 && source1.TryGetBuffer(ref buffer1))
      {
        long num = source.CanSeek ? source.Position : -1L;
        if (num >= 0L && num < (long) buffer1.Count)
        {
          if (num != 0L)
            buffer1 = new ArraySegment<byte>(buffer1.Array, (int) checked ((long) buffer1.Offset + num), (int) checked ((long) buffer1.Count - num));
          source.Position += (long) buffer1.Count;
          return StreamToStreamCopy.WriteToAnyStreamAsync(buffer1, source, destination, disposeSource, cancellationToken);
        }
      }
      if (destination is HttpContent.LimitMemoryStream destination1)
      {
        int capacity = destination1.Capacity;
        if (capacity > 0 && capacity <= destination1.MaxSize)
          return StreamToStreamCopy.CopyAsyncToPreSizedLimitMemoryStream(source, destination1, bufferSize, disposeSource, cancellationToken);
      }
      if (source1 == null || !source1.CanSeek || source1.Position != 0L || source1.Length > (long) bufferSize)
        return disposeSource ? StreamToStreamCopy.CopyAsyncAnyStreamToAnyStreamCore(source, destination, bufferSize, disposeSource, cancellationToken) : source.CopyToAsync(destination, bufferSize, cancellationToken);
      ArraySegment<byte> buffer2 = new ArraySegment<byte>(source1.ToArray());
      source1.Position = (long) buffer2.Count;
      return StreamToStreamCopy.WriteToAnyStreamAsync(buffer2, (Stream) source1, destination, disposeSource, cancellationToken);
    }
    catch (Exception ex)
    {
      return Task.FromException(ex);
    }
  }

  private static async Task WriteToAnyStreamAsync(
    ArraySegment<byte> buffer,
    Stream source,
    Stream destination,
    bool disposeSource,
    CancellationToken cancellationToken)
  {
    await destination.WriteAsync(buffer.Array, buffer.Offset, buffer.Count, cancellationToken).ConfigureAwait(false);
    StreamToStreamCopy.DisposeSource(disposeSource, source);
  }

  private static async Task CopyAsyncToPreSizedLimitMemoryStream(
    Stream source,
    HttpContent.LimitMemoryStream destination,
    int bufferSize,
    bool disposeSource,
    CancellationToken cancellationToken)
  {
    long capacity = (long) destination.Capacity;
    long startingLength = destination.Length;
    if (startingLength < capacity)
      destination.SetLength(capacity);
    try
    {
      ArraySegment<byte> entireBuffer;
      destination.TryGetBuffer(ref entireBuffer);
      int num;
      for (int spaceRemaining = (int) ((long) entireBuffer.Array.Length - destination.Position); spaceRemaining > 0; spaceRemaining -= num)
      {
        num = await source.ReadAsync(entireBuffer.Array, (int) destination.Position, spaceRemaining, cancellationToken).ConfigureAwait(false);
        if (num == 0)
        {
          StreamToStreamCopy.DisposeSource(disposeSource, source);
          return;
        }
        destination.Position += (long) num;
      }
      entireBuffer = new ArraySegment<byte>();
    }
    finally
    {
      if (startingLength < capacity)
        destination.SetLength(destination.Position);
    }
    byte[] singleByteArray = StreamToStreamCopy.RentCachedSingleByteArray();
    if (await source.ReadAsync(singleByteArray, 0, 1, cancellationToken).ConfigureAwait(false) == 0)
    {
      StreamToStreamCopy.ReturnCachedSingleByteArray(singleByteArray);
      StreamToStreamCopy.DisposeSource(disposeSource, source);
    }
    else
    {
      await ((Stream) destination).WriteAsync(singleByteArray, 0, 1, cancellationToken).ConfigureAwait(false);
      StreamToStreamCopy.ReturnCachedSingleByteArray(singleByteArray);
      await StreamToStreamCopy.CopyAsyncAnyStreamToAnyStreamCore(source, (Stream) destination, bufferSize, disposeSource, cancellationToken).ConfigureAwait(false);
    }
  }

  private static async Task CopyAsyncAnyStreamToAnyStreamCore(
    Stream source,
    Stream destination,
    int bufferSize,
    bool disposeSource,
    CancellationToken cancellationToken)
  {
    byte[] buffer = new byte[bufferSize];
    while (true)
    {
      int num = await source.ReadAsync(buffer, 0, bufferSize, cancellationToken).ConfigureAwait(false);
      int bytesRead;
      if ((bytesRead = num) > 0)
        await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
      else
        break;
    }
    StreamToStreamCopy.DisposeSource(disposeSource, source);
  }

  private static byte[] RentCachedSingleByteArray()
  {
    byte[] tSingleByteArray = StreamToStreamCopy.t_singleByteArray;
    if (tSingleByteArray == null)
      return new byte[1];
    StreamToStreamCopy.t_singleByteArray = (byte[]) null;
    return tSingleByteArray;
  }

  private static void ReturnCachedSingleByteArray(byte[] singleByteArray)
  {
    StreamToStreamCopy.t_singleByteArray = singleByteArray;
  }

  private static void DisposeSource(bool disposeSource, Stream source)
  {
    if (!disposeSource)
      return;
    try
    {
      source.Dispose();
    }
    catch (Exception ex)
    {
      if (!NetEventSource.Log.IsEnabled())
        return;
      NetEventSource.Exception(NetEventSource.ComponentType.Http, (object) null, "CopyAsync", ex);
    }
  }
}
