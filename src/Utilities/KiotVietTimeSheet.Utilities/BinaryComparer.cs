using System.Collections;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Utilities
{
    public static class BinaryComparer
    {
        /// <summary>
        /// Comparer both binary value
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <returns>
        /// 0 if equal
        /// lower 0 if lower
        /// higher 0 if higher
        /// </returns>
        public static int Compare(this byte[] item1, byte[] item2)
        {
            if (item1 == null && item2 == null)
                return 0;
            else if (item1 == null)
                return -1;
            else if (item2 == null)
                return 1;
            return ((IStructuralComparable)item1).CompareTo(item2, Comparer<byte>.Default);
        }
    }
}
