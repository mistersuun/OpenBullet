// Decompiled with JetBrains decompiler
// Type: RuriLib.LS.MouseActionParser
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Microsoft.CSharp.RuntimeBinder;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;

#nullable disable
namespace RuriLib.LS;

internal class MouseActionParser
{
  public static Action Parse(string line, BotData data)
  {
    string input = line.Trim();
    Actions actions = (Actions) null;
    try
    {
      actions = new Actions((IWebDriver) data.Driver);
    }
    catch
    {
      throw new Exception("No Browser initialized!");
    }
    int num1 = 0;
    int num2 = 0;
    int gravity = 1;
    int wind = 1;
    while (input != "")
    {
      switch (LineParser.ParseToken(ref input, TokenType.Parameter, true).ToUpper())
      {
        case "CLICK":
          if (!LineParser.CheckIdentifier(ref input, "ELEMENT"))
          {
            actions.Click();
            continue;
          }
          actions.Click(MouseActionParser.ParseElement(ref input, data));
          continue;
        case "CLICKANDHOLD":
          if (!LineParser.CheckIdentifier(ref input, "ELEMENT"))
          {
            actions.ClickAndHold();
            continue;
          }
          actions.ClickAndHold(MouseActionParser.ParseElement(ref input, data));
          continue;
        case "DOUBLECLICK":
          if (!LineParser.CheckIdentifier(ref input, "ELEMENT"))
          {
            actions.DoubleClick();
            continue;
          }
          actions.DoubleClick(MouseActionParser.ParseElement(ref input, data));
          continue;
        case "DRAGANDDROP":
          IWebElement element1 = MouseActionParser.ParseElement(ref input, data);
          LineParser.ParseToken(ref input, TokenType.Arrow, true);
          IWebElement element2 = MouseActionParser.ParseElement(ref input, data);
          actions.DragAndDrop(element1, element2);
          continue;
        case "DRAGANDDROPWITHOFFSET":
          int offsetX1 = LineParser.ParseInt(ref input, "OFFSET X");
          int offsetY1 = LineParser.ParseInt(ref input, "OFFSET Y");
          actions.DragAndDropToOffset(MouseActionParser.ParseElement(ref input, data), offsetX1, offsetY1);
          continue;
        case "DRAWLINE":
          int x1;
          int y1;
          if (LineParser.Lookahead(ref input) == TokenType.Integer)
          {
            num1 = LineParser.ParseInt(ref input, "X1");
            num2 = LineParser.ParseInt(ref input, "Y1");
            LineParser.ParseToken(ref input, TokenType.Arrow, true);
            x1 = LineParser.ParseInt(ref input, "X2");
            y1 = LineParser.ParseInt(ref input, "Y2");
          }
          else
          {
            IWebElement element3 = MouseActionParser.ParseElement(ref input, data);
            num1 = element3.Location.X;
            num2 = element3.Location.Y;
            LineParser.ParseToken(ref input, TokenType.Arrow, true);
            IWebElement element4 = MouseActionParser.ParseElement(ref input, data);
            Point location = element4.Location;
            x1 = location.X;
            location = element4.Location;
            y1 = location.Y;
          }
          LineParser.EnsureIdentifier(ref input, ":");
          int quantity = LineParser.ParseInt(ref input, "QUANTITY");
          actions.MoveToElement(data.Driver.FindElementByTagName("body"), num1, num2);
          Line line1 = new Line(new Point(num1, num2), new Point(x1, y1));
          if (data.GlobalSettings.Selenium.DrawMouseMovement)
            MouseActionParser.DrawRedDots(data.Driver, line1.getPoints(quantity), 5);
          foreach (Point offset in line1.getOffsets(quantity))
            actions.MoveByOffset(offset.X, offset.Y);
          continue;
        case "DRAWLINEHUMAN":
          int x2;
          int y2;
          if (LineParser.Lookahead(ref input) == TokenType.Integer)
          {
            num1 = LineParser.ParseInt(ref input, "X1");
            num2 = LineParser.ParseInt(ref input, "Y1");
            LineParser.ParseToken(ref input, TokenType.Arrow, true);
            x2 = LineParser.ParseInt(ref input, "X2");
            y2 = LineParser.ParseInt(ref input, "Y2");
          }
          else
          {
            IWebElement element5 = MouseActionParser.ParseElement(ref input, data);
            num1 = element5.Location.X;
            num2 = element5.Location.Y;
            LineParser.ParseToken(ref input, TokenType.Arrow, true);
            IWebElement element6 = MouseActionParser.ParseElement(ref input, data);
            Point location = element6.Location;
            x2 = location.X;
            location = element6.Location;
            y2 = location.Y;
          }
          LineParser.EnsureIdentifier(ref input, ":");
          int targetSize = LineParser.ParseInt(ref input, "QUANTITY");
          if (LineParser.Lookahead(ref input) == TokenType.Integer)
          {
            gravity = LineParser.ParseInt(ref input, "GRAVITY");
            wind = LineParser.ParseInt(ref input, "WIND");
          }
          actions.MoveToElement(data.Driver.FindElementByTagName("body"), num1, num2);
          Point[] pointArray = MouseActionParser.ShrinkArray(new Line(new Point(num1, num2), new Point(x2, y2)).HumanWindMouse((double) num1, (double) num2, (double) x2, (double) y2, (double) gravity, (double) wind, 1.0), targetSize);
          if (data.GlobalSettings.Selenium.DrawMouseMovement)
            MouseActionParser.DrawRedDots(data.Driver, pointArray, 5);
          foreach (Point offset in MouseActionParser.GetOffsets(pointArray))
            actions.MoveByOffset(offset.X, offset.Y);
          continue;
        case "DRAWPOINTS":
          int maxValue1 = LineParser.ParseInt(ref input, "MAX WIDTH");
          int maxValue2 = LineParser.ParseInt(ref input, "MAX HEIGHT");
          int num3 = LineParser.ParseInt(ref input, "AMOUNT");
          Random random = new Random();
          int num4 = 0;
          int num5 = 0;
          actions.MoveToElement(data.Driver.FindElementByTagName("body"), num1, num2);
          List<Point> pointList = new List<Point>();
          for (int index = 0; index < num3; ++index)
          {
            int x3 = random.Next(0, maxValue1);
            int y3 = random.Next(0, maxValue2);
            actions.MoveByOffset(x3 - num4, y3 - num5);
            num4 = x3;
            num5 = y3;
            pointList.Add(new Point(x3, y3));
          }
          if (data.GlobalSettings.Selenium.DrawMouseMovement)
          {
            MouseActionParser.DrawRedDots(data.Driver, pointList.ToArray(), 5);
            continue;
          }
          continue;
        case "KEYDOWN":
          string literal1 = LineParser.ParseLiteral(ref input, "KEY", true, data);
          if (!LineParser.CheckIdentifier(ref input, "ELEMENT"))
          {
            actions.KeyDown(literal1);
            continue;
          }
          actions.KeyDown(MouseActionParser.ParseElement(ref input, data), literal1);
          continue;
        case "KEYUP":
          string literal2 = LineParser.ParseLiteral(ref input, "KEY", true, data);
          if (!LineParser.CheckIdentifier(ref input, "ELEMENT"))
          {
            actions.KeyUp(literal2);
            continue;
          }
          actions.KeyUp(MouseActionParser.ParseElement(ref input, data), literal2);
          continue;
        case "MOVEBY":
          int offsetX2 = LineParser.ParseInt(ref input, "OFFSET X");
          int offsetY2 = LineParser.ParseInt(ref input, "OFFSET Y");
          actions.MoveByOffset(offsetX2, offsetY2);
          continue;
        case "MOVETO":
          actions.MoveToElement(MouseActionParser.ParseElement(ref input, data));
          continue;
        case "RELEASE":
          if (!LineParser.CheckIdentifier(ref input, "ELEMENT"))
          {
            actions.Release();
            continue;
          }
          actions.Release(MouseActionParser.ParseElement(ref input, data));
          continue;
        case "RIGHTCLICK":
          if (!LineParser.CheckIdentifier(ref input, "ELEMENT"))
          {
            actions.ContextClick();
            continue;
          }
          actions.ContextClick(MouseActionParser.ParseElement(ref input, data));
          continue;
        case "SENDKEYS":
          string literal3 = LineParser.ParseLiteral(ref input, "KEY", true, data);
          if (!LineParser.CheckIdentifier(ref input, "ELEMENT"))
          {
            actions.SendKeys(literal3);
            continue;
          }
          actions.SendKeys(MouseActionParser.ParseElement(ref input, data), literal3);
          continue;
        case "SPAWN":
          MouseActionParser.SpawnDiv(data.Driver, LineParser.ParseInt(ref input, "X"), LineParser.ParseInt(ref input, "Y"), LineParser.ParseLiteral(ref input, "ID"));
          continue;
        default:
          continue;
      }
    }
    return (Action) (() =>
    {
      actions.Build();
      actions.Perform();
      data.Log(new RuriLib.LogEntry("Executed Mouse Actions", Colors.White));
    });
  }

