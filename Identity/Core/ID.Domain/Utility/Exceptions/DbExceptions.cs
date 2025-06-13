using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ID.Domain.Utility.Exceptions;

//===================================================================//

public class MyIdDatabaseException : DbUpdateException
{
    public MyIdDatabaseException()
    { }
    public MyIdDatabaseException(string message) : base(message)
    { }

    public MyIdDatabaseException(string message, Exception? innerException) : base(message, innerException)
    { }

    public MyIdDatabaseException(string message, IReadOnlyList<EntityEntry> entries) : base(message, entries)
    { }

    public MyIdDatabaseException(string message, Exception? innerException, IReadOnlyList<EntityEntry> entries) : base(message, innerException, entries)
    { }

}

//===================================================================//

public class UniqueConstraintException : MyIdDatabaseException
{
    public UniqueConstraintException()
    { }

    public UniqueConstraintException(string message) : base(message)
    { }

    public UniqueConstraintException(string message, Exception? innerException) : base(message, innerException)
    { }

    public UniqueConstraintException(string message, IReadOnlyList<EntityEntry> entries) : base(message, entries)
    { }

    public UniqueConstraintException(string message, Exception? innerException, IReadOnlyList<EntityEntry> entries) : base(message, innerException, entries)
    { }

}

//===================================================================//

public class CannotInsertNullException : MyIdDatabaseException
{
    public CannotInsertNullException()
    { }

    public CannotInsertNullException(string message) : base(message)
    { }

    public CannotInsertNullException(string message, Exception? innerException) : base(message, innerException)
    { }

    public CannotInsertNullException(string message, IReadOnlyList<EntityEntry> entries) : base(message, entries)
    { }

    public CannotInsertNullException(string message, Exception? innerException, IReadOnlyList<EntityEntry> entries) : base(message, innerException, entries)
    { }

}

//===================================================================//

public class MaxLengthExceededException : MyIdDatabaseException
{
    public MaxLengthExceededException()
    { }

    public MaxLengthExceededException(string message) : base(message)
    { }

    public MaxLengthExceededException(string message, Exception? innerException) : base(message, innerException)
    { }

    public MaxLengthExceededException(string message, IReadOnlyList<EntityEntry> entries) : base(message, entries)
    { }

    public MaxLengthExceededException(string message, Exception? innerException, IReadOnlyList<EntityEntry> entries) : base(message, innerException, entries)
    { }

}

//===================================================================//

public class NumericOverflowException : MyIdDatabaseException
{
    public NumericOverflowException()
    { }

    public NumericOverflowException(string message) : base(message)
    { }

    public NumericOverflowException(string message, Exception? innerException) : base(message, innerException)
    { }

    public NumericOverflowException(string message, IReadOnlyList<EntityEntry> entries) : base(message, entries)
    { }

    public NumericOverflowException(string message, Exception? innerException, IReadOnlyList<EntityEntry> entries) : base(message, innerException, entries)
    { }

}

//===================================================================//

public class ReferenceConstraintException : MyIdDatabaseException
{
    public ReferenceConstraintException()
    { }

    public ReferenceConstraintException(string message) : base(message)
    { }

    public ReferenceConstraintException(string message, Exception? innerException) : base(message, innerException)
    { }

    public ReferenceConstraintException(string message, IReadOnlyList<EntityEntry> entries) : base(message, entries)
    { }

    public ReferenceConstraintException(string message, Exception? innerException, IReadOnlyList<EntityEntry> entries) : base(message, innerException, entries)
    { }

}

//===================================================================//