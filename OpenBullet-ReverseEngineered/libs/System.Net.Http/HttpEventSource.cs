// Decompiled with JetBrains decompiler
// Type: System.Net.HttpEventSource
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Diagnostics.Tracing;
using System.Net.Http;
using System.Security;

#nullable disable
namespace System.Net;

[EventSource(Name = "Microsoft-System-Net-Http", Guid = "bdd9a83e-1929-5482-0d73-2fe5e1c0e16d", LocalizationResources = "FxResources.System.Net.Http.SR")]
[SecuritySafeCritical]
internal sealed class HttpEventSource : EventSource
{
  private const int AssociateId = 1;
  private const int UriBaseAddressId = 2;
  private const int ContentNullId = 3;
  private const int ClientSendCompletedId = 4;
  private const int HeadersInvalidValueId = 5;
  private const int HandlerMessageId = 6;
  private static readonly HttpEventSource s_log = new HttpEventSource();

  private HttpEventSource()
  {
  }

  public static HttpEventSource Log => HttpEventSource.s_log;

  [NonEvent]
  internal static void Associate(object objA, object objB)
  {
    if (!HttpEventSource.s_log.IsEnabled())
      return;
    HttpEventSource.s_log.Associate(LoggingHash.GetObjectName(objA), LoggingHash.HashInt(objA), LoggingHash.GetObjectName(objB), LoggingHash.HashInt(objB));
  }

  [Event(1)]
  internal unsafe void Associate(string objectA, int objectAHash, string objectB, int objectBHash)
  {
    fixed (char* chPtr1 = objectA)
      fixed (char* chPtr2 = objectB)
      {
        EventSource.EventData* eventDataPtr = stackalloc EventSource.EventData[4];
        ((EventSource.EventData) (IntPtr) eventDataPtr).DataPointer = (IntPtr) (void*) chPtr1;
        ((EventSource.EventData) (IntPtr) eventDataPtr).Size = (objectA.Length + 1) * 2;
        ((EventSource.EventData) (IntPtr) (eventDataPtr + 1)).DataPointer = (IntPtr) (void*) &objectAHash;
        ((EventSource.EventData) (IntPtr) (eventDataPtr + 1)).Size = 4;
        ((EventSource.EventData) (IntPtr) (eventDataPtr + 2)).DataPointer = (IntPtr) (void*) chPtr2;
        ((EventSource.EventData) (IntPtr) (eventDataPtr + 2)).Size = (objectB.Length + 1) * 2;
        ((EventSource.EventData) (IntPtr) (eventDataPtr + 3)).DataPointer = (IntPtr) (void*) &objectBHash;
        ((EventSource.EventData) (IntPtr) (eventDataPtr + 3)).Size = 4;
        this.WriteEventCore(1, 4, eventDataPtr);
      }
  }

  [NonEvent]
  internal static void UriBaseAddress(object obj, string baseAddress)
  {
    if (!HttpEventSource.s_log.IsEnabled())
      return;
    HttpEventSource.s_log.UriBaseAddress(baseAddress, LoggingHash.GetObjectName(obj), LoggingHash.HashInt(obj));
  }

  [Event(2)]
  internal unsafe void UriBaseAddress(string uriBaseAddress, string objName, int objHash)
  {
    fixed (char* chPtr1 = uriBaseAddress)
      fixed (char* chPtr2 = objName)
      {
        EventSource.EventData* eventDataPtr = stackalloc EventSource.EventData[3];
        ((EventSource.EventData) (IntPtr) eventDataPtr).DataPointer = (IntPtr) (void*) chPtr1;
        ((EventSource.EventData) (IntPtr) eventDataPtr).Size = (uriBaseAddress.Length + 1) * 2;
        ((EventSource.EventData) (IntPtr) (eventDataPtr + 1)).DataPointer = (IntPtr) (void*) chPtr2;
        ((EventSource.EventData) (IntPtr) (eventDataPtr + 1)).Size = (objName.Length + 1) * 2;
        ((EventSource.EventData) (IntPtr) (eventDataPtr + 2)).DataPointer = (IntPtr) (void*) &objHash;
        ((EventSource.EventData) (IntPtr) (eventDataPtr + 2)).Size = 4;
        this.WriteEventCore(2, 3, eventDataPtr);
      }
  }

