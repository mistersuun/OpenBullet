// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.CommandInfoRepository
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace OpenQA.Selenium.Remote;

public abstract class CommandInfoRepository
{
  private readonly Dictionary<string, CommandInfo> commandDictionary;

  protected CommandInfoRepository()
  {
    this.commandDictionary = new Dictionary<string, CommandInfo>();
  }

  public abstract int SpecificationLevel { get; }

  public CommandInfo GetCommandInfo(string commandName)
  {
    CommandInfo commandInfo = (CommandInfo) null;
    if (this.commandDictionary.ContainsKey(commandName))
      commandInfo = this.commandDictionary[commandName];
    return commandInfo;
  }

  public bool TryAddCommand(string commandName, CommandInfo commandInfo)
  {
    if (string.IsNullOrEmpty(commandName))
      throw new ArgumentNullException(nameof (commandName), "The name of the command cannot be null or the empty string.");
    if (commandInfo == null)
      throw new ArgumentNullException(nameof (commandInfo), "The command information object cannot be null.");
    if (this.commandDictionary.ContainsKey(commandName))
      return false;
    this.commandDictionary.Add(commandName, commandInfo);
    return true;
  }

  protected abstract void InitializeCommandDictionary();
}
