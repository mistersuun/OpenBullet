// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Utilities.CalculatorUtilities
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Utilities;

internal static class CalculatorUtilities
{
  public static Calculator.CalculatorButtonType GetCalculatorButtonTypeFromText(string text)
  {
    switch (text)
    {
      case "\b":
        return Calculator.CalculatorButtonType.Back;
      case "\r":
      case "=":
        return Calculator.CalculatorButtonType.Equal;
      case "%":
        return Calculator.CalculatorButtonType.Percent;
      case "*":
        return Calculator.CalculatorButtonType.Multiply;
      case "+":
        return Calculator.CalculatorButtonType.Add;
      case "-":
        return Calculator.CalculatorButtonType.Subtract;
      case "/":
        return Calculator.CalculatorButtonType.Divide;
      case "0":
        return Calculator.CalculatorButtonType.Zero;
      case "1":
        return Calculator.CalculatorButtonType.One;
      case "2":
        return Calculator.CalculatorButtonType.Two;
      case "3":
        return Calculator.CalculatorButtonType.Three;
      case "4":
        return Calculator.CalculatorButtonType.Four;
      case "5":
        return Calculator.CalculatorButtonType.Five;
      case "6":
        return Calculator.CalculatorButtonType.Six;
      case "7":
        return Calculator.CalculatorButtonType.Seven;
      case "8":
        return Calculator.CalculatorButtonType.Eight;
      case "9":
        return Calculator.CalculatorButtonType.Nine;
      default:
        if (text == CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator)
          return Calculator.CalculatorButtonType.Decimal;
        return text == '\u001B'.ToString() ? Calculator.CalculatorButtonType.Clear : Calculator.CalculatorButtonType.None;
    }
  }

  public static Button FindButtonByCalculatorButtonType(
    DependencyObject parent,
    Calculator.CalculatorButtonType type)
  {
    if (parent == null)
      return (Button) null;
    for (int childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(parent); ++childIndex)
    {
      DependencyObject child = VisualTreeHelper.GetChild(parent, childIndex);
      if (child != null)
      {
        object obj = child.GetValue(ButtonBase.CommandParameterProperty);
        if (obj != null && (Calculator.CalculatorButtonType) obj == type)
          return child as Button;
        Button calculatorButtonType = CalculatorUtilities.FindButtonByCalculatorButtonType(child, type);
        if (calculatorButtonType != null)
          return calculatorButtonType;
      }
    }
    return (Button) null;
  }

