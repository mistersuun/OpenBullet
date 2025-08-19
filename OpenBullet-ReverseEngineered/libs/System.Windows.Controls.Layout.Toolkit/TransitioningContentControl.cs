// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.TransitioningContentControl
// Assembly: System.Windows.Controls.Layout.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 2878816D-F7B3-441D-96A5-F68332B17866
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Layout.Toolkit.dll

using System.Globalization;
using System.Linq;
using System.Windows.Media.Animation;

#nullable disable
namespace System.Windows.Controls;

[TemplatePart(Name = "PreviousContentPresentationSite", Type = typeof (ContentControl))]
[TemplateVisualState(GroupName = "PresentationStates", Name = "Normal")]
[TemplateVisualState(GroupName = "PresentationStates", Name = "DefaultTransition")]
[TemplatePart(Name = "CurrentContentPresentationSite", Type = typeof (ContentControl))]
public class TransitioningContentControl : ContentControl
{
  private const string PresentationGroup = "PresentationStates";
  private const string NormalState = "Normal";
  public const string DefaultTransitionState = "DefaultTransition";
  internal const string PreviousContentPresentationSitePartName = "PreviousContentPresentationSite";
  internal const string CurrentContentPresentationSitePartName = "CurrentContentPresentationSite";
  private bool _allowIsTransitioningWrite;
  public static readonly DependencyProperty IsTransitioningProperty = DependencyProperty.Register(nameof (IsTransitioning), typeof (bool), typeof (TransitioningContentControl), new PropertyMetadata(new PropertyChangedCallback(TransitioningContentControl.OnIsTransitioningPropertyChanged)));
  private Storyboard _currentTransition;
  public static readonly DependencyProperty TransitionProperty = DependencyProperty.Register(nameof (Transition), typeof (string), typeof (TransitioningContentControl), new PropertyMetadata((object) "DefaultTransition", new PropertyChangedCallback(TransitioningContentControl.OnTransitionPropertyChanged)));
  public static readonly DependencyProperty RestartTransitionOnContentChangeProperty = DependencyProperty.Register(nameof (RestartTransitionOnContentChange), typeof (bool), typeof (TransitioningContentControl), new PropertyMetadata((object) false, new PropertyChangedCallback(TransitioningContentControl.OnRestartTransitionOnContentChangePropertyChanged)));

  private ContentPresenter CurrentContentPresentationSite { get; set; }

  private ContentPresenter PreviousContentPresentationSite { get; set; }

  public bool IsTransitioning
  {
    get => (bool) this.GetValue(TransitioningContentControl.IsTransitioningProperty);
    private set
    {
      this._allowIsTransitioningWrite = true;
      this.SetValue(TransitioningContentControl.IsTransitioningProperty, (object) value);
      this._allowIsTransitioningWrite = false;
    }
  }

