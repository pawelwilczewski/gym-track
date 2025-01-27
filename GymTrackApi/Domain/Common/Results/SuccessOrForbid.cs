using OneOf;
using OneOf.Types;

namespace Domain.Common.Results;

[GenerateOneOf]
public sealed partial class SuccessOrForbid : OneOfBase<Success, Forbid>;