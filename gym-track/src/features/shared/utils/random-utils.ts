const characters =
  'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';

export function randomString(length: number): string {
  let result = '';
  for (let index = 0; index < length; ++index) {
    result += characters.charAt(Math.floor(Math.random() * characters.length));
  }
  return result;
}
