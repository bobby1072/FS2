import { SqlReader } from "node-sql-reader";
import path from "path";
import { EntityManager, QueryFailedError } from "typeorm";

export default class MigrationService {
  private readonly _entityManager: EntityManager;
  constructor(entManager: EntityManager) {
    this._entityManager = entManager;
    return this;
  }
  public async RunMigrations() {
    let allFilesFound: boolean = false;
    const migrationQueries: string[][] = [];
    let count = 1;
    while (!allFilesFound) {
      try {
        migrationQueries.push(
          SqlReader.readSqlFile(
            path.join(__dirname, `../persistence/scripts/init__V${count}.sql`)
          )
        );
        count++;
      } catch (e) {
        allFilesFound = true;
      }
    }
    const migrationPromises = migrationQueries
      .flat()
      .map((x) => async () => this._entityManager.query(x));
    migrationPromises.forEach(
      async (x) =>
        await x().catch((e: QueryFailedError) => console.log(e.message))
    );
    return true;
  }
}
