export class ApiException extends Error {
  public readonly status?: number;
  constructor(message: string, status?: number) {
    super(message);
    this.status = status;
  }
}
