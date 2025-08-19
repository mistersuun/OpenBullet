// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.VersionResourceDictionary
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.ComponentModel;
using System.Windows;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core;

public class VersionResourceDictionary : ResourceDictionary, ISupportInitialize
{
  private int _initializingCount;
  private string _assemblyName;
  private string _sourcePath;

  public VersionResourceDictionary()
  {
  }

  public VersionResourceDictionary(string assemblyName, string sourcePath)
  {
    ((ISupportInitialize) this).BeginInit();
    this.AssemblyName = assemblyName;
    this.SourcePath = sourcePath;
    ((ISupportInitialize) this).EndInit();
  }

  public string AssemblyName
  {
    get => this._assemblyName;
    set
    {
      this.EnsureInitialization();
      this._assemblyName = value;
    }
  }

  public string SourcePath
  {
    get => this._sourcePath;
    set
    {
      this.EnsureInitialization();
      this._sourcePath = value;
    }
  }

  private void EnsureInitialization()
  {
    if (this._initializingCount <= 0)
      throw new InvalidOperationException("VersionResourceDictionary properties can only be set while initializing.");
  }

  void ISupportInitialize.BeginInit()
  {
    this.BeginInit();
    ++this._initializingCount;
  }

  void ISupportInitialize.EndInit()
  {
    --this._initializingCount;
    if (this._initializingCount <= 0)
    {
      if (this.Source != (Uri) null)
        throw new InvalidOperationException("Source property cannot be initialized on the VersionResourceDictionary");
      this.Source = !string.IsNullOrEmpty(this.AssemblyName) && !string.IsNullOrEmpty(this.SourcePath) ? new Uri($"pack://application:,,,/{this.AssemblyName};v{"3.5.0.0"};component/{this.SourcePath}", UriKind.Absolute) : throw new InvalidOperationException("AssemblyName and SourcePath must be set during initialization");
    }
    this.EndInit();
  }

  private enum InitState
  {
    NotInitialized,
    Initializing,
    Initialized,
  }
}
