import { PublicUserSchema, PublicUserType } from "./Schemas/PublicUserSchema";

export default class PublicUser implements PublicUserType {
  private static readonly _schema = PublicUserSchema;
  public Username: string;
  public Name?: string | null;
  public Description?: string | null;
  public CreatedAt: Date;
  constructor({
    CreatedAt = new Date(),
    Username,
    Description,
    Name,
  }: {
    Username: string;
    Name?: string | null;
    Description?: string | null;
    CreatedAt: Date;
  }) {
    const safeVals = PublicUser._schema.parse({
      CreatedAt,
      Username,
      Name,
      Description,
    });
    this.Username = safeVals.Username;
    this.Name = safeVals.Name;
    this.Description = safeVals.Description;
    this.CreatedAt = safeVals.CreatedAt;
    return this;
  }
  public ToJson(): any {
    const parsObj = { ...this } as any;
    if ("_schema" in parsObj) {
      parsObj._schema = undefined;
    }
    return JSON.parse(JSON.stringify(parsObj));
  }
}
