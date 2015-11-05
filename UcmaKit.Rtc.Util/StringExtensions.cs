namespace ISI.Rtc.Util
{

   public static class StringExtensions
    {

       /// <summary>
       /// Trims a string to null.
       /// </summary>
       /// <param name="self"></param>
       /// <returns></returns>
       public static string TrimToNull(this string self)
       {
           return string.IsNullOrWhiteSpace(self) ? null : self.Trim();
       }

    }

}
