export type UserInfo = {
  email: string;
  isEmailConfirmed: boolean;
};

export type SignUpRequest = {
  email: string;
  password: string;
};

export type LogInRequest = {
  email: string;
  password: string;
  rememberMe?: boolean;
};
