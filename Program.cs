#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
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
                var res = Row < other.Row || Row == other.Row && Col < other.Col;
                return res ? -1 : 1;
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
            public void Sort() => _matrix.Sort();

            private int Count() => _matrix.Count;
            public int Size { get; set; }

            // ReSharper disable once MemberCanBePrivate.Local
            public override string ToString()
            {
                return _matrix.Aggregate(string.Empty, (current, t) => current + t + Environment.NewLine);
            }

            public SparseMatrix() {}

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

            public static bool operator ==(SparseMatrix a, SparseMatrix b)
            {
                var bminus = ~b;
                var sum = a + bminus;
                for (var i = 0; i < sum.Count(); i++)
                {
                    if (sum[i].Val > SparseMatrix.Epsilon)
                    {
                        return false;
                    }
                }
                return true;
            }

            public const double Epsilon = 0.0001;

            public static SparseMatrix operator ~(SparseMatrix a)
            {
                var result = new SparseMatrix();
                for (var i = 0; i < a.Count(); i++)
                    result.Push(new SparseElement()
                    {
                        Row = a[i].Row,
                        Col = a[i].Col,
                        Val = 0 - a[i].Val
                    });
                return result;
            }

            public static bool operator !=(SparseMatrix a, SparseMatrix b)
            {
                return !(a == b);
            }

            // ReSharper disable once PossibleNullReferenceException
            // ReSharper disable once AssignNullToNotNullAttribute
            public static SparseMatrix ReadSparseMatrix(string path)
            {
                var result = new SparseMatrix();
                var reader = new StreamReader(path);
                var line = reader.ReadLine();
                result.Size = int.Parse(line);
                reader.ReadLine();

                while (null != (line = reader.ReadLine()))
                {
                    var tokens = line.Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    var elem = new SparseElement()
                    {
                        Val = double.Parse(tokens[0]),
                        Row = int.Parse(tokens[1]),
                        Col = int.Parse(tokens[2]),
                    };
                    if (result[elem.Row, elem.Col] == null)
                        result.Push(elem);
                    else
                    {
                        result[elem.Row, elem.Col] = new SparseElement()
                        {
                            Row = elem.Row,
                            Col = elem.Col,
                            Val = result[elem.Row, elem.Col]!.Val + elem.Val
                        };
                    }
                }

                return result;
            }

            public static SparseMatrix ReadTriDiagonal(string path)
            {
                int lineIndex = 1;
                var result = new SparseMatrix();
                var reader = new StreamReader(path);
                result.Size = int.Parse(reader.ReadLine());
                var p = int.Parse(reader.ReadLine());
                var q = int.Parse(reader.ReadLine());
                
                
                reader.ReadLine();
                lineIndex += 4;
                var line = string.Empty;
                
                

                int counter = 0;
                while (counter < result.Size)
                {
                    line = reader.ReadLine();
                    lineIndex++;
                    var tokens = line.Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length == 0)
                    {
                        Console.WriteLine($"###{line}###");
                    }
                    var elem = new SparseElement()
                    {
                        Val = double.Parse(tokens[0]),
                        Row = counter,
                        Col = counter,
                    };
                    if (result[elem.Row, elem.Col] == null)
                        result.Push(elem);
                    else
                    {
                        result[elem.Row, elem.Col] = new SparseElement()
                        {
                            Row = elem.Row,
                            Col = elem.Col,
                            Val = result[elem.Row, elem.Col]!.Val + elem.Val
                        };
                    }

                    counter++;
                }
                reader.ReadLine();
                
                int counter2 = 0;
                while (counter2 < result.Size - p)
                {
                    line = reader.ReadLine();
                    lineIndex++;
                    var tokens = line.Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length == 0)
                    {
                        Console.WriteLine($"%%%{lineIndex}%%%");
                    }
                    var elem = new SparseElement()
                    {
                        Val = double.Parse(tokens[0]),
                        Row = counter2,
                        Col = counter2 + p,
                    };
                    if (result[elem.Row, elem.Col] == null)
                        result.Push(elem);
                    else
                    {
                        result[elem.Row, elem.Col] = new SparseElement()
                        {
                            Row = elem.Row,
                            Col = elem.Col,
                            Val = result[elem.Row, elem.Col]!.Val + elem.Val
                        };
                    }

                    counter2++;
                }
                reader.ReadLine();
                var counter3 = 0;
                while (counter3 < result.Size - q)
                {
                    line = reader.ReadLine();
                    var tokens = line.Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    var elem = new SparseElement()
                    {
                        Val = double.Parse(tokens[0]),
                        Row = counter3 + q,
                        Col = counter3,
                    };
                    if (result[elem.Row, elem.Col] == null)
                        result.Push(elem);
                    else
                    {
                        result[elem.Row, elem.Col] = new SparseElement()
                        {
                            Row = elem.Row,
                            Col = elem.Col,
                            Val = result[elem.Row, elem.Col]!.Val + elem.Val
                        };
                    }
                    counter3++;
                }

                return result;
            }

            public SparseMatrix Transpusa()
            {
                SparseMatrix result = new();

                for (var i = 0; i < Count(); i++)
                {
                    result.Push(new SparseElement()
                    {
                        Row = this[i].Col,
                        Col = this[i].Row,
                        Val = this[i].Val
                    });
                }

                result.Sort();
                return result;
            }

            public SparseMatrix Multiply(SparseMatrix B)
            {
                B = B.Transpusa();
                SparseMatrix result = new() {Size = this.Size};

                int idxA = 0, idxB = 0;
                for (idxA = 0; idxA < Count();)
                {
                    var r = this[idxA].Row;
                    for (idxB = 0; idxB < B.Count();)
                    {
                        int c = B[idxB].Row;
                        int tempA = idxA;
                        int tempB = idxB;

                        double sum = 0;
                        while (tempA < Count() && this[tempA].Row == r &&
                               tempB < B.Count() && B[tempB].Row == c)
                        {
                            if (this[tempA].Col < B[tempB].Col)
                            {
                                tempA++;
                            }
                            else if (this[tempA].Col > B[tempB].Col)
                            {
                                tempB++;
                            }
                            else
                            {
                                sum += this[tempA].Val * B[tempB].Val;
                                tempA++;
                                tempB++;
                            }
                        }

                        if (sum != 0)
                            result.Push(new SparseElement() {Row = r, Col = c, Val = sum});
                        while (idxB < B.Count() && B[idxB].Row == c)
                        {
                            idxB++;
                        }
                    }

                    while (idxA < Count() && this[idxA].Row == r)
                    {
                        idxA++;
                    }
                }

                return result;
            }
        }

        private static readonly string BasePath =
            Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName;

        private static readonly string pathMatrixA = $@"{BasePath}\a.txt";
        private static readonly string pathMatrixB = $@"{BasePath}\b.txt";
        private static readonly string pathMatrixAplusB = $@"{BasePath}\aplusb.txt";
        private static readonly string pathMatrixAoriB = $@"{BasePath}\aorib.txt";

        public static void Main()
        {
            var a = SparseMatrix.ReadSparseMatrix(pathMatrixA);
            var b = SparseMatrix.ReadTriDiagonal(pathMatrixB);

            var aplusb = a + b;
            var aorib = a.Multiply(b);
            //var givenAplusB = SparseMatrix.ReadSparseMatrix(pathMatrixAplusB);
            var givenAoriB = SparseMatrix.ReadSparseMatrix(pathMatrixAoriB);
            Console.WriteLine(givenAoriB == aorib);
        }
    }
}