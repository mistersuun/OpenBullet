// Decompiled with JetBrains decompiler
// Type: RuriLib.SBlockElementAction
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Microsoft.CSharp.RuntimeBinder;
using OpenQA.Selenium;
using RuriLib.LS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Media;

#nullable disable
namespace RuriLib;

public class SBlockElementAction : BlockBase
{
  private ElementLocator locator;
  private string elementString = "";
  private int elementIndex;
  private ElementAction action;
  private string input = "";
  private string outputVariable = "";
  private bool isCapture;
  private bool recursive;

  public ElementLocator Locator
  {
    get => this.locator;
    set
    {
      this.locator = value;
      this.OnPropertyChanged(nameof (Locator));
    }
  }

  public string ElementString
  {
    get => this.elementString;
    set
    {
      this.elementString = value;
      this.OnPropertyChanged(nameof (ElementString));
    }
  }

  public int ElementIndex
  {
    get => this.elementIndex;
    set
    {
      this.elementIndex = value;
      this.OnPropertyChanged(nameof (ElementIndex));
    }
  }

  public ElementAction Action
  {
    get => this.action;
    set
    {
      this.action = value;
      this.OnPropertyChanged(nameof (Action));
    }
  }

  public string Input
  {
    get => this.input;
    set
    {
      this.input = value;
      this.OnPropertyChanged(nameof (Input));
    }
  }

  public string OutputVariable
  {
    get => this.outputVariable;
    set
    {
      this.outputVariable = value;
      this.OnPropertyChanged(nameof (OutputVariable));
    }
  }

  public bool IsCapture
  {
    get => this.isCapture;
    set
    {
      this.isCapture = value;
      this.OnPropertyChanged(nameof (IsCapture));
    }
  }

  public bool Recursive
  {
    get => this.recursive;
    set
    {
      this.recursive = value;
      this.OnPropertyChanged(nameof (Recursive));
    }
  }

  public SBlockElementAction() => this.Label = "ELEMENT ACTION";

  public override BlockBase FromLS(string line)
  {
    string input = line.Trim();
    if (input.StartsWith("#"))
      this.Label = LineParser.ParseLabel(ref input);
    // ISSUE: reference to a compiler-generated field
    if (SBlockElementAction.\u003C\u003Eo__33.\u003C\u003Ep__0 == null)
    {
      // ISSUE: reference to a compiler-generated field
      SBlockElementAction.\u003C\u003Eo__33.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, ElementLocator>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (ElementLocator), typeof (SBlockElementAction)));
    }
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this.Locator = SBlockElementAction.\u003C\u003Eo__33.\u003C\u003Ep__0.Target((CallSite) SBlockElementAction.\u003C\u003Eo__33.\u003C\u003Ep__0, LineParser.ParseEnum(ref input, "LOCATOR", typeof (ElementLocator)));
    this.ElementString = LineParser.ParseLiteral(ref input, "STRING");
    if (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Boolean)
      LineParser.SetBool(ref input, (object) this);
    else if (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Integer)
      this.ElementIndex = LineParser.ParseInt(ref input, "INDEX");
    // ISSUE: reference to a compiler-generated field
    if (SBlockElementAction.\u003C\u003Eo__33.\u003C\u003Ep__1 == null)
    {
      // ISSUE: reference to a compiler-generated field
      SBlockElementAction.\u003C\u003Eo__33.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, ElementAction>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (ElementAction), typeof (SBlockElementAction)));
    }
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this.Action = SBlockElementAction.\u003C\u003Eo__33.\u003C\u003Ep__1.Target((CallSite) SBlockElementAction.\u003C\u003Eo__33.\u003C\u003Ep__1, LineParser.ParseEnum(ref input, "ACTION", typeof (ElementAction)));
    if (input == "")
      return (BlockBase) this;
    if (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Literal)
      this.Input = LineParser.ParseLiteral(ref input, "INPUT");
    if (LineParser.ParseToken(ref input, RuriLib.LS.TokenType.Arrow, false) == "")
      return (BlockBase) this;
    try
    {
      string token = LineParser.ParseToken(ref input, RuriLib.LS.TokenType.Parameter, true);
      if (!(token.ToUpper() == "VAR"))
      {
        if (!(token.ToUpper() == "CAP"))
          goto label_20;
      }
      this.IsCapture = token.ToUpper() == "CAP";
    }
    catch
    {
      throw new ArgumentException("Invalid or missing variable type");
    }
