namespace InfraServiceJobPackage.Library.DnsHelper.MessageWriters
{
    using System;
    using System.Buffers;

    public class PooledBytes : IDisposable
    {
        /*
        /// <summary>
        /// Allocation and de-allocation of arrays can be quite costly. Performing these allocations frequently will cause
        /// GC overwork and hurt performance. An elegant solution is the System.Buffers.ArrayPool. Basically the ArrayPool should be used for
        /// short-lived large arrays.ArrayPool<T> is a high performance pool of managed arrays.
        /// returns a shared pool instance. It’s thread safe and all you need to remember is that it has a default max array length, equal to 2^20 (1024*1024 = 1 048 576).
        /// </summary>
        
        private static readonly ArrayPool<byte> _pool = ArrayPool<byte>.Create(maxArrayLength: 4096 * 4, maxArraysPerBucket:100);
        
        Parameters
            maxArrayLength -The maximum length of an array instance that may be stored in the pool.
            maxArraysPerBucket - The maximum number of array instances that may be stored in each bucket in the pool. 
            The pool groups arrays of similar lengths into buckets for faster access.
        */


        /// <summary>
        /// https://adamsitnik.com/Array-Pool/
        /// Allocation and de-allocation of arrays can be quite costly. Performing these allocations frequently will cause
        /// GC overwork and hurt performance. An elegant solution is the System.Buffers.ArrayPool. Basically the ArrayPool should be used for
        /// short-lived large arrays.ArrayPool<T> is a high performance pool of managed arrays.
        /// ArrayPool<byte>.Shared returns a shared pool instance. It’s thread safe and it has a default max array length, equal to 2^20
        /// (1024*1024 = 1 048 576).
        /// </summary>
        private static readonly ArrayPool<byte> _pool = ArrayPool<byte>.Shared;

        private ArraySegment<byte> _buffer;
        private bool _disposed;

        public PooledBytes(int count)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            byte[] byteArrayFromPool = _pool.Rent(count);
            _buffer = new ArraySegment<byte>(byteArrayFromPool, 0, count);
        }

        public byte[] Buffer
        {
            get
            {
                return _disposed ? throw new ObjectDisposedException(nameof(PooledBytes)) : _buffer.Array;
            }
        }

        public ArraySegment<byte> BufferSegment
        {
            get
            {
                return _disposed ? throw new ObjectDisposedException(nameof(PooledBytes)) : _buffer;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                _pool.Return(_buffer.Array, true);
            }
        }
    }
}