import httpClient from './httpClient';
class Fish {
  public speciesName: string;
  public code: string;
  public latin: string;
  private nickName?: string;
  private physicalDescription?: string;
  private speciesPhoto?: string;
  private speciesNumbers?: object[];
  constructor(
    fishName: string,
    fishCode: string,
    fishLatin: string,
    fishNickName?: string
  ) {
    this.speciesName = fishName;
    this.code = fishCode;
    this.latin = fishLatin;
    fishNickName && (this.nickName = fishNickName);
  }
  public async getSpeciesInfo(): Promise<void> {
    const req = await httpClient.get(
      `https://www.fishwatch.gov/api/species/${this.speciesName}`
    );
    const fishInfo = await req.data;
    if (fishInfo.length >= 1) {
      (this.physicalDescription = fishInfo[0]['Physical Description']),
        (this.speciesPhoto = fishInfo[0]['Species Illustration Photo'].src);
    }
  }
  public async getSpeciesNumbers(): Promise<void> {
    try {
      const req = await httpClient.get(
        `http://openfisheries.org/api/landings/species/${this.code}.json`
      );
      const fishInfo = await req.data;
      if (fishInfo.length >= 1) {
        this.speciesNumbers = fishInfo;
      }
    } catch (err) {}
  }
  public createFishJson(): object {
    return {
      Code: this.code,
      Name: this.speciesName,
      ScientificName: this.latin,
      ...(this.nickName && { Alias: this.nickName }),
      ...(this.speciesNumbers && { SpecieNumbers: this.speciesNumbers }),
      ...(this.physicalDescription &&
        this.speciesPhoto && {
          SpeciesInfo: {
            PhysicalDescription: this.physicalDescription,
            SpeciesPhoto: this.speciesPhoto
          }
        })
    };
  }
}
export default Fish;
