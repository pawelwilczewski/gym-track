import { AxiosResponse } from 'axios';
import { ResponseResult } from './ResponseResult';
import { responseToResult } from './Converters';

export class ErrorHandler {
  private result: ResponseResult;
  private handlers: [ErrorHandlerDelegate<any>, any][] = [];

  static forResult(result: ResponseResult): ErrorHandler {
    return new ErrorHandler(result);
  }

  static forResponse(response: AxiosResponse): ErrorHandler {
    return new ErrorHandler(responseToResult(response));
  }

  private constructor(result: ResponseResult) {
    this.result = result;
  }

  public with<TData = void>(
    handler: ErrorHandlerDelegate<TData>,
    data?: TData
  ): ErrorHandler {
    this.handlers.push([handler, data]);
    return this;
  }

  /**
   * @returns Whether result was success.
   * @throws `Error` if response result was not handled (adjust handlers to cover all cases).
   */
  public handle(): boolean {
    const wasHandled = this.handlers.some(handlerWithData =>
      handlerWithData[0](this.result, handlerWithData[1])
    );

    if (!wasHandled) {
      throw new UnhandledError('Unhandled response result error.');
    }

    return this.result.type === 'success';
  }
}

export type ErrorHandlerDelegate<TData = void> = TData extends void
  ? (result: ResponseResult) => boolean
  : (result: ResponseResult, data: TData) => boolean;

export class UnhandledError extends Error {}
