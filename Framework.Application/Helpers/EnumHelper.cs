﻿using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Framework.Application.Helpers
{
    public static class EnumHelper<T>
        where T : struct, Enum
    {
        private static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
        private static T? TryParse(string value)
        {
            if (Enum.TryParse<T>(value, true, out T result))
            {
                return result;
            }
            return null;
        }

        private static string? LookupResource(Type? resourceManagerProvider, string? resourceKey)
        {
            var resourceKeyProperty = resourceManagerProvider?.GetProperty(resourceKey!, BindingFlags.Static | BindingFlags.Public, null, typeof(string), new Type[0], null);
            if (resourceKeyProperty != null)
            {
                return (string?)resourceKeyProperty?.GetMethod?.Invoke(null, null);
            }

            return resourceKey;
        }


        public static IList<T> GetValues(Enum value)
        {
            var enumValues = new List<T>();

            foreach (FieldInfo fi in value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumValues.Add((T)Enum.Parse(value.GetType(), fi.Name, false));
            }
            return enumValues;
        }

        public static T? GetValue(string value)
        {
            return TryParse(value);
        }

        public static IList<string> GetNames(Enum value)
        {
            return value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
        }

        public static IList<string> GetDisplayValues(Enum value)
        {
            return GetNames(value).Select(obj => GetDisplayValue(Parse(obj))).ToList();
        }

        public static string GetDisplayValue(T value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var descriptionAttributes = fieldInfo?.GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];
            if (descriptionAttributes == null || descriptionAttributes.Length == 0)
                return string.Empty;

            if (descriptionAttributes[0].ResourceType != null)
                return LookupResource(descriptionAttributes[0].ResourceType, descriptionAttributes[0].Name) ?? string.Empty;

            return ((descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString()) ?? string.Empty;
        }
    }
}