label_20:
    try
    {
      this.OutputVariable = LineParser.ParseToken(ref input, RuriLib.LS.TokenType.Literal, true);
    }
    catch
    {
      throw new ArgumentException("Variable name not specified");
    }
    return (BlockBase) this;
  }

  public override string ToLS(bool indent = true)
  {
    BlockWriter blockWriter = new BlockWriter(this.GetType(), indent, this.Disabled);
    blockWriter.Label(this.Label).Token((object) "ELEMENTACTION").Token((object) this.Locator).Literal(this.ElementString);
    if (this.Recursive)
      blockWriter.Boolean(this.Recursive, "Recursive");
    else if (this.ElementIndex != 0)
      blockWriter.Integer(this.ElementIndex, "ElementIndex");
    blockWriter.Indent().Token((object) this.Action).Literal(this.Input, "Input");
    if (!blockWriter.CheckDefault((object) this.OutputVariable, "OutputVariable"))
      blockWriter.Arrow().Token(this.IsCapture ? (object) "CAP" : (object) "VAR").Literal(this.OutputVariable);
    return blockWriter.ToString();
  }

  public override void Process(BotData data)
  {
    base.Process(data);
    if (data.Driver == null)
    {
      data.Log(new LogEntry("Open a browser first!", Colors.White));
      throw new Exception("Browser not open");
    }
    IWebElement webElement = (IWebElement) null;
    ReadOnlyCollection<IWebElement> readOnlyCollection = (ReadOnlyCollection<IWebElement>) null;
    try
    {
      if (this.action != ElementAction.WaitForElement)
      {
        readOnlyCollection = this.FindElements(data);
        webElement = readOnlyCollection[this.elementIndex];
      }
    }
    catch
    {
      data.Log(new LogEntry("Cannot find element on the page", Colors.White));
    }
    List<string> values = new List<string>();
    try
    {
      switch (this.action)
      {
        case ElementAction.Clear:
          webElement.Clear();
          break;
        case ElementAction.SendKeys:
          webElement.SendKeys(BlockBase.ReplaceValues(this.input, data));
          break;
        case ElementAction.SendKeysHuman:
          string str = BlockBase.ReplaceValues(this.input, data);
          Random random = new Random();
          foreach (char ch in str)
          {
            webElement.SendKeys(ch.ToString());
            Thread.Sleep(random.Next(100, 300));
          }
          break;
        case ElementAction.Click:
          webElement.Click();
          BlockBase.UpdateSeleniumData(data);
          break;
        case ElementAction.Submit:
          webElement.Submit();
          BlockBase.UpdateSeleniumData(data);
          break;
        case ElementAction.GetText:
          if (this.recursive)
          {
            using (IEnumerator<IWebElement> enumerator = readOnlyCollection.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                IWebElement current = enumerator.Current;
                values.Add(current.Text);
              }
              break;
            }
          }
          values.Add(webElement.Text);
          break;
        case ElementAction.GetAttribute:
          if (this.recursive)
          {
            using (IEnumerator<IWebElement> enumerator = readOnlyCollection.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                IWebElement current = enumerator.Current;
                values.Add(current.GetAttribute(BlockBase.ReplaceValues(this.input, data)));
              }
              break;
            }
          }
          values.Add(webElement.GetAttribute(BlockBase.ReplaceValues(this.input, data)));
          break;
        case ElementAction.IsDisplayed:
          values.Add(webElement.Displayed.ToString());
          break;
        case ElementAction.IsEnabled:
          values.Add(webElement.Enabled.ToString());
          break;
        case ElementAction.IsSelected:
          values.Add(webElement.Selected.ToString());
          break;
        case ElementAction.LocationX:
          values.Add(webElement.Location.X.ToString());
          break;
        case ElementAction.LocationY:
          values.Add(webElement.Location.Y.ToString());
          break;
        case ElementAction.SizeX:
          values.Add(webElement.Size.Width.ToString());
          break;
        case ElementAction.SizeY:
          values.Add(webElement.Size.Height.ToString());
          break;
        case ElementAction.Screenshot:
          RuriLib.Functions.Files.Files.SaveScreenshot(SBlockElementAction.GetElementScreenShot((IWebDriver) data.Driver, webElement), data);
          break;
        case ElementAction.SwitchToFrame:
          data.Driver.SwitchTo().Frame(webElement);
          break;
        case ElementAction.WaitForElement:
          int num1 = 0;
          int num2 = 10000;
          try
          {
            num2 = int.Parse(this.input) * 1000;
          }
          catch
          {
          }
          bool flag = false;
          while (num1 < num2)
          {
            try
            {
              IWebElement element = this.FindElements(data)[0];
              flag = true;
              break;
            }
            catch
            {
              num1 += 200;
              Thread.Sleep(200);
            }
          }
          if (!flag)
          {
            data.Log(new LogEntry("Timeout while waiting for element", Colors.White));
            break;
          }
          break;
      }
    }
    catch
    {
      data.Log(new LogEntry("Cannot execute action on the element", Colors.White));
    }
    data.Log(new LogEntry($"Executed action {this.action} on the element with input {BlockBase.ReplaceValues(this.input, data)}", Colors.White));
    if (values.Count == 0)
      return;
    BlockBase.InsertVariables(data, this.isCapture, this.recursive, values, this.outputVariable, "", "", false, true);
  }

  public static Bitmap GetElementScreenShot(IWebDriver driver, IWebElement element)
  {
    Bitmap bitmap = Image.FromStream((Stream) new MemoryStream(((ITakesScreenshot) driver).GetScreenshot().AsByteArray)) as Bitmap;
    return bitmap.Clone(new Rectangle(element.Location, element.Size), bitmap.PixelFormat);
  }

  private ReadOnlyCollection<IWebElement> FindElements(BotData data)
  {
    switch (this.locator)
    {
      case ElementLocator.Id:
        return data.Driver.FindElements(By.Id(BlockBase.ReplaceValues(this.elementString, data)));
      case ElementLocator.Class:
        return data.Driver.FindElements(By.ClassName(BlockBase.ReplaceValues(this.elementString, data)));
      case ElementLocator.Name:
        return data.Driver.FindElements(By.Name(BlockBase.ReplaceValues(this.elementString, data)));
      case ElementLocator.Tag:
        return data.Driver.FindElements(By.TagName(BlockBase.ReplaceValues(this.elementString, data)));
      case ElementLocator.Selector:
        return data.Driver.FindElements(By.CssSelector(BlockBase.ReplaceValues(this.elementString, data)));
      case ElementLocator.XPath:
        return data.Driver.FindElements(By.XPath(BlockBase.ReplaceValues(this.elementString, data)));
      default:
        return (ReadOnlyCollection<IWebElement>) null;
    }
  }
}
