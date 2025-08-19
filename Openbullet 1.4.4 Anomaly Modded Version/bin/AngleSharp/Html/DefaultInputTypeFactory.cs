// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.DefaultInputTypeFactory
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Html.Dom;
using AngleSharp.Html.InputTypes;
using System;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Html;

public class DefaultInputTypeFactory : IInputTypeFactory
{
  private readonly Dictionary<string, DefaultInputTypeFactory.Creator> _creators = new Dictionary<string, DefaultInputTypeFactory.Creator>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
  {
    {
      InputTypeNames.Text,
      (DefaultInputTypeFactory.Creator) (input => (BaseInputType) new TextInputType(input, InputTypeNames.Text))
    },
    {
      InputTypeNames.Date,
      (DefaultInputTypeFactory.Creator) (input => (BaseInputType) new DateInputType(input, InputTypeNames.Date))
    },
    {
      InputTypeNames.Week,
      (DefaultInputTypeFactory.Creator) (input => (BaseInputType) new WeekInputType(input, InputTypeNames.Week))
    },
    {
      InputTypeNames.Datetime,
      (DefaultInputTypeFactory.Creator) (input => (BaseInputType) new DatetimeInputType(input, InputTypeNames.Datetime))
    },
    {
      InputTypeNames.DatetimeLocal,
      (DefaultInputTypeFactory.Creator) (input => (BaseInputType) new DatetimeLocalInputType(input, InputTypeNames.DatetimeLocal))
    },
    {
      InputTypeNames.Time,
      (DefaultInputTypeFactory.Creator) (input => (BaseInputType) new TimeInputType(input, InputTypeNames.Time))
    },
    {
      InputTypeNames.Month,
      (DefaultInputTypeFactory.Creator) (input => (BaseInputType) new MonthInputType(input, InputTypeNames.Month))
    },
    {
      InputTypeNames.Range,
      (DefaultInputTypeFactory.Creator) (input => (BaseInputType) new NumberInputType(input, InputTypeNames.Range))
    },
    {
      InputTypeNames.Number,
      (DefaultInputTypeFactory.Creator) (input => (BaseInputType) new NumberInputType(input, InputTypeNames.Number))
    },
    {
      InputTypeNames.Hidden,
      (DefaultInputTypeFactory.Creator) (input => (BaseInputType) new ButtonInputType(input, InputTypeNames.Hidden))
    },
    {
      InputTypeNames.Search,
      (DefaultInputTypeFactory.Creator) (input => (BaseInputType) new TextInputType(input, InputTypeNames.Search))
    },
    {
      InputTypeNames.Email,
      (DefaultInputTypeFactory.Creator) (input => (BaseInputType) new EmailInputType(input, InputTypeNames.Email))
    },
    {
      InputTypeNames.Tel,
      (DefaultInputTypeFactory.Creator) (input => (BaseInputType) new PatternInputType(input, InputTypeNames.Tel))
    },
    {
      InputTypeNames.Url,
      (DefaultInputTypeFactory.Creator) (input => (BaseInputType) new UrlInputType(input, InputTypeNames.Url))
    },
    {
      InputTypeNames.Password,
      (DefaultInputTypeFactory.Creator) (input => (BaseInputType) new PatternInputType(input, InputTypeNames.Password))
    },
    {
      InputTypeNames.Color,
      (DefaultInputTypeFactory.Creator) (input => (BaseInputType) new ColorInputType(input, InputTypeNames.Color))
    },
    {
      InputTypeNames.Checkbox,
      (DefaultInputTypeFactory.Creator) (input => (BaseInputType) new CheckedInputType(input, InputTypeNames.Checkbox))
    },
    {
      InputTypeNames.Radio,
      (DefaultInputTypeFactory.Creator) (input => (BaseInputType) new CheckedInputType(input, InputTypeNames.Radio))
    },
    {
      InputTypeNames.File,
      (DefaultInputTypeFactory.Creator) (input => (BaseInputType) new FileInputType(input, InputTypeNames.File))
    },
    {
      InputTypeNames.Submit,
      (DefaultInputTypeFactory.Creator) (input => (BaseInputType) new SubmitInputType(input, InputTypeNames.Submit))
    },
    {
      InputTypeNames.Reset,
      (DefaultInputTypeFactory.Creator) (input => (BaseInputType) new ButtonInputType(input, InputTypeNames.Reset))
    },
    {
      InputTypeNames.Image,
      (DefaultInputTypeFactory.Creator) (input => (BaseInputType) new ImageInputType(input, InputTypeNames.Image))
    },
    {
      InputTypeNames.Button,
      (DefaultInputTypeFactory.Creator) (input => (BaseInputType) new ButtonInputType(input, InputTypeNames.Button))
    }
  };

  public void Register(string type, DefaultInputTypeFactory.Creator creator)
  {
    this._creators.Add(type, creator);
  }

  public DefaultInputTypeFactory.Creator Unregister(string type)
  {
    DefaultInputTypeFactory.Creator creator;
    if (this._creators.TryGetValue(type, out creator))
      this._creators.Remove(type);
    return creator;
  }

  protected virtual BaseInputType CreateDefault(IHtmlInputElement input, string type)
  {
    return this._creators[InputTypeNames.Text](input);
  }

  public BaseInputType Create(IHtmlInputElement input, string type)
  {
    DefaultInputTypeFactory.Creator creator;
    return !string.IsNullOrEmpty(type) && this._creators.TryGetValue(type, out creator) ? creator(input) : this.CreateDefault(input, type);
  }

  public delegate BaseInputType Creator(IHtmlInputElement input);
}
