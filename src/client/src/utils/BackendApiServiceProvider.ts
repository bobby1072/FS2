import axios, { AxiosInstance } from "axios";
import { ClientConfigResponse } from "../models/ClientConfigResponse";
import { UserModel } from "../models/UserModel";
import { GroupModel } from "../models/GroupModel";

export default abstract class BackendApiServiceProvider {
  private static _httpClient: AxiosInstance = axios.create({
    baseURL:
      process.env.NODE_ENV === "development"
        ? "http://localhost:7264/api"
        : "api",
    withCredentials: true,
  });
  public static async GetClientConfig() {
    const { data } = await this._httpClient.get<ClientConfigResponse>(
      "ClientConfig"
    );
    return data;
  }
  public static async GetAllListedGroups(accessToken: string) {
    const { data } = await this._httpClient.get("Group/GetAllListedGroups", {
      headers: {
        Authorization: accessToken,
      },
    });
    return data;
  }
  public static async GetUser(accessToken: string): Promise<UserModel> {
    const { data } = await this._httpClient.get<UserModel>("User/Self", {
      headers: {
        Authorization: accessToken,
      },
    });
    return data;
  }
  public static async SaveGroup(accessToken: string, group: GroupModel) {
    const { data } = await this._httpClient.post<string>(
      "Group/SaveGroup",
      group,
      {
        headers: {
          Authorization: accessToken,
        },
      }
    );
    return data;
  }
}
