using System.Collections;
using System.Collections.Generic;

namespace cntema3
{
    internal class Program
    {
        struct SparseElement
        {
            public int lin, col;
            public double val;

            public static SparseElement operator +(SparseElement a, SparseElement b)
            {
                SparseElement res = new SparseElement();
                return res;
            }
        }

        class SparseMatrix
        {
            private List<SparseElement> _matrix = new List<SparseElement>();
            
        }
        public static void Main(string[] args)
        {
            
        }
        
    }
}