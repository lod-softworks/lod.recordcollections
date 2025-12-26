namespace System.Collections;

partial class RecordCollectionComparer
{
    private sealed class OrderedEnumerableStrategy<T> : IComparisonStrategy
    {
        public bool Equals(IReadOnlyRecordCollection x, object y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (y is null) return false;

            if (x is not IEnumerable<T> seqX) return false;
            if (y is not IEnumerable<T> seqY) return false;

            EqualityComparer<T> eq = EqualityComparer<T>.Default;

            using IEnumerator<T> e1 = seqX.GetEnumerator();
            using IEnumerator<T> e2 = seqY.GetEnumerator();

            while (true)
            {
                bool m1 = e1.MoveNext();
                bool m2 = e2.MoveNext();
                if (m1 != m2) return false;
                if (!m1) return true;
                if (!eq.Equals(e1.Current, e2.Current)) return false;
            }
        }

        public int GetHashCode(IReadOnlyRecordCollection x, int startingHash)
        {
            if (x is not IEnumerable<T> seqX) return startingHash;

            unchecked
            {
                int hash = startingHash;
                int i = 0;
                EqualityComparer<T> eq = EqualityComparer<T>.Default;

                foreach (T item in seqX)
                {
                    int itemHash = eq.GetHashCode(item!);
                    hash = Combine(hash, Mix(itemHash ^ i));
                    i++;
                }

                hash = Combine(hash, i);
                return hash;
            }
        }
    }
}
