import axios, { AxiosInstance } from "axios";
import { ClientConfigResponse } from "../models/ClientConfigResponse";

export default abstract class BackendApiServiceProvider {
  private static _httpClient: AxiosInstance = axios.create({
    baseURL:
      process.env.NODE_ENV === "development"
        ? "http://localhost:7264/api"
        : undefined,
  });
  public static async GetClientConfig() {
    const { data } = await this._httpClient.get<ClientConfigResponse>(
      "ClientConfig"
    );
    return data;
  }
}
