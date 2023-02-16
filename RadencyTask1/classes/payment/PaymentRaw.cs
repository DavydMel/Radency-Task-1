using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RadencyTask1.classes.payment
{
    public class PaymentRaw
    {
        protected string first_name;
        public string First_name { get { return first_name; } }

        protected string last_name;
        public string Last_name { get { return last_name; } }

        protected string address;
        public string Address { get { return address; } }

        protected decimal payment;
        public decimal Payment { get { return payment; } }

        protected DateTime date;
        public DateTime Date { get { return date; } }

        protected long account_number;
        public long Account_number { get { return account_number; } }

        protected string service;
        public string Service { get { return service; } }

        protected List<PaymentRaw> payments = new List<PaymentRaw>();

        public PaymentRaw(string data)
        {
            //address
            Regex regex = new Regex("[“\"'][^“\"'”]+[”\"']\\s*,");
            Match match = regex.Match(data);
            if (!match.Success)
            {
                throw new Exception("Uncorrect data");
            }
            address = Regex.Replace(match.Groups[0].Value, @"[“""'”]", "").Trim();
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

            payment = Convert.ToDecimal(dataArr[2].Trim(), new NumberFormatInfo { NumberDecimalSeparator = "." });

            //date
            date = DateTime.ParseExact(dataArr[3].Trim(), "yyyy-dd-MM", CultureInfo.InvariantCulture);

            //account number
            tmpStr = Regex.Replace(dataArr[4].Trim(), @"[“""'”]", "");
            account_number = long.Parse(tmpStr);

            service = dataArr[5].Trim();
        }
    }
}