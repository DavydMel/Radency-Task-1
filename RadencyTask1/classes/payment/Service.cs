using System;

namespace RadencyTask1.classes.payment
{
    public class Service
    {
        public string name;
        public List<Payer> payers;
        public int total;

        public static void Add(List<Service> list, PaymentRaw data)
        {
            Service filtred = list.FirstOrDefault(s => s.name == data.Service);

            if (filtred != null)
            {
                Payer.Add(filtred.payers, data);
                filtred.total = filtred.payers.Count;
            }
            else
            {
                Service service = new Service();
                service.name = data.Service;
                service.payers = new List<Payer>();
                Payer.Add(service.payers, data);
                service.total = service.payers.Count;

                list.Add(service);
            }
        }
    }
}