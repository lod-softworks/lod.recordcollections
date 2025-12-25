namespace System.Collections;

partial class RecordCollectionComparer
{
    private sealed class SetStrategy<T> : IComparisonStrategy
    {
        public bool Equals(IReadOnlyRecordCollection x, object y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (y is null) return false;

            if (x is not ISet<T> setX) return false;
            if (y is not ISet<T> setY) return false;
            if (setX.Count != setY.Count) return false;

            return setX.SetEquals(setY);
        }

        public int GetHashCode(IReadOnlyRecordCollection x, int startingHash)
        {
            if (x is not IEnumerable<T> seqX) return startingHash;

            unchecked
            {
                int sum = 0;
                int xor = 0;
                int count = 0;
                EqualityComparer<T> eq = EqualityComparer<T>.Default;

                foreach (T item in seqX)
                {
                    int itemHash = eq.GetHashCode(item!);
                    int mixed = Mix(itemHash);
                    sum += mixed;
                    xor ^= mixed;
                    count++;
                }

                int hash = startingHash;
                hash = Combine(hash, count);
                hash = Combine(hash, Mix(sum));
                hash = Combine(hash, Mix(xor));
                return hash;
            }
        }
    }
}
