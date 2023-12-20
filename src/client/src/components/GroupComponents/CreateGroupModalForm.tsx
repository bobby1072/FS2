import { z } from "zod";
import { GroupModel } from "../../models/GroupModel";
import { useSaveGroupMutation } from "./hooks/SaveGroupMutation";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Grid } from "@mui/material";
const formSchema = z.object({
  name: z.string(),
  description: z.string().optional(),
  isPublic: z.boolean(),
  isListed: z.boolean(),
  emblem: z.string().optional(),
});
type FormValues = z.infer<typeof formSchema>;
const mapDefaultValues = (group?: GroupModel): FormValues | undefined => {
  if (!group) return undefined;
  return {
    name: group?.name ?? "",
    description: group?.description ?? "",
    isPublic: group?.isPublic ?? false,
    isListed: group?.isListed ?? false,
    emblem: group?.emblem?.toString() ?? "",
  };
};
export const CreateGroupModalForm: React.FC<{ group?: GroupModel }> = ({
  group,
}) => {
  const { data, mutate, error, isLoading } = useSaveGroupMutation();
  const {
    watch,
    handleSubmit,
    register,
    formState: { errors, isDirty },
  } = useForm<FormValues>({
    defaultValues: mapDefaultValues(group),
    resolver: zodResolver(formSchema),
  });
  const submitHandler = (values: FormValues) => {
    //mutate();
  };
  return (
    <form onSubmit={handleSubmit(submitHandler)}>
      <Grid
        container
        spacing={2}
        padding={2}
        width={"100%"}
        justifyContent="center"
        alignItems="center"
      >
        <Grid item></Grid>
      </Grid>
    </form>
  );
};
