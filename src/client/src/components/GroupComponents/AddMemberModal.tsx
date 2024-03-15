import { z } from "zod";
import { GroupMemberModel } from "../../models/GroupMemberModel";
import { GroupPositionModel } from "../../models/GroupPositionModel";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useSnackbar } from "notistack";
import { useSaveMemberMutation } from "./hooks/SaveMemberMutation";

const formSchema = z.object({
  id: z.number().optional(),
  groupId: z.string(),
  userId: z.string(),
  positionId: z.number(),
});

export type SaveGroupMemberInput = z.infer<typeof formSchema>;

const mapDefaultValues = (
  groupId: string,
  values?: GroupMemberModel
): Partial<SaveGroupMemberInput> => {
  if (!values) return { groupId };
  return {
    id: values.id,
    groupId: values.groupId,
    userId: values.userId,
    positionId: values.positionId,
  };
};

export const AddMemberModal: React.FC<{
  defaultValue?: GroupMemberModel;
  positions: GroupPositionModel[];
  groupId: string;
}> = ({ defaultValue, positions, groupId }) => {
  const {
    watch,
    handleSubmit,
    register,
    formState: { isDirty, errors: formError },
  } = useForm<SaveGroupMemberInput>({
    resolver: zodResolver(formSchema),
    defaultValues: mapDefaultValues(groupId, defaultValue),
  });
  const {} = useSaveMemberMutation();
  const { enqueueSnackbar } = useSnackbar();

  return null;
};
