// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.PropertyDefinitionBaseCollection`1
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

public abstract class PropertyDefinitionBaseCollection<T> : DefinitionCollectionBase<T> where T : PropertyDefinitionBase
{
  public virtual T this[object propertyId]
  {
    get
    {
      foreach (T obj in (IEnumerable<T>) this.Items)
      {
        if (obj.TargetProperties.Contains(propertyId))
          return obj;
        List<string> list = obj.TargetProperties.OfType<string>().ToList<string>();
        if (list != null && list.Count > 0)
        {
          if (propertyId is string)
          {
            string str1 = (string) propertyId;
            foreach (string str2 in list)
            {
              if (str2.Contains("*"))
              {
                string str3 = str2.Replace("*", "");
                if (str1.StartsWith(str3) || str1.EndsWith(str3))
                  return obj;
              }
            }
          }
        }
        else
        {
          Type c = propertyId as Type;
          if (c != (Type) null)
          {
            foreach (Type targetProperty in (IEnumerable) obj.TargetProperties)
            {
              if (targetProperty.IsAssignableFrom(c))
                return obj;
            }
          }
        }
      }
      return default (T);
    }
  }

  internal T GetRecursiveBaseTypes(Type type)
  {
    T recursiveBaseTypes;
    for (recursiveBaseTypes = default (T); (object) recursiveBaseTypes == null && type != (Type) null; type = type.BaseType)
      recursiveBaseTypes = this[(object) type];
    return recursiveBaseTypes;
  }
}