  private static void DrawRedDot(RemoteWebDriver driver, int x, int y, int thickness)
  {
    try
    {
      driver.ExecuteScript(MouseActionParser.RedDotScript(x, y, thickness));
    }
    catch
    {
    }
  }

  public static Point[] ShrinkArray(Point[] originArray, int targetSize)
  {
    Random random = new Random();
    List<Point> list = ((IEnumerable<Point>) originArray).ToList<Point>();
    while (list.Count > targetSize)
      list.RemoveAt(random.Next(1, list.Count - 2));
    return list.ToArray();
  }

  private static Point[] GetOffsets(Point[] originArray)
  {
    List<Point> pointList = new List<Point>();
    Point point = originArray[0];
    foreach (Point origin in originArray)
    {
      pointList.Add(new Point(origin.X - point.X, origin.Y - point.Y));
      point = origin;
    }
    return pointList.ToArray();
  }

  private static string RedDotScript(int x, int y, int thickness, string id = "reddot")
  {
    return $"\t\tvar div = document.createElement('div');\t\tdiv.style.backgroundColor = 'red';       div.id = '{id}';\t\tdiv.style.position = 'absolute';\t\tdiv.style.left = '{(object) x}px';       div.style.top = '{(object) y}px';\t    div.style.height = '{(object) thickness}px';\t\tdiv.style.width = '{(object) thickness}px';\t\tdocument.getElementsByTagName('body')[0].appendChild(div);";
  }

