// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Interactions.ActionBuilder
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace OpenQA.Selenium.Interactions;

public class ActionBuilder
{
  private Dictionary<InputDevice, ActionSequence> sequences = new Dictionary<InputDevice, ActionSequence>();

  public ActionBuilder AddAction(Interaction actionToAdd)
  {
    this.AddActions(actionToAdd);
    return this;
  }

  public ActionBuilder AddActions(params Interaction[] actionsToAdd)
  {
    this.ProcessTick(actionsToAdd);
    return this;
  }

  public IList<ActionSequence> ToActionSequenceList()
  {
    return (IList<ActionSequence>) new List<ActionSequence>((IEnumerable<ActionSequence>) this.sequences.Values).AsReadOnly();
  }

  public override string ToString()
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (ActionSequence actionSequence in this.sequences.Values)
      stringBuilder.AppendLine(actionSequence.ToString());
    return stringBuilder.ToString();
  }

  private void ProcessTick(params Interaction[] interactionsToAdd)
  {
    List<InputDevice> inputDeviceList1 = new List<InputDevice>();
    foreach (Interaction interaction in interactionsToAdd)
    {
      InputDevice sourceDevice = interaction.SourceDevice;
      if (inputDeviceList1.Contains(sourceDevice))
        throw new ArgumentException("You can only add one action per device for a single tick.");
    }
    List<InputDevice> inputDeviceList2 = new List<InputDevice>((IEnumerable<InputDevice>) this.sequences.Keys);
    foreach (Interaction interactionToAdd in interactionsToAdd)
    {
      this.FindSequence(interactionToAdd.SourceDevice).AddAction(interactionToAdd);
      inputDeviceList2.Remove(interactionToAdd.SourceDevice);
    }
    foreach (InputDevice inputDevice in inputDeviceList2)
      this.sequences[inputDevice].AddAction((Interaction) new PauseInteraction(inputDevice, TimeSpan.Zero));
  }

  private ActionSequence FindSequence(InputDevice device)
  {
    if (this.sequences.ContainsKey(device))
      return this.sequences[device];
    int num = 0;
    foreach (KeyValuePair<InputDevice, ActionSequence> sequence in this.sequences)
      num = Math.Max(num, sequence.Value.Count);
    ActionSequence sequence1 = new ActionSequence(device, num);
    this.sequences[device] = sequence1;
    return sequence1;
  }
}
