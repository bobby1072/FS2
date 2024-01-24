import {
  addDaysToDate,
  addMonthsToDate,
  // dayDiff,
  get1stOfMonth,
  getLastDayOfMonth,
  isDateBefore,
  getWorkingDaysInRange,
} from "./DateTime";
const TZERO = new Date("2020-01-01");

export const createMonthRangeFromDate = (
  referenceDate: Date,
  months: number = 1
): DateRange => {
  const endDate =
    months < 2
      ? getLastDayOfMonth(referenceDate)
      : getLastDayOfMonth(addMonthsToDate(referenceDate, months - 1));
  return new DateRange(get1stOfMonth(referenceDate), endDate);
};

export const createWeekRangeFromDate = (
  referenceDate: Date,
  weeks: number = 1
): DateRange => {
  const offsetDate = addDaysToDate(referenceDate, 7 * weeks);
  if (isDateBefore(referenceDate, offsetDate)) {
    return new DateRange(referenceDate, offsetDate);
  }
  return new DateRange(offsetDate, referenceDate);
};

export const currentMonth = (referenceDate: Date = new Date()): DateRange => {
  const start = get1stOfMonth(referenceDate);
  const end = getLastDayOfMonth(referenceDate);
  return new DateRange(start, end);
};

export const lastMonth = (
  referenceDate: Date = new Date(),
  tzeroMin: boolean = false,
  tzero: Date = TZERO
): DateRange => {
  return getPriorCompleteMonths(1, referenceDate, tzeroMin, tzero);
};

export const lastThreeMonths = (
  referenceDate: Date = new Date(),
  tzeroMin: boolean = false,
  tzero: Date = TZERO
): DateRange => {
  return getPriorCompleteMonths(3, referenceDate, tzeroMin, tzero);
};

export const lastTwelveMonths = (
  referenceDate: Date = new Date(),
  tzeroMin: boolean = false,
  tzero: Date = TZERO
): DateRange => {
  return getPriorCompleteMonths(12, referenceDate, tzeroMin, tzero);
};

export const getPriorCompleteMonths = (
  months: number = 1,
  referenceDate: Date = new Date(),
  tzeroMin: boolean = false,
  tzero: Date = TZERO
): DateRange => {
  const dateInMonthCopy = new Date(referenceDate);
  const start = new Date(
    dateInMonthCopy.getFullYear(),
    dateInMonthCopy.getMonth() - months,
    1,
    0,
    0,
    0,
    0
  );
  const lastMonth = new Date(
    dateInMonthCopy.getFullYear(),
    dateInMonthCopy.getMonth() - 1,
    1,
    0,
    0,
    0,
    0
  );
  const end = getLastDayOfMonth(lastMonth);
  if (tzeroMin && isDateBefore(start, tzero)) {
    return new DateRange(tzero, end);
  }
  return new DateRange(start, end);
};

export class DateRange {
  startDate: Date;
  endDate: Date;

  constructor(start: Date, end: Date) {
    this.startDate = start;
    this.endDate = end;
  }

  private toIsoString = (date: Date) =>
    date.toLocaleDateString("en-GB", { year: "numeric" }) +
    "-" +
    date.toLocaleDateString("en-GB", { month: "2-digit" }) +
    "-" +
    date.toLocaleDateString("en-GB", { day: "2-digit" });

  startDateToISOString = () => this.toIsoString(this.startDate);

  endDateToISOString = () => this.toIsoString(this.endDate);

  // numberDaysInRange = (): number => dayDiff(this.startDate, this.endDate);

  workingDaysInRange = (isNonWorkingDay: (date: Date) => boolean): number =>
    getWorkingDaysInRange(this.startDate, this.endDate, isNonWorkingDay, true);
}