  private static void OnIsTransitioningPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    TransitioningContentControl transitioningContentControl = (TransitioningContentControl) d;
    if (!transitioningContentControl._allowIsTransitioningWrite)
    {
      transitioningContentControl.IsTransitioning = (bool) e.OldValue;
      throw new InvalidOperationException(System.Windows.Controls.Properties.Resources.TransitiotioningContentControl_IsTransitioningReadOnly);
    }
  }

  private Storyboard CurrentTransition
  {
    get => this._currentTransition;
    set
    {
      if (this._currentTransition != null)
        this._currentTransition.Completed -= new EventHandler(this.OnTransitionCompleted);
      this._currentTransition = value;
      if (this._currentTransition == null)
        return;
      this._currentTransition.Completed += new EventHandler(this.OnTransitionCompleted);
    }
  }

  public string Transition
  {
    get => this.GetValue(TransitioningContentControl.TransitionProperty) as string;
    set => this.SetValue(TransitioningContentControl.TransitionProperty, (object) value);
  }

  private static void OnTransitionPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    TransitioningContentControl transitioningContentControl = (TransitioningContentControl) d;
    string newValue1 = e.NewValue as string;
    string newValue2 = e.NewValue as string;
    if (transitioningContentControl.IsTransitioning)
      transitioningContentControl.AbortTransition();
    Storyboard storyboard = transitioningContentControl.GetStoryboard(newValue2);
    if (storyboard == null)
    {
      if (VisualStates.TryGetVisualStateGroup((DependencyObject) transitioningContentControl, "PresentationStates") == null)
      {
        transitioningContentControl.CurrentTransition = (Storyboard) null;
      }
      else
      {
        transitioningContentControl.SetValue(TransitioningContentControl.TransitionProperty, (object) newValue1);
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, System.Windows.Controls.Properties.Resources.TransitioningContentControl_TransitionNotFound, (object) newValue2));
      }
    }
    else
      transitioningContentControl.CurrentTransition = storyboard;
  }

  public bool RestartTransitionOnContentChange
  {
    get
    {
      return (bool) this.GetValue(TransitioningContentControl.RestartTransitionOnContentChangeProperty);
    }
    set
    {
      this.SetValue(TransitioningContentControl.RestartTransitionOnContentChangeProperty, (object) value);
    }
  }

  private static void OnRestartTransitionOnContentChangePropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((TransitioningContentControl) d).OnRestartTransitionOnContentChangeChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnRestartTransitionOnContentChangeChanged(bool oldValue, bool newValue)
  {
  }

  public event RoutedEventHandler TransitionCompleted;

  static TransitioningContentControl()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (TransitioningContentControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (TransitioningContentControl)));
  }

  public override void OnApplyTemplate()
  {
    if (this.IsTransitioning)
      this.AbortTransition();
    base.OnApplyTemplate();
    this.PreviousContentPresentationSite = this.GetTemplateChild("PreviousContentPresentationSite") as ContentPresenter;
    this.CurrentContentPresentationSite = this.GetTemplateChild("CurrentContentPresentationSite") as ContentPresenter;
    if (this.CurrentContentPresentationSite != null)
      this.CurrentContentPresentationSite.Content = this.Content;
    Storyboard storyboard = this.GetStoryboard(this.Transition);
    this.CurrentTransition = storyboard;
    if (storyboard == null)
    {
      string transition = this.Transition;
      this.Transition = "DefaultTransition";
      throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, System.Windows.Controls.Properties.Resources.TransitioningContentControl_TransitionNotFound, (object) transition));
    }
    VisualStateManager.GoToState((Control) this, "Normal", false);
  }

  protected override void OnContentChanged(object oldContent, object newContent)
  {
    base.OnContentChanged(oldContent, newContent);
    this.StartTransition(oldContent, newContent);
  }

  private void StartTransition(object oldContent, object newContent)
  {
    if (this.CurrentContentPresentationSite == null || this.PreviousContentPresentationSite == null)
      return;
    this.CurrentContentPresentationSite.Content = newContent;
    this.PreviousContentPresentationSite.Content = oldContent;
    if (this.IsTransitioning && !this.RestartTransitionOnContentChange)
      return;
    this.IsTransitioning = true;
    VisualStateManager.GoToState((Control) this, "Normal", false);
    VisualStateManager.GoToState((Control) this, this.Transition, true);
  }

  private void OnTransitionCompleted(object sender, EventArgs e)
  {
    this.AbortTransition();
    RoutedEventHandler transitionCompleted = this.TransitionCompleted;
    if (transitionCompleted == null)
      return;
    transitionCompleted((object) this, new RoutedEventArgs());
  }

  public void AbortTransition()
  {
    VisualStateManager.GoToState((Control) this, "Normal", false);
    this.IsTransitioning = false;
    if (this.PreviousContentPresentationSite == null)
      return;
    this.PreviousContentPresentationSite.Content = (object) null;
  }

  private Storyboard GetStoryboard(string newTransition)
  {
    VisualStateGroup visualStateGroup = VisualStates.TryGetVisualStateGroup((DependencyObject) this, "PresentationStates");
    Storyboard storyboard = (Storyboard) null;
    if (visualStateGroup != null)
      storyboard = visualStateGroup.States.OfType<VisualState>().Where<VisualState>((Func<VisualState, bool>) (state => state.Name == newTransition)).Select<VisualState, Storyboard>((Func<VisualState, Storyboard>) (state => state.Storyboard)).FirstOrDefault<Storyboard>();
    return storyboard;
  }
}
