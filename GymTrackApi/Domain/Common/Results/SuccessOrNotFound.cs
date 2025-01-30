using OneOf;
using OneOf.Types;

namespace Domain.Common.Results;

[GenerateOneOf]
public sealed partial class SuccessOrNotFound<T> : OneOfBase<Success<T>, NotFound>;