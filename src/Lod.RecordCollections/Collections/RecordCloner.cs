using System.Collections.Concurrent;
using System.Reflection;

namespace System.Collections;

/// <summary>
/// A static utility for cloning items within a record collection.
/// </summary>
internal static class RecordCloner
{
    private static ConcurrentDictionary<Type, MethodBase?> ClonerCache { get; } = [];

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
            MethodBase? cloner = ClonerCache.GetOrAdd(typeof(T), GetCloneConstructor);

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

    static MethodBase? GetCloneConstructor(Type type) =>
        type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .OrderByDescending(c => c.IsFamily) // Prefer the protected record constructor
            .FirstOrDefault(c =>
            {
                ParameterInfo[] parameters = c.GetParameters();

                return parameters.Length == 1 && parameters[0].ParameterType == type;
            });
}