  private static void SpawnDiv(RemoteWebDriver driver, int x, int y, string id)
  {
    try
    {
      driver.ExecuteScript(MouseActionParser.RedDotScript(x, y, 1, id));
    }
    catch
    {
    }
  }

  public static void DrawRedDots(RemoteWebDriver driver, Point[] points, int thickness)
  {
    string script = "";
    for (int index = 0; index < ((IEnumerable<Point>) points).Count<Point>(); ++index)
      script += MouseActionParser.RedDotScript(points[index].X, points[index].Y, thickness);
    try
    {
      driver.ExecuteScript(script);
    }
    catch
    {
    }
  }

  public static IWebElement ParseElement(ref string input, BotData data)
  {
    LineParser.EnsureIdentifier(ref input, "ELEMENT");
    // ISSUE: reference to a compiler-generated field
    if (MouseActionParser.\u003C\u003Eo__7.\u003C\u003Ep__0 == null)
    {
      // ISSUE: reference to a compiler-generated field
      MouseActionParser.\u003C\u003Eo__7.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, ElementLocator>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (ElementLocator), typeof (MouseActionParser)));
    }
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    ElementLocator elementLocator = MouseActionParser.\u003C\u003Eo__7.\u003C\u003Ep__0.Target((CallSite) MouseActionParser.\u003C\u003Eo__7.\u003C\u003Ep__0, LineParser.ParseEnum(ref input, "Element Locator", typeof (ElementLocator)));
    string literal = LineParser.ParseLiteral(ref input, "Element Identifier");
    int index = 0;
    if (LineParser.Lookahead(ref input) == TokenType.Integer)
      index = LineParser.ParseInt(ref input, "Element Index");
    switch (elementLocator)
    {
      case ElementLocator.Id:
        return data.Driver.FindElementsById(literal)[index];
      case ElementLocator.Class:
        return data.Driver.FindElementsByClassName(literal)[index];
      case ElementLocator.Name:
        return data.Driver.FindElementsByName(literal)[index];
      case ElementLocator.Tag:
        return data.Driver.FindElementsByTagName(literal)[index];
      case ElementLocator.Selector:
        return data.Driver.FindElementsByCssSelector(literal)[index];
      case ElementLocator.XPath:
        return data.Driver.FindElementsByXPath(literal)[index];
      default:
        throw new Exception("Element not found on the page");
    }
  }
}
