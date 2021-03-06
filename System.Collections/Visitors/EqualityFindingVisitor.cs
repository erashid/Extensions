namespace System.Collections.Visitors
{
    using Generic;

    /// <summary>
	/// A visitor that searches objects for an equality, using the IEqualityComparer interface.
	/// </summary>	
	public sealed class EqualityFindingVisitor<T> : IFindingIVisitor<T>
	{
		#region Globals

		bool _found;
        readonly T _valueToSearchFor;
        readonly IEqualityComparer<T> _comparerToUse;

		#endregion

		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="EqualityFindingVisitor&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="valueToSearchFor">The value to search for.</param>
		/// <param name="comparer">The comparer to use when testing equality between instances.</param>
		public EqualityFindingVisitor(T valueToSearchFor, IEqualityComparer<T> comparer)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}

			_valueToSearchFor = valueToSearchFor;
			_comparerToUse = comparer;
		}

		#endregion

		#region IFindingIVisitor<T> Members

		/// <summary>
		/// Gets a value indicating whether this <see cref="EqualityFindingVisitor&lt;T&gt;"/> is found.
		/// </summary>
		/// <value><c>true</c> if found; otherwise, <c>false</c>.</value>
		public bool Found
		{
			get
			{
				return _found;
			}
		}

		/// <summary>
		/// Gets the search value.
		/// </summary>
		/// <value>The search value.</value>
		public T SearchValue
		{
			get
			{
				return _valueToSearchFor;
			}
		}

		#endregion

		#region IVisitor<T> Members

		/// <summary>
		/// Gets a value indicating whether this instance is done performing it's work..
		/// </summary>
		/// <value><c>true</c> if this instance is done; otherwise, <c>false</c>.</value>
		public bool HasCompleted
		{
			get
			{
				return _found;
			}
		}

		/// <summary>
		/// Visits the specified object.
		/// </summary>
		/// <param name="obj">The object to visit.</param>
		public void Visit(T obj)
		{
			if (_comparerToUse.Equals(_valueToSearchFor, obj))
			{
				_found = true;
			}
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Gets the comparer.
		/// </summary>
		/// <value>The comparer.</value>
		public IEqualityComparer<T> Comparer
		{
			get
			{
				return _comparerToUse;
			}
		}

		#endregion
	}
}
