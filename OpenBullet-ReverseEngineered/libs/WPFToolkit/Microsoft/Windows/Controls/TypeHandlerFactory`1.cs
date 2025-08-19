// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.TypeHandlerFactory`1
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Collections.Generic;
using System.Security;

#nullable disable
namespace Microsoft.Windows.Controls;

internal abstract class TypeHandlerFactory<TypeHandler>
{
  private List<TypeHandler> handlers;
  private Dictionary<Type, TypeHandler> handlerCache;

  public ICollection<TypeHandler> Handlers
  {
    get
    {
      this.InitializeIfNecessary();
      return (ICollection<TypeHandler>) this.handlers;
    }
  }

  protected TypeHandler GetHandler(Type type)
  {
    TypeHandler handler;
    if (!this.GetCachedHandler(type, out handler))
    {
      handler = this.DetermineBestHandler(this.GetDefaultHandler(type), type);
      this.CacheHandler(type, handler);
    }
    return handler;
  }

  protected bool GetCachedHandler(Type type, out TypeHandler handler)
  {
    this.InitializeIfNecessary();
    return this.handlerCache.TryGetValue(type, out handler);
  }

  protected void CacheHandler(Type type, TypeHandler handler)
  {
    this.InitializeIfNecessary();
    this.handlerCache[type] = handler;
  }

  protected TypeHandler DetermineBestHandler(TypeHandler handler, Type type)
  {
    this.InitializeIfNecessary();
    Type type1 = typeof (object);
    Type c = typeof (object);
    foreach (TypeHandler handler1 in this.handlers)
    {
      Type baseType = this.GetBaseType(handler1);
      if ((baseType.IsAssignableFrom(type) || baseType.IsGenericTypeDefinition && TypeHandlerFactory<TypeHandler>.IsGenericTypeDefinitionOf(baseType, type)) && (type1.IsAssignableFrom(baseType) || baseType.IsInterface && !baseType.IsAssignableFrom(c)))
      {
        handler = handler1;
        type1 = baseType;
        c = TypeHandlerFactory<TypeHandler>.GetImplementingType(baseType, type);
      }
    }
    return handler;
  }

  protected static Type GetImplementingType(Type baseType, Type targetType)
  {
    if (!baseType.IsInterface && baseType.IsAssignableFrom(targetType))
      return baseType;
    Type implementingType = targetType;
    while ((object) implementingType.BaseType != null && TypeHandlerFactory<TypeHandler>.DoesTypeImplement(baseType, implementingType.BaseType))
      implementingType = implementingType.BaseType;
    return implementingType;
  }

  private static bool DoesTypeImplement(Type baseType, Type targetType)
  {
    if (baseType.IsAssignableFrom(targetType))
      return true;
    return baseType.IsGenericTypeDefinition && TypeHandlerFactory<TypeHandler>.IsGenericTypeDefinitionOf(baseType, targetType);
  }

  private static bool IsGenericTypeDefinitionOf(Type baseDefinition, Type targetType)
  {
    for (; (object) targetType != null; targetType = targetType.BaseType)
    {
      Type genericTypeDefinition = TypeHandlerFactory<TypeHandler>.GetGenericTypeDefinition(targetType);
      if ((object) genericTypeDefinition != null && baseDefinition.IsAssignableFrom(genericTypeDefinition))
        return true;
    }
    return false;
  }

  private static Type GetGenericTypeDefinition(Type type)
  {
    try
    {
      if (type.IsGenericType)
        return type.GetGenericTypeDefinition();
    }
    catch (InvalidOperationException ex)
    {
    }
    catch (NotSupportedException ex)
    {
    }
    catch (InvalidCastException ex)
    {
    }
    catch (NullReferenceException ex)
    {
    }
    catch (SecurityException ex)
    {
    }
    return (Type) null;
  }

  protected void RegisterHandler(TypeHandler handler)
  {
    this.InitializeIfNecessary();
    this.handlers.Add(handler);
    this.handlerCache.Clear();
  }

  protected void UnregisterHandler(TypeHandler handler)
  {
    this.InitializeIfNecessary();
    this.handlers.Remove(handler);
    this.handlerCache.Clear();
  }

  protected bool IsInitialized => this.handlers != null;

  protected virtual void Initialize()
  {
    this.handlers = new List<TypeHandler>();
    this.handlerCache = new Dictionary<Type, TypeHandler>();
  }

  protected void InitializeIfNecessary()
  {
    if (this.IsInitialized)
      return;
    this.Initialize();
  }

  protected virtual TypeHandler GetDefaultHandler(Type type) => default (TypeHandler);

  protected abstract Type GetBaseType(TypeHandler handler);
}
