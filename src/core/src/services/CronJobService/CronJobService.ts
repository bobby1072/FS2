import { CronJob } from "cron";
import ApiError from "../../common/ApiError";
import Constants from "../../common/Constants";
import UserService from "../UserService";
import { ICronJobService } from "./ICronJobService";
import CronUtils from "../../utils/CronUtils";
import { DataSource } from "typeorm";

export default class CronJobService implements ICronJobService {
  private readonly _userService: UserService;
  private readonly _dataSource: DataSource;
  constructor(userService: UserService, dataSource: DataSource) {
    this._userService = userService;
    this._dataSource = dataSource;
    return this;
  }
  public async RegisterAllJobs(): Promise<boolean> {
    return Promise.all([
      this.RegisterHourlyJobs(),
      this.RegisterDailyJobs(),
      this.RegisterWeeklyJobs(),
      this.RegisterMonthlyJobs(),
    ])
      .then((data) => data.every((x) => !!x))
      .catch((err) => {
        throw new ApiError(
          Constants.ExceptionMessages.failedToRegisterJobs,
          500
        );
      });
  }
  public async RegisterHourlyJobs(): Promise<boolean> {
    const initialiseDbJob = new CronJob(CronUtils.Hourly(), () =>
      this._dataSource.initialize()
    );

    initialiseDbJob.start();
    if (initialiseDbJob.running) {
      return true;
    } else {
      return false;
    }
  }
  public async RegisterDailyJobs(): Promise<boolean> {
    const userNumbersReportJob = new CronJob(
      CronUtils.Daily(),
      () => this._userService.UserNumbersReport(),
      undefined,
      undefined,
      undefined,
      undefined,
      true
    );

    userNumbersReportJob.start();
    if (userNumbersReportJob.running) {
      return true;
    } else {
      return false;
    }
  }
  public async RegisterWeeklyJobs(): Promise<boolean> {
    const ensureAdminJob = new CronJob(
      CronUtils.Weekly(),
      () => this._userService.EnsureAdminUser(),
      undefined,
      undefined,
      undefined,
      undefined,
      true
    );

    ensureAdminJob.start();
    if (ensureAdminJob.running) {
      return true;
    } else {
      return false;
    }
  }
  public async RegisterMonthlyJobs(): Promise<boolean> {
    return true;
  }
}
