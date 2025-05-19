using System;

namespace Xrpl.BinaryCodec.Types
{
    /// <summary>
    /// Represents a 192-bit (24-byte) hash (e.g., MPT issuance ID).
    /// </summary>
    public class Hash192
    {
        public const int Width = 24;
        private readonly byte[] _bytes;

        private Hash192(byte[] bytes)
        {
            if (bytes == null || bytes.Length != Width)
                throw new ArgumentException($"Hash192 must be {Width} bytes.");
            _bytes = bytes;
        }

        /// <summary>
        /// Create from a hex string (uppercase or lowercase).
        /// </summary>
        public static Hash192 FromHex(string hex)
        {
            if (string.IsNullOrEmpty(hex) || hex.Length != Width * 2)
                throw new ArgumentException($"Hex string must be {Width * 2} characters.");
            var bytes = hex.FromHex();
            return new Hash192(bytes);
        }

        /// <summary>
        /// Get raw bytes.
        /// </summary>
        public byte[] ToBytes() => _bytes;
    }
}