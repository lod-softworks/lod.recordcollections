namespace System.Collections;

partial class RecordCollectionComparer
{
    private sealed class ListStrategy<T> : IComparisonStrategy
    {
        public bool Equals(IReadOnlyRecordCollection x, object y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (y is null) return false;

            if (x is not IList<T> listX) return false;
            if (y is not IList<T> listY) return false;
            if (listX.Count != listY.Count) return false;

            EqualityComparer<T> eq = EqualityComparer<T>.Default;
            for (int i = 0; i < listX.Count; i++)
            {
                if (!eq.Equals(listX[i], listY[i])) return false;
            }

            return true;
        }

        public int GetHashCode(IReadOnlyRecordCollection x, int startingHash)
        {
            if (x is not IList<T> listX) return startingHash;

            unchecked
            {
                int hash = startingHash;
                hash = Combine(hash, listX.Count);

                EqualityComparer<T> eq = EqualityComparer<T>.Default;
                for (int i = 0; i < listX.Count; i++)
                {
                    int itemHash = eq.GetHashCode(listX[i]!);
                    hash = Combine(hash, Mix(itemHash ^ i));
                }

                return hash;
            }
        }
    }
}
