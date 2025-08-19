// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.HighlightingManager
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting;

public class HighlightingManager : IHighlightingDefinitionReferenceResolver
{
  private readonly object lockObj = new object();
  private Dictionary<string, IHighlightingDefinition> highlightingsByName = new Dictionary<string, IHighlightingDefinition>();
  private Dictionary<string, IHighlightingDefinition> highlightingsByExtension = new Dictionary<string, IHighlightingDefinition>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  private List<IHighlightingDefinition> allHighlightings = new List<IHighlightingDefinition>();

  public IHighlightingDefinition GetDefinition(string name)
  {
    lock (this.lockObj)
    {
      IHighlightingDefinition highlightingDefinition;
      return this.highlightingsByName.TryGetValue(name, out highlightingDefinition) ? highlightingDefinition : (IHighlightingDefinition) null;
    }
  }

  public ReadOnlyCollection<IHighlightingDefinition> HighlightingDefinitions
  {
    get
    {
      lock (this.lockObj)
        return Array.AsReadOnly<IHighlightingDefinition>(this.allHighlightings.ToArray());
    }
  }

  public IHighlightingDefinition GetDefinitionByExtension(string extension)
  {
    lock (this.lockObj)
    {
      IHighlightingDefinition highlightingDefinition;
      return this.highlightingsByExtension.TryGetValue(extension, out highlightingDefinition) ? highlightingDefinition : (IHighlightingDefinition) null;
    }
  }

  public void RegisterHighlighting(
    string name,
    string[] extensions,
    IHighlightingDefinition highlighting)
  {
    if (highlighting == null)
      throw new ArgumentNullException(nameof (highlighting));
    lock (this.lockObj)
    {
      this.allHighlightings.Add(highlighting);
      if (name != null)
        this.highlightingsByName[name] = highlighting;
      if (extensions == null)
        return;
      foreach (string extension in extensions)
        this.highlightingsByExtension[extension] = highlighting;
    }
  }

  public void RegisterHighlighting(
    string name,
    string[] extensions,
    Func<IHighlightingDefinition> lazyLoadedHighlighting)
  {
    if (lazyLoadedHighlighting == null)
      throw new ArgumentNullException(nameof (lazyLoadedHighlighting));
    this.RegisterHighlighting(name, extensions, (IHighlightingDefinition) new HighlightingManager.DelayLoadedHighlightingDefinition(name, lazyLoadedHighlighting));
  }

  public static HighlightingManager Instance
  {
    get => (HighlightingManager) HighlightingManager.DefaultHighlightingManager.Instance;
  }

  private sealed class DelayLoadedHighlightingDefinition : IHighlightingDefinition
  {
    private readonly object lockObj = new object();
    private readonly string name;
    private Func<IHighlightingDefinition> lazyLoadingFunction;
    private IHighlightingDefinition definition;
    private Exception storedException;

    public DelayLoadedHighlightingDefinition(
      string name,
      Func<IHighlightingDefinition> lazyLoadingFunction)
    {
      this.name = name;
      this.lazyLoadingFunction = lazyLoadingFunction;
    }

    public string Name => this.name != null ? this.name : this.GetDefinition().Name;

    private IHighlightingDefinition GetDefinition()
    {
      Func<IHighlightingDefinition> lazyLoadingFunction;
      lock (this.lockObj)
      {
        if (this.definition != null)
          return this.definition;
        lazyLoadingFunction = this.lazyLoadingFunction;
      }
      Exception exception = (Exception) null;
      IHighlightingDefinition highlightingDefinition = (IHighlightingDefinition) null;
      try
      {
        using (BusyManager.BusyLock busyLock = BusyManager.Enter((object) this))
        {
          if (!busyLock.Success)
            throw new InvalidOperationException("Tried to create delay-loaded highlighting definition recursively. Make sure the are no cyclic references between the highlighting definitions.");
          highlightingDefinition = lazyLoadingFunction();
        }
        if (highlightingDefinition == null)
          throw new InvalidOperationException("Function for delay-loading highlighting definition returned null");
      }
      catch (Exception ex)
      {
        exception = ex;
      }
      lock (this.lockObj)
      {
        this.lazyLoadingFunction = (Func<IHighlightingDefinition>) null;
        if (this.definition == null && this.storedException == null)
        {
          this.definition = highlightingDefinition;
          this.storedException = exception;
        }
        if (this.storedException != null)
          throw new HighlightingDefinitionInvalidException("Error delay-loading highlighting definition", this.storedException);
        return this.definition;
      }
    }

    public HighlightingRuleSet MainRuleSet => this.GetDefinition().MainRuleSet;

    public HighlightingRuleSet GetNamedRuleSet(string name)
    {
      return this.GetDefinition().GetNamedRuleSet(name);
    }

    public HighlightingColor GetNamedColor(string name) => this.GetDefinition().GetNamedColor(name);

    public IEnumerable<HighlightingColor> NamedHighlightingColors
    {
      get => this.GetDefinition().NamedHighlightingColors;
    }

    public override string ToString() => this.Name;

    public IDictionary<string, string> Properties => this.GetDefinition().Properties;
  }

  internal sealed class DefaultHighlightingManager : HighlightingManager
  {
    public static readonly HighlightingManager.DefaultHighlightingManager Instance = new HighlightingManager.DefaultHighlightingManager();

    public DefaultHighlightingManager() => Resources.RegisterBuiltInHighlightings(this);

    internal void RegisterHighlighting(string name, string[] extensions, string resourceName)
    {
      try
      {
        this.RegisterHighlighting(name, extensions, this.LoadHighlighting(resourceName));
      }
      catch (HighlightingDefinitionInvalidException ex)
      {
        throw new InvalidOperationException($"The built-in highlighting '{name}' is invalid.", (Exception) ex);
      }
    }

    private Func<IHighlightingDefinition> LoadHighlighting(string resourceName)
    {
      return (Func<IHighlightingDefinition>) (() =>
      {
        XshdSyntaxDefinition syntaxDefinition;
        using (Stream input = Resources.OpenStream(resourceName))
        {
          using (XmlTextReader reader = new XmlTextReader(input))
            syntaxDefinition = HighlightingLoader.LoadXshd((XmlReader) reader, true);
        }
        return HighlightingLoader.Load(syntaxDefinition, (IHighlightingDefinitionReferenceResolver) this);
      });
    }
  }
}
