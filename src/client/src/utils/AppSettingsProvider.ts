import AppSettingsJson from "./ClientAppSettings.json";

export default abstract class AppSettingsProvider {
  public static TryGetValue(key: string): string | undefined | null {
    try {
      const keys = key.split(".");
      let result: any = AppSettingsJson;

      for (const k of keys) {
        if (result[k] === undefined) {
          return undefined;
        }
        result = result[k];
      }

      return result as string;
    } catch {
      return undefined;
    }
  }
}
