using System;
using System.Collections.Generic;

namespace RadencyTask1.classes.payment
{
    public class Payer
    {
        public string name;
        public decimal payment;
        public DateTime date;
        public long account_number;

        public static void Add(List<Payer> list, PaymentRaw data)
        {
            string dataFullName = $"{data.First_name} {data.Last_name}";
            Payer filtred = list.FirstOrDefault(p => p.name == dataFullName);

            if (filtred != null)
            {
                filtred.name = dataFullName;
                filtred.payment = data.Payment;
                filtred.date = data.Date;
                filtred.account_number = data.Account_number;
            }
            else
            {
                Payer payer = new Payer();
                payer.name = dataFullName;
                payer.payment = data.Payment;
                payer.date = data.Date;
                payer.account_number = data.Account_number;

                list.Add(payer);
            }
        }
    }
}