import { GroupMemberModel } from "../../models/GroupMemberModel";
import { UserModel } from "../../models/UserModel";

interface GroupMemberRowItem {}

const mapBaseDataToRowItems = (
  members: GroupMemberModel[],
  leader: UserModel
): GroupMemberRowItem => {
  return {};
};

export const GroupMembersDataTable: React.FC<{
  members: GroupMemberModel[];
  leader: UserModel;
}> = ({ leader, members }) => {
  return null;
};
