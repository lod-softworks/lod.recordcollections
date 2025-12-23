using System.Reflection;

namespace System.Collections;

/// <summary>
/// A static utility for cloning items within a record collection.
/// </summary>
internal static class RecordCloner
{
    static Dictionary<Type, MethodBase?> Cloners { get; } = [];

    /// <summary>
    /// Returns a cloned instance of <typeparamref name="T"/> if it's a record type.
    /// If the type is not clonable the original instance is returned.
    /// </summary>
    /// <typeparam name="T"/>
    /// <param name="obj"/>
    /// <returns/>
    public static T? TryClone<T>(T? obj)
    {
        T? result = obj;

        if (obj != null)
        {
            MethodBase? cloner = AddOrGetCloner<T>();

            if (cloner != null)
            {
                try
                {
                    result = cloner is ConstructorInfo cons
                        ? (T)cons.Invoke([obj,])
                        : (T?)cloner.Invoke(null, [obj,]);
                }
                catch { }
            }
        }

        return result;
    }

    static MethodBase? AddOrGetCloner<T>()
    {
        Type type = typeof(T);

        if (!Cloners.TryGetValue(type, out MethodBase? cloner))
        {
            cloner = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(c =>
                {
                    ParameterInfo[] parameters = c.GetParameters();

                    return c.IsFamily && parameters.Length == 1 && parameters[0].ParameterType == type;
                });

            Cloners[type] = cloner;
        }

        return cloner;
    }
}
