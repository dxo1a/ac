namespace ac.Models
{
    public class Operations
    {
        public int OperationNum { get; set; }
        public string OperationName { get; set; }
        public string Executor { get; set; }


        public int Status { get; set; }             //Не используется, но нужен
        public bool StatusBool { get; set; }
    }
}
