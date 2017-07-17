using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace TQL.Threading
{
    internal class DequeueCustomEnumerator : IEnumerable<IdentifiableEvaluator>
    {
        private readonly ConcurrentQueue<IdentifiableEvaluator> _queue;

        public DequeueCustomEnumerator(ConcurrentQueue<IdentifiableEvaluator> queue)
        {
            _queue = queue;
        }

        public IEnumerator<IdentifiableEvaluator> GetEnumerator() => new DequeueEnumerator(_queue);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class DequeueEnumerator : IEnumerator<IdentifiableEvaluator>
        {
            private readonly ConcurrentQueue<IdentifiableEvaluator> _queue;

            public DequeueEnumerator(ConcurrentQueue<IdentifiableEvaluator> queue)
            {
                _queue = queue;
            }

            #region Implementation of IDisposable

            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            public void Dispose()
            {
            }

            #endregion

            #region Implementation of IEnumerator

            /// <summary>Advances the enumerator to the next element of the collection.</summary>
            /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public bool MoveNext()
            {
                if (_queue.Count > 0)
                {
                    IdentifiableEvaluator evaluator;
                    var result = _queue.TryDequeue(out evaluator);
                    Current = evaluator;
                    return result;
                }
                return false;
            }

            /// <summary>Sets the enumerator to its initial position, which is before the first element in the collection.</summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public void Reset()
            {
                throw new NotSupportedException();
            }

            /// <summary>Gets the element in the collection at the current position of the enumerator.</summary>
            /// <returns>The element in the collection at the current position of the enumerator.</returns>
            public IdentifiableEvaluator Current { get; private set; }

            /// <summary>Gets the current element in the collection.</summary>
            /// <returns>The current element in the collection.</returns>
            object IEnumerator.Current
            {
                get { return Current; }
            }

            #endregion
        }
    }
}