  public static string GetCalculatorButtonContent(Calculator.CalculatorButtonType type)
  {
    string calculatorButtonContent = string.Empty;
    switch (type)
    {
      case Calculator.CalculatorButtonType.Add:
        calculatorButtonContent = "+";
        break;
      case Calculator.CalculatorButtonType.Back:
        calculatorButtonContent = "Back";
        break;
      case Calculator.CalculatorButtonType.Cancel:
        calculatorButtonContent = "CE";
        break;
      case Calculator.CalculatorButtonType.Clear:
        calculatorButtonContent = "C";
        break;
      case Calculator.CalculatorButtonType.Decimal:
        calculatorButtonContent = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
        break;
      case Calculator.CalculatorButtonType.Divide:
        calculatorButtonContent = "/";
        break;
      case Calculator.CalculatorButtonType.Eight:
        calculatorButtonContent = "8";
        break;
      case Calculator.CalculatorButtonType.Equal:
        calculatorButtonContent = "=";
        break;
      case Calculator.CalculatorButtonType.Five:
        calculatorButtonContent = "5";
        break;
      case Calculator.CalculatorButtonType.Four:
        calculatorButtonContent = "4";
        break;
      case Calculator.CalculatorButtonType.Fraction:
        calculatorButtonContent = "1/x";
        break;
      case Calculator.CalculatorButtonType.MAdd:
        calculatorButtonContent = "M+";
        break;
      case Calculator.CalculatorButtonType.MC:
        calculatorButtonContent = "MC";
        break;
      case Calculator.CalculatorButtonType.MR:
        calculatorButtonContent = "MR";
        break;
      case Calculator.CalculatorButtonType.MS:
        calculatorButtonContent = "MS";
        break;
      case Calculator.CalculatorButtonType.MSub:
        calculatorButtonContent = "M-";
        break;
      case Calculator.CalculatorButtonType.Multiply:
        calculatorButtonContent = "*";
        break;
      case Calculator.CalculatorButtonType.Negate:
        calculatorButtonContent = "+/-";
        break;
      case Calculator.CalculatorButtonType.Nine:
        calculatorButtonContent = "9";
        break;
      case Calculator.CalculatorButtonType.One:
        calculatorButtonContent = "1";
        break;
      case Calculator.CalculatorButtonType.Percent:
        calculatorButtonContent = "%";
        break;
      case Calculator.CalculatorButtonType.Seven:
        calculatorButtonContent = "7";
        break;
      case Calculator.CalculatorButtonType.Six:
        calculatorButtonContent = "6";
        break;
      case Calculator.CalculatorButtonType.Sqrt:
        calculatorButtonContent = "Sqrt";
        break;
      case Calculator.CalculatorButtonType.Subtract:
        calculatorButtonContent = "-";
        break;
      case Calculator.CalculatorButtonType.Three:
        calculatorButtonContent = "3";
        break;
      case Calculator.CalculatorButtonType.Two:
        calculatorButtonContent = "2";
        break;
      case Calculator.CalculatorButtonType.Zero:
        calculatorButtonContent = "0";
        break;
    }
    return calculatorButtonContent;
  }

  public static bool IsDigit(Calculator.CalculatorButtonType buttonType)
  {
    switch (buttonType)
    {
      case Calculator.CalculatorButtonType.Decimal:
      case Calculator.CalculatorButtonType.Eight:
      case Calculator.CalculatorButtonType.Five:
      case Calculator.CalculatorButtonType.Four:
      case Calculator.CalculatorButtonType.Nine:
      case Calculator.CalculatorButtonType.One:
      case Calculator.CalculatorButtonType.Seven:
      case Calculator.CalculatorButtonType.Six:
      case Calculator.CalculatorButtonType.Three:
      case Calculator.CalculatorButtonType.Two:
      case Calculator.CalculatorButtonType.Zero:
        return true;
      default:
        return false;
    }
  }

  public static bool IsMemory(Calculator.CalculatorButtonType buttonType)
  {
    switch (buttonType)
    {
      case Calculator.CalculatorButtonType.MAdd:
      case Calculator.CalculatorButtonType.MC:
      case Calculator.CalculatorButtonType.MR:
      case Calculator.CalculatorButtonType.MS:
      case Calculator.CalculatorButtonType.MSub:
        return true;
      default:
        return false;
    }
  }

  public static Decimal ParseDecimal(string text)
  {
    Decimal result;
    return !Decimal.TryParse(text, NumberStyles.Any, (IFormatProvider) CultureInfo.CurrentCulture, out result) ? 0M : result;
  }

  public static Decimal Add(Decimal firstNumber, Decimal secondNumber)
  {
    return firstNumber + secondNumber;
  }

  public static Decimal Subtract(Decimal firstNumber, Decimal secondNumber)
  {
    return firstNumber - secondNumber;
  }

  public static Decimal Multiply(Decimal firstNumber, Decimal secondNumber)
  {
    return firstNumber * secondNumber;
  }

  public static Decimal Divide(Decimal firstNumber, Decimal secondNumber)
  {
    return firstNumber / secondNumber;
  }

  public static Decimal Percent(Decimal firstNumber, Decimal secondNumber)
  {
    return firstNumber * secondNumber / 100M;
  }

  public static Decimal SquareRoot(Decimal operand)
  {
    return Convert.ToDecimal(Math.Sqrt(Convert.ToDouble(operand)));
  }

  public static Decimal Fraction(Decimal operand) => 1M / operand;

  public static Decimal Negate(Decimal operand) => operand * -1M;
}
