#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace cntema3
{
    internal static class Program
    {
        private class SparseElement : IComparable<SparseElement>
        {
            public int Row { get; set; }

            public int Col { get; set; }

            public double Val { get; set; }

            public int CompareTo(SparseElement other)
            {
                if (Equals(other))
                    return 0;
                var res = Row < other.Row || Row == other.Row && Col == other.Col;
                return res ? 1 : -1;
            }

            public override string ToString()
            {
                return $"( Row : {Row}; Col : {Col}; Val : {Val} )";
            }
        }

        private class SparseMatrix
        {
            private List<SparseElement> _matrix = new();
            private SparseElement this[int i] => _matrix[i];
            private void Push(SparseElement e) => _matrix.Add(e);
            private void Sort() => _matrix.Sort();

            private int Count() => _matrix.Count;

            // ReSharper disable once MemberCanBePrivate.Local
            public override string ToString()
            {
                return _matrix.Aggregate(string.Empty, (current, t) => current + t);
            }

            public SparseMatrix()
            {
                
            }

            public SparseMatrix(string filePath)
            {
                // TODO read matrix from file
                throw new NotImplementedException();
            }
            private SparseElement? this[int row, int col]
            {
                get
                {
                    for (var i = 0; i < Count(); i++)
                    {
                        if (this[i].Row == row && this[i].Col == col)
                            return this[i];
                    }

                    return null;
                }
                set
                {
                    if (value == null)
                        throw new NullReferenceException("Setter is null!");
                    for (var i = 0; i < Count(); i++)
                    {
                        if (this[i].Row == value.Row && this[i].Col == value.Col)
                        {
                            this[i].Val = value.Val;
                            return;
                        }
                    }

                    throw new
                        ArgumentException($"No value at ({value.Row}, {value.Col}) !");
                }
            }

            public static SparseMatrix operator +(SparseMatrix a, SparseMatrix b)
            {
                var res = new SparseMatrix();
                a.Sort();
                b.Sort();
                var idxA = 0;
                var idxB = 0;
                while (idxA < a.Count() && idxB < b.Count())
                {
                    if (a[idxA].Row == b[idxB].Row)
                    {
                        if (a[idxA].Col == b[idxB].Col)
                        {
                            if (a[idxA].Val + b[idxB].Val != 0)
                                res.Push(new SparseElement
                                    {Row = a[idxA].Row, Col = a[idxA].Col, Val = a[idxA].Val + b[idxB].Val});
                            idxA++;
                            idxB++;
                        }
                        else if (a[idxA].Col < b[idxB].Col)
                        {
                            res.Push(a[idxA]);
                            idxA++;
                        }
                        else
                        {
                            res.Push(b[idxB]);
                            idxB++;
                        }
                    }
                    else if (a[idxA].Row < b[idxB].Row)
                    {
                        res.Push(a[idxA]);
                        idxA++;
                    }
                    else
                    {
                        res.Push(b[idxB]);
                        idxB++;
                    }
                }

                if (idxA < a.Count())
                {
                    for (var j = idxA; j < a.Count(); j++)
                        res.Push(a[idxA]);
                }

                if (idxB < b.Count())
                {
                    for (var j = idxB; j < b.Count(); j++)
                        res.Push(b[idxB]);
                }

                return res;
            }
        }

        public static void Main()
        {
            string pathMatrixA = "";
            string pathMatrixB = "";
            var a = new SparseMatrix(pathMatrixA);
            var b = new SparseMatrix(pathMatrixB);
            Console.WriteLine(a + b);
        }
    }
}