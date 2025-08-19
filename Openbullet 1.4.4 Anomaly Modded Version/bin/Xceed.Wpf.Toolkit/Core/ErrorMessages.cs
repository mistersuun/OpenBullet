﻿// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.ErrorMessages
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Resources;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core;

internal static class ErrorMessages
{
  public const string EndAngleCannotBeSetDirectlyInSlice = "EndAngleCannotBeSetDirectlyInSlice";
  public const string SliceCannotBeSetDirectlyInEndAngle = "SliceCannotBeSetDirectlyInEndAngle";
  public const string SliceOOR = "SliceOOR";
  public const string AnimationAccelerationRatioOOR = "AnimationAccelerationRatioOOR";
  public const string AnimationDecelerationRatioOOR = "AnimationDecelerationRatioOOR";
  public const string ZoomboxContentMustBeUIElement = "ZoomboxContentMustBeUIElement";
  public const string ViewModeInvalidForSource = "ViewModeInvalidForSource";
  public const string ZoomboxTemplateNeedsContent = "ZoomboxTemplateNeedsContent";
  public const string ZoomboxHasViewFinderButNotDisplay = "ZoomboxHasViewFinderButNotDisplay";
  public const string PositionOnlyAccessibleOnAbsolute = "PositionOnlyAccessibleOnAbsolute";
  public const string ZoomboxViewAlreadyInitialized = "ZoomboxViewAlreadyInitialized";
  public const string ScaleOnlyAccessibleOnAbsolute = "ScaleOnlyAccessibleOnAbsolute";
  public const string RegionOnlyAccessibleOnRegionalView = "RegionOnlyAccessibleOnRegionalView";
  public const string UnableToConvertToZoomboxView = "UnableToConvertToZoomboxView";
  public const string ViewStackCannotBeManipulatedNow = "ViewStackCannotBeManipulatedNow";
  public const string SuppliedValueWasNotVisibility = "SuppliedValueWasNotVisibility";
  public const string NegativeTimeSpanNotSupported = "NegativeTimeSpanNotSupported";
  public const string NegativeSpeedNotSupported = "NegativeSpeedNotSupported";
  public const string InvalidRatePropertyAccessed = "InvalidRatePropertyAccessed";
  public const string AlreadyInColumnCollection = "Value already belongs to another 'ColumnDefinitionCollection'.";
  public const string AlreadyInRowCollection = "Value already belongs to another 'RowDefinitionCollection'.";
  public const string AlreadyInStackDefinition = "Value already belongs to another 'StackDefinitionCollection'.";
  public const string ArrayDestTooShort = "'array' destination not long enough.";
  public const string CollectionDisposed = "Collection was disposed, enumerator operations not valid.";
  public const string CollectionModified = "Collection was modified; enumeration operation may not execute.";
  public const string ColumnValueIsReadOnly = "Cannot modify 'ColumnDefinitionCollection' in read-only state.";
  public const string DefaultAnimatorCantAnimate = "DefaultAnimatorCantAnimate";
  public const string DefaultAnimationRateAnimationRateDefault = "DefaultAnimationRateAnimationRateDefault";
  public const string DefaultAnimatorIterativeAnimationDefault = "DefaultAnimatorIterativeAnimationDefault";
  public const string DestMultidimensional = "Destination is multidimensional. Expected array of rank 1.";
  public const string EnumerationFinished = "Enumeration already finished.";
  public const string EnumerationNotStarted = "Enumeration has not started. Call MoveNext.";
  public const string InvalidDefaultStackLength = "The default stack length must be Auto or an explicit value.";
  public const string MustBeColumnDefinition = "'ColumnDefinitionCollection' must be type 'ColumnDefinition'.";
  public const string MustBeRowDefinition = "'RowDefinitionCollection' must be type 'RowDefinition'.";
  public const string MustBeStackDefinition = "'StackDefinitionCollection' must be type 'StackDefinition'.";
  public const string RowValueIsReadOnly = "Cannot modify 'StackDefinitionCollection' in read-only state.";
  public const string StackValueIsReadOnly = "Cannot modify 'StackDefinitionCollection' in read-only state.";
  public const string UnexpectedType = "Expected type '{0}', got '{1}'.";
  private static readonly ResourceManager _resourceManager = new ResourceManager("Xceed.Wpf.Toolkit.Core.ErrorMessages", typeof (ErrorMessages).Assembly);

  public static string GetMessage(string msgId) => ErrorMessages._resourceManager.GetString(msgId);
}
