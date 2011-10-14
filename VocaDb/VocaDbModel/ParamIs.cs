using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace VocaDb.Model {

	/// <summary>
	/// Utility methods for parameter validation.
	/// </summary>
	[DebuggerStepThrough]
	public static class ParamIs {

		/// <summary>
		/// Validates that the given parameter is a positive (greater than 0) integer.
		/// </summary>
		/// <param name="expression">Parameter expression. Expression body must be a member expression, for example "() => param". Cannot be null.</param>
		public static void Between(Expression<Func<int>> expression, int min, int max) {

			if (!(expression.Body is MemberExpression))
				throw new ArgumentException("Only member expressions are supported", "expression");

			var f = expression.Compile();

			if (f() < min || f() > max) {
				var body = (MemberExpression)expression.Body;
				throw new ArgumentException(body.Member.Name + " must be between " + min + " and " + max, body.Member.Name);
			}

		}

		/// <summary>
		/// Validates that the given parameter is a positive (greater than 0) integer.
		/// </summary>
		/// <param name="val">Parameter to be validated.</param>
		/// <param name="paramName">Parameter name. Cannot be null or empty.</param>
		public static void NonNegative(int val, string paramName = null) {

			if (val < 0)
				throw new ArgumentException(paramName + " must be a non-negative integer", paramName);

		}

		/// <summary>
		/// Validates that the given parameter is a positive (greater than 0) integer.
		/// </summary>
		/// <param name="expression">Parameter expression. Expression body must be a member expression, for example "() => param". Cannot be null.</param>
		public static void NonNegative(Expression<Func<int>> expression) {

			if (!(expression.Body is MemberExpression))
				throw new ArgumentException("Only member expressions are supported", "expression");

			var f = expression.Compile();

			if (f() < 0) {
				var body = (MemberExpression)expression.Body;
				throw new ArgumentException(body.Member.Name + " must be a non-negative integer", body.Member.Name);
			}

		}

		/// <summary>
		/// Validates that the given parameter is not null.
		/// </summary>
		/// <param name="val">Parameter to be validated.</param>
		/// <param name="paramName">Parameter name. Cannot be null or empty.</param>
		/// <remarks>
		/// The overload <see cref="NotNull{Expression}"/> should be preferred if possible.
		/// </remarks>
		public static void NotNull(object val, string paramName = null) {

			if (val == null)
				throw new ArgumentNullException(paramName);

		}

		/// <summary>
		/// Validates that the given parameter is not null.
		/// </summary>
		/// <param name="expression">Parameter expression. Expression body must be a member expression, for example "() => param". Cannot be null.</param>
		public static void NotNull<T>(Expression<Func<T>> expression) where T : class {

			if (!(expression.Body is MemberExpression))
				throw new ArgumentException("Only member expressions are supported", "expression");

			var f = expression.Compile();

			if (f() == null) {
				var body = (MemberExpression)expression.Body;
				throw new ArgumentNullException(body.Member.Name);				
			}

		}

		/// <summary>
		/// Validates that the given string parameter is not null or empty.
		/// </summary>
		/// <param name="val">Parameter to be validated.</param>
		/// <param name="paramName">Parameter name. Cannot be null or empty.</param>
		/// <remarks>
		/// The overload <see cref="NotNullOrEmpty(Expression{Func{string}})"/> should be preferred if possible.
		/// </remarks>
		public static void NotNullOrEmpty(string val, string paramName = null) {

			if (string.IsNullOrEmpty(val))
				throw new ArgumentException(paramName + " cannot be null or empty", paramName);

		}

		/// <summary>
		/// Validates that the given parameter is not null or empty.
		/// </summary>
		/// <param name="expression">Parameter expression. Expression body must be a member expression. Cannot be null.</param>
		public static void NotNullOrEmpty(Expression<Func<string>> expression) {

			if (!(expression.Body is MemberExpression))
				throw new ArgumentException("Only member expressions are supported", "expression");

			var f = expression.Compile();

			if (string.IsNullOrEmpty(f())) {
				var body = (MemberExpression)expression.Body;
				throw new ArgumentException(body.Member.Name + " cannot be null or empty", body.Member.Name);
			}

		}

		/// <summary>
		/// Validates that the given parameter is a positive (greater than 0) integer.
		/// </summary>
		/// <param name="val">Parameter to be validated.</param>
		/// <param name="paramName">Parameter name. Cannot be null or empty.</param>
		public static void Positive(int val, string paramName = null) {

			if (val <= 0)
				throw new ArgumentException(paramName + " must be a positive integer", paramName);

		}

	}
}
