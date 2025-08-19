// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Logging.TextWriterEventListener
// Assembly: Microsoft.IdentityModel.Logging, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C4950808-8A1B-4796-AB1F-9647EE1EB5BA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Logging.dll

using System;
using System.Diagnostics.Tracing;
using System.IO;

#nullable disable
namespace Microsoft.IdentityModel.Logging;

public class TextWriterEventListener : EventListener
{
  private StreamWriter _streamWriter;
  private bool _disposeStreamWriter = true;
  public static readonly string DefaultLogFileName = "IdentityModelLogs.txt";

  public TextWriterEventListener()
  {
    try
    {
      this._streamWriter = new StreamWriter((Stream) new FileStream(TextWriterEventListener.DefaultLogFileName, FileMode.OpenOrCreate, FileAccess.Write));
      this._streamWriter.AutoFlush = true;
    }
    catch (Exception ex)
    {
      LogHelper.LogExceptionMessage((Exception) new InvalidOperationException("MIML10001: Cannot create the fileStream or StreamWriter to write logs. See inner exception.", ex));
      throw;
    }
  }

  public TextWriterEventListener(string filePath)
  {
    if (string.IsNullOrEmpty(filePath))
      throw LogHelper.LogArgumentNullException(nameof (filePath));
    try
    {
      this._streamWriter = new StreamWriter((Stream) new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write));
      this._streamWriter.AutoFlush = true;
    }
    catch (Exception ex)
    {
      LogHelper.LogExceptionMessage((Exception) new InvalidOperationException("MIML10001: Cannot create the fileStream or StreamWriter to write logs. See inner exception.", ex));
      throw;
    }
  }

  public TextWriterEventListener(StreamWriter streamWriter)
  {
    this._streamWriter = streamWriter != null ? streamWriter : throw LogHelper.LogArgumentNullException(nameof (streamWriter));
    this._disposeStreamWriter = false;
  }

  protected virtual void OnEventWritten(EventWrittenEventArgs eventData)
  {
    if (eventData == null)
      throw LogHelper.LogArgumentNullException(nameof (eventData));
    if (eventData.Payload == null || eventData.Payload.Count <= 0)
    {
      LogHelper.LogInformation("MIML10000: eventData.Payload is null or empty. Not logging any messages.");
    }
    else
    {
      for (int index = 0; index < eventData.Payload.Count; ++index)
        this._streamWriter.WriteLine(eventData.Payload[index].ToString());
    }
  }

  public virtual void Dispose()
  {
    if (this._disposeStreamWriter && this._streamWriter != null)
    {
      this._streamWriter.Flush();
      this._streamWriter.Dispose();
    }
    base.Dispose();
  }
}
