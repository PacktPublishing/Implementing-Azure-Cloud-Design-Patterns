namespace CircuitBreakerSample 
{
    public class Order 
    {
        public int ID { get; set; }
        public decimal Value { get; set; }

        public override string ToString()
        {
            return $"{{ Order ID: {ID}, Order Value: '{Value}'}}";
        }
    }
}