  [NonEvent]
  internal static void ContentNull(object obj)
  {
    if (!HttpEventSource.s_log.IsEnabled())
      return;
    HttpEventSource.s_log.ContentNull(LoggingHash.GetObjectName(obj), LoggingHash.HashInt(obj));
  }

  [Event(3)]
  internal void ContentNull(string objName, int objHash) => this.WriteEvent(3, objName, objHash);

  [NonEvent]
  internal static void ClientSendCompleted(
    HttpClient httpClient,
    HttpResponseMessage response,
    HttpRequestMessage request)
  {
    if (!HttpEventSource.s_log.IsEnabled())
      return;
    string responseString = "";
    if (response != null)
      responseString = response.ToString();
    HttpEventSource.s_log.ClientSendCompleted(LoggingHash.HashInt((object) request), LoggingHash.HashInt((object) response), responseString, LoggingHash.HashInt((object) httpClient));
  }

  [Event(4)]
  internal unsafe void ClientSendCompleted(
    int httpRequestMessageHash,
    int httpResponseMessageHash,
    string responseString,
    int httpClientHash)
  {
    fixed (char* chPtr = responseString)
    {
      EventSource.EventData* eventDataPtr = stackalloc EventSource.EventData[4];
      ((EventSource.EventData) (IntPtr) eventDataPtr).DataPointer = (IntPtr) (void*) &httpRequestMessageHash;
      ((EventSource.EventData) (IntPtr) eventDataPtr).Size = 4;
      ((EventSource.EventData) (IntPtr) (eventDataPtr + 1)).DataPointer = (IntPtr) (void*) &httpResponseMessageHash;
      ((EventSource.EventData) (IntPtr) (eventDataPtr + 1)).Size = 4;
      ((EventSource.EventData) (IntPtr) (eventDataPtr + 2)).DataPointer = (IntPtr) (void*) chPtr;
      ((EventSource.EventData) (IntPtr) (eventDataPtr + 2)).Size = (responseString.Length + 1) * 2;
      ((EventSource.EventData) (IntPtr) (eventDataPtr + 3)).DataPointer = (IntPtr) (void*) &httpClientHash;
      ((EventSource.EventData) (IntPtr) (eventDataPtr + 3)).Size = 4;
      this.WriteEventCore(4, 4, eventDataPtr);
    }
  }

  [Event(5)]
  internal void HeadersInvalidValue(string name, string rawValue)
  {
    this.WriteEvent(5, name, rawValue);
  }

  [Event(6)]
  internal unsafe void HandlerMessage(
    int workerId,
    int requestId,
    string memberName,
    string message)
  {
    if (memberName == null)
      memberName = string.Empty;
    if (message == null)
      message = string.Empty;
    fixed (char* chPtr1 = memberName)
      fixed (char* chPtr2 = message)
      {
        EventSource.EventData* eventDataPtr = stackalloc EventSource.EventData[4];
        ((EventSource.EventData) (IntPtr) eventDataPtr).DataPointer = (IntPtr) (void*) &workerId;
        ((EventSource.EventData) (IntPtr) eventDataPtr).Size = 4;
        ((EventSource.EventData) (IntPtr) (eventDataPtr + 1)).DataPointer = (IntPtr) (void*) &requestId;
        ((EventSource.EventData) (IntPtr) (eventDataPtr + 1)).Size = 4;
        ((EventSource.EventData) (IntPtr) (eventDataPtr + 2)).DataPointer = (IntPtr) (void*) chPtr1;
        ((EventSource.EventData) (IntPtr) (eventDataPtr + 2)).Size = (memberName.Length + 1) * 2;
        ((EventSource.EventData) (IntPtr) (eventDataPtr + 3)).DataPointer = (IntPtr) (void*) chPtr2;
        ((EventSource.EventData) (IntPtr) (eventDataPtr + 3)).Size = (message.Length + 1) * 2;
        this.WriteEventCore(6, 4, eventDataPtr);
      }
  }

  public class Keywords
  {
    public const EventKeywords Default = (EventKeywords) 1;
    public const EventKeywords Debug = (EventKeywords) 2;
  }
}
