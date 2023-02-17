namespace RadencyTask1.classes.payment
{
    public class PaymentProcessed
    {
        public string city;
        public List<Service> services;
        public int total;

        public static void Add(List<PaymentProcessed> list, PaymentRaw data)
        {
            string dataCity = data.Address.Split(',')[0].Trim();
            PaymentProcessed filtred = list.FirstOrDefault(p => p.city == dataCity);

            if (filtred != null)
            {
                Service.Add(filtred.services, data);
                filtred.total = filtred.services.Count;
            }
            else
            {
                PaymentProcessed paymentProcessed = new PaymentProcessed();
                paymentProcessed.city = dataCity;
                paymentProcessed.services = new List<Service>();
                Service.Add(paymentProcessed.services, data);
                paymentProcessed.total = paymentProcessed.services.Count;

                list.Add(paymentProcessed);
            }
        }
    }
}