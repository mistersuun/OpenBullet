// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Internal.AsyncJavaScriptExecutor
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;

#nullable disable
namespace OpenQA.Selenium.Internal;

public class AsyncJavaScriptExecutor
{
  private const string AsyncScriptTemplate = "document.__$webdriverPageId = '{0}';\r\nvar timeoutId = window.setTimeout(function() {{\r\n  window.setTimeout(function() {{\r\n    document.__$webdriverAsyncTimeout = 1;\r\n  }}, 0);\r\n}}, {1});\r\ndocument.__$webdriverAsyncTimeout = 0;\r\nvar callback = function(value) {{\r\n  document.__$webdriverAsyncTimeout = 0;\r\n  document.__$webdriverAsyncScriptResult = value;\r\n  window.clearTimeout(timeoutId);\r\n}};\r\nvar argsArray = Array.prototype.slice.call(arguments);\r\nargsArray.push(callback);\r\nif (document.__$webdriverAsyncScriptResult !== undefined) {{\r\n  delete document.__$webdriverAsyncScriptResult;\r\n}}\r\n(function() {{\r\n{2}\r\n}}).apply(null, argsArray);";
  private const string PollingScriptTemplate = "var pendingId = '{0}';\r\nif (document.__$webdriverPageId != '{1}') {{\r\n  return [pendingId, -1];\r\n}} else if ('__$webdriverAsyncScriptResult' in document) {{\r\n  var value = document.__$webdriverAsyncScriptResult;\r\n  delete document.__$webdriverAsyncScriptResult;\r\n  return value;\r\n}} else {{\r\n  return [pendingId, document.__$webdriverAsyncTimeout];\r\n}}\r\n";
  private IJavaScriptExecutor executor;
  private TimeSpan timeout = TimeSpan.FromMilliseconds(0.0);

  public AsyncJavaScriptExecutor(IJavaScriptExecutor executor) => this.executor = executor;

  public TimeSpan Timeout
  {
    get => this.timeout;
    set => this.timeout = value;
  }

  public object ExecuteScript(string script, object[] args)
  {
    string str1 = Guid.NewGuid().ToString();
    string script1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "document.__$webdriverPageId = '{0}';\r\nvar timeoutId = window.setTimeout(function() {{\r\n  window.setTimeout(function() {{\r\n    document.__$webdriverAsyncTimeout = 1;\r\n  }}, 0);\r\n}}, {1});\r\ndocument.__$webdriverAsyncTimeout = 0;\r\nvar callback = function(value) {{\r\n  document.__$webdriverAsyncTimeout = 0;\r\n  document.__$webdriverAsyncScriptResult = value;\r\n  window.clearTimeout(timeoutId);\r\n}};\r\nvar argsArray = Array.prototype.slice.call(arguments);\r\nargsArray.push(callback);\r\nif (document.__$webdriverAsyncScriptResult !== undefined) {{\r\n  delete document.__$webdriverAsyncScriptResult;\r\n}}\r\n(function() {{\r\n{2}\r\n}}).apply(null, argsArray);", (object) str1, (object) this.timeout.TotalMilliseconds, (object) script);
    string str2 = Guid.NewGuid().ToString();
    string script2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "var pendingId = '{0}';\r\nif (document.__$webdriverPageId != '{1}') {{\r\n  return [pendingId, -1];\r\n}} else if ('__$webdriverAsyncScriptResult' in document) {{\r\n  var value = document.__$webdriverAsyncScriptResult;\r\n  delete document.__$webdriverAsyncScriptResult;\r\n  return value;\r\n}} else {{\r\n  return [pendingId, document.__$webdriverAsyncTimeout];\r\n}}\r\n", (object) str2, (object) str1);
    DateTime now = DateTime.Now;
    this.executor.ExecuteScript(script1, args);
    object obj;
    TimeSpan timeSpan;
    while (true)
    {
      obj = this.executor.ExecuteScript(script2);
      if (obj is ReadOnlyCollection<object> readOnlyCollection && readOnlyCollection.Count == 2 && str2 == readOnlyCollection[0].ToString())
      {
        long num = (long) readOnlyCollection[1];
        if (num >= 0L)
        {
          timeSpan = DateTime.Now - now;
          if (num <= 0L)
            Thread.Sleep(100);
          else
            goto label_5;
        }
        else
          break;
      }
      else
        goto label_6;
    }
    throw new WebDriverException("Detected a new page load while waiting for async script result.\nScript: " + script);
label_5:
    throw new WebDriverTimeoutException($"Timed out waiting for async script callback.\nElapsed time: {(object) timeSpan.Milliseconds}milliseconds\nScript: {script}");
label_6:
    return obj;
  }
}
