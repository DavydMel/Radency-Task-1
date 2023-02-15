using System;
using System.Text.RegularExpressions;

namespace RadencyTask1.classes
{
    public class Payment
    {
        protected string first_name;
        protected string last_name;
        protected string address;
        protected decimal payment;
        protected DateTime date;
        protected long account_number;
        protected string service;

        public Payment(string data)
        {
            //address
            Regex regex = new Regex("[“\"'][^“\"'”]+[”\"']\\s*,");
            Match match = regex.Match(data);
            if (!match.Success)
            {
                throw new Exception("Uncorrect data");
            }
            Console.WriteLine(match.Groups[0].Value);
            address = match.Groups[0].Value.Trim();
            data = data.Replace(match.Groups[0].Value, "");


            var dataArr = data.Split(",");

            if (dataArr.Length != 6)
            {
                throw new Exception("Uncorrect data");
            }

            //name and secondname
            string tmpStr = dataArr[0].Trim();
            if (tmpStr.Contains(" "))
            {
                var tmp = tmpStr.Split(' ');
                first_name = tmp[0];
                last_name = tmp[1];
            }
            else
            {
                first_name = tmpStr;
                last_name = dataArr[1].Trim();
            }

            //payment
            if (!Decimal.TryParse(dataArr[2].Trim(), out payment))
            {
                throw new Exception("Uncorrect payment data");
            }

            //date
            date = DateTime.ParseExact(dataArr[3].Trim(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            
            //account number
            if (!long.TryParse(dataArr[4].Trim(), out account_number))
            {
                throw new Exception("Uncorrect payment data");
            }

            service = dataArr[5].Trim();
        }
    }
}