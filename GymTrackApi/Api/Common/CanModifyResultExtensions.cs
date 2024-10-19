using System.Diagnostics;
using Domain.Models.Common;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Common;

using ModifyResult = Results<Ok, NotFound, BadRequest<string>, UnauthorizedHttpResult>;

internal static class CanModifyResultExtensions
{
	public static Task<ModifyResult> ToResult(
		this CanModifyResult canModifyResult,
		Func<Task<ModifyResult>> onOk,
		string messageOnProhibitShared = "Can't modify shared.") =>
		canModifyResult switch
		{
			CanModifyResult.Ok             => onOk(),
			CanModifyResult.NotFound       => Task.FromResult<ModifyResult>(TypedResults.NotFound()),
			CanModifyResult.ProhibitShared => Task.FromResult<ModifyResult>(TypedResults.BadRequest(messageOnProhibitShared)),
			CanModifyResult.Unauthorized   => Task.FromResult<ModifyResult>(TypedResults.Unauthorized()),
			_                              => throw new UnreachableException()
		};
}