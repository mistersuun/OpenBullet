// Decompiled with JetBrains decompiler
// Type: RuriLib.ConfigSettings
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using RuriLib.Models;
using RuriLib.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;

#nullable disable
namespace RuriLib;

public class ConfigSettings : ViewModelBase
{
  private string name = "";
  private int suggestedBots = 1;
  private int maxCPM;
  private DateTime lastModified = DateTime.Now;
  private string additionalInfo = "";
  private string author = "";
  private string version = "1.1.5";
  private bool ignoreResponseErrors;
  private int _maxRedirects = 8;
  private bool needsProxies;
  private bool onlySocks;
  private bool onlySsl;
  private int maxProxyUses;
  private bool banProxyAfterGoodStatus;
  private bool encodeData;
  private string allowedWordlist1 = "";
  private string allowedWordlist2 = "";
  private string captchaUrl = "";
  private string base64 = "";
  private bool grayscale;
  private bool removeLines;
  private bool removeNoise;
  private bool dilate;
  private float threshold = 1f;
  private float diffKeep;
  private float diffHide;
  private bool saturate;
  private float saturation;
  private bool transparent;
  private bool contour;
  private bool onlyShow;
  private bool contrastGamma;
  private float contrast = 1f;
  private float gamma = 1f;
  private float brightness = 1f;
  private int removeLinesMin;
  private int removeLinesMax;
  private bool crop;
  private bool forceHeadless;
  private bool alwaysOpen;
  private bool alwaysQuit;
  private bool disableNotifications;
  private string customUserAgent = "";
  private bool randomUA;
  private string customCMDArgs = "";

  public string Name
  {
    get => this.name;
    set
    {
      this.name = value;
      this.OnPropertyChanged(nameof (Name));
    }
  }

  public int SuggestedBots
  {
    get => this.suggestedBots;
    set
    {
      this.suggestedBots = value;
      this.OnPropertyChanged(nameof (SuggestedBots));
    }
  }

  public int MaxCPM
  {
    get => this.maxCPM;
    set
    {
      this.maxCPM = value;
      this.OnPropertyChanged(nameof (MaxCPM));
    }
  }

  public DateTime LastModified
  {
    get => this.lastModified;
    set
    {
      this.lastModified = value;
      this.OnPropertyChanged(nameof (LastModified));
    }
  }

  public string AdditionalInfo
  {
    get => this.additionalInfo;
    set
    {
      this.additionalInfo = value;
      this.OnPropertyChanged(nameof (AdditionalInfo));
    }
  }

  public string Author
  {
    get => this.author;
    set
    {
      this.author = value;
      this.OnPropertyChanged(nameof (Author));
    }
  }

  public string Version
  {
    get => this.version;
    set
    {
      this.version = value;
      this.OnPropertyChanged(nameof (Version));
    }
  }

  public bool IgnoreResponseErrors
  {
    get => this.ignoreResponseErrors;
    set
    {
      this.ignoreResponseErrors = value;
      this.OnPropertyChanged(nameof (IgnoreResponseErrors));
    }
  }

  public int MaxRedirects
  {
    get => this._maxRedirects;
    set
    {
      this._maxRedirects = value;
      this.OnPropertyChanged(nameof (MaxRedirects));
    }
  }

  public bool NeedsProxies
  {
    get => this.needsProxies;
    set
    {
      this.needsProxies = value;
      this.OnPropertyChanged(nameof (NeedsProxies));
    }
  }

  public bool OnlySocks
  {
    get => this.onlySocks;
    set
    {
      this.onlySocks = value;
      this.OnPropertyChanged(nameof (OnlySocks));
    }
  }

  public bool OnlySsl
  {
    get => this.onlySsl;
    set
    {
      this.onlySsl = value;
      this.OnPropertyChanged(nameof (OnlySsl));
    }
  }

  public int MaxProxyUses
  {
    get => this.maxProxyUses;
    set
    {
      this.maxProxyUses = value;
      this.OnPropertyChanged(nameof (MaxProxyUses));
    }
  }

  public bool BanProxyAfterGoodStatus
  {
    get => this.banProxyAfterGoodStatus;
    set
    {
      this.banProxyAfterGoodStatus = value;
      this.OnPropertyChanged(nameof (BanProxyAfterGoodStatus));
    }
  }

  public bool EncodeData
  {
    get => this.encodeData;
    set
    {
      this.encodeData = value;
      this.OnPropertyChanged(nameof (EncodeData));
    }
  }

  public string AllowedWordlist1
  {
    get => this.allowedWordlist1;
    set
    {
      this.allowedWordlist1 = value;
      this.OnPropertyChanged(nameof (AllowedWordlist1));
    }
  }

  public string AllowedWordlist2
  {
    get => this.allowedWordlist2;
    set
    {
      this.allowedWordlist2 = value;
      this.OnPropertyChanged(nameof (AllowedWordlist2));
    }
  }

  public ObservableCollection<DataRule> DataRules { get; set; } = new ObservableCollection<DataRule>();

  public ObservableCollection<CustomInput> CustomInputs { get; set; } = new ObservableCollection<CustomInput>();

  public string CaptchaUrl
  {
    get => this.captchaUrl;
    set
    {
      this.captchaUrl = value;
      this.OnPropertyChanged(nameof (CaptchaUrl));
    }
  }

  public string Base64
  {
    get => this.base64;
    set
    {
      this.base64 = value;
      this.OnPropertyChanged(nameof (Base64));
    }
  }

