using pocstock.Data;
using System.Globalization;

namespace pocstock.Services
{
    public class Service
    {
        public string GenerateStringDateTime(DateTime dateTime, bool RemoveTimeLable = false)
        {
            if (RemoveTimeLable)
            {
                return dateTime.Year.ToString("0000")
                    + "/"
                    + dateTime.Month.ToString("00")
                    + "/"
                    + dateTime.Day.ToString("00")
                    + " "
                    + dateTime.Hour.ToString("00")
                    + ":"
                    + dateTime.Minute.ToString("00")
                    + " น.";
            }

            return dateTime.Year.ToString("0000")
                        + "/"
                        + dateTime.Month.ToString("00")
                        + "/"
                        + dateTime.Day.ToString("00")
                        + " เวลา "
                        + dateTime.Hour.ToString("00")
                        + ":"
                        + dateTime.Minute.ToString("00")
                        + " น.";
        }

        public string GenerateStringDateTimeForFileName(DateTime dateTime)
        {
            return dateTime.Year.ToString("0000")
                + "-"
                + dateTime.Month.ToString("00")
                + "-"
                + dateTime.Day.ToString("00");
                //+ " "
                //+ dateTime.Hour.ToString("00")
                //+ "-"
                //+ dateTime.Minute.ToString("00")
                //+ "-"
                //+ dateTime.Second.ToString("00");
        }

        public string ConvertToPhoneNumberFormat(string Tel)
        {
            if (Tel.Count() == 10)
            {
                return $"{Tel[..3]}-{Tel.Substring(3, 3)}-{Tel[6..]}";
            }
            else
            {
                return $"{Tel[..2]}-{Tel.Substring(2, 3)}-{Tel[5..]}";
            }
        }

        public string ConvertToThaiDateTime(DateTime dateTime, bool HaveTime = false)
        {
            var BuddhistCalendar = new ThaiBuddhistCalendar();
            var ResultDateTime = string.Empty;

            var Year = BuddhistCalendar.ToFourDigitYear(dateTime.Year);
            var Month = string.Empty;
            switch (dateTime.Month)
            {
                case 1:
                    Month = "มกราคม";
                    break;
                case 2:
                    Month = "กุมภาพันธ์";
                    break;
                case 3:
                    Month = "มีนาคม";
                    break;
                case 4:
                    Month = "เมษายน";
                    break;
                case 5:
                    Month = "พฤษภาคม";
                    break;
                case 6:
                    Month = "มิถุนายน";
                    break;
                case 7:
                    Month = "กรกฎาคม";
                    break;
                case 8:
                    Month = "สิงหาคม";
                    break;
                case 9:
                    Month = "กันยายน";
                    break;
                case 10:
                    Month = "ตุลาคม";
                    break;
                case 11:
                    Month = "พฤศจิกายน";
                    break;
                case 12:
                    Month = "ธันวาคม";
                    break;
                default:
                    // code block
                    break;
            }
            
            if (HaveTime)
            {
                ResultDateTime = $"{dateTime.Day} {Month} {Year} เวลา {dateTime.Hour}:{dateTime.Minute} น.";
            }
            else
            {
                ResultDateTime = $"{dateTime.Day} {Month} {Year}";
            }

            return ResultDateTime;
        }

        public string GetUntilOrEmpty(string text, string stopAt)
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(0, charLocation);
                }
            }

            return text;
        }
    }
}
