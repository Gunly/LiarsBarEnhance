using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace LiarsBarEnhance.Utils;

public static class FastMemberAccessor<TClass, TValue> where TClass : class
{
    private static readonly Dictionary<string, Func<TClass, TValue>> getters = [];
    private static readonly Dictionary<string, Action<TClass, TValue>> setters = [];

    private static readonly BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

    public static TValue Get([CanBeNull] TClass instance, string memberName)
    {
        if (!getters.TryGetValue(memberName, out var getter))
        {
            var (member, isStatic) = FindMember(memberName);
            if (member == null)
                throw new Exception($"Couldn't find member {memberName} for type {typeof(TClass).Name}");

            var instanceParameterExpression = Expression.Parameter(typeof(TClass), "instance");
            var memberExpression = Expression.MakeMemberAccess(isStatic ? null : instanceParameterExpression, member);
            var lambda = Expression.Lambda<Func<TClass, TValue>>(memberExpression, instanceParameterExpression);
            getter = lambda.Compile();
            getters.Add(memberName, getter);
        }

        return getter(instance);
    }

    public static void Set([CanBeNull] TClass instance, string memberName, TValue value)
    {
        if (!setters.TryGetValue(memberName, out var setter))
        {
            var (member, isStatic) = FindMember(memberName);
            if (member == null)
                throw new Exception($"Couldn't find member {memberName} for type {typeof(TClass).Name}");
            if (member is PropertyInfo { GetMethod: null })
                throw new Exception($"Property {memberName} in type {typeof(TClass).Name} have no setter");

            var instanceParameterExpression = Expression.Parameter(typeof(TClass), "instance");
            var valueParameterExpression = Expression.Parameter(typeof(TValue), "value");
            var memberExpression = Expression.MakeMemberAccess(isStatic ? null : instanceParameterExpression, member);
            var assignExpression = Expression.Assign(memberExpression, valueParameterExpression);
            var lambda = Expression.Lambda<Action<TClass, TValue>>(assignExpression, [instanceParameterExpression, valueParameterExpression]);
            setter = lambda.Compile();
            setters.Add(memberName, setter);
        }

        setter(instance, value);
    }

    private static (MemberInfo member, bool isStatic) FindMember(string memberName)
    {
        if (typeof(TClass).GetProperty(memberName, bindingAttr) is { } property)
            return (property, property.GetMethod.IsStatic);

        if (typeof(TClass).GetField(memberName, bindingAttr) is { } field)
            return (field, field.IsStatic);

        return (null, false);
    }
}