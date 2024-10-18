using System.Diagnostics;
using Domain.Models.Common;

namespace Api.Common;

internal static class CanModifyResultExtensions
{
	public static Task<IResult> ToResult(
		this CanModifyResult canModifyResult,
		Func<Task<IResult>> onOk,
		string messageOnProhibitShared = "Can't modify shared.") =>
		canModifyResult switch
		{
			CanModifyResult.Ok             => onOk(),
			CanModifyResult.NotFound       => Task.FromResult<IResult>(TypedResults.NotFound()),
			CanModifyResult.ProhibitShared => Task.FromResult<IResult>(TypedResults.BadRequest(messageOnProhibitShared)),
			CanModifyResult.Unauthorized   => Task.FromResult<IResult>(TypedResults.Unauthorized()),
			_                              => throw new UnreachableException()
		};
}