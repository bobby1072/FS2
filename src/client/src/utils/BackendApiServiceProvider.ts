import axios, { AxiosInstance } from "axios";
import { IClientConfigResponse } from "../models/IClientConfigResponse";
import { IUserModel } from "../models/IUserModel";
import { SaveGroupInput } from "../components/GroupComponents/CreateGroupModalForm";
import { IGroupModel } from "../models/IGroupModel";
import { ApiException } from "../common/ApiException";
import { SaveGroupPositionInput } from "../components/GroupComponents/GroupPositionModal";
import { IGroupMemberModel } from "../models/IGroupMemberModel";
import { SaveGroupMemberInput } from "../components/GroupComponents/AddMemberModal";
import { IWorldFishModel } from "../models/IWorldFishModel";

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
      .get<IClientConfigResponse>("ClientConfig")
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
      .get<IGroupModel[]>(
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
      .get<IGroupModel[]>(
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
  public static async GetUser(accessToken: string): Promise<IUserModel> {
    const { data } = await this._httpClient
      .get<IUserModel>("User/Self", {
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
  public static async SaveNewUsername(
    accessToken: string,
    newUsername: string
  ) {
    const { data } = await this._httpClient
      .get<string>(`User/ChangeUsername?newUsername=${newUsername}`, {
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
  public static async GetGroupAndPositions(
    groupId: string,
    accessToken: string
  ) {
    const { data } = await this._httpClient
      .get<IGroupModel>(`Group/GetGroupWithPositions?groupId=${groupId}`, {
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
  public static async DeleteGroup(groupId: string, accessToken: string) {
    const { data } = await this._httpClient
      .get(`Group/DeleteGroup?groupId=${groupId}`, {
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
  public static async SaveGroupPosition(
    position: SaveGroupPositionInput,
    accessToken: string
  ) {
    const { data } = await this._httpClient
      .post<string>("Group/SavePosition", position, {
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
  public static async GetGroupMembers(groupId: string, accessToken: string) {
    const { data } = await this._httpClient
      .get<IGroupMemberModel[]>(`Group/GetGroupMembers?groupId=${groupId}`, {
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
  public static async SaveGroupMember(
    groupMember: SaveGroupMemberInput,
    accessToken: string
  ) {
    const { data } = await this._httpClient
      .post<string>("Group/SaveGroupMember", groupMember, {
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
  public static async SearchUsers(searchTerm: string, accessToken: string) {
    const { data } = await this._httpClient
      .get<Omit<IUserModel, "email">[]>(
        `User/SearchUsers?searchTerm=${searchTerm}`,
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
  public static async DeleteGroupMember(
    groupMemberId: string,
    accessToken: string
  ) {
    const { data } = await this._httpClient
      .get<string>(`Group/DeleteGroupMember?groupMemberId=${groupMemberId}`, {
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
  public static async DeletePosition(positionId: string, accessToken: string) {
    const { data } = await this._httpClient
      .get<string>(`Group/DeletePosition?positionId=${positionId}`, {
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
  public static WorldFishClient = {
    FindSomeLink: async (fishAnyName: string, accessToken: string) => {
      const { data } = await this._httpClient
        .get<IWorldFishModel[]>(
          `WorldFish/FindSomeLink?fishAnyName=${fishAnyName}`,
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
    },
  };
}
