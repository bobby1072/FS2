export interface ICronJobService {
  RegisterAllJobs: () => Promise<Boolean>;
  RegisterHourlyJobs: () => Promise<boolean>;
  RegisterDailyJobs: () => Promise<boolean>;
  RegisterWeeklyJobs: () => Promise<boolean>;
  RegisterMonthlyJobs: () => Promise<boolean>;
}
