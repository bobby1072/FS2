import axios, { AxiosInstance } from "axios";
import { ClientConfigResponse } from "../models/ClientConfigResponse";
import { UserModel } from "../models/UserModel";
import { SaveGroupInput } from "../components/GroupComponents/CreateGroupModalForm";
import { GroupModel } from "../models/GroupModel";
import { ApiException } from "../common/ApiException";

export default abstract class BackendApiServiceProvider {
  private static FormatAccessToken(accessToken: string) {
    return `Bearer ${accessToken
      .replaceAll("Bearer ", "")
      .replaceAll("bearer ", "")}`;
  }
  private static _httpClient: AxiosInstance = axios.create({
    baseURL:
      process.env.NODE_ENV === "development"
        ? "http://localhost:5234/api"
        : "api",
    withCredentials: true,
  });
  public static async GetClientConfig() {
    const { data } = await this._httpClient
      .get<ClientConfigResponse>("ClientConfig")
      .catch((e) => {
        throw new ApiException(
          e.response.data as string,
          Number(e.response.status)
        );
      });
    return data;
  }
  public static async GetSelfGroups(
    accessToken: string,
    startIndex: number,
    count: number
  ) {
    const { data } = await this._httpClient
      .get<GroupModel[]>(
        `Group/GetSelfGroups?startIndex=${startIndex}&count=${count}`,
        {
          headers: {
            Authorization: this.FormatAccessToken(accessToken),
          },
        }
      )
      .catch((e) => {
        throw new ApiException(
          e.response.data as string,
          Number(e.response.status)
        );
      });
    return data;
  }
  public static async GetAllListedGroups(
    accessToken: string,
    startIndex: number,
    count: number
  ) {
    const { data } = await this._httpClient
      .get<GroupModel[]>(
        `Group/GetAllListedGroups?startIndex=${startIndex}&count=${count}`,
        {
          headers: {
            Authorization: this.FormatAccessToken(accessToken),
          },
        }
      )
      .catch((e) => {
        throw new ApiException(
          e.response.data as string,
          Number(e.response.status)
        );
      });
    return data;
  }
  public static async GetUser(accessToken: string): Promise<UserModel> {
    const { data } = await this._httpClient
      .get<UserModel>("User/Self", {
        headers: {
          Authorization: this.FormatAccessToken(accessToken),
        },
      })
      .catch((e) => {
        throw new ApiException(
          e.response.data as string,
          Number(e.response.status)
        );
      });
    return data;
  }
  public static async SaveGroup(accessToken: string, group: SaveGroupInput) {
    const { data } = await this._httpClient
      .post<string>("Group/SaveGroup", group, {
        headers: {
          Authorization: this.FormatAccessToken(accessToken),
        },
      })
      .catch((e) => {
        throw new ApiException(
          e.response.data as string,
          Number(e.response.status)
        );
      });
    return data;
  }
  public static async GetGroupCount(accessToken: string) {
    const { data } = await this._httpClient
      .get<number>("Group/GetGroupCount", {
        headers: {
          Authorization: this.FormatAccessToken(accessToken),
        },
      })
      .catch((e) => {
        throw new ApiException(
          e.response.data as string,
          Number(e.response.status)
        );
      });
    return data;
  }
}
