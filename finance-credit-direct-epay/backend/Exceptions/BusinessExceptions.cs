namespace EPay.Api.Exceptions
{
    /// <summary>
    /// Base exception for all EPay business logic errors.
    /// </summary>
    public class EPayException : Exception
    {
        /// <summary>
        /// Error code for categorizing exceptions.
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EPayException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="innerException">The inner exception.</param>
        public EPayException(string message, string errorCode = "EPAY_ERROR", Exception? innerException = null)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }

    /// <summary>
    /// Exception thrown when input validation fails.
    /// </summary>
    public class ValidationException : EPayException
    {
        /// <summary>
        /// The name of the field that failed validation.
        /// </summary>
        public string? FieldName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="message">The validation error message.</param>
        /// <param name="fieldName">The name of the field that failed validation.</param>
        public ValidationException(string message, string? fieldName = null)
            : base(message, "VALIDATION_ERROR")
        {
            FieldName = fieldName;
        }
    }

    /// <summary>
    /// Exception thrown when a requested resource is not found.
    /// </summary>
    public class NotFoundException : EPayException
    {
        /// <summary>
        /// The type of resource that was not found.
        /// </summary>
        public string? ResourceType { get; }

        /// <summary>
        /// The identifier of the resource that was not found.
        /// </summary>
        public string? ResourceId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="resourceType">The type of resource.</param>
        /// <param name="resourceId">The resource identifier.</param>
        public NotFoundException(string message, string? resourceType = null, string? resourceId = null)
            : base(message, "NOT_FOUND")
        {
            ResourceType = resourceType;
            ResourceId = resourceId;
        }
    }

    /// <summary>
    /// Exception thrown when a database operation fails.
    /// </summary>
    public class DataAccessException : EPayException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataAccessException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DataAccessException(string message, Exception? innerException = null)
            : base(message, "DATA_ACCESS_ERROR", innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when date validation fails.
    /// </summary>
    public class InvalidDateRangeException : ValidationException
    {
        /// <summary>
        /// The from date that was provided.
        /// </summary>
        public DateTime? FromDate { get; }

        /// <summary>
        /// The to date that was provided.
        /// </summary>
        public DateTime? ToDate { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidDateRangeException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="fromDate">The from date.</param>
        /// <param name="toDate">The to date.</param>
        public InvalidDateRangeException(string message, DateTime? fromDate = null, DateTime? toDate = null)
            : base(message, "dateRange")
        {
            FromDate = fromDate;
            ToDate = toDate;
        }
    }
}

