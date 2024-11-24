import { AxiosResponse } from 'axios';
import { ResponseResult } from './ResponseResult';
import { responseToResult } from './Converters';

export class ErrorHandler
  implements IPartiallyHandledErrorHandler, IFullyHandledErrorHandler
{
  private result: ResponseResult;
  private handlers: [
    PartialErrorHandlerDelegate<any> | FullErrorHandlerDelegate<any>,
    any,
  ][] = [];

  static forResult(result: ResponseResult): IPartiallyHandledErrorHandler {
    return new ErrorHandler(result);
  }

  static forResponse(response: AxiosResponse): IPartiallyHandledErrorHandler {
    return new ErrorHandler(responseToResult(response));
  }

  private constructor(result: ResponseResult) {
    this.result = result;
  }

  public withPartial<TData = void>(
    handler: PartialErrorHandlerDelegate<TData>,
    data?: TData
  ): IPartiallyHandledErrorHandler {
    this.handlers.push([handler, data]);
    return this;
  }

  public withFull<TData = void>(
    handler: FullErrorHandlerDelegate<TData>,
    data?: TData
  ): IFullyHandledErrorHandler {
    this.handlers.push([handler, data]);
    return this;
  }

  public handle(): boolean {
    const wasHandled = this.handlers.some(([handler, data]) => {
      const result = handler(this.result, data);
      return result.type == HandlerType.Full || result.wasHandled;
    });

    if (!wasHandled) {
      throw new UnhandledResultError('Unhandled response result error.');
    }

    return this.result.type === 'success';
  }
}

export interface IPartiallyHandledErrorHandler {
  withPartial<TData = void>(
    handler: PartialErrorHandlerDelegate<TData>,
    data?: TData
  ): IPartiallyHandledErrorHandler;

  withFull<TData = void>(
    handler: FullErrorHandlerDelegate<TData>,
    data?: TData
  ): IFullyHandledErrorHandler;
}

export interface IFullyHandledErrorHandler {
  handle(): boolean;
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
