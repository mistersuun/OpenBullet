// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.UI.SelectElement
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

#nullable disable
namespace OpenQA.Selenium.Support.UI;

public class SelectElement : IWrapsElement
{
  private readonly IWebElement element;

  public SelectElement(IWebElement element)
  {
    if (element == null)
      throw new ArgumentNullException(nameof (element), "element cannot be null");
    this.element = !string.IsNullOrEmpty(element.TagName) && string.Compare(element.TagName, "select", StringComparison.OrdinalIgnoreCase) == 0 ? element : throw new UnexpectedTagNameException("select", element.TagName);
    string attribute = element.GetAttribute("multiple");
    this.IsMultiple = attribute != null && attribute.ToLowerInvariant() != "false";
  }

  public IWebElement WrappedElement => this.element;

  public bool IsMultiple { get; private set; }

  public IList<IWebElement> Options
  {
    get => (IList<IWebElement>) this.element.FindElements(By.TagName("option"));
  }

  public IWebElement SelectedOption
  {
    get
    {
      foreach (IWebElement option in (IEnumerable<IWebElement>) this.Options)
      {
        if (option.Selected)
          return option;
      }
      throw new NoSuchElementException("No option is selected");
    }
  }

  public IList<IWebElement> AllSelectedOptions
  {
    get
    {
      List<IWebElement> allSelectedOptions = new List<IWebElement>();
      foreach (IWebElement option in (IEnumerable<IWebElement>) this.Options)
      {
        if (option.Selected)
          allSelectedOptions.Add(option);
      }
      return (IList<IWebElement>) allSelectedOptions;
    }
  }

  public void SelectByText(string text, bool partialMatch = false)
  {
    if (text == null)
      throw new ArgumentNullException(nameof (text), "text must not be null");
    bool flag = false;
    IList<IWebElement> webElementList = partialMatch ? (IList<IWebElement>) this.element.FindElements(By.XPath($".//option[contains(normalize-space(.),  {SelectElement.EscapeQuotes(text)})]")) : (IList<IWebElement>) this.element.FindElements(By.XPath($".//option[normalize-space(.) = {SelectElement.EscapeQuotes(text)}]"));
    foreach (IWebElement option in (IEnumerable<IWebElement>) webElementList)
    {
      SelectElement.SetSelected(option, true);
      if (!this.IsMultiple)
        return;
      flag = true;
    }
    if (webElementList.Count == 0 && text.Contains(" "))
    {
      string substringWithoutSpace = SelectElement.GetLongestSubstringWithoutSpace(text);
      foreach (IWebElement option in !string.IsNullOrEmpty(substringWithoutSpace) ? (IEnumerable<IWebElement>) this.element.FindElements(By.XPath($".//option[contains(., {SelectElement.EscapeQuotes(substringWithoutSpace)})]")) : (IEnumerable<IWebElement>) this.element.FindElements(By.TagName("option")))
      {
        if (text == option.Text)
        {
          SelectElement.SetSelected(option, true);
          if (!this.IsMultiple)
            return;
          flag = true;
        }
      }
    }
    if (!flag)
      throw new NoSuchElementException("Cannot locate element with text: " + text);
  }

  public void SelectByValue(string value)
  {
    StringBuilder stringBuilder = new StringBuilder(".//option[@value = ");
    stringBuilder.Append(SelectElement.EscapeQuotes(value));
    stringBuilder.Append("]");
    ReadOnlyCollection<IWebElement> elements = this.element.FindElements(By.XPath(stringBuilder.ToString()));
    bool flag = false;
    foreach (IWebElement option in (IEnumerable<IWebElement>) elements)
    {
      SelectElement.SetSelected(option, true);
      if (!this.IsMultiple)
        return;
      flag = true;
    }
    if (!flag)
      throw new NoSuchElementException("Cannot locate option with value: " + value);
  }

  public void SelectByIndex(int index)
  {
    string str = index.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    foreach (IWebElement option in (IEnumerable<IWebElement>) this.Options)
    {
      if (option.GetAttribute(nameof (index)) == str)
      {
        SelectElement.SetSelected(option, true);
        return;
      }
    }
    throw new NoSuchElementException("Cannot locate option with index: " + (object) index);
  }