  public bool Grayscale
  {
    get => this.grayscale;
    set
    {
      this.grayscale = value;
      this.OnPropertyChanged(nameof (Grayscale));
    }
  }

  public bool RemoveLines
  {
    get => this.removeLines;
    set
    {
      this.removeLines = value;
      this.OnPropertyChanged(nameof (RemoveLines));
    }
  }

  public bool RemoveNoise
  {
    get => this.removeNoise;
    set
    {
      this.removeNoise = value;
      this.OnPropertyChanged(nameof (RemoveNoise));
    }
  }

  public bool Dilate
  {
    get => this.dilate;
    set
    {
      this.dilate = value;
      this.OnPropertyChanged(nameof (Dilate));
    }
  }

  public float Threshold
  {
    get => this.threshold;
    set
    {
      this.threshold = value;
      this.OnPropertyChanged(nameof (Threshold));
    }
  }

  public float DiffKeep
  {
    get => this.diffKeep;
    set
    {
      this.diffKeep = value;
      this.OnPropertyChanged(nameof (DiffKeep));
    }
  }

  public float DiffHide
  {
    get => this.diffHide;
    set
    {
      this.diffHide = value;
      this.OnPropertyChanged(nameof (DiffHide));
    }
  }

  public bool Saturate
  {
    get => this.saturate;
    set
    {
      this.saturate = value;
      this.OnPropertyChanged(nameof (Saturate));
    }
  }

  public float Saturation
  {
    get => this.saturation;
    set
    {
      this.saturation = value;
      this.OnPropertyChanged(nameof (Saturation));
    }
  }

  public bool Transparent
  {
    get => this.transparent;
    set
    {
      this.transparent = value;
      this.OnPropertyChanged(nameof (Transparent));
    }
  }

  public bool Contour
  {
    get => this.contour;
    set
    {
      this.contour = value;
      this.OnPropertyChanged(nameof (Contour));
    }
  }

  public bool OnlyShow
  {
    get => this.onlyShow;
    set
    {
      this.onlyShow = value;
      this.OnPropertyChanged(nameof (OnlyShow));
    }
  }

  public bool ContrastGamma
  {
    get => this.contrastGamma;
    set
    {
      this.contrastGamma = value;
      this.OnPropertyChanged(nameof (ContrastGamma));
    }
  }

  public float Contrast
  {
    get => this.contrast;
    set
    {
      this.contrast = value;
      this.OnPropertyChanged(nameof (Contrast));
    }
  }

  public float Gamma
  {
    get => this.gamma;
    set
    {
      this.gamma = value;
      this.OnPropertyChanged(nameof (Gamma));
    }
  }

  public float Brightness
  {
    get => this.brightness;
    set
    {
      this.brightness = value;
      this.OnPropertyChanged(nameof (Brightness));
    }
  }

  public int RemoveLinesMin
  {
    get => this.removeLinesMin;
    set
    {
      this.removeLinesMin = value;
      this.OnPropertyChanged(nameof (RemoveLinesMin));
    }
  }

  public int RemoveLinesMax
  {
    get => this.removeLinesMax;
    set
    {
      this.removeLinesMax = value;
      this.OnPropertyChanged(nameof (RemoveLinesMax));
    }
  }

  public bool Crop
  {
    get => this.crop;
    set
    {
      this.crop = value;
      this.OnPropertyChanged(nameof (Crop));
    }
  }

  public bool ForceHeadless
  {
    get => this.forceHeadless;
    set
    {
      this.forceHeadless = value;
      this.OnPropertyChanged(nameof (ForceHeadless));
    }
  }

  public bool AlwaysOpen
  {
    get => this.alwaysOpen;
    set
    {
      this.alwaysOpen = value;
      this.OnPropertyChanged(nameof (AlwaysOpen));
    }
  }

  public bool AlwaysQuit
  {
    get => this.alwaysQuit;
    set
    {
      this.alwaysQuit = value;
      this.OnPropertyChanged(nameof (AlwaysQuit));
    }
  }

  public bool DisableNotifications
  {
    get => this.disableNotifications;
    set
    {
      this.disableNotifications = value;
      this.OnPropertyChanged(nameof (DisableNotifications));
    }
  }

  public string CustomUserAgent
  {
    get => this.customUserAgent;
    set
    {
      this.customUserAgent = value;
      this.OnPropertyChanged(nameof (CustomUserAgent));
    }
  }

  public bool RandomUA
  {
    get => this.randomUA;
    set
    {
      this.randomUA = value;
      this.OnPropertyChanged(nameof (RandomUA));
    }
  }

  public string CustomCMDArgs
  {
    get => this.customCMDArgs;
    set
    {
      this.customCMDArgs = value;
      this.OnPropertyChanged(nameof (CustomCMDArgs));
    }
  }

  public void RemoveDataRuleById(int id) => this.DataRules.Remove(this.GetDataRuleById(id));

  public DataRule GetDataRuleById(int id)
  {
    return this.DataRules.FirstOrDefault<DataRule>((Func<DataRule, bool>) (r => r.Id == id));
  }

  public void RemoveCustomInputById(int id)
  {
    this.CustomInputs.Remove(this.GetCustomInputById(id));
  }

  public CustomInput GetCustomInputById(int id)
  {
    return this.CustomInputs.FirstOrDefault<CustomInput>((Func<CustomInput, bool>) (c => c.Id == id));
  }
}
