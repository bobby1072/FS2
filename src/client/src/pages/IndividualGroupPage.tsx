import { useParams } from "react-router-dom";
import { AppAndDraw } from "../common/AppBar/AppAndDraw";
import { PageBase } from "../common/PageBase";
import { useGetFullGroup } from "../components/GroupComponents/hooks/GetFullGroup";

export const IndividualGroupPage: React.FC = () => {
  const { id: groupId } = useParams<{ id: string }>();
  const { data, isLoading, error } = useGetFullGroup(groupId);
  return (
    <PageBase>
      <AppAndDraw>
        <div>Individual Group Page</div>
      </AppAndDraw>
    </PageBase>
  );
};
