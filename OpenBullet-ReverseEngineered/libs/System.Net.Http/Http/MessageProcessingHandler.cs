// Decompiled with JetBrains decompiler
// Type: System.Net.Http.MessageProcessingHandler
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace System.Net.Http;

public abstract class MessageProcessingHandler : DelegatingHandler
{
  protected MessageProcessingHandler()
  {
  }

  protected MessageProcessingHandler(HttpMessageHandler innerHandler)
    : base(innerHandler)
  {
  }

  protected abstract HttpRequestMessage ProcessRequest(
    HttpRequestMessage request,
    CancellationToken cancellationToken);

  protected abstract HttpResponseMessage ProcessResponse(
    HttpResponseMessage response,
    CancellationToken cancellationToken);

  protected internal sealed override Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    CancellationToken cancellationToken)
  {
    if (request == null)
      throw new ArgumentNullException(nameof (request), SR.net_http_handler_norequest);
    MessageProcessingHandler.SendState sendState = new MessageProcessingHandler.SendState(this, cancellationToken);
    try
    {
      base.SendAsync(this.ProcessRequest(request, cancellationToken), cancellationToken).ContinueWithStandard<HttpResponseMessage>((object) sendState, (Action<Task<HttpResponseMessage>, object>) ((task, state) =>
      {
        MessageProcessingHandler.SendState tcs = (MessageProcessingHandler.SendState) state;
        MessageProcessingHandler handler = tcs._handler;
        CancellationToken token = tcs._token;
        if (task.IsFaulted)
          tcs.TrySetException(task.Exception.GetBaseException());
        else if (task.IsCanceled)
          tcs.TrySetCanceled();
        else if (task.Result == null)
        {
          tcs.TrySetException((Exception) new InvalidOperationException(SR.net_http_handler_noresponse));
        }
        else
        {
          try
          {
            HttpResponseMessage result = handler.ProcessResponse(task.Result, token);
            tcs.TrySetResult(result);
          }
          catch (OperationCanceledException ex)
          {
            MessageProcessingHandler.HandleCanceledOperations(token, (TaskCompletionSource<HttpResponseMessage>) tcs, ex);
          }
          catch (Exception ex)
          {
            tcs.TrySetException(ex);
          }
        }
      }));
    }
    catch (OperationCanceledException ex)
    {
      MessageProcessingHandler.HandleCanceledOperations(cancellationToken, (TaskCompletionSource<HttpResponseMessage>) sendState, ex);
    }
    catch (Exception ex)
    {
      sendState.TrySetException(ex);
    }
    return sendState.Task;
  }

  private static void HandleCanceledOperations(
    CancellationToken cancellationToken,
    TaskCompletionSource<HttpResponseMessage> tcs,
    OperationCanceledException e)
  {
    if (cancellationToken.IsCancellationRequested && e.CancellationToken == cancellationToken)
      tcs.TrySetCanceled();
    else
      tcs.TrySetException((Exception) e);
  }

  private sealed class SendState : TaskCompletionSource<HttpResponseMessage>
  {
    internal readonly MessageProcessingHandler _handler;
    internal readonly CancellationToken _token;

    public SendState(MessageProcessingHandler handler, CancellationToken token)
    {
      this._handler = handler;
      this._token = token;
    }
  }
}
