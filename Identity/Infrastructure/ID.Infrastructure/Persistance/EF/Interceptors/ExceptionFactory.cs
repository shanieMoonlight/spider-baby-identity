﻿using ID.Domain.Utility.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Data.Common;

namespace ID.Infrastructure.Persistance.EF.Interceptors;
internal static class ExceptionFactory
{
    internal static Exception? Create<T>(ExceptionProcessorInterceptor<T>.DatabaseError error, DbUpdateException exception, IReadOnlyList<EntityEntry> entries) where T : DbException
    {
        return error switch
        {
            ExceptionProcessorInterceptor<T>.DatabaseError.CannotInsertNull when entries.Count > 0 => new CannotInsertNullException("Cannot insert null", exception.InnerException, entries),
            ExceptionProcessorInterceptor<T>.DatabaseError.CannotInsertNull when entries.Count == 0 => new CannotInsertNullException("Cannot insert null", exception.InnerException),
            ExceptionProcessorInterceptor<T>.DatabaseError.MaxLength when entries.Count > 0 => new MaxLengthExceededException("Maximum length exceeded", exception.InnerException, entries),
            ExceptionProcessorInterceptor<T>.DatabaseError.MaxLength when entries.Count == 0 => new MaxLengthExceededException("Maximum length exceeded", exception.InnerException),
            ExceptionProcessorInterceptor<T>.DatabaseError.NumericOverflow when entries.Count > 0 => new NumericOverflowException("Numeric overflow", exception.InnerException, entries),
            ExceptionProcessorInterceptor<T>.DatabaseError.NumericOverflow when entries.Count == 0 => new NumericOverflowException("Numeric overflow", exception.InnerException),
            ExceptionProcessorInterceptor<T>.DatabaseError.ReferenceConstraint when entries.Count > 0 => new ReferenceConstraintException("Reference constraint violation", exception.InnerException, entries),
            ExceptionProcessorInterceptor<T>.DatabaseError.ReferenceConstraint when entries.Count == 0 => new ReferenceConstraintException("Reference constraint violation", exception.InnerException),
            ExceptionProcessorInterceptor<T>.DatabaseError.UniqueConstraint when entries.Count > 0 => new UniqueConstraintException("Unique constraint violation", exception.InnerException, entries),
            ExceptionProcessorInterceptor<T>.DatabaseError.UniqueConstraint when entries.Count == 0 => new UniqueConstraintException("Unique constraint violation", exception.InnerException),
            _ => null,
        };
    }
}