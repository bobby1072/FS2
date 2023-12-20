import axios, { AxiosInstance } from "axios";
import { ClientConfigResponse } from "../models/ClientConfigResponse";
import { UserModel } from "../models/UserModel";
import { SaveGroupInput } from "../components/GroupComponents/CreateGroupModalForm";
import { GroupModel } from "../models/GroupModel";

export default abstract class BackendApiServiceProvider {
  private static FormatAccessToken(accessToken: string) {
    return `Bearer ${accessToken
      .replaceAll("Bearer ", "")
      .replaceAll("bearer ", "")}`;
  }
  private static _httpClient: AxiosInstance = axios.create({
    baseURL:
      process.env.NODE_ENV === "development"
        ? "https://localhost:7264/api"
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
    const { data } = await this._httpClient.get<GroupModel[]>(
      "Group/GetAllListedGroups",
      {
        headers: {
          Authorization: this.FormatAccessToken(accessToken),
        },
      }
    );
    return data;
  }
  public static async GetUser(accessToken: string): Promise<UserModel> {
    const { data } = await this._httpClient.get<UserModel>("User/Self", {
      headers: {
        Authorization: this.FormatAccessToken(accessToken),
      },
    });
    return data;
  }
  public static async SaveGroup(accessToken: string, group: SaveGroupInput) {
    const { data } = await this._httpClient.post<string>(
      "Group/SaveGroup",
      group,
      {
        headers: {
          Authorization: this.FormatAccessToken(accessToken),
        },
      }
    );
    return data;
  }
}
