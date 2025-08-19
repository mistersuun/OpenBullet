// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Interactions.ActionSequence
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#nullable disable
namespace OpenQA.Selenium.Interactions;

public class ActionSequence
{
  private List<Interaction> interactions = new List<Interaction>();
  private InputDevice device;

  public ActionSequence(InputDevice device)
    : this(device, 0)
  {
  }

  public ActionSequence(InputDevice device, int initialSize)
  {
    this.device = device != null ? device : throw new ArgumentNullException(nameof (device), "Input device cannot be null.");
    for (int index = 0; index < initialSize; ++index)
      this.AddAction((Interaction) new PauseInteraction(device, TimeSpan.Zero));
  }

  public int Count => this.interactions.Count;

  public ActionSequence AddAction(Interaction interactionToAdd)
  {
    if (interactionToAdd == null)
      throw new ArgumentNullException(nameof (interactionToAdd), "Interaction to add to sequence must not be null");
    if (!interactionToAdd.IsValidFor(this.device.DeviceKind))
      throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Interaction {0} is invalid for device type {1}.", (object) interactionToAdd.GetType(), (object) this.device.DeviceKind), nameof (interactionToAdd));
    this.interactions.Add(interactionToAdd);
    return this;
  }

  public Dictionary<string, object> ToDictionary()
  {
    Dictionary<string, object> dictionary = this.device.ToDictionary();
    List<object> objectList = new List<object>();
    foreach (Interaction interaction in this.interactions)
      objectList.Add((object) interaction.ToDictionary());
    dictionary["actions"] = (object) objectList;
    return dictionary;
  }

  public override string ToString()
  {
    StringBuilder stringBuilder = new StringBuilder("Action sequence - ").Append(this.device.ToString());
    foreach (Interaction interaction in this.interactions)
    {
      stringBuilder.AppendLine();
      stringBuilder.AppendFormat("    {0}", (object) interaction.ToString());
    }
    return stringBuilder.ToString();
  }
}
