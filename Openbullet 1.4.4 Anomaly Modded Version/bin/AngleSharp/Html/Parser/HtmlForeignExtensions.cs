// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Parser.HtmlForeignExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Html.Parser.Tokens;
using AngleSharp.Mathml.Dom;
using AngleSharp.Svg.Dom;
using AngleSharp.Text;
using System;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Html.Parser;

internal static class HtmlForeignExtensions
{
  private static readonly Dictionary<string, string> svgAttributeNames = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal)
  {
    {
      "attributename",
      "attributeName"
    },
    {
      "attributetype",
      "attributeType"
    },
    {
      "basefrequency",
      "baseFrequency"
    },
    {
      "baseprofile",
      "baseProfile"
    },
    {
      "calcmode",
      "calcMode"
    },
    {
      "clippathunits",
      "clipPathUnits"
    },
    {
      "contentscripttype",
      "contentScriptType"
    },
    {
      "contentstyletype",
      "contentStyleType"
    },
    {
      "diffuseconstant",
      "diffuseConstant"
    },
    {
      "edgemode",
      "edgeMode"
    },
    {
      "externalresourcesrequired",
      "externalResourcesRequired"
    },
    {
      "filterres",
      "filterRes"
    },
    {
      "filterunits",
      "filterUnits"
    },
    {
      "glyphref",
      "glyphRef"
    },
    {
      "gradienttransform",
      "gradientTransform"
    },
    {
      "gradientunits",
      "gradientUnits"
    },
    {
      "kernelmatrix",
      "kernelMatrix"
    },
    {
      "kernelunitlength",
      "kernelUnitLength"
    },
    {
      "keypoints",
      "keyPoints"
    },
    {
      "keysplines",
      "keySplines"
    },
    {
      "keytimes",
      "keyTimes"
    },
    {
      "lengthadjust",
      "lengthAdjust"
    },
    {
      "limitingconeangle",
      "limitingConeAngle"
    },
    {
      "markerheight",
      "markerHeight"
    },
    {
      "markerunits",
      "markerUnits"
    },
    {
      "markerwidth",
      "markerWidth"
    },
    {
      "maskcontentunits",
      "maskContentUnits"
    },
    {
      "maskunits",
      "maskUnits"
    },
    {
      "numoctaves",
      "numOctaves"
    },
    {
      "pathlength",
      "pathLength"
    },
    {
      "patterncontentunits",
      "patternContentUnits"
    },
    {
      "patterntransform",
      "patternTransform"
    },
    {
      "patternunits",
      "patternUnits"
    },
    {
      "pointsatx",
      "pointsAtX"
    },
    {
      "pointsaty",
      "pointsAtY"
    },
    {
      "pointsatz",
      "pointsAtZ"
    },
    {
      "preservealpha",
      "preserveAlpha"
    },
    {
      "preserveaspectratio",
      "preserveAspectRatio"
    },
    {
      "primitiveunits",
      "primitiveUnits"
    },
    {
      "refx",
      "refX"
    },
    {
      "refy",
      "refY"
    },
    {
      "repeatcount",
      "repeatCount"
    },
    {
      "repeatdur",
      "repeatDur"
    },
    {
      "requiredextensions",
      "requiredExtensions"
    },
    {
      "requiredfeatures",
      "requiredFeatures"
    },
    {
      "specularconstant",
      "specularConstant"
    },
    {
      "specularexponent",
      "specularExponent"
    },
    {
      "spreadmethod",
      "spreadMethod"
    },
    {
      "startoffset",
      "startOffset"
    },
    {
      "stddeviation",
      "stdDeviation"
    },
    {
      "stitchtiles",
      "stitchTiles"
    },
    {
      "surfacescale",
      "surfaceScale"
    },
    {
      "systemlanguage",
      "systemLanguage"
    },
    {
      "tablevalues",
      "tableValues"
    },
    {
      "targetx",
      "targetX"
    },
    {
      "targety",
      "targetY"
    },
    {
      "textlength",
      "textLength"
    },
    {
      "viewbox",
      "viewBox"
    },
    {
      "viewtarget",
      "viewTarget"
    },
    {
      "xchannelselector",
      "xChannelSelector"
    },
    {
      "ychannelselector",
      "yChannelSelector"
    },
    {
      "zoomandpan",
      "zoomAndPan"
    }
  };
  private static readonly Dictionary<string, string> svgAdjustedTagNames = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal)
  {
    {
      "altglyph",
      "altGlyph"
    },
    {
      "altglyphdef",
      "altGlyphDef"
    },
    {
      "altglyphitem",
      "altGlyphItem"
    },
    {
      "animatecolor",
      "animateColor"
    },
    {
      "animatemotion",
      "animateMotion"
    },
    {
      "animatetransform",
      "animateTransform"
    },
    {
      "clippath",
      "clipPath"
    },
    {
      "feblend",
      "feBlend"
    },
    {
      "fecolormatrix",
      "feColorMatrix"
    },
    {
      "fecomponenttransfer",
      "feComponentTransfer"
    },
    {
      "fecomposite",
      "feComposite"
    },
    {
      "feconvolvematrix",
      "feConvolveMatrix"
    },
    {
      "fediffuselighting",
      "feDiffuseLighting"
    },
    {
      "fedisplacementmap",
      "feDisplacementMap"
    },
    {
      "fedistantlight",
      "feDistantLight"
    },
    {
      "feflood",
      "feFlood"
    },
    {
      "fefunca",
      "feFuncA"
    },
    {
      "fefuncb",
      "feFuncB"
    },
    {
      "fefuncg",
      "feFuncG"
    },
    {
      "fefuncr",
      "feFuncR"
    },
    {
      "fegaussianblur",
      "feGaussianBlur"
    },
    {
      "feimage",
      "feImage"
    },
    {
      "femerge",
      "feMerge"
    },
    {
      "femergenode",
      "feMergeNode"
    },
    {
      "femorphology",
      "feMorphology"
    },
    {
      "feoffset",
      "feOffset"
    },
    {
      "fepointlight",
      "fePointLight"
    },
    {
      "fespecularlighting",
      "feSpecularLighting"
    },
    {
      "fespotlight",
      "feSpotLight"
    },
    {
      "fetile",
      "feTile"
    },
    {
      "feturbulence",
      "feTurbulence"
    },
    {
      "foreignobject",
      "foreignObject"
    },
    {
      "glyphref",
      "glyphRef"
    },
    {
      "lineargradient",
      "linearGradient"
    },
    {
      "radialgradient",
      "radialGradient"
    },
    {
      "textpath",
      "textPath"
    }
  };

  public static string SanatizeSvgTagName(this string localName)
  {
    string str;
    return HtmlForeignExtensions.svgAdjustedTagNames.TryGetValue(localName, out str) ? str : localName;
  }

  public static MathElement Setup(this MathElement element, HtmlTagToken tag)
  {
    int count = tag.Attributes.Count;
    for (int index = 0; index < count; ++index)
    {
      HtmlAttributeToken attribute = tag.Attributes[index];
      string name = attribute.Name;
      string str = attribute.Value;
      element.AdjustAttribute(name.AdjustToMathAttribute(), str);
    }
    return element;
  }

  public static SvgElement Setup(this SvgElement element, HtmlTagToken tag)
  {
    int count = tag.Attributes.Count;
    for (int index = 0; index < count; ++index)
    {
      HtmlAttributeToken attribute = tag.Attributes[index];
      string name = attribute.Name;
      string str = attribute.Value;
      element.AdjustAttribute(name.AdjustToSvgAttribute(), str);
    }
    return element;
  }

  public static void AdjustAttribute(this Element element, string name, string value)
  {
    string namespaceUri = (string) null;
    if (HtmlForeignExtensions.IsXLinkAttribute(name))
    {
      string str = name.Substring(name.IndexOf(':') + 1);
      if (str.IsXmlName() && str.IsQualifiedName())
      {
        namespaceUri = NamespaceNames.XLinkUri;
        name = str;
      }
    }
    else if (HtmlForeignExtensions.IsXmlAttribute(name))
      namespaceUri = NamespaceNames.XmlUri;
    else if (HtmlForeignExtensions.IsXmlNamespaceAttribute(name))
      namespaceUri = NamespaceNames.XmlNsUri;
    if (namespaceUri == null)
      element.SetOwnAttribute(name, value);
    else
      element.SetAttribute(namespaceUri, name, value);
  }

  public static string AdjustToMathAttribute(this string attributeName)
  {
    return attributeName.Is("definitionurl") ? "definitionURL" : attributeName;
  }

  public static string AdjustToSvgAttribute(this string attributeName)
  {
    string str;
    return HtmlForeignExtensions.svgAttributeNames.TryGetValue(attributeName, out str) ? str : attributeName;
  }

  private static bool IsXmlNamespaceAttribute(string name)
  {
    if (name.Length <= 4)
      return false;
    return name.Is(NamespaceNames.XmlNsPrefix) || name.Is("xmlns:xlink");
  }

  private static bool IsXmlAttribute(string name)
  {
    if (name.Length <= 7 || !"xml:".EqualsSubset(name, 0, 4))
      return false;
    return TagNames.Base.EqualsSubset(name, 4, 4) || AttributeNames.Lang.EqualsSubset(name, 4, 4) || AttributeNames.Space.EqualsSubset(name, 4, 5);
  }

  private static bool IsXLinkAttribute(string name)
  {
    if (name.Length <= 9 || !"xlink:".EqualsSubset(name, 0, 6))
      return false;
    return AttributeNames.Actuate.EqualsSubset(name, 6, 7) || AttributeNames.Arcrole.EqualsSubset(name, 6, 7) || AttributeNames.Href.EqualsSubset(name, 6, 4) || AttributeNames.Role.EqualsSubset(name, 6, 4) || AttributeNames.Show.EqualsSubset(name, 6, 4) || AttributeNames.Type.EqualsSubset(name, 6, 4) || AttributeNames.Title.EqualsSubset(name, 6, 5);
  }

  private static bool EqualsSubset(this string a, string b, int index, int length)
  {
    return string.Compare(a, 0, b, index, length, StringComparison.Ordinal) == 0;
  }
}
