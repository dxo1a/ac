namespace ac.Models
{
    public class Operations
    {
        public string PP { get; set; }              // ПП

        public int OperationNum { get; set; }
        public string OperationName { get; set; }
        public string Executor { get; set; }
        public double Price { get; set; }
        public double Cost { get; set; }

        public int Status { get; set; }             //Не используется, но нужен
        public bool StatusBool { get; set; }
    }
}
