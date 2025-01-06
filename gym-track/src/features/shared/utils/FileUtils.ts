import { AxiosInstance } from 'axios';
import { ErrorHandler } from '../../app/errors/ErrorHandler';
import { toastErrorHandler } from '../../app/errors/Handlers';

export function getImageMimeType(
  path: string
): 'image/jpeg' | 'image/png' | undefined {
  if (path.endsWith('.png')) {
    return 'image/png';
  } else if (path.endsWith('.jpg') || path.endsWith('.jpeg')) {
    return 'image/jpeg';
  }
  return undefined;
}

export async function getImageFileFromUrl(
  url: string,
  client: AxiosInstance
): Promise<File | undefined> {
  const mimeType = getImageMimeType(url);
  if (!mimeType) {
    return undefined;
  }

  const response = await client.get(url);

  if (
    ErrorHandler.forResponse(response).handleFully(toastErrorHandler).wasError()
  ) {
    return undefined;
  }

  const data = await response.data.blob();
  const name = url.substring(url.lastIndexOf('/') + 1);
  const metadata = {
    type: mimeType,
  };

  return new File([data], name, metadata);
}
