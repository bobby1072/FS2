export default abstract class CronUtils {
  public static Hourly(): string {
    return "0 * * * *";
  }
  public static Daily(): string {
    const currentDate = new Date();
    return `0 ${currentDate.getHours()} * * *`;
  }
  public static Weekly(): string {
    const currentDate = new Date();
    return `0 ${currentDate.getHours()} * * ${currentDate.getDay()}`;
  }
  public static Monthly(): string {
    const currentDate = new Date();
    return `0 ${currentDate.getHours()} ${currentDate.getDate()} * *`;
  }
}
