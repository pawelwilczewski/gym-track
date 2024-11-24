export type ResponseResult =
  | ISuccess
  | IEmptyError
  | IErrorWithMessage
  | IValidationError
  | IAuthError;

export interface ISuccess {
  type: 'success';
}

export interface IEmptyError {
  type: 'empty';
}

export interface IErrorWithMessage {
  type: 'message';
  message: string;
}

export interface IInvalidField {
  field: string;
  error: string;
}

export interface IValidationError {
  type: 'validation';
  errors: IInvalidField[];
}

export interface IAuthError {
  type: 'auth';
}
