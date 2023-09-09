import {
  AccessibilitySchema,
  AccessibilityType,
} from "./Schemas/AccessibilitySchema";

export default class Accessibility implements AccessibilityType {
  public readonly Title: string;
  private static readonly _schema = AccessibilitySchema;
  constructor({ title }: { title: string }) {
    const { Title } = Accessibility._schema.parse({ Title: title });
    this.Title = Title;
  }
}
