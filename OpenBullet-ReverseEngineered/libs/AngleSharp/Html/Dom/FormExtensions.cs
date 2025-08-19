// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.FormExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Common;
using AngleSharp.Dom;
using AngleSharp.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Html.Dom;

public static class FormExtensions
{
  public static IHtmlFormElement SetValues(
    this IHtmlFormElement form,
    IDictionary<string, string> fields,
    bool createMissing = false)
  {
    if (form == null)
      throw new ArgumentNullException(nameof (form));
    if (fields == null)
      throw new ArgumentNullException(nameof (fields));
    IEnumerable<HtmlFormControlElement> source = form.Elements.OfType<HtmlFormControlElement>();
    foreach (KeyValuePair<string, string> field1 in (IEnumerable<KeyValuePair<string, string>>) fields)
    {
      KeyValuePair<string, string> field = field1;
      HtmlFormControlElement targetInput = source.FirstOrDefault<HtmlFormControlElement>((Func<HtmlFormControlElement, bool>) (e => e.Name.Is(field.Key)));
      if (targetInput != null)
      {
        if (targetInput is IHtmlInputElement htmlInputElement1)
        {
          if (htmlInputElement1.Type.Is(InputTypeNames.Radio))
          {
            foreach (IHtmlInputElement htmlInputElement in source.OfType<IHtmlInputElement>().Where<IHtmlInputElement>((Func<IHtmlInputElement, bool>) (i => i.Name.Is(targetInput.Name))))
              htmlInputElement.IsChecked = htmlInputElement.Value.Is(field.Value);
          }
          else
            htmlInputElement1.Value = field.Value;
        }
        else if (targetInput is IHtmlTextAreaElement htmlTextAreaElement)
          htmlTextAreaElement.Value = field.Value;
        else if (targetInput is IHtmlSelectElement htmlSelectElement)
          htmlSelectElement.Value = field.Value;
      }
      else
      {
        if (!createMissing)
          throw new KeyNotFoundException($"Field {field.Key} not found.");
        IHtmlInputElement element = form.Owner.CreateElement<IHtmlInputElement>();
        element.Type = InputTypeNames.Hidden;
        element.Name = field.Key;
        element.Value = field.Value;
        form.AppendChild((INode) element);
      }
    }
    return form;
  }

  public static Task<IDocument> SubmitAsync(this IHtmlFormElement form, object fields)
  {
    return FormExtensions.SubmitAsync(form, (IDictionary<string, string>) fields.ToDictionary());
  }

  public static Task<IDocument> SubmitAsync(
    this IHtmlFormElement form,
    IDictionary<string, string> fields,
    bool createMissing = false)
  {
    form.SetValues(fields, createMissing);
    return form.SubmitAsync();
  }

  public static Task<IDocument> SubmitAsync(this IHtmlElement element, object fields = null)
  {
    return FormExtensions.SubmitAsync(element, (IDictionary<string, string>) fields.ToDictionary());
  }

  public static Task<IDocument> SubmitAsync(
    this IHtmlElement element,
    IDictionary<string, string> fields,
    bool createMissing = false)
  {
    IHtmlFormElement form = element is HtmlFormControlElement sourceElement ? sourceElement.Form : throw new ArgumentException(nameof (element));
    if (form == null)
      return (Task<IDocument>) null;
    form.SetValues(fields, createMissing);
    return form.SubmitAsync((IHtmlElement) sourceElement);
  }
}
