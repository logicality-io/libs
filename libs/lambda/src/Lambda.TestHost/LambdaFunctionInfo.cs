using System;
using System.Reflection;
using Amazon.Lambda.Core;

namespace Logicality.AWS.Lambda.TestHost
{
    public class LambdaFunctionInfo
    {
        /// <summary>
        ///     Information about a lambda function that can be invoked.
        /// </summary>
        /// <param name="name">
        ///     The name of the function.
        /// </param>
        /// <param name="functionType">
        ///     The lambda function type that will be activate.
        /// </param>
        /// <param name="handlerMethod">
        ///     The lambda function method that will be invoked.
        /// </param>
        /// <param name="reservedConcurrency">
        ///     The reserved concurrency.
        /// </param>
        public LambdaFunctionInfo(
            string name,
            Type functionType,
            string handlerMethod,
            int? reservedConcurrency = null)
        {
            Name = name;
            Type = functionType;

            HandlerMethod = functionType.GetMethod(handlerMethod, BindingFlags.Public | BindingFlags.Instance)!;

            // Search to see if a Lambda serializer is registered.
            var attribute = HandlerMethod.GetCustomAttribute(typeof(LambdaSerializerAttribute)) as LambdaSerializerAttribute ??
                            functionType.Assembly.GetCustomAttribute(typeof(LambdaSerializerAttribute)) as LambdaSerializerAttribute;

            if (attribute != null)
            {
                Serializer = (Activator.CreateInstance(attribute.SerializerType) as ILambdaSerializer)!;
            }

            ReservedConcurrency = reservedConcurrency;

            HandlerString = $"{functionType.Assembly.GetName().Name}::{functionType.FullName}::{handlerMethod}";
        }

        public Type Type { get; }

        public string Name { get; }

        public MethodInfo HandlerMethod { get; }

        public ILambdaSerializer? Serializer { get; }

        public int? ReservedConcurrency { get; } = null;

        public string HandlerString { get; }
    }
}