export default class ApiError extends Error {
  public readonly Status?: number;
  constructor(message?: string, status?: number) {
    super(message);
    this.Status = status;
    return this;
  }
}
