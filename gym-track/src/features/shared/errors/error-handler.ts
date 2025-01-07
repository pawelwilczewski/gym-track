import { AxiosResponse } from 'axios';
import { ResponseResult } from './response-result';
import { responseToResult } from './converters';

export class ErrorHandler
  implements IPartiallyHandledErrorHandler, IFullyHandledErrorHandler
{
  private result: ResponseResult;
  private errorHandled = false;

  static forResult(result: ResponseResult): IPartiallyHandledErrorHandler {
    return new ErrorHandler(result);
  }

  static forResponse(response: AxiosResponse): IPartiallyHandledErrorHandler {
    return new ErrorHandler(responseToResult(response));
  }

  private constructor(result: ResponseResult) {
    this.result = result;
  }

  public handlePartially<TData = void>(
    handler: PartialErrorHandlerDelegate<TData>,
    data?: TData
  ): IPartiallyHandledErrorHandler {
    if (this.errorHandled) {
      return this;
    }

    const result = handler(this.result, data!);
    this.errorHandled = result.wasHandled;
    return this;
  }

  public handleFully<TData = void>(
    handler: FullErrorHandlerDelegate<TData>,
    data?: TData
  ): IFullyHandledErrorHandler {
    if (this.errorHandled) {
      return this;
    }

    handler(this.result, data!);
    this.errorHandled = true;
    return this;
  }

  public wasSuccess(): boolean {
    return this.result.type === 'success';
  }

  public wasError(): boolean {
    return !this.wasSuccess();
  }
}

export interface IPartiallyHandledErrorHandler {
  handlePartially<TData = void>(
    handler: PartialErrorHandlerDelegate<TData>,
    data?: TData
  ): IPartiallyHandledErrorHandler;

  handleFully<TData = void>(
    handler: FullErrorHandlerDelegate<TData>,
    data?: TData
  ): IFullyHandledErrorHandler;
}

export interface IFullyHandledErrorHandler {
  wasSuccess(): boolean;
  wasError(): boolean;
}

export enum HandlerType {
  Partial,
  Full,
}

export type PartiallyHandledErrorInfo = {
  type: HandlerType.Partial;
  wasHandled: boolean;
};

export type FullyHandledErrorInfo = {
  type: HandlerType.Full;
};

export type PartialErrorHandlerDelegate<TData = void> = TData extends void
  ? (result: ResponseResult) => PartiallyHandledErrorInfo
  : (result: ResponseResult, data: TData) => PartiallyHandledErrorInfo;

export type FullErrorHandlerDelegate<TData = void> = TData extends void
  ? (result: ResponseResult) => FullyHandledErrorInfo
  : (result: ResponseResult, data: TData) => FullyHandledErrorInfo;

export class UnhandledResultError extends Error {}