  public void DeselectAll()
  {
    if (!this.IsMultiple)
      throw new InvalidOperationException("You may only deselect all options if multi-select is supported");
    foreach (IWebElement option in (IEnumerable<IWebElement>) this.Options)
      SelectElement.SetSelected(option, false);
  }

  public void DeselectByText(string text)
  {
    if (!this.IsMultiple)
      throw new InvalidOperationException("You may only deselect option if multi-select is supported");
    bool flag = false;
    StringBuilder stringBuilder = new StringBuilder(".//option[normalize-space(.) = ");
    stringBuilder.Append(SelectElement.EscapeQuotes(text));
    stringBuilder.Append("]");
    foreach (IWebElement element in (IEnumerable<IWebElement>) this.element.FindElements(By.XPath(stringBuilder.ToString())))
    {
      SelectElement.SetSelected(element, false);
      flag = true;
    }
    if (!flag)
      throw new NoSuchElementException("Cannot locate option with text: " + text);
  }

  public void DeselectByValue(string value)
  {
    if (!this.IsMultiple)
      throw new InvalidOperationException("You may only deselect option if multi-select is supported");
    bool flag = false;
    StringBuilder stringBuilder = new StringBuilder(".//option[@value = ");
    stringBuilder.Append(SelectElement.EscapeQuotes(value));
    stringBuilder.Append("]");
    foreach (IWebElement element in (IEnumerable<IWebElement>) this.element.FindElements(By.XPath(stringBuilder.ToString())))
    {
      SelectElement.SetSelected(element, false);
      flag = true;
    }
    if (!flag)
      throw new NoSuchElementException("Cannot locate option with value: " + value);
  }

  public void DeselectByIndex(int index)
  {
    if (!this.IsMultiple)
      throw new InvalidOperationException("You may only deselect option if multi-select is supported");
    string str = index.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    foreach (IWebElement option in (IEnumerable<IWebElement>) this.Options)
    {
      if (str == option.GetAttribute(nameof (index)))
      {
        SelectElement.SetSelected(option, false);
        return;
      }
    }
    throw new NoSuchElementException("Cannot locate option with index: " + (object) index);
  }

  private static string EscapeQuotes(string toEscape)
  {
    if (toEscape.IndexOf("\"", StringComparison.OrdinalIgnoreCase) > -1 && toEscape.IndexOf("'", StringComparison.OrdinalIgnoreCase) > -1)
    {
      bool flag = false;
      if (toEscape.LastIndexOf("\"", StringComparison.OrdinalIgnoreCase) == toEscape.Length - 1)
        flag = true;
      List<string> stringList = new List<string>((IEnumerable<string>) toEscape.Split('"'));
      if (flag && string.IsNullOrEmpty(stringList[stringList.Count - 1]))
        stringList.RemoveAt(stringList.Count - 1);
      StringBuilder stringBuilder = new StringBuilder("concat(");
      for (int index = 0; index < stringList.Count; ++index)
      {
        stringBuilder.Append("\"").Append(stringList[index]).Append("\"");
        if (index == stringList.Count - 1)
        {
          if (flag)
            stringBuilder.Append(", '\"')");
          else
            stringBuilder.Append(")");
        }
        else
          stringBuilder.Append(", '\"', ");
      }
      return stringBuilder.ToString();
    }
    return toEscape.IndexOf("\"", StringComparison.OrdinalIgnoreCase) > -1 ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "'{0}'", (object) toEscape) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) toEscape);
  }

  private static string GetLongestSubstringWithoutSpace(string s)
  {
    string substringWithoutSpace = string.Empty;
    string str1 = s;
    char[] chArray = new char[1]{ ' ' };
    foreach (string str2 in str1.Split(chArray))
    {
      if (str2.Length > substringWithoutSpace.Length)
        substringWithoutSpace = str2;
    }
    return substringWithoutSpace;
  }

  private static void SetSelected(IWebElement option, bool select)
  {
    bool selected = option.Selected;
    if (!(!selected & select) && (!selected || select))
      return;
    option.Click();
  }
}
