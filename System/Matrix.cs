namespace System
{
    using Collections.Generic;
    using Text;

    /// <summary>
    ///   Complex Matrix Class
    /// </summary>
    public sealed class Matrix : ICloneable
    {
        private const double EPSILON = 0.00001;

        #region Structs & Enums

        #region DefinitenessType enum

        /// <summary>
        /// </summary>
        public enum DefinitenessType
        {
            /// <summary>
            /// </summary>
            Indefinite,

            /// <summary>
            /// </summary>
            NegativeDefinite,

            /// <summary>
            /// </summary>
            NegativeSemidefinite,

            /// <summary>
            /// </summary>
            PositiveDefinite,

            /// <summary>
            /// </summary>
            PositiveSemidefinite,
        }

        #endregion

        #region Dimens enum

        /// <summary>
        /// </summary>
        public enum Dimens
        {
            /// <summary>
            /// </summary>
            Row,

            /// <summary>
            /// </summary>
            Column,
        };

        #endregion

        #endregion

        private Complex[,] _matrix;

        /// <summary>
        /// </summary>
        public Complex[] Characteristic
        {
            get
            {
                if (!IsSquare) return default(Complex[]);
                var dims = Math.Min(Rows, Cols);
                var charCoeff = new Complex[dims + 1];
                var adjCoeff = new Matrix[dims + 1];
                charCoeff[0] = 1;
                var I = adjCoeff[0] = new Matrix(dims);
                for (var i = 1; i < dims + 1; ++i)
                {
                    var detCoeff = this * adjCoeff[i - 1];
                    charCoeff[i] = -detCoeff.Trace / i;
                    adjCoeff[i] = detCoeff + charCoeff[i] * I;
                }
                return charCoeff;
            }
        }

        #region ICloneable Members

        Object ICloneable.Clone()
        {
            return new Matrix(this); //this.MemberwiseClone();
        }

        #endregion

        private void ReSize(int m, int n, bool preserve = false)
        {
            if (default(Complex[,]) != _matrix && m == Rows && n == Cols) return;
            var rows = Rows;
            var cols = Cols;
            if (0 == m && 0 == n) _matrix = new Complex[0, 0];
            else
            {
                var martix = default(Complex[,]);
                if (default(Complex[,]) != _matrix)
                {
                    if (preserve)
                    {
                        martix = new Complex[rows, cols];
                        for (uint i = 0; i < rows; ++i) for (uint j = 0; j < cols; ++j) martix[i, j] = _matrix[i, j];
                    }
                }
                _matrix = new Complex[m, n];
                if (default(Complex[,]) != martix)
                {
                    var rowLim = Math.Min((int) rows, m);
                    var colLim = Math.Min((int) cols, n);
                    for (var i = 0; i < rowLim; ++i) for (var j = 0; j < colLim; ++j) _matrix[i, j] = martix[i, j];
                }
            }
        }

        #region Constructors and Destructors

        /// <summary>
        ///   Constructs a matrix and assigns value to diagonal elements.
        /// </summary>
        /// <param name="m"> Number of rows. </param>
        /// <param name="n"> Number of columns. </param>
        /// <param name="diagonal"> Value to assign to the diagnoal elements. </param>
        public Matrix(uint m, uint n, Complex diagonal)
        {
            _matrix = new Complex[m, n];
            for (var i = 0; i < m; ++i) for (var j = 0; j < n; ++j) _matrix[i, j] = (i == j) ? diagonal : Complex.Zero;
        }

        /// <summary>
        ///   Constructs an empty matrix.
        /// </summary>
        /// <param name="m"> Number of rows. </param>
        /// <param name="n"> Number of columns. </param>
        public Matrix(uint m, uint n)
            : this(m, n, Complex.Zero) { }

        /// <summary>
        ///   Constructs a square matrix and complex diagonal elements.
        /// </summary>
        /// <param name="sqr"> Number of rows and columns. </param>
        /// <param name="diagonal"> Value assign to the diagnoal elements. </param>
        public Matrix(uint sqr, Complex diagonal)
            : this(sqr, sqr, diagonal) { }

        /// <summary>
        ///   Constructs a unit square matrix
        /// </summary>
        /// <param name="sqr"> Number of rows and columns. </param>
        public Matrix(uint sqr)
            : this(sqr, Complex.One) { }

        /// <summary>
        ///   Constructs a square matrix and assigns diagonal[] elements.
        /// </summary>
        /// <param name="diagonals"> Value assign to the diagnoal elements. </param>
        public Matrix(params Complex[] diagonals)
            : this((uint) diagonals.Length, (uint) diagonals.Length)
        {
            var rows = Rows;
            var cols = Cols;
            for (var i = 0; i < rows; ++i) for (var j = 0; j < cols; ++j) if (i == j) _matrix[i, j] = diagonals[i];
        }

        /// <summary>
        ///   Constructs a matrix and assigns diagonal[] elements.
        /// </summary>
        /// <param name="m"> Number of rows. </param>
        /// <param name="n"> Number of columns. </param>
        /// <param name="diagonals"> Value assign to the diagnoal elements. </param>
        public Matrix(uint m, uint n, params Complex[] diagonals)
            : this(m, n)
        {
            for (var i = 0; i < m; ++i)
            {
                for (var j = 0; j < n; ++j)
                {
                    if (i == j)
                    {
                        _matrix[i, j] = (i < diagonals.Length)
                                            ? diagonals[i]
                                            : default(Complex);
                    }
                }
            }
        }

        /// <summary>
        ///   Constructs a 1x1 matrix
        /// </summary>
        public Matrix(Complex complex)
            : this(1, 1, complex) { }

        /// <summary>
        ///   Constructs row/column vector from Complex array.
        /// </summary>
        public Matrix(IEnumerable<Complex> complex, Dimens dim)
        {
            if (null == complex) throw new ArgumentNullException("complex");
        }

        /// <summary>
        ///   Constructs a matrix from the array.
        /// </summary>
        /// <param name="table"> The array the matrix gets constructed from. </param>
        public Matrix(Complex[,] table)
            : this((uint) table.GetLength(0), (uint) table.GetLength(1))
        {
            Initialize(table);
        }

        /// <summary>
        ///   Copy Constructor.
        /// </summary>
        public Matrix(Matrix mtx)
            : this(mtx._matrix) { }

        /// <summary>
        ///   Destructor.
        /// </summary>
        ~Matrix()
        {
            _matrix = default(Complex[,]);
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Returns the number of Rows.
        /// </summary>
        public uint Rows
        {
            get { return (uint) _matrix.GetLength(0); }
            set
            {
                //if( value > 0 )
                {
                    ReSize((int) value, (int) Cols, true);
                }
            }
        }

        /// <summary>
        ///   Returns the number of Columns.
        /// </summary>
        public uint Cols
        {
            get { return (uint) _matrix.GetLength(1); }
            set
            {
                //if( value > 0 )
                {
                    ReSize((int) Rows, (int) value, true);
                }
            }
        }

        /// <summary>
        ///   Returns the product of Rows and Columns.
        /// </summary>
        public uint Order
        {
            get { return Rows * Cols; }
            //set { }
        }

        /// <summary>
        /// </summary>
        public String Dimensions
        {
            get { return String.Format("({0},{1})", Rows, Cols); }
            //set { }
        }

        /// <summary>
        ///   Access the element at the given location.
        /// </summary>
        public Complex this[int i, int j]
        {
            get { return _matrix[i % Rows, j % Cols]; }
            set
            {
                //// dynamically add new rows/columns...
                //if (i >= Rows || j >= Cols)
                //{
                //    ReSize(i + 1, j + 1, true);
                //}
                //_matrix[i, j] = value;
                _matrix[i % Rows, j % Cols] = value;
            }
        }

        /// <summary>
        /// </summary>
        public Complex[,] Elements
        {
            get
            {
                uint rows = Rows, cols = Cols;
                var elements = new Complex[rows, cols];
                for (uint i = 0; i < rows; ++i) for (uint j = 0; j < cols; ++j) elements[i, j] = _matrix[i, j];
                return elements;
            }
            set { Initialize(value); }
        }

        /// <summary>
        ///   Access to the i-th component of an n by one matrix (column vector) or one by n matrix (row vector).
        /// </summary>
        /// <param name="index"> One-based index. </param>
        public Complex this[int index]
        {
            get
            {
                if (1 == Rows) // row vector
                    return _matrix[0, index];
                if (1 == Cols) // coumn vector
                    return _matrix[index, 0];
                throw new InvalidOperationException("General matrix acces requires double indexing.");
            }
            set
            {
                var rows = (int) Rows;
                var cols = (int) Cols;
                if (1 == rows) // row vector
                {
                    // dynamically extend vector if necessary
                    if (index >= cols) ReSize(rows, index + 1, true);
                    _matrix[0, index] = value;
                    return;
                }
                if (1 == cols) // column vector
                {
                    // dynamically extend vector if necessary
                    if (index >= rows) ReSize(index + 1, cols, true);
                    _matrix[index, 0] = value;
                    return;
                }
                throw new InvalidOperationException("Cannot access multidimensional matrix via single index.");
            }
        }

        #region ReadOnly Boolean

        /// <summary>
        ///   Return <see langword="true" /> if the matrix is a Square matrix.
        /// </summary>
        public bool IsSquare
        {
            get { return Rows == Cols; }
        }

        /// <summary>
        ///   Checks if matrix consists only of real entries.
        /// </summary>
        /// <returns> True iff all entries are real. </returns>
        public bool IsReal
        {
            get
            {
                var rows = Rows;
                var cols = Cols;
                for (var i = 0; i < rows; ++i) for (var j = 0; j < cols; ++j) if (!_matrix[i, j].IsReal) return false;
                return true;
            }
        }

        /// <summary>
        ///   Checks if matrix consists only of imaginary entries.
        /// </summary>
        /// <returns> True iff all entries are imaginary. </returns>
        public bool IsImag
        {
            get
            {
                var rows = Rows;
                var cols = Cols;
                for (var i = 0; i < rows; ++i) for (var j = 0; j < cols; ++j) if (!_matrix[i, j].IsImag) return false;
                return true;
            }
        }

        /// <summary>
        ///   Return <see langword="true" /> if Off Diagonals are zero.
        /// </summary>
        public bool IsDiagonal
        {
            get
            {
                if (!IsSquare) return false;
                var rows = Rows;
                var cols = Cols;
                for (var i = 0; i < rows; ++i)
                {
                    for (var j = 0; j < cols; ++j) //    if (i == j) continue;
                        //    if (M[i, j] != 0) return false;
                        if (i != j && 0 != _matrix[i, j]) return false;
                }
                return true;
            }
        }

        /// <summary>
        ///   Returns <see langword="true" /> if the matrix is Symmetric.
        /// </summary>
        public bool IsSymmetric
        {
            get
            {
                return IsSquare && this == Transpose();
                //if (!IsSquare) return false;
                //var rows = Rows;
                //var cols = Cols;
                //for (var i = 0; i < rows; ++i)
                //    //for (int j = 0; j < i; ++j)
                //    for (var j = i + 1; j < cols; ++j)
                //        if (_matrix[i, j] != _matrix[j, i])
                //            return false;
                //return true;
            }
        }

        /// <summary>
        ///   Matrix A is Hermitian iff A^H = A, where A^H is the conjugated transposed of A.
        /// </summary>
        /// <returns> True iff matrix is Hermitian. </returns>
        public bool IsHermitian
        {
            get { return IsSquare && this == ConjTranspose(); }
        }

        /// <summary>
        ///   Returns <see langword="true" /> if the matrix is Singular.
        /// </summary>
        public bool IsSingular
        {
            get { return (0.0 == Determinant); }
        }

        /// <summary>
        ///   Returns <see langword="true" /> if all elements are +ve and each row sum up to 1.
        /// </summary>
        public bool IsStochastic
        {
            get
            {
                var rows = Rows;
                var cols = Cols;
                for (var i = 0; i < rows; ++i)
                {
                    var rowSum = 0.0;
                    for (var j = 0; j < cols; ++j)
                    {
                        var real = _matrix[i, j].Real; //.Abs();
                        if (real < 0) return false;
                        rowSum += real;
                    }
                    if (Math.Abs(rowSum - 1) > EPSILON) return false;
                }
                return true;
            }
        }

        /// <summary>
        /// </summary>
        public bool IsNilPotent
        {
            get
            {
                if (!IsSquare) return false;
                var nilPotent = this;
                var nullMatrix = Zeros(Rows, Cols);
                for (var nil = 0; nil < 5; ++nil) if ((nilPotent *= this) == nullMatrix) return true;
                return false;
            }
        }

        /// <summary>
        ///   Is M*M = M.
        /// </summary>
        public bool IsIdemPotent
        {
            get { return IsSquare && this * this == this; }
        }

        /// <summary>
        ///   Is M*M = I.
        /// </summary>
        public bool IsInvolutary
        {
            get { return IsSquare && this * this == Identity(Rows); }
        }

        #endregion

        #region Norms and Condition #

        /// <summary>
        ///   Returns the Frobenius Norm for the matrix.
        /// </summary>
        /// <value> The square root of sum of square of all elements. </value>
        public double FrobeniusNorm
        {
            get
            {
                //Complex sumSqr = 0.0;
                var sumSqr = 0.0;
                var rows = Rows;
                var cols = Cols;
                for (var i = 0; i < rows; ++i)
                {
                    for (var j = 0; j < cols; ++j) //sumSqr += Complex.Pow(_matrix[i, j], 2);
                        sumSqr += (_matrix[i, j] * _matrix[i, j].Conjugate()).Real;
                }
                //return Complex.Sqrt(sumSqr).Abs();
                return Math.Sqrt(sumSqr);
            }
        }

        /// <summary>
        ///   Returns the Row Sum Norm for the matrix.
        /// </summary>
        /// <value> The max of sum of Abs rows. </value>
        public double RowSumNorm
        {
            get
            {
                var maxSumAbsRow = 0.0;
                var rows = Rows;
                var cols = Cols;
                var sumAbsRow = new Complex[rows];
                for (var i = 0; i < rows; ++i) sumAbsRow[i] = Complex.Zero;
                for (var i = 0; i < rows; ++i)
                {
                    for (var j = 0; j < cols; ++j) sumAbsRow[i] += _matrix[i, j].Abs();
                    maxSumAbsRow = Math.Max(maxSumAbsRow, sumAbsRow[i].Abs());
                }
                return maxSumAbsRow;
            }
        }

        /// <summary>
        ///   Returns the Column Sum Norm for the matrix.
        /// </summary>
        /// <value> The max of sum of Abs columns. </value>
        public double ColSumNorm
        {
            get
            {
                var maxSumAbsCol = 0.0;
                var rows = Rows;
                var cols = Cols;
                var sumAbsCol = new Complex[cols];
                for (var i = 0; i < cols; ++i) sumAbsCol[i] = Complex.Zero;
                for (var j = 0; j < cols; ++j)
                {
                    for (var i = 0; i < rows; ++i) sumAbsCol[j] += _matrix[i, j].Abs();
                    maxSumAbsCol = Math.Max(maxSumAbsCol, sumAbsCol[j].Abs());
                }
                return maxSumAbsCol;
            }
        }

        /// <summary>
        /// </summary>
        public double RowConditionNum
        {
            get { return RowSumNorm * Inverse().RowSumNorm; }
        }

        /// <summary>
        /// </summary>
        public double ColConditionNum
        {
            get { return ColSumNorm * Inverse().ColSumNorm; }
        }

        #endregion

        #region Advance

        /// <summary>
        ///   Returns the trace of the matrix.
        /// </summary>
        /// <returns> Sum of the diagonal elements. </returns>
        public Complex Trace
        {
            get
            {
                if (!IsSquare) throw new InvalidOperationException("Non-square matrix.");
                var trace = new Complex(0);
                var dims = Math.Min(Rows, Cols);
                for (var d = 0; d < dims; ++d) trace += _matrix[d, d];
                return trace;
            }
        }

        /// <summary>
        ///   Determinant if matrix is square.
        /// </summary>
        public Complex Determinant
        {
            get
            {
                if (!IsSquare) throw new InvalidOperationException("Non-square matrix.");
                var dims = Math.Min(Rows, Cols);
                if (dims == 1) return _matrix[0, 0];
                if (dims == 2) return _matrix[0, 0] * _matrix[1, 1] - _matrix[0, 1] * _matrix[1, 0];
                var det = new Complex(0);
                var expandBy = Dimens.Row;
                const uint which = 0;
                switch (expandBy)
                {
                case Dimens.Row:
                    for (uint i = 0; i < dims; ++i) det += _matrix[i, which] * CoFactor(i, which);
                    break;
                case Dimens.Column:
                    for (uint j = 0; j < dims; ++j) det += _matrix[which, j] * CoFactor(which, j);
                    break;
                }
                return det;
            }
        }

        #endregion

        #endregion

        #region Statics

        #region Properties

        #endregion

        #region Methods

        /// <summary>
        ///   Creates m by n matrix filled with zeros.
        /// </summary>
        /// <param name="m"> Number of rows. </param>
        /// <param name="n"> Number of columns. </param>
        /// <returns> m by n matrix filled with zeros. </returns>
        public static Matrix Zeros(uint m, uint n)
        {
            return new Matrix(m, n);
        }

        /// <summary>
        ///   Creates n by n matrix filled with zeros.
        /// </summary>
        /// <param name="n"> Number of rows and columns, resp. </param>
        /// <returns> n by n matrix filled with zeros. </returns>
        public static Matrix Zeros(uint n)
        {
            return new Matrix(n, n);
        }

        /// <summary>
        ///   Creates m by n matrix filled with ones.
        /// </summary>
        /// <param name="m"> Number of rows. </param>
        /// <param name="n"> Number of columns. </param>
        /// <returns> m by n matrix filled with ones. </returns>
        public static Matrix Ones(uint m, uint n)
        {
            return new Matrix(m, n, Complex.One);
        }

        /// <summary>
        ///   Creates n by n matrix filled with ones.
        /// </summary>
        /// <param name="n"> Number of columns. </param>
        /// <returns> n by n matrix filled with ones. </returns>
        public static Matrix Ones(uint n)
        {
            return new Matrix(n, n, Complex.One);
        }

        /// <summary>
        ///   Creates n by n identity matrix.
        /// </summary>
        /// <param name="n"> Number of rows and columns respectively. </param>
        /// <returns> n by n identity matrix. </returns>
        public static Matrix Identity(uint n)
        {
            return new Matrix(n);
        }

        /// <summary>
        /// </summary>
        /// <param name="source"> </param>
        /// <param name="destiny"> </param>
        public static void Copy(Matrix source, ref Matrix destiny)
        {
            if (source.Dimensions != destiny.Dimensions) throw new ArgumentException("Can't copy. Dimensions doesn't matches (+,-).");
            destiny = new Matrix(source);
        }

        /// <summary>
        ///   Determines weather two instances are equal.
        /// </summary>
        public static bool Equals(Matrix m1, Matrix m2)
        {
            return (m1 == m2);
        }

        /// <summary>
        ///   Determines weather two instances are not equal.
        /// </summary>
        public static bool NotEquals(Matrix m1, Matrix m2)
        {
            return !Equals(m1, m2);
        }

        /// <summary>
        ///   Determines weather two instances are Dimensionally equal.
        /// </summary>
        public static bool Equalent(Matrix m1, Matrix m2)
        {
            return (m1.Rows == m2.Rows) && (m1.Cols == m2.Cols); //(m1.Dimensions == m2.Dimensions);
        }

        /// <summary>
        ///   Returns a Scalar matrix of the given size.
        /// </summary>
        public static Matrix Scalar(uint rows, uint cols, Complex complex)
        {
            return new Matrix(rows, cols, complex);
        }

        /// <summary>
        ///   Returns a diagonal matrix of the given size.
        /// </summary>
        public static Matrix Diagonals(uint rows, uint cols, params Complex[] diag)
        {
            return new Matrix(rows, cols, diag);
        }

        /// <summary>
        ///   Returns a matrix filled with random values.
        /// </summary>
        public static Matrix Randoms(uint rows, uint cols)
        {
            var arrRnd = new Complex[rows, cols];
            var random = new Random();
            for (var i = 0; i < rows; ++i) for (var j = 0; j < cols; ++j) arrRnd[i, j] = new Complex(random.NextDouble(), random.NextDouble());
            return new Matrix(arrRnd);
        }

        /// <summary>
        ///   Constructs block matrix [A, B; C, D].
        /// </summary>
        /// <param name="A"> Upper left sub matrix. </param>
        /// <param name="B"> Upper right sub matrix. </param>
        /// <param name="C"> Lower left sub matrix. </param>
        /// <param name="D"> Lower right sub matrix. </param>
        public static Matrix BlockMatrix(Matrix A, Matrix B, Matrix C, Matrix D)
        {
            var rowsA = A.Rows;
            var rowsB = B.Rows;
            var rowsC = C.Rows;
            var rowsD = D.Rows;
            var colsA = A.Cols;
            var colsB = B.Cols;
            var colsC = C.Cols;
            var colsD = D.Cols;
            if (rowsA != rowsB || rowsC != rowsD || colsA != colsC || colsB != colsD) throw new ArgumentException("Matrix dimensions must agree.");
            var block = new Matrix(rowsA + rowsC, colsA + colsB);
            for (var i = 0; i < rowsA + rowsC; ++i)
            {
                for (var j = 0; j < colsA + colsB; ++j)
                {

                    //if (i < rowsA)
                    //{
                    //    if (j < colsA)
                    //        block[i, j] = A[i, j];
                    //    else
                    //        block[i, j] = B[i, j - colsA];
                    //}
                    //else
                    //{
                    //    if (j < colsC)
                    //        block[i, j] = C[i - rowsA, j];
                    //    else
                    //        block[i, j] = D[i - rowsA, j - colsC];
                    //}

                    block[i, j] = (i < rowsA)
                                      ? ((j < colsA) ? A[i, j] : B[i, (int) (j - colsA)])
                                      : ((j < colsC) ? C[(int) (i - rowsA), j] : D[(int) (i - rowsA), (int) (j - colsC)]);
                }
            }
            return block;
        }

        #endregion

        #region Operators

        // Explicit Casting.
        //public static explicit operator Matrix(double[,] table)
        //{
        //    return new Matrix(table);
        //}

        /// <summary>
        ///   Implicit Casting.
        /// </summary>
        /// <param name="table"> </param>
        /// <returns> </returns>
        public static implicit operator Matrix(Complex[,] table)
        {
            return new Matrix(table);
        }

        /// <summary>
        ///   Unary minus.
        /// </summary>
        public static Matrix operator -(Matrix mtx)
        {
            if (default(Matrix) == mtx) throw new ArgumentNullException("mtx", "Null Matrix");
            var rows = mtx.Rows;
            var cols = mtx.Cols;
            var negate = new Complex[rows, cols];
            for (var i = 0; i < rows; ++i) for (var j = 0; j < cols; ++j) negate[i, j] = -mtx[i, j];
            return new Matrix(negate);
        }

        /// <summary>
        ///   Matrix-Matrix addition.
        /// </summary>
        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            if (default(Matrix) == m1) throw new ArgumentNullException("m1", "Null Matrix (m1)");
            if (default(Matrix) == m2) throw new ArgumentNullException("m2", "Null Matrix (m2)");
            if (m1.Dimensions != m2.Dimensions) throw new ArgumentException("Dimensions doesnot matches (+,-).");
            var rows = m1.Rows;
            var cols = m2.Cols;
            var add = new Complex[rows, cols];
            for (var i = 0; i < rows; ++i) for (var j = 0; j < cols; ++j) add[i, j] = m1[i, j] + m2[i, j];
            return new Matrix(add);
        }

        /// <summary>
        ///   Matrix-Matrix subtraction.
        /// </summary>
        public static Matrix operator -(Matrix m1, Matrix m2)
        {
            return m1 + (-m2);
        }

        /// <summary>
        ///   Matrix-Matrix multiplication.
        /// </summary>
        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            if (default(Matrix) == m1) throw new ArgumentNullException("m1", "Null Matrix (m1)");
            if (default(Matrix) == m2) throw new ArgumentNullException("m2", "Null Matrix (m2)");
            if (m1.Cols != m2.Rows) throw new ArgumentException("Dimensions doesnot matches (*,/).");
            var rows = m1.Rows;
            var cols = m2.Cols;
            var mul = new Complex[rows, cols];
            var midRowCol = Math.Min(m1.Cols, m2.Rows);
            for (var i = 0; i < rows; ++i)
            {
                for (var j = 0; j < cols; ++j)
                {
                    mul[i, j] = 0.0;
                    for (var k = 0; k < midRowCol; ++k) mul[i, j] += m1[i, k] * m2[k, j];
                }
            }
            return new Matrix(mul);
        }

        /// <summary>
        ///   Matrix-Matrix division.
        /// </summary>
        public static Matrix operator /(Matrix m1, Matrix m2)
        {
            if (m1 == default(Matrix)) throw new ArgumentNullException("m1", "Null Matrix (m1)");
            if (m2 == default(Matrix)) throw new ArgumentNullException("m2", "Null Matrix (m2)");
            if (!m2.IsSquare) throw new ArgumentException("m2 NotSquare");
            if (m1.Cols != m2.Rows) throw new ArgumentException("Dimensions doesnot matches (*,/).");
            var dimension = Math.Min(m1.Rows, m2.Cols);
            return (m1 == m2)
                       ? new Matrix(dimension, 1.0)
                       : m1 * m2.Inverse();
        }

        /// <summary>
        ///   Matrix-Scalar addition.
        /// </summary>
        public static Matrix operator +(Matrix mtx, Complex complex)
        {
            var rows = mtx.Rows;
            var cols = mtx.Cols;
            var add = new Complex[rows, cols];
            for (var i = 0; i < rows; ++i) for (var j = 0; j < cols; ++j) add[i, j] = mtx[i, j] + complex;
            return new Matrix(add);
        }

        /// <summary>
        ///   Matrix-Scalar subtraction.
        /// </summary>
        public static Matrix operator -(Matrix mtx, Complex complex)
        {
            var rows = mtx.Rows;
            var cols = mtx.Cols;
            var sub = new Complex[rows, cols];
            for (var i = 0; i < rows; ++i) for (var j = 0; j < cols; ++j) sub[i, j] = mtx[i, j] - complex;
            return new Matrix(sub);
        }

        /// <summary>
        ///   Matrix-Scalar multiplication.
        /// </summary>
        public static Matrix operator *(Matrix mtx, Complex complex)
        {
            return complex * mtx;
        }

        /// <summary>
        ///   Scalar-Matrix multiplication.
        /// </summary>
        public static Matrix operator *(Complex complex, Matrix mtx)
        {
            var rows = mtx.Rows;
            var cols = mtx.Cols;
            var mul = new Complex[rows, cols];
            for (var i = 0; i < rows; ++i) for (var j = 0; j < cols; ++j) mul[i, j] = mtx[i, j] * complex;
            return new Matrix(mul);
        }

        /// <summary>
        ///   Matrix-Scalar division.
        /// </summary>
        public static Matrix operator /(Matrix mtx, Complex complex)
        {
            try
            {
                if (complex == 0) throw new ArgumentException("Divide by 0.0...");
                return mtx * (1 / complex);
            }
            catch (Exception exp)
            {
                Console.Error.WriteLine(exp.Message);
            }
            return default(Matrix);
        }

        /// <summary>
        ///   Horizontal Concatenation.
        /// </summary>
        public static Matrix operator |(Matrix m1, Matrix m2)
        {
            var rows = Math.Min(m1.Rows, m2.Rows);
            var colsM1 = m1.Cols;
            var colsM2 = m2.Cols;
            if (m1.Rows != m2.Rows)
            {
                throw new ArgumentException(
                    "Concatenation of matrice is impossible.\nDimension invalid: m1[R,_]*m2[R,_]");
            }
            var concateH = new Complex[rows, colsM1 + colsM2];
            for (var i = 0; i < rows; ++i)
            {
                for (var j = 0; j < (colsM1 + colsM2); ++j)
                {
                    concateH[i, j] = (j < colsM1)
                                         ? m1[i, j]
                                         : m2[i, (int) (j - colsM1)];
                }
            }
            return new Matrix(concateH);
        }

        /// <summary>
        ///   Vertical Concatenation.
        /// </summary>
        public static Matrix operator &(Matrix m1, Matrix m2)
        {
            var cols = Math.Min(m1.Cols, m2.Cols);
            var rowsM1 = m1.Rows;
            var rowsM2 = m2.Rows;
            if (m1.Cols != m2.Cols)
            {
                throw new ArgumentException(
                    "Concatenation of matrice is impossible.\nDimension invalid: m1[_,C]*m2[_,C]");
            }
            var concateV = new Complex[rowsM1 + rowsM2, cols];
            for (var j = 0; j < cols; ++j)
            {
                for (var i = 0; i < (rowsM1 + rowsM2); ++i)
                {
                    concateV[i, j] = (i < rowsM1)
                                         ? m1[i, j]
                                         : m2[(int) (i - rowsM1), j];
                }
            }
            return new Matrix(concateV);
        }

        /// <summary>
        /// </summary>
        /// <param name="arrMtx"> </param>
        /// <returns> </returns>
        public static Matrix Add(params Matrix[] arrMtx)
        {
            if (default(Matrix[]) == arrMtx) throw new ArgumentNullException();
            var add = arrMtx[0];
            for (var index = 1; index < arrMtx.Length; ++index) add += arrMtx[index];
            return add;
        }

        /// <summary>
        /// </summary>
        /// <param name="arrMtx"> </param>
        /// <returns> </returns>
        public static Matrix Mutiply(params Matrix[] arrMtx)
        {
            if (default(Matrix[]) == arrMtx) throw new ArgumentNullException();
            var mul = arrMtx[0];
            for (var index = 1; index < arrMtx.Length; ++index) mul *= arrMtx[index];
            return mul;
        }

        /// <summary>
        /// </summary>
        /// <param name="arrMtx"> </param>
        /// <returns> </returns>
        public static Matrix HorizontalConcat(params Matrix[] arrMtx)
        {
            if (default(Matrix[]) == arrMtx) throw new ArgumentNullException();
            var concateH = arrMtx[0];
            for (var index = 1; index < arrMtx.Length; ++index) concateH |= arrMtx[index];
            return concateH;
        }

        /// <summary>
        /// </summary>
        /// <param name="arrMtx"> </param>
        /// <returns> </returns>
        public static Matrix VerticalConcat(params Matrix[] arrMtx)
        {
            if (default(Matrix[]) == arrMtx) throw new ArgumentNullException();
            var concateV = arrMtx[0];
            for (var index = 1; index < arrMtx.Length; ++index) concateV &= arrMtx[index];
            return concateV;
        }

        /// <summary>
        /// </summary>
        /// <param name="m1"> </param>
        /// <param name="m2"> </param>
        /// <returns> </returns>
        public static bool operator ==(Matrix m1, Matrix m2)
        {
            if (ReferenceEquals(m1, m2)) return true;
            if (null == m1 || null == m2) return false;
            if (m1.Dimensions != m2.Dimensions) return false;
            var rows = m1.Rows;
            var cols = m2.Cols;
            for (var i = 0; i < rows; ++i) for (var j = 0; j < cols; ++j) if (m1[i, j] != m2[i, j]) return false;
            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="m1"> </param>
        /// <param name="m2"> </param>
        /// <returns> </returns>
        public static bool operator !=(Matrix m1, Matrix m2)
        {
            return !(m1 == m2);
        }

        #endregion

        #endregion

        #region Dynamics

        #region Properties

        /// <summary>
        ///   Returns the matrix of the real parts.
        /// </summary>
        public Matrix Real
        {
            get
            {
                var rows = Rows;
                var cols = Cols;
                var realM = new Matrix(rows, cols);
                for (var i = 0; i < rows; ++i) for (var j = 0; j < cols; ++j) realM[i, j] = _matrix[i, j].Real;
                return realM;
            }
        }

        /// <summary>
        ///   Returns the matrix of the imaginary parts.
        /// </summary>
        public Matrix Imag
        {
            get
            {
                var rows = Rows;
                var cols = Cols;
                var imagM = new Matrix(rows, cols);
                for (var i = 0; i < rows; ++i) for (var j = 0; j < cols; ++j) imagM[i, j] = _matrix[i, j].Imag;
                return imagM;
            }
        }

        #endregion

        #region Methods

        #region Row Operations

        /// <summary>
        ///   Swaps rows at specified indices. When equal, nothing is done.
        /// </summary>
        /// <param name="i0"> index of first row. </param>
        /// <param name="i1"> index of second row. </param>
        public Matrix SwapRows(uint i0, uint i1)
        {
            var rows = Rows;
            var cols = Cols;
            if (i0 >= rows || i1 >= rows) throw new ArgumentException("Indices must be positive and < number of rows.");
            if (i0 != i1)
            {
                for (var j = 0; j < cols; ++j)
                {
                    var tmp = _matrix[i0, j];
                    _matrix[i0, j] = _matrix[i1, j];
                    _matrix[i1, j] = tmp;
                }
            }
            return this;
        }

        /// <summary>
        ///   Swaps columns at specified indices. When equal, nothing is done.
        /// </summary>
        /// <param name="j0"> index of first col. </param>
        /// <param name="j1"> index of second col. </param>
        public Matrix SwapColumns(int j0, int j1)
        {
            var rows = Rows;
            var cols = Cols;
            if (j0 >= cols || j1 >= cols) throw new ArgumentException("Indices must be positive and < number of cols.");
            if (j0 != j1)
            {
                for (var i = 0; i < rows; ++i)
                {
                    var tmp = _matrix[i, j0];
                    _matrix[i, j0] = _matrix[i, j1];
                    _matrix[i, j1] = tmp;
                }
            }
            return this;
        }

        /// <summary>
        /// </summary>
        /// <param name="row"> </param>
        /// <param name="complex"> </param>
        /// <returns> </returns>
        public Matrix ScaleRow(uint row, Complex complex)
        {
            row %= Rows;
            var cols = Cols;
            for (var col = 0; col < cols; ++col) _matrix[row, col] *= complex;
            return this;
        }

        /// <summary>
        /// </summary>
        /// <param name="row0"> </param>
        /// <param name="row1"> </param>
        /// <param name="scale"> </param>
        /// <returns> </returns>
        public Matrix AddScaleRow(uint row0, uint row1, Complex scale)
        {
            row0 %= Rows;
            row1 %= Rows;
            var cols = Cols;
            for (var col = 0; col < cols; ++col) _matrix[row0, col] += _matrix[row1, col] * scale;
            return this;
        }

        /// <summary>
        /// </summary>
        /// <param name="row0"> </param>
        /// <param name="rows"> </param>
        /// <returns> </returns>
        public Matrix AddRows(uint row0, params uint[] rows)
        {
            var cols = Cols;
            foreach (var i in rows) for (var j = 0; j < cols; ++j) _matrix[row0, j] += _matrix[i, j];
            return this;
        }

        #endregion


        ///// <summary>
        ///// Performs Hessenberg-Householder reduction, where {H, Q}
        ///// is returned, with H Hessenbergian, Q orthogonal and H = Q'AQ.
        ///// </summary>
        //public xMatrix[] HessenbergHouseholder()
        //{
        //    //throw new NotImplementedException("Still buggy!");
        //    if (!IsSquare)
        //        throw new InvalidOperationException("Cannot perform Hessenberg Householder decomposition of non-square matrix.");

        //    int rows = Rows;
        //    xMatrix Q = Identity(rows);
        //    xMatrix H = this.Clone();
        //    xMatrix I, N, R, P;
        //    xMatrix[] vbeta = new xMatrix[2];
        //    int m;

        //    // don't try to understand from the code alone.
        //    // this is pure magic to me - mathematics, reborn as code.
        //    for (int k = 1; k <= rows - 2; k++)
        //    {
        //        vbeta = HouseholderVector(H.Extract(k + 1, rows, k, k));
        //        I = Identity(k);
        //        N = Zeros(k, rows - k);

        //        m = vbeta[0].VectorLength();
        //        R = Identity(m) - vbeta[1][1, 1] * vbeta[0] * vbeta[0].Transpose();

        //        H.Insert(k + 1, k, R * H.Extract(k + 1, rows, k, rows));
        //        H.Insert(1, k + 1, H.Extract(1, rows, k + 1, rows) * R);

        //        P = BlockMatrix(I, N, N.Transpose(), R);

        //        Q = Q * P;
        //    }
        //    return new xMatrix[] { H, Q };
        //}


        /// <summary>
        ///   Retrieves row with index i.
        /// </summary>
        /// <returns> i-th row... </returns>
        public Matrix Row(int i)
        {
            var cols = Cols;
            if (i >= Rows) throw new ArgumentException("Index exceed matrix dimension.");
            var row = new Matrix(1, cols);
            for (var j = 0; j < cols; ++j) row[j] = _matrix[i, j];
            return row;
        }

        /// <summary>
        ///   Retrieves column with index j.
        /// </summary>
        /// <returns> j-th column... </returns>
        public Matrix Column(int j)
        {
            var rows = Rows;
            if (j >= Cols) throw new ArgumentException("Index exceed matrix dimension.");
            var col = new Matrix(rows, 1);
            for (var i = 0; i < rows; ++i) col[i] = _matrix[i, j];
            return col;
        }

        /// <summary>
        ///   Splits matrix into its row vectors.
        /// </summary>
        /// <returns> Array of row vectors. </returns>
        public Matrix[] RowVectorize()
        {
            var rows = Rows;
            var rowVec = new Matrix[rows];
            for (var i = 0; i < rowVec.Length; ++i) rowVec[i] = Row(i);
            return rowVec;
        }

        /// <summary>
        ///   Splits matrix into its column vectors.
        /// </summary>
        /// <returns> Array of column vectors. </returns>
        public Matrix[] ColumnVectorize()
        {
            var cols = Cols;
            var colVec = new Matrix[cols];
            for (var j = 0; j < colVec.Length; ++j) colVec[j] = Column(j);
            return colVec;
        }

        /// <summary>
        ///   Returns the transposed matrix.
        /// </summary>
        public Matrix Transpose()
        {
            var rows = Rows;
            var cols = Cols;
            var transpose = new Complex[cols, rows];
            for (var i = 0; i < rows; ++i) for (var j = 0; j < cols; ++j) transpose[j, i] = _matrix[i, j];
            return new Matrix(transpose);
        }

        /// <summary>
        ///   Replaces each matrix entry z = x + iy with x - iy.
        /// </summary>
        public Matrix Conjugate()
        {
            var rows = Rows;
            var cols = Cols;
            var conjugate = new Complex[rows, cols];
            for (var i = 0; i < rows; ++i) for (var j = 0; j < cols; ++j) conjugate[i, j] = _matrix[i, j].Conjugate();
            return new Matrix(conjugate);
        }

        /// <summary>
        ///   Conjuagtes and transposes a matrix.
        /// </summary>
        public Matrix ConjTranspose()
        {
            return Conjugate().Transpose();
        }

        /// <summary>
        ///   Adjoint Matrix if matrix is square.
        /// </summary>
        public Matrix Adjoint()
        {
            var rows = Rows;
            var cols = Cols;
            if (!IsSquare) throw new ArgumentException("Not SquareMatrix");
            var adjoint = new Complex[rows, cols];
            for (uint i = 0; i < rows; ++i) for (uint j = 0; j < cols; ++j) adjoint[i, j] = CoFactor(j, i);
            return new Matrix(adjoint);
        }

        /// <summary>
        ///   Inverse of the matrix if matrix is square, (null)pseudoinverse otherwise.
        /// </summary>
        public Matrix Inverse()
        {
            if (!IsSquare) throw new ArgumentException("Not SquareMatrix");
            var det = Determinant;
            if (det == 0) throw new DivideByZeroException("ScalarMatrix, │D│=0");
            return (Cols == 1) ? new Matrix(1 / det) : Adjoint() / det;
        }

        /// <summary>
        ///   RowEchelon
        /// </summary>
        /// <returns> </returns>
        public Matrix RowEchelon()
        {
            var echelon = Elements;
            var rows = (int) Rows;
            var cols = (int) Cols;
            var row = 0;
            var col = 0;
            for (; row < rows; )
            {
                var maxPivot = new Complex(0); //echelon[row, col]
                //First Leftmost Max Column
                var pivotRow = row;
                for (var i = row; i < rows; ++i)
                {
                    var pivot = echelon[i, col];
                    if (!(pivot.Abs() > maxPivot.Abs())) continue;
                    pivotRow = i;
                    maxPivot = pivot;
                }
                if (maxPivot == 0)
                {
                    ++col;
                    if (col == cols) break;
                    continue;
                }
                //Interchange rows
                if (row != pivotRow)
                {
                    var tmpRow = new Complex[cols];
                    for (var j = 0; j < cols; ++j) tmpRow[j] = echelon[row, j];
                    for (var j = 0; j < cols; ++j) echelon[row, j] = echelon[pivotRow, j];
                    for (var j = 0; j < cols; ++j) echelon[pivotRow, j] = tmpRow[j];
                }
                //Leading 1
                for (var j = col; j < cols; ++j) echelon[row, j] /= maxPivot;
                //Leading 0
                for (var i = row + 1; i < rows; ++i)
                {
                    var multiple = echelon[i, col];
                    for (var j = col; j < cols; ++j) echelon[i, j] -= multiple * echelon[row, j];
                }
                ++row;
                ++col;
            }
            return new Matrix(echelon);
        }

        /// <summary>
        ///   Reduce RowEchelon
        /// </summary>
        /// <returns> </returns>
        public Matrix ReduceRowEchelon()
        {
            var rechelon = RowEchelon().Elements;
            var rows = (int) Rows;
            var cols = (int) Cols;
            for (var row = rows - 1; row >= 0; --row)
            {
                var col = 0;
                var nonzero = new Complex(0);
                //First Leftmost Nonzero Column (Leading 1)
                for (; col < cols; ++col)
                {
                    nonzero = rechelon[row, col];
                    if (Math.Abs(nonzero.Abs() - 0) > EPSILON) break;
                }
                if (nonzero == 0) continue;
                //Trailing 0
                for (var i = row - 1; i >= 0; --i)
                {
                    var multiple = rechelon[i, col];
                    for (uint j = 0; j < cols; ++j) rechelon[i, j] -= multiple * rechelon[row, j];
                }
            }
            return new Matrix(rechelon);
        }

        /// <summary>
        ///   Makes square matrix symmetric by copying the upper half to the lower half.
        /// </summary>
        public void SymmetrizeDown()
        {
            if (!IsSquare) throw new InvalidOperationException("Cannot symmetrize non-square matrix.");
            var cols = Cols;
            for (var j = 0; j < cols; ++j) for (var i = j + 1; i < cols; ++i) this[i, j] = this[j, i];
        }

        /// <summary>
        ///   Makes square matrix symmetric by copying the lower half to the upper half.
        /// </summary>
        public void SymmetrizeUp()
        {
            if (!IsSquare) throw new InvalidOperationException("Cannot symmetrize non-square matrix.");
            var rows = Rows;
            for (var i = 0; i < rows; ++i) for (var j = i + 1; j < rows; ++j) this[i, j] = this[j, i];
        }


        ///// <summary>
        ///// Flips matrix vertically.
        ///// </summary>
        //public void VerticalFlip()
        //{
        //    Values.Reverse();
        //}

        ///// <summary>
        ///// Flips matrix horizontally.
        ///// </summary>
        //public void HorizontalFlip()
        //{
        //    for (var i = 0; i < Rows; ++i)
        //        ((ArrayList) Values[i]).Reverse();
        //}


        /// <summary>
        ///   Creates a copy of the matrix.
        /// </summary>
        public Object Clone()
        {
            return new Matrix(this);
        }

        public bool Equals(Matrix mtx)
        {
            if (null == (Object) mtx) return false;
            return (this == mtx);
        }

        #region Overrided

        /// <summary>
        ///   Serves as a hash function for a particular type, suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        public override int GetHashCode()
        {
            return (int) (Rows ^ Cols) ^ base.GetHashCode();
        }

        /// <summary>
        ///   Determines weather two instances are equal.
        /// </summary>
        public override bool Equals(Object obj)
        {
            if (default(Object) == obj || GetType() != obj.GetType()) return false;
            var mtx = obj as Matrix;
            //if (default(Object) == (Object) mtx) return false;
            return (null != mtx) && ToString() == mtx.ToString();
        }

        /// <summary>
        ///   Returns the matrix in a textual form.
        /// </summary>
        public override String ToString()
        {
            var sb = new StringBuilder();
            var rows = Rows;
            var cols = Cols;
            sb.Append("┌");
            for (var i = 0; i < cols; ++i) sb.Append("{0,12}", String.Empty);
            sb.AppendLine("┐");
            for (var i = 0; i < rows; ++i)
            {
                sb.Append("│");
                for (var j = 0; j < cols; ++j)
                {
                    var format =
                        //"+0.##;-0.##"; // +0.03
                        //"G3"; // +3e-2
                        "#.####;-#.####;0"; // +.03
                    sb.Append("{0,10}  ", _matrix[i, j].ToString(format));
                }
                sb.AppendLine("│");
            }
            sb.Append("└");
            for (var i = 0; i < cols; ++i) sb.Append("{0,12}", String.Empty);
            sb.AppendLine("┘");
            return sb.ToString();
        }

        #endregion

        #region Initializers

        /// <summary>
        /// </summary>
        /// <param name="table"> </param>
        public void Initialize(Complex[,] table)
        {
            for (var i = 0; i < 2; ++i) if (_matrix.GetLength(i) != table.GetLength(i)) throw new ArgumentException("Dimensions doesnot matches (+,-).");
            //M = table; // Logical Error (reference type)
            var rows = Rows;
            var cols = Cols;
            for (var i = 0; i < rows; ++i) for (var j = 0; j < cols; ++j) _matrix[i, j] = table[i, j];
        }

        /// <summary>
        /// </summary>
        /// <param name="nameMatrix"> </param>
        public void Input(String nameMatrix)
        {
            Console.WriteLine("---\nInitialization of matrice " + nameMatrix + " [ Dimensions = " + Dimensions + " ]");
            var rows = Rows;
            var cols = Cols;
            for (var i = 0; i < rows; ++i)
            {
                for (var j = 0; j < cols; ++j)
                {
                    Console.Write("{0}[{1},{2}] =? ", nameMatrix, (i + 1), (j + 1));
                    _matrix[i, j] = Double.Parse(Console.ReadLine());
                }
            }
        }

        #endregion

        #region Generals

        /// <summary>
        ///   Returns a sub matrix extracting specified row and column from the current matrix.
        /// </summary>
        /// <param name="row"> row index </param>
        /// <param name="col"> column index </param>
        public Matrix RemoveRowCol(uint row, uint col)
        {
            var rows = Rows;
            var cols = Cols;
            row %= rows;
            col %= cols;
            var subMatrix = new Complex[rows - 1, cols - 1];
            var x = 0;
            for (var i = 0; i < rows; ++i)
            {
                if (i == row) continue;
                var y = 0;
                for (var j = 0; j < cols; ++j)
                {
                    if (j == col) continue;
                    subMatrix[x, y] = _matrix[i, j];
                    ++y;
                }
                ++x;
            }

            #region other solution

            //var x = 0;
            //for (var i = 0; i < rows; ++i)
            //{
            //    if (i != row)
            //    {
            //        var y = 0;
            //        for (var j = 0; j < cols; ++j)
            //        {
            //            if (j != col)
            //            {
            //                subMatrix[x, y] = _matrix[i, j];
            //                ++y;
            //            }
            //        }
            //        ++x;
            //    }
            //}
            //for (var i = 0; i < rows; ++i)
            //{
            //    var y = 0;
            //    for (var j = 0; j < cols; ++j)
            //        if (i != row && j != col)
            //        {
            //            subMatrix[x, y] = M[i, j];
            //            if (++y == rows - 1) ++x;
            //        }
            //} 

            #endregion

            return new Matrix(subMatrix);
        }

        /// <summary>
        ///   Returns a sub matrix extracted from the current matrix.
        /// </summary>
        /// <param name="i0"> Start row index </param>
        /// <param name="i1"> End row index </param>
        /// <param name="j0"> Start column index </param>
        /// <param name="j1"> End column index </param>
        public Matrix SubMatrix(uint i0, uint i1, uint j0, uint j1)
        {
            var rows = Rows;
            var cols = Cols;
            if (i0 > i1 || j0 > j1 ||
                i0 >= rows || i1 >= rows ||
                j0 >= cols || j1 >= cols) throw new ArgumentException("Argument Out of Range.");
            var subMatrix = new Complex[i1 - i0 + 1, j1 - j0 + 1];
            for (var i = i0; i <= i1; ++i) for (var j = j0; j <= j1; ++j) subMatrix[i - i0, j - j0] = _matrix[i, j];
            return new Matrix(subMatrix);
        }

        /// <summary>
        ///   Returns a sub matrix extracted from the current matrix.
        /// </summary>
        /// <param name="arrRows"> Array of row indices </param>
        /// <param name="arrCols"> Array of column indices </param>
        public Matrix SubMatrix(uint[] arrRows, uint[] arrCols)
        {
            var subMatrix = new Complex[arrRows.Length, arrCols.Length];
            for (var i = 0; i < arrRows.Length; ++i)
            {
                for (var j = 0; j < arrCols.Length; ++j)
                {
                    if ( //arrRows[i] < 0 ||
                        //arrCols[j] < 0 ||
                        arrRows[i] >= Rows ||
                        arrCols[j] >= Cols) throw new ArgumentException("Argument Out of Range.");
                    subMatrix[i, j] = _matrix[arrRows[i], arrCols[j]];
                }
            }
            return new Matrix(subMatrix);
        }

        /// <summary>
        ///   Returns a sub matrix extracted from the current matrix.
        /// </summary>
        /// <param name="arrRows"> Array of row indices </param>
        /// <param name="j0"> Start column index </param>
        /// <param name="j1"> End column index </param>
        public Matrix SubMatrix(uint[] arrRows, uint j0, uint j1)
        {
            var rows = Rows;
            var cols = Cols;
            if (j0 > j1 || j0 >= cols || j1 >= cols) throw new ArgumentException("Argument Out of Range.");
            var subMatrix = new Complex[arrRows.Length, j1 - j0 + 1];
            for (var i = 0; i < arrRows.Length; ++i)
            {
                for (var j = j0; j <= j1; ++j)
                {
                    if ( //arrRows[i] < 0 ||
                        arrRows[i] >= rows) throw new ArgumentException("Argument Out of Range.");
                    subMatrix[i, j - j0] = _matrix[arrRows[i], j];
                }
            }
            return new Matrix(subMatrix);
        }

        /// <summary>
        ///   Returns a sub matrix extracted from the current matrix.
        /// </summary>
        /// <param name="i0"> Starttial row index </param>
        /// <param name="i1"> End row index </param>
        /// <param name="arrCols"> Array of column indices </param>
        public Matrix SubMatrix(uint i0, uint i1, uint[] arrCols)
        {
            var rows = Rows;
            var cols = Cols;
            if (i0 > i1 || i0 >= rows || i1 >= rows) throw new ArgumentException("Argument Out of Range.");
            var subMatrix = new Complex[i1 - i0 + 1, arrCols.Length];
            for (var i = i0; i <= i1; ++i)
            {
                for (var j = 0; j < arrCols.Length; ++j)
                {
                    if ( //arrCols[j] < 0 ||
                        arrCols[j] >= cols) throw new ArgumentException("Argument Out of Range.");
                    subMatrix[i - i0, j] = _matrix[i, arrCols[j]];
                }
            }
            return new Matrix(subMatrix);
        }

        /// <summary>
        ///   Minor
        /// </summary>
        /// <param name="row"> </param>
        /// <param name="col"> </param>
        /// <returns> </returns>
        public Complex Minor(uint row, uint col)
        {
            if (!IsSquare) throw new ArgumentException("Not SquareMatrix");
            return RemoveRowCol(row, col).Determinant;
        }

        /// <summary>
        ///   CoFactor
        /// </summary>
        /// <param name="row"> </param>
        /// <param name="col"> </param>
        /// <returns> </returns>
        public Complex CoFactor(uint row, uint col)
        {
            if (!IsSquare) throw new ArgumentException("Not SquareMatrix");
            return ((row + col) % 2 == 0 ? 1 : -1) * Minor(row, col);
        }

        #endregion

        #endregion

        #endregion
    }

}