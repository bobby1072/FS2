import { useSaveGroupMutation } from "./hooks/SaveGroupMutation";

export const CreateGroupModalForm: React.FC = () => {
  const { data, mutate, error, isLoading } = useSaveGroupMutation();
  return null;
};
