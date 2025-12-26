namespace System.Collections;

partial class RecordCollectionComparer
{
    private sealed class DictionaryStrategy<TKey, TValue> : IComparisonStrategy
        where TKey : notnull
    {
        public bool Equals(IReadOnlyRecordCollection x, object y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (y is null) return false;

            if (x is not IDictionary<TKey, TValue> dictX) return false;
            if (y is not IDictionary<TKey, TValue> dictY) return false;
            if (dictX.Count != dictY.Count) return false;

            EqualityComparer<TValue> eq = EqualityComparer<TValue>.Default;

            if (dictY is Dictionary<TKey, TValue> concreteY)
            {
                foreach (KeyValuePair<TKey, TValue> kv in dictX)
                {
                    if (!concreteY.TryGetValue(kv.Key, out TValue? otherValue)) return false;
                    if (!eq.Equals(kv.Value, otherValue)) return false;
                }

                return true;
            }

            foreach (KeyValuePair<TKey, TValue> kv in dictX)
            {
                if (!dictY.TryGetValue(kv.Key, out TValue? value)) return false;
                if (!eq.Equals(kv.Value, value)) return false;
            }

            return true;
        }

        public int GetHashCode(IReadOnlyRecordCollection x, int startingHash)
        {
            if (x is not IEnumerable<KeyValuePair<TKey, TValue>> seqX) return startingHash;

            unchecked
            {
                int sum = 0;
                int xor = 0;
                int count = 0;

                EqualityComparer<TKey> keyEq = EqualityComparer<TKey>.Default;
                EqualityComparer<TValue> valueEq = EqualityComparer<TValue>.Default;

                foreach (KeyValuePair<TKey, TValue> kv in seqX)
                {
                    int keyHash = keyEq.GetHashCode(kv.Key);
                    int valueHash = valueEq.GetHashCode(kv.Value!);
                    int entryHash = Combine(Mix(keyHash), Mix(valueHash));

                    sum += entryHash;
                    xor ^= entryHash;
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
