import axios from "axios";

export default abstract class WorldFishApiServiceProvider {
  private static readonly _openFisheriesHttpClient = axios.create({
    baseURL: "http://openfisheries.org/api",
  });
  private static readonly _fishWatchHttpClient = axios.create({
    baseURL: "https://www.fishwatch.gov/api",
  });
  public static async GetFishInfo(speciesName: string): Promise<
    | {
        PhysicalDescription: string | undefined;
        SpeciesPhoto: string | undefined;
      }
    | undefined
  > {
    try {
      const request = await this._fishWatchHttpClient.get(
        `species/${speciesName}`
      );
      if (request.data.length >= 1) {
        return {
          PhysicalDescription: request.data[0]["Physical Description"],
          SpeciesPhoto: request.data[0]["Species Illustration Photo"].src,
        };
      } else return undefined;
    } catch (e) {
      return undefined;
    }
  }
  public static async GetSpeciesNumbers(
    code: string
  ): Promise<object[] | undefined> {
    try {
      const request = await this._openFisheriesHttpClient.get(
        `landings/species/${code}.json`
      );
      if (request.data.length >= 1) {
        return request.data;
      } else return undefined;
    } catch (e) {
      return undefined;
    }
  }
}
