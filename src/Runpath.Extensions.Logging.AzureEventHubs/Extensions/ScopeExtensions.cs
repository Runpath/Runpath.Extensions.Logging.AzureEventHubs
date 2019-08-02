using System;

// ReSharper disable once CheckNamespace
namespace Runpath.Extensions.Logging.AzureEventHubs
{
    public static class ScopeExtensions
    {
        /// <summary>
        /// Executes a callback for each currently active scope object, in order of creation.
        /// </summary>
        /// <param name="scopeCallback"></param>
        /// <param name="callback">The callback to be executed for every scope object</param>
        /// <param name="state">The state object to be passed into the callback</param>
        public static void ForEachScope<TState>(this ScopeCallback scopeCallback, Action<object, TState> callback, TState state)
            => scopeCallback((s1, s2) => callback(s1, (TState)s2), state);
    }
}
