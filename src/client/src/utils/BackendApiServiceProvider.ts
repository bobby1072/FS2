import axios, { AxiosInstance } from "axios";
import { IClientConfigResponse } from "../models/IClientConfigResponse";
import { IUserModel, IUserWithPermissionsRawModel } from "../models/IUserModel";
import { IGroupModel } from "../models/IGroupModel";
import { ApiException } from "../common/ApiException";
import { SaveGroupPositionInput } from "../components/GroupComponents/GroupPositionModal";
import { IGroupMemberModel } from "../models/IGroupMemberModel";
import { SaveGroupMemberInput } from "../components/GroupComponents/AddMemberModal";
import { IWorldFishModel } from "../models/IWorldFishModel";
import {
  IGroupCatchModel,
  IPartialGroupCatchModel,
} from "../models/IGroupCatchModel";
interface ValidationErrorResponse {
  type: string;
  title: string;
  status: number;
  traceId: string;
  errors: {
    [key: string]: string[];
  };
}

export default abstract class BackendApiServiceProvider {
  private static _handleApiValidationError = (e: ValidationErrorResponse) =>
    Object.values(e.errors).flat().join(" ");
  private static _isValidationErrorResponse(
    val: any
  ): val is ValidationErrorResponse {
    return (
      typeof val === "object" &&
      val !== null &&
      typeof val.type === "string" &&
      typeof val.title === "string" &&
      typeof val.status === "number" &&
      typeof val.traceId === "string" &&
      typeof val.errors === "object"
    );
  }
  private static _generalErrorHandler(e: any): PromiseLike<never> {
    if (BackendApiServiceProvider._isValidationErrorResponse(e.response.data)) {
      throw new ApiException(
        BackendApiServiceProvider._handleApiValidationError(e.response.data),
        Number(e.response.status)
      );
    } else {
      throw new ApiException(
        e.response.data as string,
        Number(e.response.status)
      );
    }
  }
  private static _formatAccessToken(accessToken: string) {
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
      .catch(this._generalErrorHandler);
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
            Authorization: this._formatAccessToken(accessToken),
          },
        }
      )
      .catch(this._generalErrorHandler);
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
            Authorization: this._formatAccessToken(accessToken),
          },
        }
      )
      .catch(this._generalErrorHandler);
    return data;
  }
  public static async GetUser(accessToken: string): Promise<IUserModel> {
    const { data } = await this._httpClient
      .get<IUserModel>("User/Self", {
        headers: {
          Authorization: this._formatAccessToken(accessToken),
        },
      })
      .catch(this._generalErrorHandler);
    return data;
  }
  public static async GetUserWithGroupPermissions(accessToken: string) {
    const { data } = await this._httpClient
      .get<IUserWithPermissionsRawModel>("User/SelfWithGroupPermissions", {
        headers: {
          Authorization: this._formatAccessToken(accessToken),
        },
      })
      .catch(this._generalErrorHandler);
    return data;
  }
  public static async SaveGroup(accessToken: string, group: FormData) {
    const { data } = await this._httpClient
      .post<string>("Group/SaveGroup", group, {
        headers: {
          Authorization: this._formatAccessToken(accessToken),
        },
      })
      .catch(this._generalErrorHandler);
    return data;
  }
  public static async GetGroupCount(accessToken: string) {
    const { data } = await this._httpClient
      .get<number>("Group/GetGroupCount", {
        headers: {
          Authorization: this._formatAccessToken(accessToken),
        },
      })
      .catch(this._generalErrorHandler);
    return data;
  }
  public static async GetUsersGroup(
    accessToken: string,
    startIndex: number,
    count: number
  ) {
    const { data } = await this._httpClient
      .get<IGroupModel[]>(
        `Group/GetUsersGroups?startIndex=${startIndex}&count=${count}`,
        {
          headers: {
            Authorization: this._formatAccessToken(accessToken),
          },
        }
      )
      .catch(this._generalErrorHandler);
    return data;
  }
  public static async SaveNewUsername(
    accessToken: string,
    newUsername: string
  ) {
    const { data } = await this._httpClient
      .get<string>(`User/ChangeUsername?newUsername=${newUsername}`, {
        headers: {
          Authorization: this._formatAccessToken(accessToken),
        },
      })
      .catch(this._generalErrorHandler);
    return data;
  }
  public static async GetGroupAndPositions(
    groupId: string,
    accessToken: string
  ) {
    const { data } = await this._httpClient
      .get<IGroupModel>(`Group/GetGroupWithPositions?groupId=${groupId}`, {
        headers: {
          Authorization: this._formatAccessToken(accessToken),
        },
      })
      .catch(this._generalErrorHandler);
    return data;
  }
  public static async DeleteGroup(groupId: string, accessToken: string) {
    const { data } = await this._httpClient
      .get(`Group/DeleteGroup?groupId=${groupId}`, {
        headers: {
          Authorization: this._formatAccessToken(accessToken),
        },
      })
      .catch(this._generalErrorHandler);
    return data;
  }
  public static async SaveGroupPosition(
    position: SaveGroupPositionInput,
    accessToken: string
  ) {
    const { data } = await this._httpClient
      .post<string>("Group/SavePosition", position, {
        headers: {
          Authorization: this._formatAccessToken(accessToken),
        },
      })
      .catch(this._generalErrorHandler);
    return data;
  }
  public static async GetGroupMembers(groupId: string, accessToken: string) {
    const { data } = await this._httpClient
      .get<IGroupMemberModel[]>(`Group/GetGroupMembers?groupId=${groupId}`, {
        headers: {
          Authorization: this._formatAccessToken(accessToken),
        },
      })
      .catch(this._generalErrorHandler);
    return data;
  }
  public static async SaveGroupMember(
    groupMember: SaveGroupMemberInput,
    accessToken: string
  ) {
    const { data } = await this._httpClient
      .post<string>("Group/SaveGroupMember", groupMember, {
        headers: {
          Authorization: this._formatAccessToken(accessToken),
        },
      })
      .catch(this._generalErrorHandler);
    return data;
  }
  public static async SearchUsers(searchTerm: string, accessToken: string) {
    const { data } = await this._httpClient
      .get<Omit<IUserModel, "email">[]>(
        `User/SearchUsers?searchTerm=${searchTerm}`,
        {
          headers: {
            Authorization: this._formatAccessToken(accessToken),
          },
        }
      )
      .catch(this._generalErrorHandler);
    return data;
  }
  public static async DeleteGroupMember(
    groupMemberId: string,
    accessToken: string
  ) {
    const { data } = await this._httpClient
      .get<string>(`Group/DeleteGroupMember?groupMemberId=${groupMemberId}`, {
        headers: {
          Authorization: this._formatAccessToken(accessToken),
        },
      })
      .catch(this._generalErrorHandler);
    return data;
  }
  public static async DeletePosition(positionId: string, accessToken: string) {
    const { data } = await this._httpClient
      .get<string>(`Group/DeletePosition?positionId=${positionId}`, {
        headers: {
          Authorization: this._formatAccessToken(accessToken),
        },
      })
      .catch(this._generalErrorHandler);
    return data;
  }
  public static WorldFishClient = {
    FindSomeLike: async (fishAnyName: string, accessToken: string) => {
      const { data } = await this._httpClient
        .get<IWorldFishModel[]>(
          `WorldFish/FindSomeLike?fishAnyName=${fishAnyName}`,
          {
            headers: {
              Authorization: this._formatAccessToken(accessToken),
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
  public static async SearchListedGroups(
    groupName: string,
    accessToken: string
  ) {
    const { data } = await this._httpClient
      .get<IGroupModel[]>(
        `Group/SearchAllListedGroups?groupName=${groupName}`,
        {
          headers: {
            Authorization: this._formatAccessToken(accessToken),
          },
        }
      )
      .catch(this._generalErrorHandler);
    return data;
  }
  public static async SaveGroupCatch(gc: FormData, accessToken: string) {
    const { data } = await this._httpClient
      .post<string>(`GroupCatch/SaveGroupCatch`, gc, {
        headers: {
          Authorization: this._formatAccessToken(accessToken),
        },
      })
      .catch(this._generalErrorHandler);
    return data;
  }
  public static async GetAllPartialCatchesForGroup(
    groupId: string,
    accessToken: string
  ) {
    const { data } = await this._httpClient
      .get<IPartialGroupCatchModel[]>(
        `GroupCatch/GetCatchesInGroup?groupId=${groupId}`,
        {
          headers: {
            Authorization: this._formatAccessToken(accessToken),
          },
        }
      )
      .catch(this._generalErrorHandler);
    return data;
  }
  public static async GetFullCatchById(catchId: string, accessToken: string) {
    const { data } = await this._httpClient
      .get<IGroupCatchModel>(`GroupCatch/GetFullCatchById?catchId=${catchId}`, {
        headers: {
          Authorization: this._formatAccessToken(accessToken),
        },
      })
      .catch(this._generalErrorHandler);
    return data;
  }
  public static async DeleteGroupCatch(catchId: string, accessToken: string) {
    const { data } = await this._httpClient
      .get<string>(`GroupCatch/DeleteGroupCatch?id=${catchId}`, {
        headers: {
          Authorization: this._formatAccessToken(accessToken),
        },
      })
      .catch(this._generalErrorHandler);
    return data;
  }
}
