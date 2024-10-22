using System.Diagnostics;
using Domain.Models.Common;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Common;

using ModifyResult = Results<Ok, NotFound, BadRequest<string>, UnauthorizedHttpResult>;

internal static class CantModifyReasonExtensions
{
	public static ModifyResult ToResult(
		this CantModifyReason cantModifyReason,
		string messageOnProhibitShared = "Can't modify shared.") =>
		cantModifyReason switch
		{
			CantModifyReason.NotFound       => TypedResults.NotFound(),
			CantModifyReason.ProhibitShared => TypedResults.BadRequest(messageOnProhibitShared),
			CantModifyReason.Unauthorized   => TypedResults.Unauthorized(),
			_                               => throw new UnreachableException()
		};
}