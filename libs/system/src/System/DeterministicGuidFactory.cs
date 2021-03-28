using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Logicality.System
{
    /// <summary>
    /// Represent a mechanism to deterministically create GUIDs based on an input and an initial namespace.
    ///
    /// The namespace is to allow different consumers to generate different GUIDs from the same input.
    /// </summary>
    public class DeterministicGuidFactory
    {
        private static readonly Tuple<int, int>[] ByteOrderPairsToSwap = {
            Tuple.Create(0, 3),
            Tuple.Create(1, 2),
            Tuple.Create(4, 5),
            Tuple.Create(6, 7),
        };

        private readonly byte[] _namespace;

        public DeterministicGuidFactory(Guid @namespace)
        {
            _namespace = @namespace.ToByteArray();
            SwapPairs(_namespace, ByteOrderPairsToSwap);
        }

        /// <summary>
        /// Creates a deterministic GUID using the source input. A consumer supplying the same input
        /// will get the same GUID as return.
        /// </summary>
        /// <param name="input">The input from which to create a deterministic GUID.</param>
        /// <returns>The generated GUID.</returns>
        public Guid Create(byte[] input)
        {
            byte[] hash;
            using (var algorithm = SHA1.Create())
            {
                algorithm.TransformBlock(_namespace, 0, _namespace.Length, null, 0);
                algorithm.TransformFinalBlock(input, 0, input.Length);
                hash = algorithm.Hash;
            }

            var buffer = new byte[16];
            Array.Copy(hash, 0, buffer, 0, 16);

            buffer[6] = (byte)((buffer[6] & 0x0F) | (5 << 4));
            buffer[8] = (byte)((buffer[8] & 0x3F) | 0x80);

            SwapPairs(buffer, ByteOrderPairsToSwap);
            return new Guid(buffer);
        }

        private static void SwapPairs(byte[] buffer, IEnumerable<Tuple<int, int>> pairs)
        {
            foreach (var (left, right) in pairs)
            {
                var b = buffer[left];
                buffer[left] = buffer[right];
                buffer[right] = b;
            }
        }
    }

    public static class DeterministicGuidFactoryExtensions
    {
        /// <summary>
        /// Creates a deterministic GUID from the supplied input. The input is converted to a byte array using UTF8 encding.
        /// A consumer supplying the same input will get the same GUID as return.
        /// </summary>
        /// <param name="deterministicGuidFactory">A deterministicGuidFactory.</param>
        /// <param name="input">Source input.</param>
        /// <returns></returns>
        public static Guid Create(this DeterministicGuidFactory deterministicGuidFactory, string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            var bytes = Encoding.UTF8.GetBytes(input);
            return deterministicGuidFactory.Create(bytes);
        }
    }
}
