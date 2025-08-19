// Decompiled with JetBrains decompiler
// Type: RuriLib.Models.KeyChain
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using RuriLib.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Media;

#nullable disable
namespace RuriLib.Models;

public class KeyChain : ViewModelBase
{
  private KeyChain.KeychainType type;
  private KeyChain.KeychainMode mode;
  private string customType = "CUSTOM";

  public KeyChain.KeychainType Type
  {
    get => this.type;
    set
    {
      this.type = value;
      this.OnPropertyChanged(nameof (Type));
    }
  }

  public KeyChain.KeychainMode Mode
  {
    get => this.mode;
    set
    {
      this.mode = value;
      this.OnPropertyChanged(nameof (Mode));
    }
  }

  public string CustomType
  {
    get => this.customType;
    set
    {
      this.customType = value;
      this.OnPropertyChanged(nameof (CustomType));
    }
  }

  public ObservableCollection<Key> Keys { get; set; } = new ObservableCollection<Key>();

  public bool CheckKeys(BotData data)
  {
    switch (this.Mode)
    {
      case KeyChain.KeychainMode.OR:
        foreach (Key key in (Collection<Key>) this.Keys)
        {
          if (key.CheckKey(data))
          {
            data.Log(new LogEntry($"Found 'OR' Key {BlockBase.TruncatePretty(BlockBase.ReplaceValues(key.LeftTerm, data), 20)} {key.Comparer.ToString()} {BlockBase.ReplaceValues(key.RightTerm, data)}", Colors.White));
            return true;
          }
        }
        return false;
      case KeyChain.KeychainMode.AND:
        foreach (Key key in (Collection<Key>) this.Keys)
        {
          if (!key.CheckKey(data))
            return false;
          data.Log(new LogEntry($"Found 'AND' Key {BlockBase.TruncatePretty(BlockBase.ReplaceValues(key.LeftTerm, data), 20)} {key.Comparer.ToString()} {BlockBase.ReplaceValues(key.RightTerm, data)}", Colors.White));
        }
        return true;
      default:
        return false;
    }
  }

  public enum KeychainType
  {
    Success,
    Failure,
    Ban,
    Retry,
    Custom,
  }

  public enum KeychainMode
  {
    OR,
    AND,
  }
}
