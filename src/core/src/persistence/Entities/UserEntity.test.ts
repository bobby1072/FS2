import UserEntity from "./UserEntity";

describe("UserEntity", () => {
  describe("Sync", () => {
    it("parse shoulnt throw error if right val given", () => {
      expect(() =>
        UserEntity.ParseSync({
          email: "bobby@ee.com",
          password_hash: "cdcewew",
        })
      ).not.toThrow();
    });
    it.each([
      { emai: "bbbb", password: "test" },
      { email: "test@test.com", password: "fvhuv", phone_number: "443243" },
    ])("parse throws error if bad format", (details) => {
      expect(() => UserEntity.ParseSync(details)).toThrow();
    });
    it("try parse returns a user entity with correctly formatted details", () => {
      const ent = UserEntity.TryParseSync({
        email: "bobby@ee.com",
        password_hash: "cdcewew",
        phone_number: "07915464788",
      });
      expect(ent instanceof UserEntity).toBe(true);
    });
  });
  describe("Async", () => {
    it("parse shoulnt throw error if right val given", async () => {
      expect(() =>
        UserEntity.ParseAsync({
          email: "bobby@ee.com",
          password_hash: "cdcewew",
        })
      ).not.toThrow();
    });
    it.each([
      { emai: "bbbb", password: "test" },
      { email: "test@test.com", password: "fvhuv", phone_number: "443243" },
    ])("parse throws error if bad format", (details) => {
      expect(() => UserEntity.ParseAsync(details)).rejects.toThrow();
    });
    it("try parse returns a user entity with correctly formatted details", async () => {
      const ent = await UserEntity.TryParseAsync({
        email: "bobby@ee.com",
        password_hash: "cdcewew",
        phone_number: "07915464788",
      });
      expect(ent instanceof UserEntity).toBe(true);
    });
  });
});
